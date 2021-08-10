using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using SmartSolutions.InventoryControl.DAL.Models.Product;
using SmartSolutions.InventoryControl.DAL.Models.PurchaseOrder;
using SmartSolutions.InventoryControl.DAL.Models.Warehouse;

namespace SmartSolutions.InventoryControl.DAL.Models.Stock
{
    /// <summary>
    /// Calss that is Responsible for Holding Inventory/Stock In
    /// </summary>
    public class StockInModel : BaseModel
    {
        #region Constructor
        public StockInModel()
        {
            Partner = new BussinessPartnerModel();
            PurchaseOrder = new PurchaseOrderModel();
            PurchaseOrderDetail = new PurchaseOrderDetailModel();
            Warehouse = new WarehouseModel();
        }
        #endregion

        #region Properties
        public BussinessPartnerModel Partner { get; set; }
        public PurchaseOrderModel PurchaseOrder { get; set; }
        public PurchaseOrderDetailModel PurchaseOrderDetail { get; set; }
        public ProductModel Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public string Description { get; set; }
        public WarehouseModel Warehouse { get; set; }
        #endregion
    }
}
