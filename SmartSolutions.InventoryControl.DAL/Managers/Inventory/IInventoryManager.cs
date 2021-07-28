using SmartSolutions.InventoryControl.DAL.Models.Inventory;
using SmartSolutions.InventoryControl.DAL.Models.Product;
using SmartSolutions.InventoryControl.DAL.Models.Stock;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Inventory
{
    public interface IInventoryManager
    {
        /// <summary>
        /// Gets the Last Row Id From Table
        /// </summary>
        /// <returns></returns>
        Task<int> GetLastTransationIdAsync();
        Task<double> GetLastBalanceAsync(int? Id);
        /// <summary>
        /// Get the last Purchase Price of Selected Product 
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="productColorId"></param>
        /// <param name="productSizeId"></param>
        /// <returns></returns>
        Task<double> GetProductPurchasePrice(int? productId,int? productColorId,int? productSizeId);
        /// <summary>
        /// adding Purchase to Inventory
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> AddInventoryAsync(InventoryModel model);
        Task<bool> AddBulkInventoryAsync(List<InventoryModel> models);
        /// <summary>
        /// Update Inventory According to Sales
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> RemoveInventoryAsync(InventoryModel model);
        /// <summary>
        /// Update Invoice According To Invoice
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        Task<bool> RemoveBulkInventoryAsync(List<InventoryModel> models);
        Task<bool> AddInventoryReturnAsync(InventoryModel model);
        /// <summary>
        /// Gets the Last Stock In Hand of Specific Product
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<InventoryModel> GetLastStockInHandAsync(ProductModel product,ProductColorModel color,ProductSizeModel size);
    }
}
