using SmartSolutions.InventoryControl.DAL.Models.PurchaseOrder;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.LogUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Purchase
{
    [Export(typeof(IPurchaseReturnManager)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PurchaseReturnManager : BaseManager, IPurchaseReturnManager
    {
        #region Private Members
        private readonly IRepository Repository;
        #endregion

        #region Constructor
        public PurchaseReturnManager()
        {
            Repository = GetRepository<PurchaseReturnInvoiceModel>();
        }
        #endregion

        #region [Methods]      
        /// <summary>
        /// Create Purchase Return Invoice 
        /// </summary>
        /// <param name="returnInvoice"></param>
        /// <returns></returns>
        public async Task<bool> AddPurchaseReturnAsync(PurchaseReturnInvoiceModel returnInvoice)
        {
            bool retVal = false;
            try
            {
                string query = string.Empty;
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_ProductId"] = returnInvoice.Product?.Id;
                parameters["@v_PurchaseReturnGuid"] = returnInvoice.PurchaseReturnGuid;
                parameters["@v_PartnerId"] = returnInvoice.Partner?.Id;
                parameters["@v_PurchaseInvoiceId"] = returnInvoice.PurchaseInvoiceId;
                parameters["@v_Total"] = returnInvoice.Total;
                parameters["@v_Description"] = returnInvoice.Description == null ? DBNull.Value : (object)returnInvoice.Description;
                parameters["@v_IsActive"] = returnInvoice.IsActive = true;
                parameters["@v_CreatedAt"] = returnInvoice.CreatedAt == null ? DateTime.Now : returnInvoice.CreatedAt;
                parameters["@v_CreatedBy"] = returnInvoice.CreatedBy == null ? DBNull.Value : (object)returnInvoice.CreatedBy;
                parameters["@v_UpdatedAt"] = returnInvoice.UpdatedAt == null ? DBNull.Value : (object)returnInvoice.UpdatedAt;
                parameters["@v_UpdatedBy"] = returnInvoice.UpdatedBy == null ? DBNull.Value : (object)returnInvoice.UpdatedBy;
                query = @"INSERT INTO PurchaseReturnInvoice(ProductId,PurchaseReturnGuid,PartnerId,PurchaseInvoiceId,Total,Description,IsActive,CreatedAt,CreatedBy,UpdatedAt,UpdatedBy)
                                                     VALUES(@v_ProductId,@v_PurchaseReturnGuid,@v_PartnerId,@v_PurchaseInvoiceId,@v_Total,@v_Description,@v_IsActive,@v_CreatedAt,@v_CreatedBy,@v_UpdatedAt,@v_UpdatedBy)";
                var result = await Repository.NonQueryAsync(query:query,parameters:parameters);
                retVal = result > 0 ? true : false;
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
