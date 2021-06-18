using SmartSolutions.InventoryControl.Plugins.Repositories;
using System;
using System.Data.Common;

namespace SmartSolutions.InventoryControl.SQLiteCipher
{
    internal class SQLiteRepositoryTransaction : IRepositoryTransaction
    {
        private readonly DbConnection _connection;
        public DbConnection Connection => _connection;

        private readonly DbTransaction _transaction;
        public DbTransaction Transaction => _transaction;

        private readonly SQLiteRepository _repository;

        //private Semaphore _pool;

        private bool is_completed = false;

        internal SQLiteRepositoryTransaction(SQLiteRepository repository, DbConnection connection, DbTransaction transaction/*, Semaphore pool*/)
        {
            this._connection = connection;
            this._transaction = transaction;
            this._repository = repository;
            //_pool = pool;
        }

        public void Commit()
        {
            _transaction.Commit();
            _repository.EndTransaction();
            is_completed = true;
        }

        public void RollBack()
        {
            _transaction.Rollback();
            _repository.EndTransaction();
            is_completed = true;
        }

        public void Dispose()
        {
            try
            {
                if (!is_completed)
                {
                    _transaction.Rollback();
                    _repository.EndTransaction();
                    is_completed = true;
                }
                //_connection.Close();
                _transaction.Dispose();
                //_connection.Dispose();
                ////_pool?.Release();
            }
            catch (Exception)
            {
            }
        }
    }
}
