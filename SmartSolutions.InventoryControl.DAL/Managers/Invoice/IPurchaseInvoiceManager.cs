using SmartSolutions.InventoryControl.DAL.Models.PurchaseOrder;
using System;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Invoice
{
    public interface IPurchaseInvoiceManager
    {
        /// <summary>
        /// Get the Last Purchase Invoice Of Selected Partner
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<PurchaseInvoiceModel> GetPartnerLastPurchaseInvoiceAsync(int? Id);

        /// <summary>
        /// Genrate Or Create Unique Invoice Number
        /// </summary>
        /// <param name="Initials"></param>
        /// <returns></returns>
        string GenrateInvoiceNumber(string Initials);
        /// <summary>
        /// Save The Purchase Invoice
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<bool> SavePurchaseInoiceAsync(PurchaseInvoiceModel invoice);

        /// <summary>
        /// Gets the Last Row ID of Table
        /// </summary>
        /// <returns></returns>
        int? GetLastRowId();
        /// <summary>
        /// Remove the Last or Specific Invoice 
        /// </summary>
        /// <param name="InvoiceGuid"></param>
        /// <returns></returns>
        Task<bool> RemoveLastPurchaseInvoiceAsync(Guid InvoiceGuid);
    }
}
