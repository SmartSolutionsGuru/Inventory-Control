using SmartSolutions.InventoryControl.DAL.Models.Product;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Product.ProductColor
{
    public interface IProductColorManager
    {
        Task<bool> AddProductColorAsync(ProductColorModel model);
        Task<IEnumerable<ProductColorModel>> GetProductAllColorsAsync();
        Task<ProductColorModel> GetProductColorByIdAsync(int Id);
        Task<ProductColorModel> UpdateProductColorAsync(ProductColorModel model);
        Task<bool> RemoveProductColorAsync(int? Id);
    }
}
