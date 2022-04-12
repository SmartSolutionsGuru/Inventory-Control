using Caliburn.Micro;
using Notifications.Wpf;
using SmartSolutions.InventoryControl.Core.ViewModels.Commands;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.LogUtils;
using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.SqlServer.Management.Smo;
using SmartSolutions.InventoryControl.DAL;
using System.Collections.Generic;

namespace SmartSolutions.InventoryControl.Core.ViewModels
{
    [Export(typeof(IShell)), PartCreationPolicy(CreationPolicy.Shared)]
    public class ShellViewModel : BaseViewModel, IShell, IHandle<Screen>
    {
        #region Private Members
        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManager _windowManager;
        private string DirectoryName = "Smart Solutions";
        private readonly IDialogManager _dialog;
        private static string dbFile = null;
        private readonly DAL.Managers.Proprietor.IProprietorInformationManager _proprietorInformationManager;
        private readonly DAL.Managers.Settings.ISystemSettingManager _systemSettingManager;
        private readonly IRepository repository;
        #endregion

        #region Constructor
        public ShellViewModel() { }

        public ShellViewModel(Window window)
        {
            mWindow = window;
        }
        [ImportingConstructor]
        public ShellViewModel(IEventAggregator eventAggregator
                              , IWindowManager windowManager
                              , IDialogManager dialogManager
                              , DAL.Managers.Proprietor.IProprietorInformationManager proprietorInformationManager
                              , DAL.Managers.Settings.ISystemSettingManager systemSettingManager)
        {
            _eventAggregator = eventAggregator;
            _windowManager = windowManager;
            _dialog = dialogManager;
            _proprietorInformationManager = proprietorInformationManager;
            _systemSettingManager = systemSettingManager;
            
        }
        #endregion

        #region Public Properties
        public IDialogManager Dialog => _dialog;
        #endregion

        #region Commands
        public RelayCommand MenuCommand { get; set; }
        #endregion

