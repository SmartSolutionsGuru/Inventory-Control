using SmartSolutions.InventoryControl.DAL.Models.Warehouse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Warehouse
{
    public interface IWarehouseManager
    {
        Task<IEnumerable<WarehouseModel>> GetAllWarehousesAsync();
        Task<bool> SaveWarehouseAsync(WarehouseModel warehouse);
        /// <summary>
        /// verifies that with this name Already Exist Or Not
        /// </summary>
        /// <param name="warehouseName"></param>
        /// <returns>true if Exist</returns>
        Task<bool> IsWarehouseNameAlreadyExist(string warehouseName);
    }
}
