using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using SmartSolutions.InventoryControl.DAL.Models.Product;
using SmartSolutions.InventoryControl.DAL.Models.PurchaseOrder;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSolutions.InventoryControl.DAL.Models
{
    public class WarehouseModel : BaseModel
    {
        #region Properties
        public ProductModel Product { get; set; }
        public BussinessPartnerModel Partner { get; set; }
        public PurchaseOrderModel PurchaseOrder { get; set; }
        public PurchaseOrderDetailModel PurchaseOrderDetail { get; set; }
        public int InwardQuantity { get; set; }
        public int QuantityInHand { get; set; }
        public string Description { get; set; }
        public int CityId { get; set; }
        #endregion
    }
}
