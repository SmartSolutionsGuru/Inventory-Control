using SmartSolutions.InventoryControl.DAL.Models.Purchase;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Purchase
{
    public interface IPurchaseManager
    {
        /// <summary>
        /// Gets the Last Row Id From Table
        /// </summary>
        /// <returns></returns>
        Task<int> GetLastTransationIdAsync();
        Task<double> GetLastBalanceAsync(int? Id);
        Task<bool> AddPurchaseAsync(PurchaseModel model);
        Task<bool> AddPurchaseReturnAsync(PurchaseModel model);
    }
}
