using SmartSolutions.InventoryControl.DAL.Models.Sales;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Sale
{
    public interface ISaleOrderDetailManager
    {
        Task<bool> AddSaleOrderDetailAsync(SaleOrderDetailModel orderDetail);
        Task<bool> AddSaleOrderBulkDetailAsync(List<SaleOrderDetailModel> orderDetails);
        Task<List<string>> GetOrderDetailIdByOrderIdAsync(int? orderId);
    }
}
