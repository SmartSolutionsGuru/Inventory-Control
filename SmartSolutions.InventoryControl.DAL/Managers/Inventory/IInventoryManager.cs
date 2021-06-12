using SmartSolutions.InventoryControl.DAL.Models.Inventory;
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
        /// adding Purchase to Inventory
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> AddInventoryAsync(InventoryModel model);
        Task<bool> AddBulkInventoryAsync(List<InventoryModel> models);
        Task<bool> AddInventoryReturnAsync(InventoryModel model);
    }
}
