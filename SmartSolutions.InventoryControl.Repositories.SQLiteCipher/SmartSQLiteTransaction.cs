using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace SmartSolutions.InventoryControl.SQLiteCipher
{
    internal class SmartSQLiteTransaction : DbTransaction
    {
        public override IsolationLevel IsolationLevel => IsolationLevel.Unspecified;

        private readonly SmartSQLiteConnection _dbConnection;
        protected override DbConnection DbConnection => _dbConnection;

        public SmartSQLiteTransaction(SmartSQLiteConnection connection)
        {
            this._dbConnection = connection;
            connection.connection.BeginTransaction();
        }

        public override void Commit()
        {
            _dbConnection.connection.Commit();
            _dbConnection.EndTransaction();
        }

        public override void Rollback()
        {
            _dbConnection.connection.Rollback();
            _dbConnection.EndTransaction();
        }
    }
}
