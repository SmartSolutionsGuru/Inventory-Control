using SmartSolutions.InventoryControl.DAL.Models.Stock;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.LogUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Stock.StockIn
{
    [Export(typeof(IStockInManager)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class StockInManager : BaseManager, IStockInManager
    {
        #region Private Members
        private readonly IRepository Repository;
        #endregion

        #region Constructor
        public StockInManager()
        {
            Repository = GetRepository<StockInModel>();
        }
        #endregion

        #region ADD
        public async Task<bool> AddBulkStockInAsync(List<StockInModel> models)
        {
            bool retVal = false;
            try
            {
                if (models != null || models?.Count > 0)
                {
                    foreach (var model in models)
                    {
                        retVal = await AddStockInAsync(model);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }

        public async Task<bool> AddStockInAsync(StockInModel model)
        {
            bool retVal = false;
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_PartnerId"] = model.Partner?.Id;
                parameters["@v_PurchaseOrderDetailId"] = model?.PurchaseOrderDetail?.Id;
                parameters["@v_ProductId"] = model?.Product?.Id;
                parameters["@v_Quantity"] = model?.Quantity;
                parameters["@v_PurchaseOrderId"] = model?.PurchaseOrder?.Id;
                parameters["@v_PurchaseInvoiceId"] = model?.PurchaseInvoiceId;
                parameters["@v_Price"] = model?.Price;
                parameters["@v_Total"] = model.Total;
                parameters["@v_Description"] = string.IsNullOrEmpty(model?.Description) ? DBNull.Value : (object)model.Description;
                parameters["@v_IsActive"] = model.IsActive = true;
                parameters["@v_CreatedAt"] = model.CreatedAt == null ? DateTime.Now : model.CreatedAt;
                parameters["@v_CreatedBy"] = model.CreatedBy == null ? AppSettings.LoggedInUser.DisplayName : (object)model.CreatedBy;
                parameters["@v_UpdatedAt"] = model.UpdatedAt == null ? DBNull.Value : (object)model.UpdatedAt;
                parameters["@v_UpdatedBy"] = model.UpdatedBy == null ? DBNull.Value : (object)model.UpdatedBy;
                parameters["@v_WarehouseId"] = model.Warehouse?.Id == null ? DBNull.Value : (object)model.Warehouse?.Id;
                string query = @"INSERT INTO StockIn (PartnerId,PurchaseOrderDetailId,ProductId,Quantity,PurchaseOrderId,PurchaseInvoiceId,Price,Total,Description,IsActive,CreatedAt,CreatedBy,UpdatedAt,UpdatedBy,WarehouseId)
                                                VALUES(@v_PartnerId,@v_PurchaseOrderDetailId,@v_ProductId,@v_Quantity,@v_PurchaseOrderId,@v_PurchaseInvoiceId,@v_Price,@v_Total,@v_Description,@v_IsActive,@v_CreatedAt,@v_CreatedBy,@v_UpdatedAt,@v_UpdatedBy,@v_WarehouseId)";
                var result = await Repository.NonQueryAsync(query: query, parameters: parameters);
                retVal = result > 0 ? true : false;

            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }
        #endregion

        #region Remove
        public async Task<bool> RemoveBulkStockInAsync(List<StockInModel> models)
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

        public async Task<bool> RemoveStockInAsync(int? Id)
        {
            bool retVal = false;
            try
            {
                if (Id == null || Id == 0) return false;
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_Id"] = Id;
                string query = @"UPDATE Table StockIn SET IsActive = 0 WHERE Id = @v_Id";
                var result = await Repository.NonQueryAsync(query, parameters: parameters);
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
