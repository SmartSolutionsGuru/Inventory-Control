using SmartSolutions.InventoryControl.DAL.Models.Stock;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Stock.ClosingStock
{
    public interface IClosingStockManager
    {
        Task<bool> AddClosingStockAsync(ClosingStockModel closingStock);
    }
}
