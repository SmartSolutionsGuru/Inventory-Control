using SmartSolutions.InventoryControl.DAL.Models.Transaction;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.LogUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Transaction
{
    [Export(typeof(ITransactionManager)),PartCreationPolicy(CreationPolicy.Shared)]
    public class TransactionManager : BaseManager, ITransactionManager
    {
        #region Private Members
        private readonly IRepository Repository;
        #endregion

        #region Costructor
        [ImportingConstructor]
        public TransactionManager()
        {
            Repository = GetRepository<TransactionModel>();
        }
        #endregion

        #region Public Members
        public async Task<bool> SaveTransactionAsync(TransactionModel transaction)
        {
            bool retVal = false;
            try
            {
                if (transaction == null) return false;
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                string query = string.Empty;
                await Repository.QueryAsync(query, parameters: parameters);
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }
        #endregion
    }
}
