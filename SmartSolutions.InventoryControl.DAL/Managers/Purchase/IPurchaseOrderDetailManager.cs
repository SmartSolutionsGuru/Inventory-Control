using SmartSolutions.InventoryControl.DAL.Models.PurchaseOrder;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Purchase
{
    public interface IPurchaseOrderDetailManager
    {
        Task<bool> AddPurchaseOrderDetailAsync(PurchaseOrderDetailModel orderDetail);
        Task<bool> AddPurchaseOrderBulkDetailAsync(List<PurchaseOrderDetailModel> orderDetails);
        Task<List<string>> GetOrderDetailIdByOrderIdAsync(int? orderId);
    }
}
