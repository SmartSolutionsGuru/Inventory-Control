using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using SmartSolutions.InventoryControl.DAL.Models.Product;
using System;

namespace SmartSolutions.InventoryControl.DAL.Models.PurchaseOrder
{
    public class PurchaseReturnInvoiceModel : BaseModel
    {
        #region Constructor
        public void PurchaseReturnModel()
        {
            Partner = new BussinessPartnerModel();

        }
        #endregion

        #region Properties
        public ProductModel Product { get; set; }
        /// <summary>
        /// Purchase Invoice Id 
        /// </summary>
        public string PurchaseInvoiceId { get; set; }
        public Guid PurchaseReturnGuid { get; set; }
        public BussinessPartnerModel Partner { get; set; }
        public string Description { get; set; }
        public decimal Total { get; set; }
        #endregion
    }
}
