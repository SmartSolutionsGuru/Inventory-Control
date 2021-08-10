using SmartSolutions.InventoryControl.DAL.Models.Product;
using SmartSolutions.InventoryControl.DAL.Models.Warehouse;

namespace SmartSolutions.InventoryControl.DAL.Models.Stock
{
    public class ClosingStockModel : BaseModel
    {
        #region Constructor
        public ClosingStockModel()
        {
            Product = new ProductModel();
            Warehouse = new WarehouseModel();
        }
        #endregion

        #region Properties
        public ProductModel Product { get; set; }
        public WarehouseModel Warehouse { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public string Description { get; set; }
        #endregion
    }
}
