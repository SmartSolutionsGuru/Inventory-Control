using SmartSolutions.InventoryControl.DAL.Models.Product;
using SmartSolutions.InventoryControl.DAL.Models.Warehouse;

namespace SmartSolutions.InventoryControl.DAL.Models.PurchaseOrder
{
    public class PurchaseOrderDetailModel : BaseModel
    {
        #region Constructor
        public PurchaseOrderDetailModel()
        {
            PurchaseOrder = new PurchaseOrderModel();
            Product = new ProductModel();
            ProductColor = new ProductColorModel();
            ProductSize = new ProductSizeModel();
        }
        #endregion

        #region Properties
        public PurchaseOrderModel PurchaseOrder { get; set; }
        public ProductModel Product { get; set; }
        public ProductColorModel ProductColor { get; set; }
        public ProductSizeModel ProductSize { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public decimal? Discount { get; set; }
        public decimal? Total { get; set; }
        public WarehouseModel  Warehouse { get; set; }
        #endregion
    }
}
