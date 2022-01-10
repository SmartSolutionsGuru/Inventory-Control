using SmartSolutions.InventoryControl.DAL.Managers.Stock.StockOut;
using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using SmartSolutions.InventoryControl.DAL.Models.Product;
using SmartSolutions.InventoryControl.DAL.Models.Sales;
using SmartSolutions.InventoryControl.DAL.Models.Warehouse;
using SmartSolutions.Util.LogUtils;
using System;

namespace SmartSolutions.InventoryControl.DAL.Models.Inventory
{
    /// <summary>
    /// Class that is Responsible for holding Inventory/Stock Out
    /// </summary>
    public class StockOutModel : BaseModel
    {

        #region Private Members
        private readonly DAL.Managers.Stock.StockOut.IStockOutManager _stockOutManager;
        #endregion

        #region Constructor
        public StockOutModel()
        {
            Partner = new BussinessPartnerModel();
            SaleOrder = new SaleOrderModel();
            SaleOrderDetail = new SaleOrderDetailModel();
            Product = new ProductModel();
            Warehouse = new WarehouseModel();
            _stockOutManager = new StockOutManager();
    }
        #endregion

        #region Properties
        public BussinessPartnerModel Partner { get; set; }
        public SaleOrderModel SaleOrder { get; set; }
        public SaleOrderDetailModel SaleOrderDetail { get; set; }
        //public ProductModel Product { get; set; }
        private ProductModel _Product;
        public ProductModel Product
        {
            get { return _Product; }
            set { _Product = value; GetProductAvailableStock(Product?.Id); }
        }
        public int? SaleInvoiceId { get; set; }
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
        //private int? _StockInHand;
        //public int? StockInHand
        //{
        //    get { return _StockInHand; }
        //    set { _StockInHand = value; NotifyOfPropertyChange(nameof(StockInHand));  }
        //}
        public int? StockInHand { get; set; }
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

        public async void GetProductAvailableStock(int? productId)
        {
            try
            {
                if (productId == null || productId == 0) return;
                var availableStock = await _stockOutManager.GetStockInHandAsync(productId);
                StockInHand = availableStock.Value;
                NotifyOfPropertyChange(nameof(StockInHand));
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        #endregion

    }
}
