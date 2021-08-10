using SmartSolutions.InventoryControl.DAL.Models.Stock;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Stock.OpeningStock
{
    public interface IOpeningStockManager
    {
        Task<bool> AddOpeningStockAsync(OpeningStockModel openingStock);
        
    }
}
