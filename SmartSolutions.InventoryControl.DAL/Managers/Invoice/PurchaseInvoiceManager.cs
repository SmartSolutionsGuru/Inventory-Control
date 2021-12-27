using SmartSolutions.InventoryControl.DAL.Models.PurchaseOrder;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.DictionaryUtils;
using SmartSolutions.Util.LogUtils;
using SmartSolutions.Util.NumericUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
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
                string query = @"SELECT IDENT_CURRENT('PurchaseInvoice') AS Id";
                var result = Repository.Query(query);
                lastRowId = result?.FirstOrDefault().GetValueFromDictonary("Id")?.ToString()?.ToNullableInt();
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
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_Id"] = Id ?? 0;
                string query = @"SELECT * FROM PurchaseInvoice WHERE Id = @v_Id AND IsActive = 1";
                var values = await Repository.QueryAsync(query, parameters: parameters);
                if (values != null || values?.Count > 0)
                {
                    //TODO: have to Cmplete this
                    var value = values?.FirstOrDefault();
                    invoice.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                }
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
            if (invoice == null) return false;
            bool retVal = false;
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_InvoiceId"] = invoice?.InvoiceId;
                parameters["@v_InvoiceGuid"] = invoice?.InvoiceGuid.ToString();
                parameters["@v_PaymentTypeId"] = invoice?.SelectedPaymentType?.Id ?? 0;
                parameters["@v_PartnerId"] = invoice?.SelectedPartner?.Id;
                parameters["@v_PaymentId"] = invoice?.Payment?.Id;
                parameters["@v_InvoiceTotal"] = invoice?.InvoiceTotal;
                parameters["@v_Discount"] = invoice?.Discount;
                //parameters["@v_PaymentImage"] = invoice?.PaymentImage == null ? DBNull.Value : (object)invoice.PaymentImage;
                parameters["@V_ImagePath"] = invoice?.ImagePath == null ? DBNull.Value : (object)invoice?.ImagePath;
                parameters["@v_IsActive"] = invoice.IsActive = true;
                parameters["@v_CreatedAt"] = invoice?.CreatedAt == null ? DateTime.Now : invoice.CreatedAt;
                parameters["@v_CreatedBy"] = invoice?.CreatedBy == null ? DBNull.Value : (object)invoice.CreatedBy;
                parameters["@v_UpdatedAt"] = invoice?.UpdatedAt == null ? DBNull.Value : (object)invoice.UpdatedAt;
                parameters["@v_UpdatedBy"] = invoice?.UpdatedBy == null ? DBNull.Value : (object)invoice.UpdatedBy;
                string query = @"INSERT INTO PurchaseInvoice(InvoiceId,InvoiceGuid,PaymentTypeId,PartnerId,PaymentId,InvoiceTotal,Discount,ImagePath,IsActive,CreatedAt,CreatedBy,UpdatedAt,UpdatedBy)
                                                    VALUES(@v_InvoiceId,@v_InvoiceGuid,@v_PaymentTypeId,@v_PartnerId,@v_PaymentId,@v_InvoiceTotal,@v_Discount,@V_ImagePath,@v_IsActive,@v_CreatedAt,@v_CreatedBy,@v_UpdatedAt,@v_UpdatedBy)";
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
