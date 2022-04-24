using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.LogUtils;
using System;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
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
        /// <summary>
        /// Create Full Database With DateTime 
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public async Task<bool> CreateBackupAsync(string dbName, string folderPath)
        {
            try
            {
                var fileName = BuildBackUpWithFileName(folderPath, dbName);
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_Database"] = dbName;
                parameters["@v_Path"] = folderPath;
                string query = @"BACKUP DATABASE @v_Database TO DISK = @v_Path";
                var result = await Repository.NonQueryAsync(query, parameters: parameters);
                retVal = result > 0 ? true : false;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }

        private string BuildBackUpWithFileName(string filePath, string databaseName)
        {
            var fileName = databaseName + "--" + DateTime.Now.ToString("yyyy-MM-dd") + ".bak";
            return Path.Combine(filePath, fileName);
        }
        public async Task<bool> CreateFullBackupAsync(Server myServer, Database myDatabase, string folderPath)
        {
            bool retVal = false;
            await Task.Run(() =>
            {
                try
                {
                    Backup bkpDBFull = new Backup();
                    /* Specify whether you want to back up database or files or log */
                    bkpDBFull.Action = BackupActionType.Database;
                    /* Specify the name of the database to back up */
                    bkpDBFull.Database = myDatabase.Name;
                    /* You can take backup on several media type (disk or tape), here I am using
                     * File type and storing backup on the file system */
                    //bkpDBFull.Devices.AddDevice(@"D:\AdventureWorksFull.bak", DeviceType.File);
                    bkpDBFull.Devices.AddDevice($"{folderPath}\\{myDatabase.Name}{DateTime.Now.ToString("yyyy-MM-dd")}.bak", DeviceType.File);
                    bkpDBFull.BackupSetName = $"{myDatabase.Name} database Backup";
                    bkpDBFull.BackupSetDescription = $"{myDatabase.Name} database - Full Backup At {DateTime.Now}";
                    /* You can specify the expiration date for your backup data
                     * after that date backup data would not be relevant */
                    bkpDBFull.ExpirationDate = DateTime.Today.AddDays(30);
                    /* You can specify Initialize = false (default) to create a new 
                     * backup set which will be appended as last backup set on the media. You can
                     * specify Initialize = true to make the backup as first set on the mediuam and
                     * to overwrite any other existing backup sets if the all the backup sets have
                     * expired and specified backup set name matches with the name on the medium */
                    bkpDBFull.Initialize = false;
                    /* Wiring up events for progress monitoring */
                    bkpDBFull.PercentComplete += CompletionStatusInPercent;
                    bkpDBFull.Complete += Backup_Completed;
                    /* SqlBackup method starts to take back up
                     * You cab also use SqlBackupAsync method to perform backup 
                     * operation asynchronously */
                    bkpDBFull.SqlBackupAsync(myServer);
                    retVal = true;
                }
                catch (Exception ex)
                {
                    LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
                }
                finally
                {
                    if (myServer.ConnectionContext.IsOpen)
                        myServer.ConnectionContext.Disconnect();
                }
            });
            return retVal;
        }

        public async Task<bool> CreateDifferentialBackupAsync(Server myServer, Database myDatabase, string folderPath)
        {
            bool retVal = false;
            await Task.Run(() =>
            {
                try
                {
                    Backup bkpDBDifferential = new Backup();
                    /* Specify whether you want to back up database or files or log */
                    bkpDBDifferential.Action = BackupActionType.Database;
                    /* Specify the name of the database to back up */
                    bkpDBDifferential.Database = myDatabase.Name;
                    /* You can take backup on several media type (disk or tape), here I am using
                     * File type and storing backup on the file system */
                    bkpDBDifferential.Devices.AddDevice($"{folderPath}\\{myDatabase.Name}{DateTime.Now.ToString("yyyy-MM-dd")}.bak", DeviceType.File);
                    bkpDBDifferential.BackupSetName = $"{myDatabase.Name} database Backup";
                    bkpDBDifferential.BackupSetDescription = $"{myDatabase.Name} database - Differential Backup At {DateTime.Now}";
                    /* You can specify the expiration date for your backup data
                     * after that date backup data would not be relevant */
                    bkpDBDifferential.ExpirationDate = DateTime.Today.AddDays(30);
                    /* You can specify Initialize = false (default) to create a new 
                     * backup set which will be appended as last backup set on the media. You can
                     * specify Initialize = true to make the backup as first set on the mediuam and
                     * to overwrite any other existing backup sets if the all the backup sets have
                     * expired and specified backup set name matches with the name on the medium */
                    bkpDBDifferential.Initialize = false;
                    /* You can specify Incremental = false (default) to perform full backup
                     * or Incremental = true to perform differential backup since most recent
                     * full backup */
                    bkpDBDifferential.Incremental = true;
                    /* Wiring up events for progress monitoring */
                    bkpDBDifferential.PercentComplete += CompletionStatusInPercent;
                    bkpDBDifferential.Complete += Backup_Completed;
                    /* SqlBackup method starts to take back up
                     * You cab also use SqlBackupAsync method to perform backup 
                     * operation asynchronously */
                    bkpDBDifferential.SqlBackup(myServer);
                    retVal = true;
                }
                catch (Exception ex)
                {
                    LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
                }
                finally
                {
                    if (myServer.ConnectionContext.IsOpen)
                        myServer.ConnectionContext.Disconnect();
                }
            });
            return retVal;
        }

        public async Task<bool> RestoreDatabaseAsync(Server myServer, Database myDatabase, string folderPath)
        {
            bool retVal = false;
            await Task.Run(() =>
            {
                try
                {
                    Restore restoreDB = new Restore();
                    restoreDB.Database = myDatabase.Name;
                    /* Specify whether you want to restore database or files or log etc */
                    restoreDB.Action = RestoreActionType.Database;
                    restoreDB.Devices.AddDevice($"{folderPath}\\{myDatabase.Name}{DateTime.Now.ToString("yyyy-MM-dd")}.bak", DeviceType.File);
                    /* You can specify ReplaceDatabase = false (default) to not create a new image
                     * of the database, the specified database must exist on SQL Server instance.
                     * If you can specify ReplaceDatabase = true to create new database image 
                     * regardless of the existence of specified database with same name */
                    restoreDB.ReplaceDatabase = true;

                    /* If you have differential or log restore to be followed, you would need
                     * to specify NoRecovery = true, this will ensure no recovery is done after the 
                     * restore and subsequent restores are allowed. It means it will database
                     * in the Restoring state. */
                    restoreDB.NoRecovery = true;
                    /* Wiring up events for progress monitoring */
                    restoreDB.PercentComplete += CompletionStatusInPercent;
                    restoreDB.Complete += Restore_Completed;
                    /* SqlRestore method starts to restore database
                     * You cab also use SqlRestoreAsync method to perform restore 
                     * operation asynchronously */
                    restoreDB.SqlRestore(myServer);
                    retVal = true;
                }
                catch (Exception ex)
                {
                    LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
                }
                finally
                {
                    if (myServer.ConnectionContext.IsOpen)
                        myServer.ConnectionContext.Disconnect();
                }
            });
            return retVal;
        }

        #endregion

        #region [Events]

        private static void CompletionStatusInPercent(object sender, PercentCompleteEventArgs args)
        {
            //Console.Clear();
            //Console.WriteLine("Percent completed: {0}%.", args.Percent);
        }
        private static void Backup_Completed(object sender, ServerMessageEventArgs args)
        {
            Console.WriteLine("Hurray...Backup completed.");
            Console.WriteLine(args.Error.Message);
        }
        private static void Restore_Completed(object sender, ServerMessageEventArgs args)
        {
            Console.WriteLine("Hurray...Restore completed.");
            Console.WriteLine(args.Error.Message);
        }
        #endregion
    }
}
