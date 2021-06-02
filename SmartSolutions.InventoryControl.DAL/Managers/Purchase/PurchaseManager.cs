using SmartSolutions.InventoryControl.DAL.Models;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.LogUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
        #endregion
    }
}
