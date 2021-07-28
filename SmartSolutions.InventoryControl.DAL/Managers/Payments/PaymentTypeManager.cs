﻿using SmartSolutions.InventoryControl.DAL.Models.Payments;
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
        public async Task<IEnumerable<PaymentTypeModel>> GetAllPaymentTypesAsync()
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
    }
}