        #region Methods
        public void Handle(Screen screen)
        {
            if (screen is MainViewModel || screen is Login.LoginViewModel || screen is ProprietorInformationViewModel)
            {
                ActiveItem = screen;
                ActivateItem(screen);
            }

        }
        protected async override void OnInitialize()
        {
            base.OnInitialize();
            dbFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DirectoryName, "InventoryControl.db");
            var result = InitializeDatabaseConnection();
            //await InitializeDatabaseScripts();
            var settingResult = await _systemSettingManager.GetsystemSettingByKeyAsync("IsProprietorAvailable");
            if (settingResult != null)
            {
                if (settingResult.SettingValue == 1)
                {
                    DAL.AppSettings.Proprietor = await _proprietorInformationManager.GetProprietorInfoAsync();
                    Handle(IoC.Get<Login.LoginViewModel>());
                }
                else
                    Handle(IoC.Get<ProprietorInformationViewModel>());
            }

        }
        protected override void OnActivate()
        {
            base.OnActivate();
            _eventAggregator.Subscribe(this);
            //TODO: This Will be Work if login As Admin
            //var isDbExist = VerifyDatabaseExist();
            //if (isDbExist)
            //{
            //    var result = InitializeDatabaseConnection();
            //}
            //else
            //{
            //TODO : here we Run The Script For Creating Database
            //}
            //var result = InitializeDatabaseConnection();

        }
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            GetBaseWindow(view);
            MenuCommand = new RelayCommand(() => SystemCommands.ShowSystemMenu(mWindow, GetMousePosition()));
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            _eventAggregator.Unsubscribe(this);
        }
        private bool VerifyDatabaseExist()
        {
            bool retVal = false;
            try
            {
                //var dbExists = new Server(serverOrInstanceName).Databases.Contains(dataBaseName);
                retVal = new Server(Environment.MachineName).Databases.Contains("SmartSolutions.InventoryControl");
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }
        private bool InitializeDatabaseConnection()
        {
            bool retVal = false;
            try
            {
                if (IoC.Get<IRepository>().Type == DBTypes.SQLITE)
                {

                    if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DirectoryName)))
                    {
                        Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DirectoryName));
                    }
                    if (!File.Exists(dbFile))
                    {
                        //Get the Curent Directory
                        var currentDirectory = Environment.CurrentDirectory;
                        var splitString = currentDirectory.Split(new string[] { "bin" }, StringSplitOptions.None);
                        var sourcePath = string.Concat(splitString[0], "\\Assets\\InventoryControl.db");
                        string destinationName = string.Empty;
                        DirectoryInfo directory = null;
                        //Create Config Directory
                        if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DirectoryName)))
                            directory = Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DirectoryName));
                        //Get the destinationPath and Copy Db   
                        destinationName = dbFile;//$"{Environment.SpecialFolder.LocalApplicationData}\\{DirectoryName}\\InventoryControl.db";
                        File.Copy(sourcePath, destinationName);
                        ConnectionInfo.Instance.ConnectionString = destinationName;
                        ConnectionInfo.Instance.Password = "7JByVhs7*ETu5-by";
                        ConnectionInfo.Instance.Type = DBTypes.SQLITE;
                    }
                    else
                    {
                        ConnectionInfo.Instance.Path = dbFile;
                        ConnectionInfo.Instance.Password = "7JByVhs7*ETu5-by";
                        ConnectionInfo.Instance.Type = DBTypes.SQLITE;
                    }
                }
                else if (IoC.Get<IRepository>().Type == DBTypes.SQLServer)
                {
                    //string host = "DESKTOP-MLLBV2C\\Shabab Butt";
                    //string host = "Nofal-PC";
                    string host = Environment.MachineName;
                    string port = "1433";
                    string database = "SmartSolutions.InventoryControl";
                    //string username = WindowsIdentity.GetCurrent().Name;
                    string username = "sa";
                    string password = "Pakistan@123";
                    //string password = "";

                    var args = Environment.GetCommandLineArgs();
                    LogMessage.Write("Arguments: " + Newtonsoft.Json.JsonConvert.SerializeObject(args));
                    if (args != null)
                    {
                        foreach (var arg in args)
                        {
                            var parts = arg?.Split(':');
                            if ((parts?.Length ?? 0) > 1)
                            {
                                string key = parts?.ElementAtOrDefault(0);
                                string value = parts?.ElementAtOrDefault(1);
                                switch (key)
                                {
                                    case "host":
                                        host = value;
                                        break;
                                    case "port":
                                        port = value;
                                        break;
                                    case "database":
                                        database = value;
                                        break;
                                    case "username":
                                        username = value;
                                        break;
                                    case "password":
                                        password = value;
                                        break;
                                    case "dbPath":
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                    ConnectionInfo.Instance.IPAddress = host;
                    ConnectionInfo.Instance.Port = port;
                    ConnectionInfo.Instance.Database = database;
                    ConnectionInfo.Instance.UserName = username;
                    ConnectionInfo.Instance.Password = password;
                    ConnectionInfo.Instance.Type = DBTypes.SQLServer;
                    //AppSettings.IsLoggedInUserAdmin = IsUserAdmin();
                    AppSettings.IsLoggedInUserAdmin = true;
                    if (AppSettings.IsLoggedInUserAdmin)
                        ConnectionInfo.Instance.ConnectionString = string.Format($"data source=localhost; Initial Catalog=" + database + ";Integrated Security = SSPI;");
                    else
                        ConnectionInfo.Instance.ConnectionString = string.Format($"data source=localhost; Initial Catalog=" + database + ";User id=" + username + ";Password=" + password);

#if DEBUG
                    LogMessage.Write($"::: DB Config ::: Host={host}, Port={port}, Username={username}, Password={password}, Database={database}");
#endif

                    if (ConnectionInfo.Instance == null)
                    {
                        ConnectionInfo.Instance.IPAddress = host;
                        ConnectionInfo.Instance.Database = database;
                        ConnectionInfo.Instance.Port = port;
                        ConnectionInfo.Instance.UserName = username;
                        ConnectionInfo.Instance.Password = password;
                        ConnectionInfo.Instance.ConnectionString = string.Format("Server=" + host + ";Port=" + port + ";Database=" + database + ";Uid=" + username + ";Pwd=" + password + ";convert zero datetime=True;");

                    }
                }
                // Verify the Connection is Valid Or Not
                Exception ex = null;
                retVal = true == IoC.Get<IRepository>()?.IsValidConnection(out ex);
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }
        private async Task<bool> InitializeDatabaseScripts()
        {
            bool retVal = false;
            try
            {
                var resultDir = Environment.CurrentDirectory;
                string[] appPath = resultDir.Split(new string[] { "bin" }, StringSplitOptions.None);
                var scriptArray = Directory.GetFiles(string.Concat(appPath[0], "Scripts"));
                foreach (var script in scriptArray)
                {
                    FileInfo file = new FileInfo(script);
                    await repository.QueryAsync(script);
                }
                
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }

            return retVal;
        }
        #endregion

        #region Helper Method
        public void GetBaseWindow(object view)
        {
            // Listen out for the window resizing
            mWindow = view as Window;
            if (mWindow != null)
            {
                mWindow.StateChanged += (sender, e) =>
                {
                    // Fire off events for all properties that are affected by a resize
                    base.WindowResized();
                };

                mWindowResizer = new Helpers.WindowHelper.WindowResizer(mWindow);

                // Listen out for dock changes
                mWindowResizer.WindowDockChanged += (dock) =>
                {
                    // Store last position
                    mDockPosition = dock;

                    // Fire off resize events
                    WindowResized();
                };

                // On window being moved/dragged
                mWindowResizer.WindowStartedMove += () =>
                {
                    // Update being moved flag
                    BeingMoved = true;
                };

                // Fix dropping an undocked window at top which should be positioned at the
                // very top of screen
                mWindowResizer.WindowFinishedMove += () =>
                {
                    // Update being moved flag
                    BeingMoved = false;

                    // Check for moved to top of window and not at an edge
                    if (mDockPosition == Helpers.WindowHelper.WindowDockPosition.Undocked && mWindow.Top == mWindowResizer.CurrentScreenSize.Top)
                        // If so, move it to the true top (the border size)
                        mWindow.Top = -OuterMarginSize.Top;
                };
            }
            else
                return;
        }

        private bool IsUserAdmin()
        {
            //bool value to hold our return value
            bool isAdmin;
            try
            {
                //get the currently logged in user
                WindowsIdentity user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
                isAdmin = false;
            }
            catch (Exception ex)
            {
                isAdmin = false;
            }
            return isAdmin;
        }

        #endregion
    }
}
