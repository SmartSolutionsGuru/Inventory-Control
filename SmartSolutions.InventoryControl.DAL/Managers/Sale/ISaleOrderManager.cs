using SmartSolutions.InventoryControl.DAL.Models.Sales;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Sale
{
    public interface ISaleOrderManager
    {
        Task<bool> CreateSaleOrderAsync(SaleOrderModel saleOrder);
        Task<int?> GetLastSaleOrderIdAsync();
        Task<SaleOrderModel> GetSaleOrderAsync(int? Id);
        /// <summary>
        /// Gets the Last price of specific product
        /// </summary>
        /// <param name="productId">product Id</param>
        /// <returns> return last sale  price of Product </returns>
        Task<int>GetProductLastPriceAsync(int? productId);
    }
}
