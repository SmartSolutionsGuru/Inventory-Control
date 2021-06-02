using SmartSolutions.InventoryControl.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Purchase
{
    public interface IPurchaseManager
    {
        Task<bool> AddPurchaseAsync(PurchaseModel model);
        Task<bool> AddPurchaseReturnAsync(PurchaseModel model);
    }
}
