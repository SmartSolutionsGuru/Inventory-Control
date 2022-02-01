using SmartSolutions.InventoryControl.DAL.Models.Inventory;
using SmartSolutions.InventoryControl.DAL.Models.Product;
using SmartSolutions.InventoryControl.DAL.Models.Stock;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Stock.StockOut
{
    public interface IStockOutManager
    {
        /// <summary>
        /// adding Purchase to StockIn Table
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> AddStockOutAsync(StockOutModel model);
        /// <summary>
        /// adding Bulk Purchase to StockIn Table
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> AddBulkStockOutAsync(List<StockOutModel> models);
        /// <summary>
        /// Update Inventory According to Sales
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> RemoveStockOutAsync(int? Id);
        /// <summary>
        /// Update Stock In hand Level According To Sale Invoice
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        Task<bool> RemoveBulkStockOutAsync(List<StockOutModel> models);
        /// <summary>
        /// Get the Latest Stock In Hand Product
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        Task<int?> GetStockInHandAsync(int? productId);
    }
}
