using SmartSolutions.InventoryControl.DAL.Models.Sales;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.DictionaryUtils;
using SmartSolutions.Util.EnumUtils;
using SmartSolutions.Util.LogUtils;
using SmartSolutions.Util.NumericUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SmartSolutions.InventoryControl.DAL.Models.Sales.SaleOrderModel;

namespace SmartSolutions.InventoryControl.DAL.Managers.Sale
{
    [Export(typeof(ISaleOrderManager)),PartCreationPolicy(CreationPolicy.Shared)]
    public class SaleOrderManager : BaseManager, ISaleOrderManager
    {
        #region Privats Mmbers
        private readonly IRepository Repository;
        #endregion

        #region Constructor
        public SaleOrderManager()
        {
            Repository = GetRepository<SaleOrderModel>();
        }
        #endregion

        #region Methods
        public async Task<bool> CreateSaleOrderAsync(SaleOrderModel saleOrder)
        {
            bool retVal = false;
            if (saleOrder == null) return false;
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_PartnerId"] = saleOrder.SalePartner?.Id;
                parameters["@v_Status"] = saleOrder.Status;
                parameters["@v_Description"] = string.IsNullOrEmpty(saleOrder.Description) ? DBNull.Value : (object)saleOrder.Description;
                parameters["@v_ShippingId"] = saleOrder.Shipping?.Id == null ? 0 : saleOrder.Shipping?.Id;
                parameters["@v_SubTotal"] = saleOrder.SubTotal;
                parameters["@v_Discount"] = saleOrder.Discount;
                parameters["@v_GrandTotal"] = saleOrder.GrandTotal;
                parameters["@v_IsActive"] = saleOrder.IsActive = true;
                parameters["@v_CreatedAt"] = saleOrder.CreatedAt == null ? DateTime.Now : saleOrder.CreatedAt;
                parameters["@v_CreatedBy"] = saleOrder.CreatedBy == null ? DBNull.Value : (object)saleOrder.CreatedBy;
                parameters["@v_UpdatedAt"] = saleOrder.UpdatedAt == null ? DBNull.Value : (object)saleOrder.UpdatedAt;
                parameters["@v_UpdatedBy"] = saleOrder.UpdatedBy == null ? DBNull.Value : (object)saleOrder.UpdatedBy;

                string query = @"INSERT INTO SaleOrderMaster (PartnerId,Status,Description,ShippingId,SubTotal,Discount,GrandTotal,IsActive,CreatedAt,CreatedBy,UpdatedAt,UpdatedBy)
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

        public async Task<int?> GetLastSaleOrderIdAsync()
        {
            int? lastRowId = null;
            try
            {
                //string query = "SELECT last_insert_rowid()";
                string query = @"SELECT IDENT_CURRENT('SaleOrderMaster') AS Id";
                var result = await Repository.QueryAsync(query);
                if (result != null || result?.Count > 0)
                    lastRowId = result?.FirstOrDefault().GetValueFromDictonary("Id")?.ToString()?.ToInt() ?? 0;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return lastRowId;
        }

        public async Task<SaleOrderModel> GetSaleOrderAsync(int? Id)
        {
            var saleOrder = new SaleOrderModel();
            if (Id == null || Id == 0) return null;
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_Id"] = Id;
                string query = @"SELECT * FROM SaleOrderMaster WHERE Id = @v_Id AND IsActive = 1";
                var values = await Repository.QueryAsync(query, parameters: parameters);
                var value = values.FirstOrDefault();
                if (values != null || values?.Count > 0)
                {
                    //var saleOrder = new SaleOrderModel();
                    saleOrder.Id = value?.GetValueFromDictonary("Id").ToString()?.ToInt();
                    saleOrder.Status = value?.GetValueFromDictonary("Status")?.ToString().ToEnum<OrderStatus>() ?? OrderStatus.None;
                    saleOrder.SubTotal = Convert.ToDecimal(value?.GetValueFromDictonary("SubTotal")?.ToString());
                    saleOrder.GrandTotal = Convert.ToDecimal(value?.GetValueFromDictonary("Total")?.ToString());
                    saleOrder.SalePartner = new Models.BussinessPartner.BussinessPartnerModel
                    {
                        Id = value?.GetValueFromDictonary("Id")?.ToString().ToInt(),
                    };
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return saleOrder;
        }
        #endregion

    }
}
