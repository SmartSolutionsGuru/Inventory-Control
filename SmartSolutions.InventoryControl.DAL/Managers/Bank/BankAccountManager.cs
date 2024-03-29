﻿using SmartSolutions.InventoryControl.DAL.Models.Bank;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.DictionaryUtils;
using SmartSolutions.Util.LogUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Bank
{
    [Export(typeof(IBankAccountManager)), PartCreationPolicy(CreationPolicy.Shared)]
    public class BankAccountManager : BaseManager, IBankAccountManager
    {
        #region Private Members
        private readonly IRepository Repository;
        #endregion

        #region Constructor
        public BankAccountManager()
        {
            Repository = GetRepository<BankAccountModel>();
        }
        #endregion

        #region Methods
        public async Task<bool> AddBankAccountAsync(BankAccountModel bankAccount)
        {
            if (bankAccount == null) return false;
            bool retVal = false;
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_BranchId"] = bankAccount?.Branch?.Id;
                parameters["@v_AccountType"] = bankAccount?.AccountType;
                parameters["@v_AccountStatus"] = bankAccount?.AccountStatus;
                parameters["@v_OpeningDate"] = bankAccount?.OpeningDate;
                parameters["@v_AccountNumber"] = bankAccount?.AccountNumber;
                parameters["@v_OpeningBalance"] = bankAccount?.OpeningBalance;
                parameters["@v_DR"] = bankAccount?.DR == null ? DBNull.Value : (object)bankAccount?.DR;
                parameters["@v_CR"] = bankAccount?.CR == null ? DBNull.Value : (object)bankAccount?.CR;
                parameters["@v_Description"] = bankAccount.Description == null ? DBNull.Value : (object)bankAccount.Description;
                parameters["@v_IsActive"] = bankAccount.IsActive = true;
                parameters["@v_CreatedAt"] = bankAccount.CreatedAt == null ? DateTime.Now : bankAccount.CreatedAt;
                parameters["@v_CreatedBy"] = bankAccount.CreatedBy == null ? DBNull.Value : (object)bankAccount.CreatedBy;
                parameters["@v_UpdatedAt"] = bankAccount.UpdatedAt == null ? DBNull.Value : (object)bankAccount.UpdatedAt;
                parameters["@v_UpdatedBy"] = bankAccount.UpdatedBy == null ? DBNull.Value : (object)bankAccount.UpdatedBy;
                string query = @"INSERT INTO BankAccount (BranchId,AccountType,AccountStatus,OpeningDate,AccountNumber,OpeningBalance,Description,IsActive,CreatedAt,CreatedBy,UpdatedAt,UpdatedBy,DR,CR)
                                                    VALUES(@v_BranchId,@v_AccountType,@v_AccountStatus,@v_OpeningDate,@v_AccountNumber,@v_OpeningBalance,@v_Description,@v_IsActive,@v_CreatedAt,@v_CreatedBy,@v_UpdatedAt,@v_UpdatedBy,@v_DR,@v_CR)";
                var result = await Repository.NonQueryAsync(query, parameters: parameters);
                retVal = result > 0 ? true : false;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }

        public async Task<bool> AddBankTransactionAsync(BankAccountModel account)
        {
            bool retVal = false;
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_BranchId"] = account?.Branch?.Id;
                parameters["@v_AccountType"] = account?.AccountType;
                parameters["@v_AccountStatus"] = account?.AccountStatus;
                parameters["@v_OpeningDate"] = account?.OpeningDate == null ? DBNull.Value : (object)account?.OpeningDate;
                parameters["@v_DR"] = account?.DR == null ? DBNull.Value : (object)account?.DR;
                parameters["@v_CR"] = account?.CR == null ? DBNull.Value : (object)account?.CR;
                parameters["@v_AccountNumber"] = account?.AccountNumber;
                parameters["@v_OpeningBalance"] = account?.OpeningBalance == null ? DBNull.Value : (object)account?.OpeningBalance;
                parameters["@v_Description"] = account?.Description;
                parameters["@v_IsActive"] = account.IsActive = true;
                parameters["@v_CreatedBy"] = AppSettings.LoggedInUser.DisplayName;
                parameters["@v_CreatedAt"] = account.CreatedAt == null ? DateTime.Now : account.CreatedAt;
                string query = @"INSERT INTO dbo.BankAccount
                                                (BranchId,
                                                 AccountType,
                                                 AccountStatus,
                                                 OpeningDate,
                                                 AccountNumber,
                                                 OpeningBalance,
                                                 DR,
                                                 CR,
                                                 Description,
                                                 IsActive,
                                                 CreatedAt,
                                                 CreatedBy
                                                )
                                                VALUES
                                                (@v_BranchId,            
                                                 @v_AccountType,           
                                                 @v_AccountStatus,           
                                                 @v_OpeningDate, 
                                                 @v_AccountNumber,           
                                                 @v_OpeningBalance,         
                                                 @v_DR,                 
                                                 @v_CR,                 
                                                 @v_Description,           
                                                 @v_IsActive,            
                                                 @v_CreatedAt, 
                                                 @v_CreatedBy)";
               var result =  await Repository.NonQueryAsync(query,parameters:parameters);
                retVal = result > 0 ? true : false;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }

        public async Task<IEnumerable<BankAccountModel>> GetAllBankAccountByBranchAsync(int? branchId)
        {
            List<BankAccountModel> bankAccounts = new List<BankAccountModel>();
            if (branchId == null || branchId == 0) return null;
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_BranchId"] = branchId;
                string query = @"SELECT * FROM dbo.BankAccount WHERE BranchId = @v_BranchId AND IsActive = 1";
                var values = await Repository.QueryAsync(query, parameters: parameters);
                if (values != null && values?.Count > 0)
                {
                    foreach (var value in values)
                    {
                        var bankAccount = new BankAccountModel();
                        bankAccount.AccountType = value?.GetValueFromDictonary("AccountType")?.ToString();
                        bankAccount.AccountNumber = value?.GetValueFromDictonary("AccountNumber")?.ToString();
                        bankAccount.AccountStatus = value?.GetValueFromDictonary("AccountStatus")?.ToString() ?? string.Empty;
                        bankAccounts.Add(bankAccount);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return bankAccounts;
        }

        public async Task<IEnumerable<BankAccountModel>> GetAllBankAccountsAsync()
        {
            var retVal = new List<BankAccountModel>();
            try
            {
                string query = string.Empty;
                await Repository.QueryAsync(query);
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }

        public async Task<bool> IsAccountAlreadyExist(int? branchId, string accountNumber)
        {
            if (branchId == null || accountNumber == null || branchId == 0 || string.IsNullOrEmpty(accountNumber)) return false;
            bool retVal = false;
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_BranchId"] = branchId;
                parameters["v_AccountNumber"] = accountNumber;
                string query = @"SELECT * FROM dbo.BankAccount WHERE BranchId = @v_BranchId AND AccountNumber = @v_AccountNumber AND IsActive = 1;";
                var values = await Repository.QueryAsync(query, parameters: parameters);
                if(values != null || values?.Count > 0)
                    retVal = true;
                else retVal = false;
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
