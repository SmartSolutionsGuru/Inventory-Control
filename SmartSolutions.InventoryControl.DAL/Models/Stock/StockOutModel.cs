using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using SmartSolutions.InventoryControl.DAL.Models.Product;
using SmartSolutions.InventoryControl.DAL.Models.Sales;
using SmartSolutions.InventoryControl.DAL.Models.Warehouse;

namespace SmartSolutions.InventoryControl.DAL.Models.Inventory
{
    /// <summary>
    /// Class that is Responsible for holding Inventory/Stock Out
    /// </summary>
    public class StockOutModel : BaseModel
    {
        #region Constructor
        public StockOutModel()
        {
            Partner = new BussinessPartnerModel();
            SaleOrder = new SaleOrderModel();
            SaleOrderDetail = new SaleOrderDetailModel();
            Product = new ProductModel();
            Warehouse = new WarehouseModel();
        }
        #endregion

        #region Properties
        public BussinessPartnerModel Partner { get; set; }
        public SaleOrderModel SaleOrder { get; set; }
        public SaleOrderDetailModel SaleOrderDetail { get; set; }
        public ProductModel Product { get; set; }
        public int? Quantity { get; set; }
        public double? Price { get; set; }
        public decimal? Total { get; set; }
        public string Description { get; set; }
        public WarehouseModel Warehouse { get; set; }
        #endregion

    }
}
