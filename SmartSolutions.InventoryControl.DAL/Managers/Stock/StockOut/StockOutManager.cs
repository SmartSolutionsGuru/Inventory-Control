using SmartSolutions.InventoryControl.DAL.Models.Inventory;
using SmartSolutions.InventoryControl.DAL.Models.Stock;
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

namespace SmartSolutions.InventoryControl.DAL.Managers.Stock.StockOut
{
    [Export(typeof(IStockOutManager)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class StockOutManager : BaseManager, IStockOutManager
    {
        #region Private Members
        private readonly IRepository Repository;
        #endregion

        #region Constructor
        public StockOutManager()
        {
            Repository = GetRepository<StockOutModel>();
        }
        #endregion

        #region Public Methods
        public async Task<bool> AddBulkStockOutAsync(List<StockOutModel> models)
        {
            bool retVal = false;
            try
            {
                if (models != null || models?.Count > 0)
                {
                    foreach (var model in models)
                    {
                       retVal =  await AddStockOutAsync(model);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }

        public async Task<bool> AddStockOutAsync(StockOutModel model)
        {
            bool retVal = false;
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_PartnerId"] = model.Partner?.Id;
                parameters["@v_SaleOrderId"] = model?.SaleOrder.Id;
                parameters["@v_SaleOrderDetailId"] = model?.SaleOrderDetail?.Id;
                parameters["@v_ProductId"] = model?.Product?.Id;
                parameters["@v_Quantity"] = model?.Quantity;
                parameters["@v_Price"] = model?.Price;
                parameters["@v_Description"] = string.IsNullOrEmpty(model?.Description) ? DBNull.Value : (object)model.Description;
                parameters["@v_IsActive"] = model.IsActive = true;
                parameters["@v_CreatedAt"] = model.CreatedAt == null ? DateTime.Now : model.CreatedAt;
                parameters["@v_CreatedBy"] = model.CreatedBy == null ? DBNull.Value : (object)model.CreatedBy;
                parameters["@v_UpdatedAt"] = model.UpdatedAt == null ? DBNull.Value : (object)model.UpdatedAt;
                parameters["@v_UpdatedBy"] = model.UpdatedBy == null ? DBNull.Value : (object)model.UpdatedBy;
                parameters["@v_WarehouseId"] = model.Warehouse?.Id == null ? DBNull.Value : (object)model.Warehouse?.Id;
                string query = @"INSERT INTO StockOut (PartnerId,SaleOrderId,SaleOrderDetailId,ProductId,Quantity,Price,Description,IsActive,CreatedAt,createdBy,UpdatedAt,UpdatedBy,WarehouseId)
                                                VALUES(@v_PartnerId,@v_SaleOrderId,@v_SaleOrderDetailId,@v_ProductId,@v_Quantity,@v_Price,v_Description,@v_IsActive,@v_CreatedAt,@v_CreatedBy,@v_UpdatedAt,@v_UpdatedBy,@v_WarehouseId)";
                var result = await Repository.NonQueryAsync(query, parameters: parameters);
                retVal = result > 0 ? true : false;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }

        public async Task<int?> GetStockInHandAsync(int? productId)
        {
            if (productId == null || productId == 0) return null;
            int? stockInHand = null;
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_productId"] = productId;
                string query = @" SELECT SUM(Total) AS StockInHand FROM StockIn WHERE ProductId = @v_productId - ISNULL((SELECT SUM(Total) FROM StockOut WHERE ProductId = @v_productId),0)";
                var values = await Repository.QueryAsync(query,parameters:parameters);
                if(values != null || values?.Count > 0)
                {
                    var value = values?.FirstOrDefault();
                    stockInHand = value?.GetValueFromDictonary("StockInHand")?.ToString()?.ToNullableInt();
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return stockInHand;
        }

        public async Task<bool> RemoveBulkStockOutAsync(List<StockOutModel> models)
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
      

        public async Task<bool> RemoveStockOutAsync(int? Id)
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
        #endregion
    }
}
