using Caliburn.Micro;
using Microsoft.SqlServer.Management.Smo;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.LogUtils;
using System;
using System.ComponentModel.Composition;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.Core.ViewModels.Dialogs
{
    [Export(typeof(PathInsertionViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PathInsertionViewModel : BaseViewModel
    {
        #region Private Members
        private readonly string _databaseName;
        private readonly DAL.Managers.BackUp.IDatabaseBackupManager _databaseBackupManager;
        private readonly DAL.Managers.Settings.ISystemSettingManager _systemSettingManager;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public PathInsertionViewModel(DAL.Managers.BackUp.IDatabaseBackupManager databaseBackupManager
                                     , DAL.Managers.Settings.ISystemSettingManager systemSettingManager)
        {
            _databaseBackupManager = databaseBackupManager;
            _systemSettingManager = systemSettingManager;
            _databaseName = ConnectionInfo.Instance.Database;
        }
        #endregion

        #region Protected Methods
        protected override void OnActivate()
        {
            base.OnActivate();
        }
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            if (Execute.InDesignMode)
            {
                BackupPath = "C:\\BackUpFolder";
            }
        }
        #endregion

        #region Public Methods
        public void Close()
        {
            TryClose();
        }

        public void Cancel()
        {
            TryClose();
        }
        /// <summary>
        /// Save the Path and update systemsettings so Second time we will not open this Box
        /// and Update Database
        /// </summary>
        public async void Save()
        {
            try
            {
                if (!string.IsNullOrEmpty(BackupPath))
                {
                    

                    DAL.Models.SystemSettingModel setting = new DAL.Models.SystemSettingModel();
                    setting.Name = "IsDbPathInserted";
                    setting.SettingKey = "Is_DbPath_Inserted";
                    setting.SettingValue = 1;
                    setting.DefaultValue = false;
                    setting.Value = BackupPath;
                    setting.Description = string.IsNullOrEmpty(FileNameWithPath) ? setting.Value : FileNameWithPath;
                    var result = await _systemSettingManager.SaveSettingAsync(setting);
                    if (result)
                    {
                        //This Code is Used For Diffrential Backup and Using SMO
                        // we are using Traditonal Backup Style Now and use it after Testing

                        Server myServer = new Server();
                        //Using windows authentication
                        myServer.ConnectionContext.LoginSecure = true;
                        myServer.ConnectionContext.Connect();
                        Database myDatabase = myServer.Databases[$"{_databaseName}"];
                        var backupResult = await _databaseBackupManager.CreateFullBackupAsync(myServer, myDatabase, BackupPath);

                        //var backupResult = await _databaseBackupManager.CreateBackupAsync(_databaseName, BackupPath);
                        if (backupResult)
                        {
                            Close();
                            NotificationManager.Show(new Notifications.Wpf.NotificationContent
                            {
                                Title = "Success",
                                Message = "BackUp Path Successfully Enter",
                                Type = Notifications.Wpf.NotificationType.Success
                            });
                        }
                    }
                }
                else
                {
                    IsPathEmpty = true;
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }

        public async Task BackUpDataBase(string databaseName)
        {
            FileNameWithPath = BuildBackUpWithFileName(_databaseName);
            await _databaseBackupManager.CreateBackupAsync(_databaseName, FileNameWithPath);
        }
        private string BuildBackUpWithFileName(string databaseName)
        {
            string fileName = string.Format($"{0}--{1}.bak", databaseName, DateTime.Now.ToString("yyyy-MM-dd"));
            return Path.Combine(BackupPath, fileName);
        }

        #endregion

        #region [Private Helpers]
       
        #endregion

        #region Public Properties
        private string m_BackupPath;

        public string BackupPath
        {
            get { return m_BackupPath; }
            set { m_BackupPath = value; NotifyOfPropertyChange(nameof(BackupPath)); }
        }
        private bool _IsPathEmpty;
        /// <summary>
        /// Verified Path Is Filled or Empty
        /// </summary>
        public bool IsPathEmpty
        {
            get { return _IsPathEmpty; }
            set { _IsPathEmpty = value; NotifyOfPropertyChange(nameof(IsPathEmpty)); }
        }

        public string FileNameWithPath { get; set; }

        #endregion
    }
}
