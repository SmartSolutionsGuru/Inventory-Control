﻿using SmartSolutions.InventoryControl.DAL.Models;
using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using SmartSolutions.InventoryControl.Plugins.Repositories;
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

namespace SmartSolutions.InventoryControl.DAL.Managers.Bussiness_Partner
{
    [Export(typeof(IPartnerLedgerManager)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class PartnerLedgerManager : BaseManager, IPartnerLedgerManager
    {
        #region Private Members
        private readonly IRepository Repository;
        private readonly IBussinessPartnerManager _bussinessPartnerManager;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public PartnerLedgerManager()
        {
            Repository = GetRepository<BussinessPartnerLedgerModel>();
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Add Balance Amount in The Bussiness Partner Account
        /// </summary>
        /// <param name="partnerLedger"></param>
        /// <returns></returns>
        public async Task<bool> AddPartnerBalance(BussinessPartnerLedgerModel partnerLedger)
        {
            bool retVal = false;
            try
            {
                if (partnerLedger == null) return false;
                string query = string.Empty;
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_PartnerId"] = partnerLedger?.Partner?.Id;
                parameters["@v_PaymentId"] = partnerLedger?.Payment?.Id;
                parameters["@v_CurrentBalance"] = partnerLedger.CurrentBalance;
                parameters["@v_CurrentBalanceType"] = partnerLedger?.CurrentBalanceType;
                parameters["@v_Description"] = partnerLedger?.Description == null ? DBNull.Value : (object)partnerLedger.Description;
                parameters["@v_IsActive"] = partnerLedger.IsActive = true;
                parameters["@v_CreatedAt"] = partnerLedger.CreatedAt == null ? DateTime.Now : partnerLedger.CreatedAt;
                parameters["@v_CreatedBy"] = partnerLedger.CreatedBy == null ? DBNull.Value : (object)partnerLedger.CreatedBy;
                parameters["@v_UpdatedAt"] = partnerLedger.UpdatedAt == null ? DBNull.Value : (object)partnerLedger.UpdatedAt;
                parameters["@v_UpdatedBy"] = partnerLedger.UpdatedBy == null ? DBNull.Value : (object)partnerLedger.UpdatedBy;

                query = @"INSERT INTO PartnerLedgerAccounts(PartnerId,PaymentId,CurrentBalance,CurrentBalanceType,Description,IsActive,CreatedAt,CreatedBy,UpdatedAt,UpdatedBy)
                                    VALUES(@v_PartnerId,@v_PaymentId,@v_CurrentBalance,@v_CurrentBalanceType,@v_Description,@v_IsActive,@v_CreatedAt,@v_CreatedBy,@v_UpdatedAt,@v_UpdatedBy);";
               var result =  await Repository.NonQueryAsync(query: query, parameters: parameters);
                retVal = result > 0 ?  true : false;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }

        public async Task<BussinessPartnerLedgerModel> GetPartnerLedgerLastBalance(int partnerId)
        {
            if (partnerId == 0) return null;
            BussinessPartnerLedgerModel retVal = null;
            try
            {
                //Verify Partner Id
                if (partnerId == 0) return null;
                string query = string.Empty;
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_PartnerId"] = partnerId;
                query = @"SELECT * FROM PartnerLedger WHERE PartnerId = @v_PartnerId Order By 1 DESC LIMIT 1";
                var values = await Repository.QueryAsync(query: query,parameters:parameters);

                if (values != null || values?.Count > 0)
                {
                    var value = values?.FirstOrDefault();
                    var partnerLedger = new BussinessPartnerLedgerModel();
                    partnerLedger.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                    partnerLedger.Payment = new Models.Payments.PaymentModel { Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt()};
                    partnerLedger.CurrentBalance = value?.GetValueFromDictonary("CurrentBalance")?.ToString()?.ToDecimal() ?? 0;
                    partnerLedger.CurrentBalanceType =value?.GetValueFromDictonary("CurrentBalanceType")?.ToString().ToEnum<PaymentType>() ?? PaymentType.None;
                    retVal = partnerLedger; 
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }

        public async Task<bool> UpdatePartnerCurrentBalance(BussinessPartnerLedgerModel partnerLedger)
        {
            bool retVal = false;
            try
            {
                //Null Guard
                if (partnerLedger == null) return false;
                string query = string.Empty;
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_PartnerId"] = partnerLedger.Partner?.Id;
                parameters["@v_PaymentId"] = partnerLedger?.Payment?.Id;
                parameters["@v_CurrentBalance"] = partnerLedger.CurrentBalance;
                parameters["@v_CurrentBalanceType"] = partnerLedger?.CurrentBalanceType;
                parameters["@v_Description"] = partnerLedger.Description;
                parameters["@v_IsActive"] = partnerLedger.IsActive = true;
                parameters["@v_CreatedAt"] = partnerLedger.CreatedAt == null ? DateTime.Now : partnerLedger.CreatedAt;
                parameters["@v_CreatedBy"] = partnerLedger.CreatedBy == null ? DBNull.Value : (object)partnerLedger.CreatedBy;
                parameters["@v_UpdatedAt"] = partnerLedger.UpdatedAt == null ? DBNull.Value : (object)partnerLedger.UpdatedAt;
                parameters["@v_UpdatedBy"] = partnerLedger.UpdatedBy == null ? DBNull.Value : (object)partnerLedger.UpdatedBy;

                query = @"INSERT INTO PartnerLedger(PartnerId,PaymentId,CuurentBalance,CurrentBalanceType,Description,IsActive,CreatedAt,CreatedBy,UpdatedAt,UpdatedBy)
                                    VALUES(@v_PartnerId,@v_PaymentId,@v_CurrentBalance,@v_CurrentBalanceType,@v_Description,@v_IsActive,@v_CreatedAt,@v_CreatedBy,@v_UpdatedAt,@v_UpdatedBy);";
                var result = await Repository.NonQueryAsync(query: query, parameters: parameters);
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
