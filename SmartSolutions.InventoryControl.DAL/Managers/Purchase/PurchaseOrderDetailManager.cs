using SmartSolutions.InventoryControl.DAL.Models.PurchaseOrder;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.LogUtils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Purchase
{
    public class PurchaseOrderDetailManager : BaseManager, IPurchaseOrderDetailManager
    {
        #region Private Members
        private readonly IRepository Repository;
        #endregion

        #region Constructor
        public PurchaseOrderDetailManager()
        {
            Repository = GetRepository<PurchaseOrderDetailModel>();
        }
        #endregion
        public async Task<bool> AddPurchaseOrderBulkDetailAsync(List<PurchaseOrderDetailModel> orderDetails)
        {
            bool retVal = false;
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();

                string query = @"";
                var result =  await Repository.NonQueryAsync(query, parameters:parameters);
                retVal = result > 0 ? true : false;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }

        public async Task<bool> AddPurchaseOrderDetailAsync(PurchaseOrderDetailModel orderDetail)
        {
            bool retVal = false;
            try
            {
                await Repository.NonQueryAsync();
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }
    }
}
