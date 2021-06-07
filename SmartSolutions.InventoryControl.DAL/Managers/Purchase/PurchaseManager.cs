using SmartSolutions.InventoryControl.DAL.Models.Purchase;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.DictionaryUtils;
using SmartSolutions.Util.LogUtils;
using SmartSolutions.Util.NumericUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Purchase
{
    [Export(typeof(IPurchaseManager)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class PurchaseManager : BaseManager, IPurchaseManager
    {
        #region Private Members
        private readonly IRepository Repository;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public PurchaseManager()
        {
            Repository = GetRepository<PurchaseModel>();
        }
        #endregion

        #region Public Methods
        public async Task<bool> AddPurchaseAsync(PurchaseModel model)
        {
            bool retVal = false;
            try
            {
                string query = string.Empty;
                await Repository.NonQueryAsync(query);
            }
            catch (Exception ex)
            {

                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }

        public Task<bool> AddPurchaseReturnAsync(PurchaseModel model)
        {
            throw new NotImplementedException();
        }

        public async Task<double> GetLastBalanceAsync(int? Id)
        {
            double balance = 0d;
            try
            {
                string query = string.Empty;
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_Id"] = Id;
                query =  @"SELECT * FROM PurchaseTransaction WHERE Id = @v_Id AND IsActive = 1 AND IsDeleted = 0";
                var values = await Repository.QueryAsync(query,parameters:parameters);
                if(values != null)
                {
                    foreach (var value in values)
                    {
                        value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                        value?.GetValueFromDictonary("Total")?.ToString()?.ToInt();
                        value?.GetValueFromDictonary("Balnce")?.ToString()?.ToInt();
                    }
                }

            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return balance;
        }

        public async Task<int> GetLastTransationIdAsync()
        {
            int? Id = 0;
            try
            {
                string query = @"SELECT MAX(ROWID) FROM PurchaseTransaction";
               var values =  await Repository.QueryAsync(query);
                if(values != null)
                {
                    var value = values.FirstOrDefault();
                    Id = value?.GetValueFromDictonary("MAX(ROWID)")?.ToString()?.ToInt();
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return Id.Value;
        }
        #endregion
    }
}
