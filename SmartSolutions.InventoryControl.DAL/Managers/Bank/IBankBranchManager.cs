using SmartSolutions.InventoryControl.DAL.Models.Bank;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Bank
{
    public interface IBankBranchManager
    {
        Task<IEnumerable<BankBranchModel>> GetAllBankBranchesAsync();
        Task<IEnumerable<BankBranchModel>> GetBankBrachesByBankIdAsync(int? Id);
        Task<bool> AddBankBranchAsync(BankBranchModel bankBranch);
    }
}
