using SmartSolutions.InventoryControl.DAL.Models.Bank;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.DictionaryUtils;
using SmartSolutions.Util.LogUtils;
using SmartSolutions.Util.NumericUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Bank
{
    [Export(typeof(IBankBranchManager)),PartCreationPolicy(CreationPolicy.Shared)]
    public class BankBranchManager : BaseManager, IBankBranchManager
    {
        #region Private Members
        private readonly IRepository Repository;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public BankBranchManager()
        {
            Repository = GetRepository<BankBranchModel>();
        }
        #endregion

        #region Public Methods
        public async Task<bool> AddBankBranchAsync(BankBranchModel bankBranch)
        {
            if (bankBranch == null) return false;
            bool retVal = false;
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_Name"] = bankBranch?.Name;
                parameters["@v_BankId"] = bankBranch?.Bank?.Id;
                parameters["@v_Address"] = bankBranch.Address == null ? DBNull.Value : (object)bankBranch.Address;
                parameters["@v_BarnchDetail"] = bankBranch.BarnchDetails == null ? DBNull.Value : (object)bankBranch.BarnchDetails;
                parameters["@v_BussinessPhone"] = bankBranch.BussinessPhone == null ? DBNull.Value : (object)bankBranch.BussinessPhone;
                parameters["@v_BussinessPhone1"] = bankBranch.BussinessPhone1 == null ? DBNull.Value : (object)bankBranch.BussinessPhone1;
                parameters["@v_MobilePhone"] = bankBranch.MobilePhone == null ? DBNull.Value : (object)bankBranch.MobilePhone;
                parameters["@v_MobilePhone1"] = bankBranch.MobilePhone1 == null ? DBNull.Value : (object)bankBranch.MobilePhone1;
                parameters["@v_Email"] = bankBranch.Email == null ? DBNull.Value : (object)bankBranch.Email;
                parameters["@v_Description"] = bankBranch.Description == null ? DBNull.Value : (object)bankBranch.Description;
                parameters["@v_IsActive"] = bankBranch.IsActive = true;
                parameters["@v_CreatedAt"] = bankBranch.CreatedAt == null ? DateTime.Now : bankBranch.CreatedAt;
                parameters["@v_CreatedBy"] = bankBranch.CreatedBy == null ? DBNull.Value : (object)bankBranch.CreatedBy;
                parameters["@v_UpdatedAt"] = bankBranch.UpdatedAt == null ? DBNull.Value : (object)bankBranch.UpdatedAt;
                parameters["@v_UpdatedBy"] = bankBranch.UpdatedBy == null ? DBNull.Value : (object)bankBranch.UpdatedBy;
                string query = @"INSERT INTO BankBranch (Name,BankId,Address,BranchDetail,BussinessPhone,BussinessPhone1,MobilePhone,MobilePhone1,Email,Description,IsActive,CreatedAt,CreatedBy,UpdatedAt,UpdatedBy)
                                       VALUES(@v_Name,@v_BankId,@v_Address,@v_BarnchDetail,@v_BussinessPhone,@v_BussinessPhone1,@v_MobilePhone,@v_MobilePhone1,@v_Email,@v_Description,@v_IsActive,@v_CreatedAt,@v_CreatedBy,@v_UpdatedAt,@v_UpdatedBy)";
                var result = await Repository.NonQueryAsync(query,parameters:parameters);
                retVal = result > 0 ? true : false;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }
        /// <summary>
        /// Get All the Branche of All Banks etc...
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<BankBranchModel>> GetAllBankBranchesAsync()
        {
            var branches = new List<BankBranchModel>();
            try
            {
                string query = @"SELECT * FROM BankBranches WHERE IsActive = 1";
                var values = await Repository.QueryAsync(query);
                if (values != null || values?.Count > 0)
                {
                    foreach (var value in values)
                    {
                        var branch = new BankBranchModel();
                        branch.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                        branch.Name = value?.GetValueFromDictonary("Name")?.ToString();
                        branch.BussinessPhone = value?.GetValueFromDictonary("BussinessPhone")?.ToString();
                        branch.BussinessPhone1 = value?.GetValueFromDictonary("BussinessPhone1")?.ToString();
                        branch.MobilePhone = value?.GetValueFromDictonary("MobilePhone")?.ToString();
                        branch.MobilePhone1 = value?.GetValueFromDictonary("MobilePhone1")?.ToString();
                        branch.Address = value?.GetValueFromDictonary("Address")?.ToString();
                        branch.Email = value?.GetValueFromDictonary("Email")?.ToString();
                        branch.BarnchDetails = value?.GetValueFromDictonary("BranchDetails")?.ToString();
                        branch.Bank = new BankModel { Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt() };
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return branches;
        }
        /// <summary>
        /// Get All the Branches Of Specific Bank Like ABL,MCB etc...
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<IEnumerable<BankBranchModel>> GetBankBrachesByBankIdAsync(int? Id)
        {
            if (Id == null || Id == 0) return null;
            var branches = new List<BankBranchModel>();
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_Id"] = Id;
                string query = @"SELECT * FROM BankBranch WHERE BankId = @v_Id AND IsActive = 1";
                var values = await Repository.QueryAsync(query,parameters:parameters);
                if (values != null || values?.Count > 0)
                {
                    foreach (var value in values)
                    {
                        var branch = new BankBranchModel();
                        branch.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                        branch.Name = value?.GetValueFromDictonary("Name")?.ToString();
                        branch.BussinessPhone = value?.GetValueFromDictonary("BussinessPhone")?.ToString();
                        branch.BussinessPhone1 = value?.GetValueFromDictonary("BussinessPhone1")?.ToString();
                        branch.MobilePhone = value?.GetValueFromDictonary("MobilePhone")?.ToString();
                        branch.MobilePhone1 = value?.GetValueFromDictonary("MobilePhone1")?.ToString();
                        branch.Address = value?.GetValueFromDictonary("Address")?.ToString();
                        branch.Email = value?.GetValueFromDictonary("Email")?.ToString();
                        branch.BarnchDetails = value?.GetValueFromDictonary("BranchDetails")?.ToString();
                        branch.Bank = new BankModel { Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt() };
                        branches.Add(branch);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return branches;
        }
        #endregion
    }
}
