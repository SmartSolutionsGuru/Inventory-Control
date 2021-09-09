using SmartSolutions.InventoryControl.DAL.Models.Product;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Product
{
    public interface IProductManager
    {
        Task<bool> AddProductAsync(ProductModel model);
        Task<bool> UpdateProductAsync(ProductModel product);
        Task<bool> RemoveProductAsync(int? Id);
        Task<IEnumerable<ProductModel>> GetAllProductsAsync(string serachText = null);
        /// <summary>
        /// For Sale Purpose Only Display Product 
        /// Available in Stock
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ProductModel>> GetAllProductsAvailableInStockAsync();
        /// <summary>
        /// Get All Products Who are Purchased Or Sell By This Partner
        /// </summary>
        /// <param name="partnerId"></param>
        /// <returns></returns>
        Task<IEnumerable<ProductModel>> GetAllProductsPurchasedByPartnerAsync(int? partnerId);
        Task<ProductModel> GetProductByIdAsync(int? Id);
        Task<ProductModel> GetLastAddedProduct();
    }
}
