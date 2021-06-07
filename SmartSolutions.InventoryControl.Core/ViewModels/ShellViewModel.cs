using Caliburn.Micro;
using SmartSolutions.InventoryControl.Core.ViewModels.Commands;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.LogUtils;
using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace SmartSolutions.InventoryControl.Core.ViewModels
{
    [Export(typeof(IShell)), PartCreationPolicy(CreationPolicy.Shared)]
    public class ShellViewModel : BaseViewModel, IShell, IHandle<Screen>
    {
        #region Private Members
        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManager _windowManager;
        private string DirectoryName = "Config";
        private readonly IDialogManager _dialog;
        private static string dbFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\InventoryControl.db";
        #endregion

        #region Constructor
        public ShellViewModel() { }
       
        public ShellViewModel(Window window)
        {
            mWindow = window;
        }
        [ImportingConstructor]
        public ShellViewModel(IEventAggregator eventAggregator
                              ,IWindowManager windowManager
                              , IDialogManager dialogManager)
        {
            _eventAggregator = eventAggregator;
            _windowManager = windowManager;
            _dialog = dialogManager;
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
            if (screen is MainViewModel)
                ActivateItem(screen);
        }
        protected override void OnInitialize()
        {
            base.OnInitialize();
            Handle(IoC.Get<MainViewModel>());
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            _eventAggregator.Subscribe(this);
            InitializeDatabaseConnection();
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
        private bool InitializeDatabaseConnection()
        {
            bool retVal = false;
            try
            {
                //Get the Db File
                dbFile = Path.Combine(Environment.CurrentDirectory, DirectoryName) + "\\InventoryControl.db";
                if (!File.Exists(dbFile))
                {
                    //Get the Curent Directory
                    var currentDirectory = Environment.CurrentDirectory;
                    var splitString = currentDirectory.Split(new string[] { "bin" }, StringSplitOptions.None);
                    var sourcePath = string.Concat(splitString[0], "\\Assets\\InventoryControl.db");
                    string destinationName = string.Empty;
                    DirectoryInfo directory = null;
                    //Create Config Directory
                    if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, DirectoryName)))
                        directory = Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, DirectoryName));
                    //Get the destinationPath and Copy Db   
                    destinationName = $"{Environment.CurrentDirectory}\\{DirectoryName}\\InventoryControl.db";
                    File.Copy(sourcePath, destinationName);
                    ConnectionInfo.Instance.ConnectionString = destinationName;
                    ConnectionInfo.Instance.Password = "7JByVhs7*ETu5-by";
                }
                else
                {
                    ConnectionInfo.Instance.Path = dbFile;
                    ConnectionInfo.Instance.Password = "7JByVhs7*ETu5-by";
                }
                Exception ex = null;
                retVal = true == IoC.Get<Plugins.Repositories.IRepository>()?.IsValidConnection(out ex);

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
        #endregion
    }
}
