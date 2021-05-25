using SmartSolutions.InventoryControl.DAL.Models.Product;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Product.ProductSize
{
    public interface IProductSizeManager
    {
        Task<IEnumerable<ProductSizeModel>> GetProductAllSizeAsync();
        Task<ProductSizeModel> GetProductSizeAsync(int? Id);
        Task<bool> AddProductSizeAsync(ProductSizeModel model);
        Task<bool> UpdateProductSizeAsync(ProductSizeModel model);
        Task<bool> RemoveProductSizeAsync(int? Id);

    }
}
