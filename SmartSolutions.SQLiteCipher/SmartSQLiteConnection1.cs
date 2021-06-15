using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace SmartSolutions.InventoryControl.SQLiteCipher
{
    internal class SmartSQLiteConnection : DbConnection
    {
        internal SQLite.SQLiteConnection connection { get; private set; }
        internal SmartSQLiteTransaction ActiveTransaction { get; set; }
        internal bool IsInTransaction => connection?.IsInTransaction == true;

        public override string ConnectionString { get => connection?.DatabasePath; set { } }

        public override string Database => "main";

        public override string DataSource => connection?.DatabasePath;

        public override string ServerVersion => connection?.LibVersionNumber.ToString();

        public override ConnectionState State => connection == null ? ConnectionState.Closed : ConnectionState.Open;

        public SmartSQLiteConnection(SQLite.SQLiteConnection connection)
        {
            this.connection = connection;

            this.connection.BindFunction("REGEXP", 2, (args) =>
            {
                return System.Text.RegularExpressions.Regex.IsMatch(Convert.ToString(args[1]), Convert.ToString(args[0]));
            });
        }

        public override void ChangeDatabase(string databaseName)
        {
            throw new NotSupportedException();
        }

        public override void Close()
        {
            connection.Close();
        }

        public override void Open()
        {
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return ActiveTransaction = new SmartSQLiteTransaction(this);
        }

        internal void EndTransaction()
        {
            ActiveTransaction = null;
        }

        protected override DbCommand CreateDbCommand()
        {
            return new SmartSQLiteCommand(this);
        }
    }
}
