using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using SmartSolutions.InventoryControl.DAL.Models.Product;
using SmartSolutions.InventoryControl.DAL.Models.PurchaseOrder;
using SmartSolutions.InventoryControl.DAL.Models.Region;

namespace SmartSolutions.InventoryControl.DAL.Models.Warehouse
{
    public class WarehouseIssueModel : BaseModel
    {
        #region Constructor
        public WarehouseIssueModel()
        {
            Product = new ProductModel();
            Partner = new BussinessPartnerModel();
            PurchaseOrder = new PurchaseOrderModel();
            PurchaseOrderDetail = new PurchaseOrderDetailModel();
            City = new CityModel();
        }
        #endregion

        #region Properties
        public ProductModel Product { get; set; }
        public BussinessPartnerModel Partner { get; set; }
        public PurchaseOrderModel PurchaseOrder { get; set; }
        public PurchaseOrderDetailModel PurchaseOrderDetail { get; set; }
        public int InwardQuantity { get; set; }
        public int QuantityInHand { get; set; }
        public string Description { get; set; }
        public CityModel City { get; set; }
        #endregion
    }
}
