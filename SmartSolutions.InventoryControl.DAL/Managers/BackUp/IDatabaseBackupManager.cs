using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.BackUp
{
    public interface IDatabaseBackupManager
    {
        Task<bool> CreateBackupAsync(string dbName,string folderPath);
    }
}
