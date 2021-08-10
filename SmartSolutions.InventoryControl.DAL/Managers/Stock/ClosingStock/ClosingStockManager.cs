using SmartSolutions.InventoryControl.DAL.Models.Stock;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.LogUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Stock.ClosingStock
{
    [Export(typeof(IClosingStockManager)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class ClosingStockManager : BaseManager, IClosingStockManager
    {
        #region Private Members
        private readonly IRepository Repository;
        #endregion

        #region Constructor
        public ClosingStockManager()
        {
            Repository = GetRepository<ClosingStockModel>();
        }
        #endregion

        #region Public Methods
        public async Task<bool> AddClosingStockAsync(ClosingStockModel closingStock)
        {
            bool retVal = false;
            if (closingStock == null) return false;
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_ProductId"] = closingStock.Product?.Id;
                parameters["@v_WarehouseId"] = closingStock?.Warehouse?.Id;
                parameters["@v_Quantity"] = closingStock?.Quantity;
                parameters["@v_Price"] = closingStock?.Price;
                parameters["@v_Total"] = closingStock?.Total;
                parameters["@v_Description"] = closingStock?.Description;
                parameters["@v_IsActive"] = closingStock?.IsActive;
                parameters["@v_CreatedAt"] = closingStock.CreatedAt == null ? DateTime.Now : closingStock.CreatedAt;
                parameters["@v_CreatedBy"] = closingStock.CreatedBy == null ? DBNull.Value : (object)closingStock.CreatedBy;
                parameters["@v_UpdatedAt"] = closingStock.UpdatedAt == null ? DBNull.Value : (object)closingStock.UpdatedAt;
                parameters["@v_UpdatedBy"] = closingStock.UpdatedBy == null ? DBNull.Value : (object)closingStock.UpdatedBy;
                string query = @"INSERT INTO ClosingStock (ProductId,WarehouseId,Quantity,Price,Total,Description,IsActive,CreatedAt,CreatedBy,UpdatedAt,UpdatedBy)
                                                    VALUES(@v_ProductId,@v_WarehouseId,@v_Quantity,@v_Price,@v_Total,@v_Description,@v_IsActive,@v_CreatedAt,@v_UpdatedAt,@v_UpdatedBy)";
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
    }
}
