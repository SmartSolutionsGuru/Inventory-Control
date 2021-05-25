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
        Task<IEnumerable<ProductModel>> GetAllProductsAsync();
        Task<ProductModel> GetProductByIdAsync(int? Id);
    }
}
