using SmartSolutions.InventoryControl.DAL.Models.PurchaseOrder;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.DecimalsUtils;
using SmartSolutions.Util.DictionaryUtils;
using SmartSolutions.Util.EnumUtils;
using SmartSolutions.Util.LogUtils;
using SmartSolutions.Util.NumericUtils;
using SmartSolutions.Util.ObjectUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SmartSolutions.InventoryControl.DAL.Models.PurchaseOrder.PurchaseOrderModel;

namespace SmartSolutions.InventoryControl.DAL.Managers.Purchase
{
    [Export(typeof(IPurchaseOrderManager)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PurchaseOrderManager : BaseManager, IPurchaseOrderManager
    {
        #region Privats Mmbers
        private readonly IRepository Repository;
        #endregion

        #region Constructor
        public PurchaseOrderManager()
        {
            Repository = GetRepository<PurchaseOrderModel>();
        }
        #endregion
        public async Task<bool> CreatePurchaseOrderAsync(PurchaseOrderModel purchaseOrder)
        {
            bool retVal = false;
            if (purchaseOrder == null) return false;
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_PartnerId"] = purchaseOrder.Partner?.Id;
                parameters["@v_Status"] = purchaseOrder.Status;
                parameters["@v_Description"] = string.IsNullOrEmpty(purchaseOrder.Description) ? DBNull.Value : (object)purchaseOrder.Description;
                parameters["@v_ShippingId"] = purchaseOrder.Shipping?.Id == null ? 0 : purchaseOrder.Shipping?.Id;
                parameters["@v_SubTotal"] = purchaseOrder.SubTotal;
                parameters["@v_Discount"] = purchaseOrder.Discount;
                parameters["@v_GrandTotal"] = purchaseOrder.GrandTotal;
                parameters["@v_IsActive"] = purchaseOrder.IsActive = true;
                parameters["@v_CreatedAt"] = purchaseOrder.CreatedAt == null ? DateTime.Now : purchaseOrder.CreatedAt;
                parameters["@v_CreatedBy"] = purchaseOrder.CreatedBy == null ? DBNull.Value : (object)purchaseOrder.CreatedBy;
                parameters["@v_UpdatedAt"] = purchaseOrder.UpdatedAt == null ? DBNull.Value : (object)purchaseOrder.UpdatedAt;
                parameters["@v_UpdatedBy"] = purchaseOrder.UpdatedBy == null ? DBNull.Value : (object)purchaseOrder.UpdatedBy;

                string query = @"INSERT INTO PurchaseOrderMaster (PartnerId,Status,Description,ShippingId,SubTotal,Discount,GrandTotal,IsActive,CreatedAt,CreatedBy,UpdatedAt,UpdatedBy)
                                                            VALUES(@v_PartnerId,@v_Status,@v_Description,@v_ShippingId,@v_SubTotal,@v_Discount,@v_GrandTotal,@v_IsActive,@v_CreatedAt,@v_CreatedBy,@v_UpdatedAt,@v_UpdatedBy);";
                var result = await Repository.NonQueryAsync(query, parameters: parameters);
                retVal = result > 0 ? true : false;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }

        public async Task<int?> GetLastPurchaseOrderIdAsync()
        {
            int? lastRowId = null;
            try
            {
                string query = "SELECT last_insert_rowid()";
                var result = await Repository.QueryAsync(query);
                lastRowId = result?.FirstOrDefault().GetValueFromDictonary("last_insert_rowid()")?.ToString()?.ToNullableInt();
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return lastRowId;
        }

        public async Task<PurchaseOrderModel> GetPurchaseOrderAsync(int? Id)
        {
            var purchasOrder = new PurchaseOrderModel();
            if (Id == null || Id == 0) return null;
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_Id"] = Id;
                string query = @"SELECT * FROM PurchaseOrderMaster WHERE Id = @v_Id AND IsActive = 1";
                var values = await Repository.QueryAsync(query, parameters: parameters);
                var value = values.FirstOrDefault();
                if (values != null || values?.Count > 0)
                {
                    var purcahseOrder = new PurchaseOrderModel();
                    purcahseOrder.Id = value?.GetValueFromDictonary("Id").ToString()?.ToInt();
                    purcahseOrder.Status =value?.GetValueFromDictonary("Status")?.ToString().ToEnum<OrderStatus>() ?? OrderStatus.None;
                    purcahseOrder.SubTotal = Convert.ToDecimal(value?.GetValueFromDictonary("SubTotal")?.ToString());
                    purcahseOrder.GrandTotal = Convert.ToDecimal(value?.GetValueFromDictonary("Total")?.ToString());
                    purcahseOrder.Partner = new Models.BussinessPartner.BussinessPartnerModel
                    {
                        Id = value?.GetValueFromDictonary("Id")?.ToString().ToInt(),
                    }; 
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return purchasOrder;
        }
    }
}
