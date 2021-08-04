using SmartSolutions.InventoryControl.DAL.Models.Product;

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
        public double Price { get; set; }
        public int Quantity { get; set; }
        public double Discount { get; set; }
        public double Total { get; set; }
        #endregion
    }
}
