using SmartSolutions.InventoryControl.DAL.Models.Inventory;
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
    [Export(typeof(IInvoiceManager)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InvoiceManager : BaseManager, IInvoiceManager
    {
        #region Private Members
        private readonly IRepository Repository;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public InvoiceManager()
        {
            Repository = GetRepository<InvoiceModel>();
        }

        #endregion

        #region Methods
        public string GenrateInvoiceNumber(string Initials)
        {
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
            int? lastRowId = 0;
            try
            {
                string query = "SELECT last_insert_rowid()";
                var result = Repository.Query(query);
                 lastRowId = result?.FirstOrDefault().GetValueFromDictonary("Last(ROW)")?.ToString()?.ToNullableInt();
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return lastRowId;
        }

        public async Task<bool> SaveInoiceAsync(InvoiceModel invoice)
        {          
            bool retVal = false;
            try
            {
                if (invoice == null) return false;
                string query = string.Empty;
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_InvoiceId"] = invoice.InvoiceId;
                parameters["@v_InvoiceGuid"] = invoice.InvoiceGuid;
                parameters["@v_IsPurchaseInvoice"] = invoice.IsPurchaseReturnInvoice;
                parameters["@v_IsSaleInvoice"] = invoice.IsSaleInvoice;
                parameters["@v_IsPurchaseReturnInvoice"] = invoice.IsPurchaseReturnInvoice;
                parameters["@v_IsSaleReturnInvoice"] = invoice.IsSaleReturnInvoice;
                parameters["@v_IsAmountRecived"] = invoice.IsAmountPaid;
                parameters["@v_IsAmountPaid"] = invoice.IsAmountRecived;
                parameters["@v_InvoiceType"] = invoice.TransactionType;
                parameters["@v_SelectedPartnerId"] = invoice.SelectedPartner?.Id;
                parameters["@v_SelectedPaymentType"] = invoice.SelectedPaymentType;
                parameters["@v_PercentDiscount"] = invoice.PercentDiscount;
                parameters["@v_DiscountAmount"] = invoice.Discount == 0 ? DBNull.Value : (object)invoice.Discount;
                parameters["@v_Description"] = invoice.Description == null ? DBNull.Value : (object)invoice.Description;
                parameters["@v_AmountImage"] = invoice.PaymentImage == null ? DBNull.Value : (object)invoice.PaymentImage;
                parameters["@v_InvoiceTotal"] = invoice.InvoiceTotal;
                parameters["@v_IsActive"] = invoice.IsActive = true;
                parameters["@v_IsDeleted"] = invoice.IsDeleted = false;
                parameters["@v_CreatedAt"] = invoice.CreatedAt == null ? DateTime.Now : invoice.CreatedAt;
                parameters["@v_CreatedBy"] = invoice.CreatedBy == null ? DBNull.Value : (object)invoice.CreatedBy;
                parameters["@v_UpdatedAt"] = invoice.UpdatedAt == null ? DBNull.Value : (object)invoice.UpdatedAt;
                parameters["@v_UpdatedBy"] = invoice.UpdatedBy == null ? DBNull.Value : (object)invoice.UpdatedBy;

                query = @"INSERT INTO Invoice (InvoiceId,InvoiceGuid,IsPurchaseInvoice,IsSaleInvoice,IsPurchaseReturnInvoice,IsSaleReturnInvoice,IsAmountRecived,IsAmountPaid,InvoiceType,SelectedPartnerId,SelectedPaymentType,PercentDiscount,DiscountAmount,Description,AmountImage,Payment,InvoiceTotal,IsActive,IsDeleted,CreatedAt,CreatedBy,UpdatedAt,UpdatedBy)
                                            VALUES(@v_InvoiceId,@v_InvoiceGuid,@v_IsPurchaseInvoice,@v_IsSaleInvoice,@v_IsPurchaseReturnInvoice,@v_IsSaleReturnInvoice,@v_IsAmountRecived,@v_IsAmountPaid,@v_InvoiceType,@v_SelectedPartnerId,@v_SelectedPaymentType,@v_PercentDiscount,@v_Discount,@v_Description,@v_AmountImage,@v_InviceTotal,@v_IsActive,@v_IsDeleted,@v_CreatedAt,@v_CreatedBy,@v_UpdatedAt,@v_UpdatedBy)";

                var result = await Repository.NonQueryAsync(query, parameters: parameters);
                retVal = result > 0 ? true : false;
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
