using SmartSolutions.InventoryControl.DAL.Models.PurchaseOrder;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.DictionaryUtils;
using SmartSolutions.Util.LogUtils;
using SmartSolutions.Util.NumericUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Invoice
{
    [Export(typeof(IPurchaseInvoiceManager)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PurchaseInvoiceManager : BaseManager, IPurchaseInvoiceManager
    {
        #region Private Members
        private readonly IRepository Repository;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public PurchaseInvoiceManager()
        {
            Repository = GetRepository<PurchaseInvoiceModel>();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gnrate the New Invoice Number
        /// </summary>
        /// <param name="Initials"></param>
        /// <returns></returns>
        public string GenrateInvoiceNumber(string Initials)
        {
            if (string.IsNullOrEmpty(Initials))
                Initials = "PI";
            string InvoiceNumber = string.Empty;
            if (!string.IsNullOrEmpty(Initials))
                Initials = Initials.ToUpper();
            else
                Initials = string.Empty;
            try
            {
                string numbers = "1234567890";
                string characters = numbers;
                int length = 10;
                string id = string.Empty;
                for (int i = 0; i < length; i++)
                {
                    string character = string.Empty;
                    do
                    {
                        int index = new Random().Next(0, characters.Length);
                        character = characters.ToCharArray()[index].ToString();
                    } while (id.IndexOf(character) != -1);
                    id += character;
                    InvoiceNumber = Initials + '_' + id;
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return InvoiceNumber;
        }

        public int? GetLastRowId()
        {
            int? lastRowId = null;
            try
            {
                string query = "SELECT last_insert_rowid()";
                var result = Repository.Query(query);
                lastRowId = result?.FirstOrDefault().GetValueFromDictonary("last_insert_rowid()")?.ToString()?.ToNullableInt();
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return lastRowId;
        }

        public async Task<PurchaseInvoiceModel> GetPartnerLastPurchaseInvoiceAsync(int? Id)
        {
            if (Id == null || Id == 0) return null;
            PurchaseInvoiceModel invoice = null;
            try
            {

            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return invoice;
        }

        public async Task<bool> RemoveLastPurchaseInvoiceAsync(Guid InvoiceGuid)
        {
            bool retVal = false;
            try
            {

            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }

        public async Task<bool> SavePurchaseInoiceAsync(PurchaseInvoiceModel invoice)
        {
            bool retVal = false;
            try
            {

            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }
        #endregion
    }
}
