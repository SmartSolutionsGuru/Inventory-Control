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
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Payments
{
    [Export(typeof(IPaymentManager)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class PaymentManager : BaseManager, IPaymentManager
    {
        #region Private Members
        private readonly IRepository Repository;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public PaymentManager()
        {
            Repository = GetRepository<PaymentModel>();
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
                parameters["@v_PaymentRefrencePartnerId"] = payment?.PaymentRefrencePartner?.Id == null ? DBNull.Value : (object)payment.PaymentRefrencePartner;
                parameters["@v_PaymentImage"] = payment?.PaymentImage == null ? DBNull.Value : (object)payment.PaymentImage;
                parameters["@v_PaymentAmount"] = payment?.PaymentAmount;
                parameters["@v_IsPaymentReceived"] = payment?.IsPaymentReceived;
                parameters["@v_IsActive"] = payment.IsActive = true;
                parameters["@v_CreatedAt"] = payment.CreatedAt == null ? DateTime.Now : payment.CreatedAt;
                parameters["@v_CreatedBy"] = payment.CreatedBy == null ? AppSettings.LoggedInUser.DisplayName : (object)payment.CreatedBy;
                parameters["@v_UpdatedAt"] = payment.UpdatedAt == null ? DBNull.Value : (object)payment.UpdatedAt;
                parameters["@v_UpdatedBy"] = payment.UpdatedBy == null ? DBNull.Value : (object)payment.UpdatedBy;
                string query = @"INSERT INTO Payment (PartnerId,PaymentType,PaymentRefrencePartnerId,PaymentAmount,IsPaymentReceived,IsActive,CreatedAt,CreatedBy,UpdatedAt,UpdatedBy)
                                                 VALUES(@v_PartnerId,@v_PaymentType,@v_PaymentRefrencePartnerId,@v_PaymentAmount,@v_IsPaymentReceived,@v_IsActive,@v_CreatedAt,@v_CreatedBy,@v_UpdatedAt,@v_UpdatedBy)";
                var result = await Repository.NonQueryAsync(query: query, parameters: parameters);
                retVal = result > 0 ? true : false;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }

        public async Task<PaymentModel> GetLastPaymentByPartnerId(int? Id)
        {
            var lastPayment = new PaymentModel();
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_Id"] = Id;
                string query = @"SELECT * FROM Payment WHERE PartnerId = @v_Id ORDER BY 1 DESC";
               var values =  await Repository.QueryAsync(query,parameters:parameters);
                if(values != null || values?.Count > 0)
                {
                    foreach (var value in values)
                    {
                        lastPayment.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                        lastPayment.Partner = new Models.BussinessPartner.BussinessPartnerModel { Id = value?.GetValueFromDictonary("Id").ToString()?.ToInt()};
                        lastPayment.PaymentType = value?.GetValueFromDictonary("PaymentType")?.ToString()?.ToEnum<PaymentType>() ?? PaymentType.None;
                        lastPayment.PaymentAmount = value?.GetValueFromDictonary("PaymentAmount")?.ToString()?.ToDecimal() ?? 0;
                        lastPayment.IsPaymentReceived = value?.GetValueFromDictonary("IsPaymentReceived")?.ToString()?.ToNullableBoolean() ?? false;
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return lastPayment;
        }

        public async Task<IEnumerable<PaymentModel>> GetPaymentsByPartnerId(int? Id)
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
                    foreach (var value  in values)
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
