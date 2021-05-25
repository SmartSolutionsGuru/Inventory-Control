using SmartSolutions.InventoryControl.DAL.Models.Product;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Product.ProductType
{
    public interface IProductTypeManager
    {
        Task<IEnumerable<ProductTypeModel>> GetAllProductsTypesAsync(string searchText = null);
        Task<ProductTypeModel> GetProductTypeById(int? Id);
        Task<bool> AddProductTypeAsync(ProductTypeModel model);
        Task<bool> RemoveProductType(int? Id);

    }
}
