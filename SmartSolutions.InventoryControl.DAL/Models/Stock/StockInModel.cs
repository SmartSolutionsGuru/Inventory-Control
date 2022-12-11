using Fractions;
using SmartSolutions.InventoryControl.DAL.Managers.Purchase;
using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using SmartSolutions.InventoryControl.DAL.Models.Product;
using SmartSolutions.InventoryControl.DAL.Models.PurchaseOrder;
using SmartSolutions.InventoryControl.DAL.Models.Warehouse;
using SmartSolutions.Util.LogUtils;
using System;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Models.Stock
{
    /// <summary>
    /// Calss that is Responsible for Holding Inventory/Stock In
    /// </summary>
    public class StockInModel : BaseModel
    {
        #region [Private Members]
        private readonly DAL.Managers.Purchase.IPurchaseOrderDetailManager _purchaseOrderDetailManager;
        #endregion
        #region Constructor
        public StockInModel()
        {
            Partner = new BussinessPartnerModel();
            PurchaseOrder = new PurchaseOrderModel();
            PurchaseOrderDetail = new PurchaseOrderDetailModel();
            Warehouse = new WarehouseModel();
            _purchaseOrderDetailManager = new PurchaseOrderDetailManager();
        }
        #endregion

        #region Properties
        /// <summary>
        /// get or set Partner for Invoice
        /// </summary>
        public BussinessPartnerModel Partner { get; set; }
        public PurchaseOrderModel PurchaseOrder { get; set; }
        public PurchaseOrderDetailModel PurchaseOrderDetail { get; set; }
        private ProductModel _Product;

        public ProductModel Product
        {
            get { return _Product; }
            set { _Product = value; NotifyOfPropertyChange(nameof(Product)); GetProductLastPrice(Partner?.Id, Product?.Id);}
        }

        public int? PurchaseInvoiceId { get; set; }
        private int? _Quantity;
        public int? Quantity
        {
            get { return _Quantity; }
            set { _Quantity = value; NotifyOfPropertyChange(nameof(Quantity)); }
        }
        //private decimal? _Price;
        //public decimal? Price
        //{
        //    get { return _Price; }
        //    set { _Price = value; NotifyOfPropertyChange(nameof(Price)); OnPriceChange(); }
        //}

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

        /// <summary>
        /// gets or set Description
        /// </summary>
        public string Description { get; set; }
       
        private WarehouseModel _Warehouse;
        /// <summary>
        /// gets or set warehouse for Item
        /// </summary>
        public WarehouseModel Warehouse
        {
            get { return _Warehouse; }
            set { _Warehouse = value; NotifyOfPropertyChange(nameof(Warehouse)); }
        }


        private int _ProductLastPrice;
        /// <summary>
        /// gets or set product Last Price
        /// </summary>
        public int ProductLastPrice
        {
            get { return _ProductLastPrice; }
            set { _ProductLastPrice = value; NotifyOfPropertyChange(nameof(ProductLastPrice)); }
        }

        #endregion

        #region Private Helpers
        public async void GetProductLastPrice(int? partnerId, int? productId)
        {
            //null guard
            if (partnerId == null || productId == null || partnerId == 0 || productId == 0) return;
            try
            {
                ProductLastPrice = await _purchaseOrderDetailManager.GetProductLastPriceByPartnerAsync(partnerId, productId);
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }

        }
        private void OnPriceChange()
        {
            //if (Quantity == 0 || Quantity == null || Price == 0 || Price == null) return;
            if (Price == null)
                Price = 0;
            if(Quantity == null)
                Quantity = 0;
            Total = Quantity * (decimal)Price;

        }
        #endregion
    }
}
