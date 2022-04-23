using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.BackUp
{
    public interface IDatabaseBackupManager
    {
        Task<bool> CreateBackupAsync(string dbName, string folderPath);
        /// <summary>
        /// Create Full Database Backup
        /// </summary>
        /// <param name="myServer"></param>
        /// <param name="myDatabase"></param>
        /// <returns>return true if Back up is Created</returns>
        Task<bool> CreateFullBackupAsync(Server myServer, Database myDatabase, string folderPath);
        /// <summary>
        /// Create Diffrential Database Or Update It Diffrential
        /// </summary>
        /// <returns>return true if Back up is Created</returns>
        Task<bool> CreateDifferentialBackupAsync(Server myServer, Database myDatabase, string folderPath);
        Task<bool> RestoreDatabaseAsync(Server myServer, Database myDatabase, string folderPath);
        Task<bool> BackUpDataBaseAsync(string databaseName);
    }
}
