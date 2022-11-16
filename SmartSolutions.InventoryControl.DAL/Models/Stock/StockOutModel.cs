using SmartSolutions.InventoryControl.DAL.Managers.Stock.StockOut;
using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using SmartSolutions.InventoryControl.DAL.Models.Product;
using SmartSolutions.InventoryControl.DAL.Models.Sales;
using SmartSolutions.InventoryControl.DAL.Models.Warehouse;
using SmartSolutions.Util.LogUtils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartSolutions.InventoryControl.DAL.Models.Inventory
{
    /// <summary>
    /// Class that is Responsible for holding Inventory/Stock Out
    /// </summary>
    public class StockOutModel : BaseModel
    {

        #region Private Members
        private readonly DAL.Managers.Stock.StockOut.IStockOutManager _stockOutManager;
        private readonly DAL.Managers.Warehouse.IWarehouseManager _warehouseManager;
        private readonly DAL.Managers.Sale.ISaleOrderManager _saleOrderManager;
        #endregion

        #region Constructor
        public StockOutModel()
        {
            Partner = new BussinessPartnerModel();
            SaleOrder = new SaleOrderModel();
            SaleOrderDetail = new SaleOrderDetailModel();
            Product = new ProductModel();
            Warehouses = new List<WarehouseModel>();
            SelectedWarehouse = new WarehouseModel();
            _stockOutManager = new StockOutManager();
            _warehouseManager = new DAL.Managers.Warehouse.WarehouseManager();
            _saleOrderManager = new DAL.Managers.Sale.SaleOrderManager();
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
        private int _productLastPrice;

        public int ProductLastPrice
        {
            get { return _productLastPrice; }
            set { _productLastPrice = value; NotifyOfPropertyChange(nameof(ProductLastPrice)); }
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
        //public List<WarehouseModel> Warehouse { get; set; }
        private List<WarehouseModel> _Warehouses;
        /// <summary>
        /// List Of Warehouse in Which Stock Is Available
        /// </summary>
        public List<WarehouseModel> Warehouses
        {
            get { return _Warehouses; }
            set { _Warehouses = value; NotifyOfPropertyChange(nameof(Warehouses)); }
        }

        private WarehouseModel _SelectedWarehouse;
        /// <summary>
        /// Selected Warehouse from which Stock Is Selected
        /// </summary>
        public WarehouseModel SelectedWarehouse
        {
            get { return _SelectedWarehouse; }
            set { _SelectedWarehouse = value; NotifyOfPropertyChange(nameof(SelectedWarehouse)); }
        }

        #region Calculated Properties

        /// <summary>
        /// Calculated Property That Will Get the Stock In Hand only
        /// </summary>
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

        /// <summary>
        /// TODO: Calculate the Warehouse Which Has the Quantity So Select That
        /// </summary>
        /// <param name="productId"></param>
        public async void GetProductAvailableStock(int? productId)
        {
            try
            {
                if (productId == null || productId == 0) return;
                var availableStock = await _stockOutManager.GetStockInHandAsync(productId);              
                StockInHand = availableStock.Value;
                ProductLastPrice = await _saleOrderManager.GetProductLastPriceAsync(productId);
                Warehouses = (await _warehouseManager.GetAllWarehouseByProductId(productId ?? 0)).ToList();

                SelectedWarehouse = Warehouses.FirstOrDefault();
                NotifyOfPropertyChange(nameof(StockInHand));
                NotifyOfPropertyChange(nameof(Warehouses));
                NotifyOfPropertyChange(nameof(SelectedWarehouse));
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        #endregion

    }
}
