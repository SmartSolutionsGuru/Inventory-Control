using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using SmartSolutions.InventoryControl.DAL.Models.Shipping;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSolutions.InventoryControl.DAL.Models.PurchaseOrder
{
    public class PurchaseOrderModel : BaseModel
    {
        public BussinessPartnerModel Partner { get; set; }
        /// <summary>
        /// Status Of Order Like Completed,NotComplete etc...
        /// </summary>
        public string Status { get; set; }
        public string Description { get; set; }
        public ShippingModel Shipping { get; set; }
        public decimal SubTotaL { get; set; }
        public decimal Discount { get; set; }
        public decimal GrandTotal { get; set; }
    }
}
