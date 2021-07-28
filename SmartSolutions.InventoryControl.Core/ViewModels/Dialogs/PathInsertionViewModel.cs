using Caliburn.Micro;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.LogUtils;
using System;
using System.ComponentModel.Composition;
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
                    setting.Value = FileNameWithPath;
                    setting.Description = "Is Path Created Or Not";
                    var result = await _systemSettingManager.SaveSettingAsync(setting);
                    if (result)
                        await BackUpDataBase(_databaseName);
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
