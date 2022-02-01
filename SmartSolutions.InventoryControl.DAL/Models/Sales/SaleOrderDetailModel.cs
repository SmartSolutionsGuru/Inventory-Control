using SmartSolutions.InventoryControl.DAL.Models.Product;
using SmartSolutions.InventoryControl.DAL.Models.Warehouse;

namespace SmartSolutions.InventoryControl.DAL.Models.Sales
{
    public class SaleOrderDetailModel :BaseModel
    {
        #region Constructor
        public SaleOrderDetailModel()
        {
            SaleOrder = new SaleOrderModel();
            Product = new ProductModel();
        }
        #endregion

        #region Properties
        public SaleOrderModel SaleOrder { get; set; }
        public ProductModel Product { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public decimal? Discount { get; set; }
        public decimal? Total { get; set; }
        public WarehouseModel Warehouse { get; set; }
        #endregion
    }
}
