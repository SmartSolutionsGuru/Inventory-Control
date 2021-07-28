using SmartSolutions.InventoryControl.DAL.Models.Warehouse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Warehouse
{
    public interface IWarehouseManager
    {
        Task<IEnumerable<WarehouseModel>> GetAllWarehousesAsync();
        Task<bool> SaveWarehouseAsync(WarehouseModel warehouse);
    }
}
