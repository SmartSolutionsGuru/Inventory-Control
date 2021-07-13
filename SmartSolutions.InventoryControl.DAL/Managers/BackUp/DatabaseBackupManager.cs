using SmartSolutions.Util.LogUtils;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.BackUp
{
    [Export(typeof(IDatabaseBackupManager)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DatabaseBackupManager : BaseManager, IDatabaseBackupManager
    {
        bool retVal = false;
        public async Task<bool> CreateBackupAsync(string dbName, string folderPath)
        {
            try
            {

            }
            catch (Exception ex)
            {

                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }
    }
}
