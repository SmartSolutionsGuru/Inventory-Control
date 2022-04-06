using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.LogUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.BackUp
{
    [Export(typeof(IDatabaseBackupManager)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DatabaseBackupManager : BaseManager, IDatabaseBackupManager
    {
        #region Private Members
        bool retVal = false;
        private readonly IRepository Repository;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public DatabaseBackupManager()
        {
            Repository = GetRepository<DatabaseBackupManager>();
        }
        #endregion

        #region Public Methods
        public async Task<bool> BackUpDataBaseAsync(string databaseName)
        {
            bool retVal = false;
            try
            {
                //var FileNameWithPath = BuildBackUpWithFileName(, databaseName);
                //await CreateBackupAsync(databaseName);
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }

        public async Task<bool> CreateBackupAsync(string dbName, string folderPath)
        {
            try
            {
                var fileName = BuildBackUpWithFileName(folderPath, dbName);
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_Database"] = dbName;
                parameters["@v_Path"] = folderPath;
                string query = @"BACKUP DATABASE @v_Database TO DISK = @v_Path";
                 var result =  await Repository.NonQueryAsync(query,parameters:parameters);
                retVal = result > 0 ? true : false;
                //await Task.Run(() =>
                //{
                //    if (File.Exists(folderPath))
                //    {
                //        File.Replace(fileName, folderPath, folderPath);
                //        retVal = true;
                //    }  
                //    else
                //    {
                //        File.Create(fileName);
                //        retVal = true;
                //    }
                       
                //});
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }

        
        private string BuildBackUpWithFileName(string filePath,string databaseName)
        {
            //var fileName = String.Format($"{0}--{1}.bak", databaseName, DateTime.Now.ToString("yyyy-MM-dd"));
            var fileName = databaseName + "--" + DateTime.Now.ToString("yyyy-MM-dd") + ".bak";
            return Path.Combine(filePath, fileName);
            //return fileName;
        }
        #endregion
    }
}
