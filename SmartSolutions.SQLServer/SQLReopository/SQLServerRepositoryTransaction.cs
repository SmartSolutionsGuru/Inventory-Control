using SmartSolutions.InventoryControl.Plugins.Repositories;
using System.Data.Common;

namespace SmartSolutions.SQLServer.SQLReopository
{
    public class SQLServerRepositoryTransaction  : IRepositoryTransaction
    {
        private readonly DbConnection _connection;
        public DbConnection Connection => _connection;

        private readonly DbTransaction _transaction;
        public DbTransaction Transaction => _transaction;

        internal SQLServerRepositoryTransaction(DbConnection connection, DbTransaction transaction)
        {
            this._connection = connection;
            this._transaction = transaction;
        }

        public void Commit()
        {
            _transaction.Commit();
        }

        public void RollBack()
        {
            _transaction.Rollback();
        }

        public void Dispose()
        {
            _connection.Close();
            _transaction.Dispose();
            _connection.Dispose();
        }
    }
}
