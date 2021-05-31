using Caliburn.Micro;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.LogUtils;
using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.Core.ViewModels
{
    [Export(typeof(IShell)), PartCreationPolicy(CreationPolicy.Shared)]
    public class ShellViewModel : Conductor<Screen>, IShell, IHandle<Screen>
    {
        #region Private Members
        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManager _windowManager;
        private string DirectoryName = "Config";
        private readonly IDialogManager _dialog;
        private static string dbFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\InventoryControl.db";
        #endregion

        #region Constructor

        [ImportingConstructor]
        public ShellViewModel(IEventAggregator eventAggregator
                              ,IWindowManager windowManager
                              , IDialogManager dialogManager)
        {
            _eventAggregator = eventAggregator;
            _windowManager = windowManager;
            _dialog = dialogManager;
        }
        public ShellViewModel(object window)
        {

        }
        #endregion

        #region Public Properties
        public IDialogManager Dialog => _dialog;
        #endregion

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
    }
}
