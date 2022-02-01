using SmartSolutions.InventoryControl.DAL.Models.Bank;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.LogUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Bank
{
    [Export(typeof(IBankAccountManager)),PartCreationPolicy(CreationPolicy.Shared)]
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
                parameters["@v_Description"] = bankAccount.Description == null ? DBNull.Value : (object)bankAccount.Description;
                parameters["@v_IsActive"] = bankAccount.IsActive = true;
                parameters["@v_CreatedAt"] = bankAccount.CreatedAt == null ? DateTime.Now : bankAccount.CreatedAt;
                parameters["@v_CreatedBy"] = bankAccount.CreatedBy == null ? DBNull.Value : (object)bankAccount.CreatedBy;
                parameters["@v_UpdatedAt"] = bankAccount.UpdatedAt == null ? DBNull.Value : (object)bankAccount.UpdatedAt;
                parameters["@v_UpdatedBy"] = bankAccount.UpdatedBy == null ? DBNull.Value : (object)bankAccount.UpdatedBy;
                string query = @"INSERT INTO BankAccount (BranchId,AccountType,AccountStatus,OpeningDate,AccountNumber,OpeningBalance,Description,IsActive,CreatedAt,CreatedBy,UpdatedAt,UpdatedBy)
                                                    VALUES(@v_BranchId,@v_AccountType,@v_AccountStatus,@v_OpeningDate,@v_AccountNumber,@v_OpeningBalance,@v_Description,@v_IsActive,@v_CreatedAt,@v_CreatedBy,@v_UpdatedAt,@v_UpdatedBy)";
               var result =  await Repository.NonQueryAsync(query,parameters:parameters);
                retVal = result > 0 ? true : false;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
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
        #endregion
    }
}
