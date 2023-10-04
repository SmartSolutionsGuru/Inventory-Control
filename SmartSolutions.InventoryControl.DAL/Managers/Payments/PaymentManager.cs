using SmartSolutions.InventoryControl.DAL.Models;
using SmartSolutions.InventoryControl.DAL.Models.Payments;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.BooleanUtils;
using SmartSolutions.Util.DecimalsUtils;
using SmartSolutions.Util.DictionaryUtils;
using SmartSolutions.Util.EnumUtils;
using SmartSolutions.Util.LogUtils;
using SmartSolutions.Util.NumericUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Payments
{
    [Export(typeof(IPaymentManager)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PaymentManager : BaseManager, IPaymentManager
    {
        #region Private Members
        private readonly IRepository Repository;
        private readonly IPaymentTypeManager _paymentTypeManager;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public PaymentManager(IPaymentTypeManager paymentTypeManager)
        {
            Repository = GetRepository<PaymentModel>();
            _paymentTypeManager = paymentTypeManager;
        }
        #endregion

        #region Publuc Methods
        public async Task<bool> AddPaymentAsync(PaymentModel payment)
        {
            if (payment == null) return false;
            bool retVal = false;
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_PartnerId"] = payment?.Partner?.Id;
                parameters["@v_PaymentType"] = payment?.PaymentType.ToString();
                parameters["@v_PaymentMethodId"] = payment?.PaymentMethod?.Id;
                parameters["@v_PaymentReferencePartnerId"] = payment?.PaymentRefrencePartner?.Id == null ? DBNull.Value : (object)payment.PaymentRefrencePartner?.Id;
                //parameters["@v_PaymentImage"] = payment?.PaymentImage == null ? DBNull.Value : (object)payment.PaymentImage;
                parameters["@v_ImagePath"] = payment?.ImagePath == null ? DBNull.Value : (object)payment?.ImagePath;
                parameters["@v_PaymentAmount"] = payment?.PaymentAmount;
                parameters["@v_IsActive"] = payment.IsActive = true;
                parameters["@v_CreatedAt"] = payment.CreatedAt == null ? DateTime.Now : payment.CreatedAt;
                parameters["@v_CreatedBy"] = payment.CreatedBy == null ? AppSettings.LoggedInUser.DisplayName : (object)payment.CreatedBy;
                parameters["@v_UpdatedAt"] = payment.UpdatedAt == null ? DBNull.Value : (object)payment.UpdatedAt;
                parameters["@v_UpdatedBy"] = payment.UpdatedBy == null ? DBNull.Value : (object)payment.UpdatedBy;
                parameters["@v_Description"] = payment.Description == null ? DBNull.Value : (object)payment.Description;
                parameters["@v_DR"] = payment.Receivable == null ? DBNull.Value : (object)payment.Receivable;
                parameters["@v_CR"] = payment.Payable == null ? DBNull.Value : (object)payment.Payable;
                string query = @"INSERT INTO Payment(PartnerId,PaymentReferencePartnerId,PaymentAmount,IsActive,CreatedAt,CreatedBy,UpdatedAt,UpdatedBy,PaymentMethodId,Description,DR,CR,ImagePath)
                                                 VALUES(@v_PartnerId,@v_PaymentReferencePartnerId,@v_PaymentAmount,@v_IsActive,@v_CreatedAt,@v_CreatedBy,@v_UpdatedAt,@v_UpdatedBy,@v_PaymentMethodId,@v_Description,@v_DR,@v_CR,@v_ImagePath)";
                var result = await Repository.NonQueryAsync(query: query, parameters: parameters);
                retVal = result > 0 ? true : false;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }

        public async Task<PaymentModel> GetLastPaymentByPartnerIdAsync(int? Id)
        {
            var lastPayment = new PaymentModel();
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_Id"] = Id;
                string query = @"SELECT TOP 1 * FROM Payment WHERE PartnerId = @v_Id ORDER By PaymentAmount DESC";
                var values = await Repository.QueryAsync(query, parameters: parameters);
                if (values != null || values?.Count > 0)
                {
                    foreach (var value in values)
                    {
                        lastPayment.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                        lastPayment.Partner = new Models.BussinessPartner.BussinessPartnerModel { Id = value?.GetValueFromDictonary("Id").ToString()?.ToInt() };
                        lastPayment.PaymentType = value?.GetValueFromDictonary("PaymentType")?.ToString()?.ToEnum<PaymentType>() ?? PaymentType.None;
                        lastPayment.PaymentMethod = new PaymentTypeModel { Id = value?.GetValueFromDictonary("PaymentMethodId")?.ToString()?.ToInt() ?? 0 };
                        lastPayment.PaymentAmount = value?.GetValueFromDictonary("PaymentAmount")?.ToString()?.ToDecimal() ?? 0;
                        lastPayment.Receivable = Convert.ToDecimal(value?.GetValueFromDictonary("DR").ToString());
                        lastPayment.Payable = Convert.ToDecimal(value?.GetValueFromDictonary("CR").ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return lastPayment;
        }

        public async Task<PaymentModel> GetPaymentByIdAsync(int? paymentId)
        {
            if (paymentId == null || paymentId == 0) return null;
            var payment = new PaymentModel();
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_Id"] = paymentId;
                string query = "SELECT * FROM Payment WHERE Id = @v_Id AND IsActive = 1";
                var values = await Repository.QueryAsync(query: query, parameters: parameters);
                if (values != null || values?.Count > 0)
                {
                    var value = values.FirstOrDefault();
                    payment.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                    payment.PaymentMethod = new PaymentTypeModel { Id = value?.GetValueFromDictonary("PaymentMethodId")?.ToString()?.ToInt() ?? 0 };
                    payment.PaymentMethod = await _paymentTypeManager.GetPaymentMethodByIdAsync(payment?.PaymentMethod?.Id ?? 0);
                    payment.PaymentAmount = value?.GetValueFromDictonary("PaymentAmount")?.ToString()?.ToDecimal() ?? 0;
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return payment;
        }

        public async Task<IEnumerable<PaymentModel>> GetPaymentsByPartnerIdAsync(int? Id)
        {
            if (Id == null || Id == 0) return null;
            var payments = new List<PaymentModel>();
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_Id"] = Id;
                string query = @"SELECT * FROM Payment WHERE PartnerId = @v_Id";
                var values = await Repository.QueryAsync(query, parameters: parameters);
                if (values != null || values?.Count > 0)
                {
                    foreach (var value in values)
                    {
                        var payment = new PaymentModel();
                        payment.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                        payment.PaymentAmount = value?.GetValueFromDictonary("PaymentAmount")?.ToString()?.ToDecimal() ?? 0;
                        payment.IsPaymentReceived = value?.GetValueFromDictonary("IsPaymentRecived")?.ToString()?.ToNullableBoolean() ?? false;
                        payments.Add(payment);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return payments;
        }
        #endregion
    }
}
