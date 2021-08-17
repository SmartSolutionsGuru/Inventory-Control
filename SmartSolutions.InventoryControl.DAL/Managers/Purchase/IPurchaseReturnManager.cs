using SmartSolutions.InventoryControl.DAL.Models.PurchaseOrder;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Purchase
{
    public interface IPurchaseReturnManager
    {
        Task<bool> AddPurchaseReturnAsync(PurchaseReturnInvoiceModel returnInvoice);
    }
}
