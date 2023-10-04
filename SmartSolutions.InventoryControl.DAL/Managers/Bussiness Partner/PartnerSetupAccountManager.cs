using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.DictionaryUtils;
using SmartSolutions.Util.EnumUtils;
using SmartSolutions.Util.LogUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
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
        [ImportingConstructor]
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
                parameters["@v_Description"] = partnersetupAccount?.Description;
                parameters["@v_IsActive"] = partnersetupAccount.IsActive = true;
                parameters["@v_CreatedAt"] = partnersetupAccount.CreatedAt == null ? DateTime.Now : partnersetupAccount.CreatedAt;
                parameters["@v_CreatedBy"] = partnersetupAccount.CreatedBy == null ? AppSettings.LoggedInUser.DisplayName : (object)partnersetupAccount.CreatedBy;
                parameters["@v_UpdatedAt"] = partnersetupAccount.UpdatedAt == null ? DBNull.Value : (object)partnersetupAccount.UpdatedAt;
                parameters["@v_UpdatedBy"] = partnersetupAccount.UpdatedBy == null ? DBNull.Value : (object)partnersetupAccount.UpdatedBy;
                string query = @"INSERT INTO PartnerSetupAccount(PartnerId,PartnerAccountTypeId,PartnerAccountCode,Description,IsActive,CreatedAt,CreatedBy,UpdatedAt,UpdatedBy)
                                                        VALUES(@v_PartnerId,@v_PartnerAccountTypeId,@v_PartnerAccountCode,@v_Description,@v_IsActive,@v_CreatedAt,@v_CreatedBy,@v_UpdatedAt,@v_UpdatedBy)";
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

        #region Public Helepers
        /// <summary>
        ///  Method that Will 
        /// Genrate Account Code According To Specification
        /// </summary>
        /// <returns></returns>
        public async Task<List<Dictionary<string,string>>> GenratePartnerAccountCodeAsync(string partnerType,int partnerId)
        {
            if (string.IsNullOrEmpty(partnerType) || partnerId == 0) return new List<Dictionary<string,string>>();
            var charAccount = new ChartOfAccountModel();
            List<Dictionary<string,string>> accountCode = new List<Dictionary<string,string>>();
            try
            {
                switch (partnerType)
                {
                    case "Vender":
                         charAccount = await _chartOfAccountManager.GetChartOfAccountByHeadingAsync(AccountHeading.AccountsPayable.ToDescription());
                        if(charAccount != null && !string.IsNullOrEmpty(charAccount.Description))
                        {
                            Dictionary<string, string> account = new Dictionary<string, string>();
                            account["Code"] = $"{charAccount.AccountNumber}-{partnerId}";
                            account["Description"] = charAccount.Description;
                            accountCode.Add(account);
                            //accountCode.Add($"{charAccount.AccountNumber}-{partnerId}");
                        }
                        break;
                    case "Customer":
                        charAccount = await _chartOfAccountManager.GetChartOfAccountByHeadingAsync(AccountHeading.AccountsReceivable.ToDescription());
                        if (charAccount != null && !string.IsNullOrEmpty(charAccount.Description))
                        {
                            Dictionary<string, string> account = new Dictionary<string, string>();
                            account["Code"] = $"{charAccount.AccountNumber}-{partnerId}";
                            account["Description"] = charAccount.Description;
                            accountCode.Add(account);
                            //accountCode.Add($"{charAccount.AccountNumber}-{partnerId}");
                        }
                        break;
                    case "Both":
                        charAccount = await _chartOfAccountManager.GetChartOfAccountByHeadingAsync(AccountHeading.AccountsPayable.ToDescription());
                        if (charAccount != null && !string.IsNullOrEmpty(charAccount.Description))
                        {
                            Dictionary<string, string> account = new Dictionary<string, string>();
                            account["Code"] = $"{charAccount.AccountNumber}-{partnerId}";
                            account["Description"] = charAccount.Description;
                            accountCode.Add(account);
                            //accountCode.Add($"{charAccount.AccountNumber}-{partnerId}");
                        }
                        charAccount = await _chartOfAccountManager.GetChartOfAccountByHeadingAsync(AccountHeading.AccountsReceivable.ToDescription());
                        if (charAccount != null && !string.IsNullOrEmpty(charAccount.Description))
                        {
                            Dictionary<string, string> account = new Dictionary<string, string>();
                            account["Code"] = $"{charAccount.AccountNumber}-{partnerId}";
                            account["Description"] = charAccount.Description;
                            accountCode.Add(account);
                            //accountCode.Add($"{charAccount.AccountNumber}-{partnerId}");
                        }
                        break;
                    case "broker":
                        charAccount = await _chartOfAccountManager.GetChartOfAccountByHeadingAsync(AccountHeading.PurchaseCommisions.ToDescription());
                        if (charAccount != null && !string.IsNullOrEmpty(charAccount.Description))
                        {
                            Dictionary<string, string> account = new Dictionary<string, string>();
                            account["Code"] = $"{charAccount.AccountNumber}-{partnerId}";
                            account["Description"] = charAccount.Description;
                            accountCode.Add(account);
                            //accountCode.Add($"{charAccount.AccountNumber}-{partnerId}");
                        }
                        charAccount = await _chartOfAccountManager.GetChartOfAccountByHeadingAsync(AccountHeading.SaleCommisions.ToDescription());
                        if (charAccount != null && !string.IsNullOrEmpty(charAccount.Description))
                        {
                            Dictionary<string, string> account = new Dictionary<string, string>();
                            account["Code"] = $"{charAccount.AccountNumber}-{partnerId}";
                            account["Description"] = charAccount.Description;
                            accountCode.Add(account);
                            //accountCode.Add($"{charAccount.AccountNumber}-{partnerId}");
                        }
                        break;
                    case "Shiper":
                        //TODO: Replace it With Poper Account
                        charAccount = await _chartOfAccountManager.GetChartOfAccountByHeadingAsync(AccountHeading.MotorVehicale.ToDescription());
                        if (charAccount != null && !string.IsNullOrEmpty(charAccount.Description))
                        {
                            Dictionary<string, string> account = new Dictionary<string, string>();
                            account["Code"] = $"{charAccount.AccountNumber}-{partnerId}";
                            account["Description"] = charAccount.Description;
                            accountCode.Add(account);
                            //accountCode.Add($"{charAccount.AccountNumber}-{partnerId}");
                        }
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="partnerId"></param>
        /// <param name="descriptionType"></param>
        /// <returns></returns>
        public async Task<string> GetPartnerAccountCodeByIdAsync(int partnerId,string descriptionType)
        {
            // null guard
            if(partnerId < 0)  return string.Empty;
            var setupAccountCode = string.Empty;
            try
            {
                Dictionary<string,object>parameters = new Dictionary<string, object>();
                parameters["@v_description"] = descriptionType;
                parameters["@v_partnerId"] = partnerId;
                string query = @"SELECT * FROM PartnerSetupAccount WHERE PartnerId = @v_partnerId AND Description LIKE '%@v_description%'";
                var values = await  Repository.QueryAsync(query,parameters:parameters);
                if(values != null && values?.Count  > 0) 
                {
                    var value = values.FirstOrDefault();
                    setupAccountCode = value.GetValueFromDictonary("PartnerAccountCode")?.ToString() ?? string.Empty;

                }
            }
            catch (Exception ex)
            {

                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return setupAccountCode;
        }
        #endregion
    }
}
