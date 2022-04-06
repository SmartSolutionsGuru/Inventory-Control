using SmartSolutions.InventoryControl.DAL.Models.Bank;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Bank
{
    public interface IBankAccountManager
    {
        Task<bool> AddBankAccountAsync(BankAccountModel bankAccount);
        Task<IEnumerable<BankAccountModel>> GetAllBankAccountsAsync();
        /// <summary>
        /// Get All Bank Accounts From specific branch
        /// </summary>
        /// <param name="branchId">Branch Id</param>
        /// <returns></returns>
        Task<IEnumerable<BankAccountModel>> GetAllBankAccountByBranchAsync(int? branchId);
        /// <summary>
        /// Verify That Already BankAccount Exist
        /// </summary>
        /// <param name="branchId"> Id Of Specific Branch</param>
        /// <param name="accountNumber">Uniq Account Number Of Bank</param>
        /// <returns></returns>
        Task<bool> IsAccountAlreadyExist(int? branchId, string accountNumber);
        /// <summary>
        /// Adding Bank Transaction Like DR Or CR from 
        /// </summary>
        /// <param name="account">Account Model</param>
        /// <returns></returns>
        Task<bool> AddBankTransactionAsync(BankAccountModel account);
       
    }
}
