using System;
using System.Data;
using System.Data.Common;

namespace SmartSolutions.InventoryControl.SQLiteCipher
{
    internal class SmartSQLiteCommand : DbCommand
    {
        internal SQLite.SQLiteCommand command { get; private set; }
        internal SmartSQLiteConnection connection { get; private set; }

        public override string CommandText { get => command?.CommandText; set { command = connection?.connection?.CreateCommand(value); } }
        public override int CommandTimeout { get => 10000; set { } }
        public override CommandType CommandType { get => CommandType.Text; set { } }
        public override bool DesignTimeVisible { get => false; set { } }
        public override UpdateRowSource UpdatedRowSource { get => UpdateRowSource.None; set { } }
        protected override DbConnection DbConnection { get => connection; set { } }

        private DbParameterCollection m_DbParameterCollection = new SQLiteParameterCollection();
        protected override DbParameterCollection DbParameterCollection => m_DbParameterCollection;

        protected override DbTransaction DbTransaction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public SmartSQLiteCommand(SmartSQLiteConnection connection)
        {
            this.connection = connection;
        }

        public SmartSQLiteCommand(SmartSQLiteConnection connection, SQLite.SQLiteCommand command)
        {
            this.connection = connection;
            this.command = command;
        }

        public override void Cancel()
        {
            throw new NotSupportedException();
        }

        public override int ExecuteNonQuery()
        {
            foreach (DbParameter param in Parameters)
            {
                if (param.Value == DBNull.Value)
                    this.command.Bind(param.ParameterName, null);
                else
                    this.command.Bind(param.ParameterName, param.Value);
            }

            return command.ExecuteNonQuery();
        }

        public override object ExecuteScalar()
        {
            foreach (DbParameter param in Parameters)
            {
                if (param.Value == DBNull.Value)
                    this.command.Bind(param.ParameterName, null);
                else
                    this.command.Bind(param.ParameterName, param.Value);
            }

            return command.ExecuteScalar();
        }

        public override void Prepare()
        {
            throw new NotSupportedException();
        }

        protected override DbParameter CreateDbParameter()
        {
            return new SQLiteDbParameter();
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            foreach (DbParameter param in Parameters)
            {
                if (param.Value == DBNull.Value)
                    this.command.Bind(param.ParameterName, null);
                else
                    this.command.Bind(param.ParameterName, param.Value);
            }

            return new SQLiteDbDataReader(this.command.ExecuteQuery());
        }
    }
}
