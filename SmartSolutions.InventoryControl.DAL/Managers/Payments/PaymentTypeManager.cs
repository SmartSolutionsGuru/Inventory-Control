using SmartSolutions.InventoryControl.DAL.Models.Payments;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.DictionaryUtils;
using SmartSolutions.Util.LogUtils;
using SmartSolutions.Util.NumericUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Payments
{
    [Export(typeof(IPaymentTypeManager)), PartCreationPolicy(CreationPolicy.Shared)]
    public class PaymentTypeManager : BaseManager, IPaymentTypeManager
    {
        #region Private Members
        private readonly IRepository Repository;
        #endregion

        #region Constructor
        public PaymentTypeManager()
        {
            Repository = GetRepository<PaymentTypeModel>();
        }
        #endregion
        public async Task<IEnumerable<PaymentTypeModel>> GetAllPaymentMethodsAsync()
        {
            List<PaymentTypeModel> paymentTypes = new List<PaymentTypeModel>();
            try
            {
                string query = @"SELECT * FROM PaymentType WHERE IsActive = 1";
                var values = await Repository.QueryAsync(query);
                if(values != null || values?.Count > 0)
                {
                    foreach (var value in values)
                    {
                        var paymentType = new PaymentTypeModel();
                        paymentType.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                        paymentType.Name = value?.GetValueFromDictonary("Name")?.ToString();
                        paymentType.PaymentType = value?.GetValueFromDictonary("PaymentType")?.ToString();
                        paymentType.Description = value?.GetValueFromDictonary("Description")?.ToString();
                        paymentTypes.Add(paymentType);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return paymentTypes;
        }

        public async Task<PaymentTypeModel> GetPaymentMethodByIdAsync(int? Id)
        {
            if (Id == null || Id == 0) return null;
            var paymentMethod = new PaymentTypeModel();
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_Id"] = Id;
                string query = @"SELECT * FROM PaymentType WHERE Id = @v_Id AND IsActive = 1";
                var values = await Repository.QueryAsync(query,parameters:parameters);
                if (values != null || values?.Count > 0)
                {
                    foreach (var value in values)
                    {
                        paymentMethod.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                        paymentMethod.Name = value?.GetValueFromDictonary("Name")?.ToString();
                        paymentMethod.PaymentType = value?.GetValueFromDictonary("PaymentType")?.ToString();
                        paymentMethod.Description = value?.GetValueFromDictonary("Description")?.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return paymentMethod;
        }
    }
}
