using SmartSolutions.InventoryControl.DAL.Models.Sales;
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
    [Export(typeof(ISaleInvoiceManager)),PartCreationPolicy(CreationPolicy.Shared)]
    public class SaleInvoiceManager : BaseManager, ISaleInvoiceManager
    {
        #region Private Members
        private readonly IRepository Repository;

        #endregion

        #region Constructor
        public SaleInvoiceManager()
        {
            Repository = GetRepository<SaleInvoiceModel>();
        }
        #endregion

        #region Public Methods

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
                lastRowId = result?.FirstOrDefault().GetValueFromDictonary("last_insert_rowid()")?.ToString()?.ToNullableInt();
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return lastRowId;
        }

        public async Task<SaleInvoiceModel> GetPartnerLastSaleInvoiceAsync(int? Id)
        {
            var lastInvoice = new SaleInvoiceModel();
            try
            {

                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_Id"] = Id;
                string query = @"SELECT * FROM SaleInvoice WHERE SelectedPartnerId= Id = @v_Id AND IsActive = 1 ORDER BY 1 DESC LIMIT 1;";
                var values = await Repository.QueryAsync(query, parameters: parameters);
                if (values != null)
                {
                    var value = values?.FirstOrDefault();
                    lastInvoice.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                    lastInvoice.InvoiceId = value?.GetValueFromDictonary("InvoiceId")?.ToString();
                    lastInvoice.InvoiceGuid = Guid.Parse(value?.GetValueFromDictonary("InvoiceGuid")?.ToString());
                    lastInvoice.Payment = value?.GetValueFromDictonary("Payment")?.ToString()?.ToNullableInt() ?? 0;
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return lastInvoice;
        }

        public async Task<bool> RemoveLastInvoiceAsync(Guid InvoiceGuid)
        {
            bool retVal = false;
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_InvoiceGuid"] = InvoiceGuid;
                string query = @"UPDATE SaleInvoice SET IsActive = 0 WHERE InvoiceGuid = @v_InvoiceGuid";
                var result = await Repository.NonQueryAsync(query: query);
                retVal = result > 0 ? true : false;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }

        public async Task<bool> SaveSaleInoiceAsync(SaleInvoiceModel invoice)
        {
            bool retVal = false;
            try
            {
                if (invoice == null) return false;
                string query = string.Empty;
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_InvoiceId"] = invoice.InvoiceId;
                parameters["@v_InvoiceGuid"] = invoice.InvoiceGuid;
                parameters["@v_SelectedPartnerId"] = invoice.SelectedPartner?.Id;
                parameters["@v_SelectedPaymentType"] = invoice.SelectedPaymentType;
                parameters["@v_PercentDiscount"] = invoice.PercentDiscount;
                parameters["@v_DiscountAmount"] = invoice.Discount == 0 ? DBNull.Value : (object)invoice.Discount;
                parameters["@v_Description"] = invoice.Description == null ? DBNull.Value : (object)invoice.Description;
                parameters["@v_AmountImage"] = invoice.PaymentImage == null ? DBNull.Value : (object)invoice.PaymentImage;
                parameters["@v_Payment"] = invoice.Payment;
                parameters["@v_InvoiceTotal"] = invoice.InvoiceTotal;
                parameters["@v_IsActive"] = invoice.IsActive = true;
                parameters["@v_IsDeleted"] = invoice.IsDeleted = false;
                parameters["@v_CreatedAt"] = invoice.CreatedAt == null ? DateTime.Now : invoice.CreatedAt;
                parameters["@v_CreatedBy"] = invoice.CreatedBy == null ? DBNull.Value : (object)invoice.CreatedBy;
                parameters["@v_UpdatedAt"] = invoice.UpdatedAt == null ? DBNull.Value : (object)invoice.UpdatedAt;
                parameters["@v_UpdatedBy"] = invoice.UpdatedBy == null ? DBNull.Value : (object)invoice.UpdatedBy;

                query = @"INSERT INTO SaleInvoice (InvoiceId,InvoiceGuid,SelectedPartnerId,SelectedPaymentType,PercentDiscount,DiscountAmount,Description,AmountImage,Payment,InvoiceTotal,IsActive,IsDeleted,CreatedAt,CreatedBy,UpdatedAt,UpdatedBy)
                                            VALUES(@v_InvoiceId,@v_InvoiceGuid,@v_SelectedPartnerId,@v_SelectedPaymentType,@v_PercentDiscount,@v_DiscountAmount,@v_Description,@v_AmountImage,@v_Payment,@v_InviceTotal,@v_IsActive,@v_CreatedAt,@v_CreatedBy,@v_UpdatedAt,@v_UpdatedBy)";

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
