using SmartSolutions.InventoryControl.DAL.Models.Product;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Product.ProductSubType
{
    public interface IProductSubTypeManager
    {
        Task<IEnumerable<ProductSubTypeModel>> GetAllProductSubTypeAsync(int? Id);
        IEnumerable<ProductSubTypeModel> GetAllProductSubType(int? Id);
        Task<ProductSubTypeModel> GetProductSubTypeById(int? Id);
        Task<bool> AddProductSubTypeAsync(int? productId,ProductSubTypeModel model);
        Task<bool> RemoveProductSubTypeAsync(int? Id);
    }
}
