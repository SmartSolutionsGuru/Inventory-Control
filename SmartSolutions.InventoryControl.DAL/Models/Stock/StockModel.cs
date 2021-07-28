using SmartSolutions.InventoryControl.DAL.Models.Inventory;
using SmartSolutions.InventoryControl.DAL.Models.Product;

namespace SmartSolutions.InventoryControl.DAL.Models.Stock
{
    public class StockModel : BaseModel
    {
        #region costructor
        public StockModel()
        {

        }
        #endregion

        #region Properties
        public ProductModel Product { get; set; }
        public StockInModel StockIn { get; set; }
        public StockOutModel StockOut { get; set; }
        public int StockQuantity { get; set; }
        #endregion
    }
}
