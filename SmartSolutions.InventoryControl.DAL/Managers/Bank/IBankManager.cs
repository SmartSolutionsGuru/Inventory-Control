using SmartSolutions.InventoryControl.DAL.Models.Bank;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Bank
{
    public interface IBankManager
    {
        Task<IEnumerable<BankModel>> GetAllBanksAsync();
        Task<BankModel> GetBankByIdAsync(int? Id);
        Task<bool> AddBankAsync(BankModel bank);
        Task<bool> UpdateBankAsync(int? bank);
        Task<bool> RemoveBankAsync(int? Id);
    }
}
