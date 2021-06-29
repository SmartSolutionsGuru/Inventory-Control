using SmartSolutions.InventoryControl.DAL.Models.Transaction;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Transaction
{
    public interface ITransactionManager
    {
        /// <summary>
        /// Save Transaction
        /// </summary>
        /// <returns></returns>
        Task<bool> SaveTransactionAsync(TransactionModel transaction);
    }
}
