using SmartSolutions.Util.LogUtils;
using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.BackUp
{
    [Export(typeof(IDatabaseBackupManager)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DatabaseBackupManager : BaseManager, IDatabaseBackupManager
    {
        bool retVal = false;

        public async Task<bool> BackUpDataBase(string databaseName)
        {
            bool retVal = false;
            try
            {

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
                await Task.Run(() =>
                {
                    if (File.Exists(folderPath))
                        File.Replace(dbName, folderPath, folderPath);
                    else
                        File.Create(folderPath);
                });
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }
    }
}
