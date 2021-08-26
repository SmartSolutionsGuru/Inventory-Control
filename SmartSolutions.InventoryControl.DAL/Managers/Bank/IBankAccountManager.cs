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
       
    }
}
