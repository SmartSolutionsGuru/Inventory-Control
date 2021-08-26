using SmartSolutions.InventoryControl.DAL.Models.Bank;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.DictionaryUtils;
using SmartSolutions.Util.LogUtils;
using SmartSolutions.Util.NumericUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Bank
{
    [Export(typeof(IBankManager)), PartCreationPolicy(CreationPolicy.Shared)]
    public class BankManager : BaseManager, IBankManager
    {
        #region Private Members
        private readonly IRepository Repository;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public BankManager()
        {
            Repository = GetRepository<BankModel>();
        }
        #endregion

        #region Public Methods
        public async Task<bool> AddBankAsync(BankModel bank)
        {
            if (bank == null) return false;
            bool retVal = false;
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_Name"] = bank.Name;
                parameters["@v_Description"] = bank.Description == null ? DBNull.Value : (object)bank.Description;
                parameters["@v_IsActive"] = true;
                parameters["@v_CreatedAt"] = DateTime.Now;
                parameters["@v_CreatedBy"] = bank.CreatedBy == null ? AppSettings.LoggedInUser.DisplayName : (object)bank.CreatedBy;
                parameters["@v_UpdatedAt"] = bank.UpdatedAt == null ? DBNull.Value : (object)bank.UpdatedAt;
                parameters["@v_UpdatedBy"] = bank.UpdatedBy == null ? DBNull.Value : (object)bank.UpdatedBy;
                string query = @"INSERT INTO Bank(Name,Description,IsActive,CreatedAt,CreatedBy,UpdatedAt,UpdatedBy)
                                            VALUES(@v_Name,@v_Description,@v_IsActive,@v_CreatedAt,@v_CreatedBy,@v_UpdatedAt,@v_UpdatedBy)";
                var result = await Repository.NonQueryAsync(query, parameters: parameters);
                retVal = result > 0 ? true : false;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }

        public async Task<IEnumerable<BankModel>> GetAllBanksAsync()
        {
            var banks = new List<BankModel>();
            try
            {
                string query = @"SELECT * FROM Bank WHERE IsActive = 1";
                var values = await Repository.QueryAsync(query);
                if (values != null || values?.Count > 0)
                {
                    foreach (var value in values)
                    {
                        var bank = new BankModel();
                        bank.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                        bank.Name = value?.GetValueFromDictonary("Name")?.ToString();
                        banks.Add(bank);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return banks;
        }
        public async Task<BankModel> GetBankByIdAsync(int? Id)
        {
            if (Id == null || Id == 0) return null;
            var retVal = new BankModel();
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_Id"] = Id;
                string query = @"SELECT * FROM Bank WHERE Id = @v_Id IsActive = 1";
                var values = await Repository.QueryAsync(query, parameters: parameters);
                if (values != null || values?.Count > 0)
                {
                    var value = values?.FirstOrDefault();
                    retVal.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                    retVal.Name = value?.GetValueFromDictonary("Name")?.ToString();
                }

            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }

        public async Task<bool> RemoveBankAsync(int? Id)
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

        public async Task<bool> UpdateBankAsync(int? bank)
        {
            bool retVal = false;
            try
            {

            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            };
            return retVal;
        }
        #endregion
    }
}
