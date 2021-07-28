using SmartSolutions.InventoryControl.DAL.Models.Sales;
using System;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Invoice
{
    public interface ISaleInvoiceManager
    {
        /// <summary>
        /// Get the Last Sale Invoice Of Selected Partner
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<SaleInvoiceModel> GetPartnerLastSaleInvoiceAsync(int? Id);
        /// <summary>
        /// Save The Sale Invoice
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<bool> SaveSaleInoiceAsync(SaleInvoiceModel invoice);
        /// <summary>
        /// Genrate Or Create Unique Invoice Number
        /// </summary>
        /// <param name="Initials"></param>
        /// <returns></returns>
        string GenrateInvoiceNumber(string Initials);
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
        Task<bool> RemoveLastInvoiceAsync(Guid InvoiceGuid);
    }
}
