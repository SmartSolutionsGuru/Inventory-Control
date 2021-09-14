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
        private int? _Quantity;
        public int? Quantity
        {
            get { return _Quantity; }
            set { _Quantity = value; NotifyOfPropertyChange(nameof(Quantity)); }
        }
        private decimal? _Price;
        public decimal? Price
        {
            get { return _Price; }
            set { _Price = value; NotifyOfPropertyChange(nameof(Price)); OnPriceChange(); }
        }

        private decimal? _Total;
        public decimal? Total
        {
            get { return _Total = (Price ?? 0) * (Quantity ?? 0); }
            set { _Total = value; NotifyOfPropertyChange(nameof(Total)); }
        }
        public string Description { get; set; }
        public WarehouseModel Warehouse { get; set; }

        #region Calculated Properties
        /// <summary>
        /// Calculated Property That Will Get the Stock In Hand only
        /// </summary>
        private int _StockInHand;
        public int StockInHand
        {
            get { return _StockInHand; }
            set { _StockInHand = value; NotifyOfPropertyChange(nameof(StockInHand)); }
        }
        private ProductColorModel _ProductColor;

        public ProductColorModel ProductColor
        {
            get { return _ProductColor; }
            set { _ProductColor = value; NotifyOfPropertyChange(nameof(ProductColor)); }
        }
        private ProductSizeModel _ProductSize;

        public ProductSizeModel ProductSize
        {
            get { return _ProductSize; }
            set { _ProductSize = value; NotifyOfPropertyChange(nameof(ProductSize)); }
        }

        #endregion

        #endregion

        #region Private Helpers
        private void OnPriceChange()
        {
            if (Quantity == 0 || Price == 0) return;
            Total = Quantity * (decimal)Price;

        }
        #endregion

    }
}
