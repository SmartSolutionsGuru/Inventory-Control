using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.LogUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Bussiness_Partner
{
    [Export(typeof(IPartnerSetupAccountManager)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PartnerSetupAccountManager : BaseManager, IPartnerSetupAccountManager
    {
        #region Private Members
        private readonly IRepository Repository;
        private readonly IChartOfAccountManager _chartOfAccountManager;
        #endregion

        #region Costructor
        public PartnerSetupAccountManager(IChartOfAccountManager chartOfAccountManager)
        {
            Repository = GetRepository<BussinessPartnerSetupAccountModel>();
            _chartOfAccountManager = chartOfAccountManager;
        }
        #endregion

        #region Public Members
        public async Task<bool> SavePartnerSetAccountAsync(BussinessPartnerSetupAccountModel partnersetupAccount)
        {
            if (partnersetupAccount == null) return false;
            bool retVal = false;
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_PartnerId"] = partnersetupAccount?.Partner?.Id;
                parameters["@v_PartnerAccountTypeId"] = partnersetupAccount?.PartnerAccountType?.Id;
                parameters["@v_PartnerAccountCode"] = partnersetupAccount?.PartnerAccountCode;
                parameters["@v_IsActive"] = partnersetupAccount.IsActive = true;
                parameters["@v_CreatedAt"] = partnersetupAccount.CreatedAt == null ? DateTime.Now : partnersetupAccount.CreatedAt;
                parameters["@v_CreatedBy"] = partnersetupAccount.CreatedBy == null ? DBNull.Value : (object)partnersetupAccount.CreatedBy;
                parameters["@v_UpdatedAt"] = partnersetupAccount.UpdatedAt == null ? DBNull.Value : (object)partnersetupAccount.UpdatedAt;
                parameters["@v_UpdatedBy"] = partnersetupAccount.UpdatedBy == null ? DBNull.Value : (object)partnersetupAccount.UpdatedBy;
                string query = @"INSERT INTO PartnerSetupAccount(PartnerId,PartnerAccountCode,PartnerAccountCode,IsActive,CreatedAt,CreatedBy,UpdatedAt,UpdatedBy)
                                                        VALUES(@v_PartnerId,@v_PartnerAccountTypeId,@v_PartnerAccountCode,@v_IsActive,@v_CreatedAt,@v_CreatedBy,@v_UpdatedAt,@v_UpdatedBy)";
               var result =  await Repository.NonQueryAsync(query, parameters: parameters);
                retVal = result > 0 ? true : false;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }
        #endregion

        #region Private Helepers
        /// <summary>
        ///  Method that Will 
        /// Genrate Account Code According To Specification
        /// </summary>
        /// <returns></returns>
        public async Task<string> GenratePartnerAccountCodeAsync(string partnerType,int partnerId)
        {
            if (string.IsNullOrEmpty(partnerType) || partnerId == 0) return string.Empty;
            string accountCode = string.Empty;
            try
            {
                switch (partnerType)
                {
                    case "Vender":
                        break;
                    case "Customer":
                        break;
                    case "Both":
                        break;
                    case "broker":
                        break;
                    case "Shiper":
                        break;
                    default:
                        break;
                }
                var result =  await _chartOfAccountManager.GetChartOfAccountByHeadingAsync(partnerType);
                if(result != null)
                {
                    
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return accountCode;
        }
        #endregion
    }
}
