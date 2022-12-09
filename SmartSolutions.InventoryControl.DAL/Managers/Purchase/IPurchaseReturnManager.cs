using SmartSolutions.InventoryControl.DAL.Models.PurchaseOrder;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Purchase
{
    public interface IPurchaseReturnManager
    {
        /// <summary>
        /// Create Purchase Return Invoice
        /// </summary>
        /// <param name="returnInvoice"> get the object of Purchase return Invoice</param>
        /// <returns>return if Invoice is saved in Db Or not</returns>
        Task<bool> AddPurchaseReturnAsync(PurchaseReturnInvoiceModel returnInvoice);
    }
}
