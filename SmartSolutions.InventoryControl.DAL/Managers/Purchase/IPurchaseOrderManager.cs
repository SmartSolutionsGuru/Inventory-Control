using SmartSolutions.InventoryControl.DAL.Models.PurchaseOrder;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Purchase
{
    public interface IPurchaseOrderManager
    {
        Task<bool> CreatePurchaseOrderAsync(PurchaseOrderModel purchaseOrder);
        Task<int?> GetLastPurchaseOrderIdAsync();
        Task<PurchaseOrderModel> GetPurchaseOrderAsync(int? Id);
    }
}
