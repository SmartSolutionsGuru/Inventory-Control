using SmartSolutions.InventoryControl.DAL.Models.Sales;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.DictionaryUtils;
using SmartSolutions.Util.LogUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Sale
{
    /// <summary>
    /// Class That  hold the Sale Order Details
    /// </summary>
    
    [Export(typeof(ISaleOrderDetailManager)),PartCreationPolicy(CreationPolicy.Shared)]
    public class SaleOrderDetailManager : BaseManager, ISaleOrderDetailManager
    {
        #region Privats Mmbers
        private readonly IRepository Repository;
        #endregion

        #region Constructor
        public SaleOrderDetailManager()
        {
            Repository = GetRepository<SaleOrderModel>();
        }
        #endregion

        #region Methods
        public async Task<bool> AddSaleOrderBulkDetailAsync(List<SaleOrderDetailModel> orderDetails)
        {
            bool retVal = false;
            try
            {
                if (orderDetails != null || orderDetails?.Count > 0)
                {
                    foreach (var orderDetail in orderDetails)
                    {
                        retVal = await AddSaleOrderDetailAsync(orderDetail);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }

        public async Task<bool> AddSaleOrderDetailAsync(SaleOrderDetailModel orderDetail)
        {
            bool retVal = false;
            if (orderDetail == null) return false;
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_SaleOrderId"] = orderDetail.SaleOrder?.Id ?? 0;
                parameters["@v_ProductId"] = orderDetail.Product?.Id ?? 0;
                parameters["@v_Description"] = string.IsNullOrEmpty(orderDetail.Description) ? DBNull.Value : (object)orderDetail.Description;
                parameters["@v_Price"] = orderDetail.Price;
                parameters["@v_Quantity"] = orderDetail.Quantity;
                parameters["@v_Discount"] = orderDetail.Discount == null ? 0 : orderDetail?.Discount;
                parameters["@v_Total"] = orderDetail.Total;
                parameters["@v_IsActive"] = orderDetail.IsActive ?? true;
                parameters["@v_CreatedAt"] = orderDetail.CreatedAt == null ? DateTime.Now : orderDetail.CreatedAt;
                parameters["@v_CreatedBy"] = orderDetail.CreatedBy == null ? DBNull.Value : (object)orderDetail.CreatedBy;
                parameters["@v_UpdatedAt"] = orderDetail.UpdatedAt == null ? DBNull.Value : (object)orderDetail.UpdatedAt;
                parameters["@v_UpdatedBy"] = orderDetail.UpdatedBy == null ? DBNull.Value : (object)orderDetail.UpdatedBy;
                string query = @"INSERT INTO SaleOrderDetail (SaleOrderId,ProductId,Description,Price,Quantity,Discount,Total,IsActive,CreatedAt,CreatedBy,UpdatedAt,UpdatedBy)
                                                            VALUES(@v_SaleOrderId,@v_ProductId,@v_Description,@v_Price,@v_Quantity,@v_Discount,@v_Total,@v_IsActive,@v_CreatedAt,@v_CreatedBy,@v_UpdatedAt,@v_UpdatedBy)";
                var result = await Repository.NonQueryAsync(query, parameters: parameters);
                retVal = result > 0 ? true : false;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }

        public async Task<List<string>> GetOrderDetailIdByOrderIdAsync(int? orderId)
        {
            if (orderId == null || orderId == 0) return null;
            var orderDetailIds = new List<string>();
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_orderId"] = orderId;
                string query = @"SELECT Id FROM SaleOrderDetail WHERE SaleOrderId = @v_orderId";
                var values = await Repository.QueryAsync(query, parameters: parameters);
                if (values != null || values?.Count > 0)
                {
                    foreach (var value in values)
                    {
                        string Id = value?.GetValueFromDictonary("Id")?.ToString();
                        orderDetailIds.Add(Id);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return orderDetailIds;
        }
        #endregion


    }
}
