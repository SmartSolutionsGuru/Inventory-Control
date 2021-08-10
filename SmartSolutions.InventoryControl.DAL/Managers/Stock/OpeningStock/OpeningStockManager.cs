using SmartSolutions.InventoryControl.DAL.Models.Stock;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.LogUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Stock.OpeningStock
{
    [Export(typeof(IOpeningStockManager)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class OpeningStockManager : BaseManager, IOpeningStockManager
    {
        #region Properties
        private readonly IRepository Repository;
        #endregion

        #region Constructor
        public OpeningStockManager()
        {
            Repository = GetRepository<OpeningStockModel>();
        }
        #endregion

        #region Public Methods
        public async Task<bool> AddOpeningStockAsync(OpeningStockModel openingStock)
        {
            if (openingStock == null) return false;
            bool retVal = false;
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_ProductId"] = openingStock.Product?.Id;
                parameters["@v_WarehouseId"] = openingStock?.Warehouse?.Id;
                parameters["@v_Quantity"] = openingStock?.Quantity;
                parameters["@v_Price"] = openingStock?.Price;
                parameters["@v_Total"] = openingStock?.Total;
                parameters["@v_Description"] = openingStock.Description == null ? DBNull.Value : (object)openingStock.Description;
                parameters["@v_IsActive"] = openingStock.IsActive = true;
                parameters["@v_CreatedAt"] = openingStock.CreatedAt == null ? DateTime.Now : openingStock.CreatedAt;
                parameters["@v_CreatedBy"] = openingStock.CreatedBy == null ? DBNull.Value : (object)openingStock.CreatedBy;
                parameters["@v_UpdatedAt"] = openingStock.UpdatedAt == null ? DBNull.Value : (object)openingStock.UpdatedAt;
                parameters["@v_UpdatedBy"] = openingStock.UpdatedBy == null ? DBNull.Value : (object)openingStock.UpdatedBy;
                string query = @"INSERT INTO OpeningStock (ProductId,WarehouseId,Quantity,Price,Total,Description,IsActive,CreatedAt,CreatedBy,UpdatedAt,UpdatedBy)
                                                    VALUES(@v_ProductId,@v_WarehouseId,@v_Quantity,@v_Price,@v_Total,@v_Description,@v_IsActive,@v_CreatedAt,@v_CreatedBy,@v_UpdatedAt,@v_UpdatedBy)";
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
