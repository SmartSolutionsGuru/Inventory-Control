using SmartSolutions.InventoryControl.DAL.Models.Inventory;
using SmartSolutions.InventoryControl.DAL.Models.Stock;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Stock.StockIn
{
    public interface IStockInManager
    {
        /// <summary>
        /// adding Purchase to StockIn Table
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> AddStockInAsync(StockInModel model);
        /// <summary>
        /// adding Bulk Purchase to StockIn Table
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> AddBulkStockInAsync(List<StockInModel> models);
        /// <summary>
        /// Update Inventory According to Sales
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> RemoveStockInAsync(int? Id);
        /// <summary>
        /// Update Invoice According To Invoice
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        Task<bool> RemoveBulkStockInAsync(List<StockInModel> models);
    }
}
