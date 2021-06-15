using SmartSolutions.InventoryControl.DAL.Models.Inventory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Invoice
{
    public interface IInvoiceManager
    {
        /// <summary>
        /// Genrate Or Create Unique Invoice Number
        /// </summary>
        /// <param name="Initials"></param>
        /// <returns></returns>
        string GenrateInvoiceNumber(string Initials);
        /// <summary>
        /// Save The Transaction
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<bool> SaveInoiceAsync(InvoiceModel transaction);
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
