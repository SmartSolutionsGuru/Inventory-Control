using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using SmartSolutions.InventoryControl.DAL.Models.Product;
using SmartSolutions.InventoryControl.DAL.Models.PurchaseOrder;
using SmartSolutions.InventoryControl.DAL.Models.Warehouse;

namespace SmartSolutions.InventoryControl.DAL.Models.Stock
{
    /// <summary>
    /// Calss that is Responsible for Holding Inventory/Stock In
    /// </summary>
    public class StockInModel : BaseModel
    {
        #region Constructor
        public StockInModel()
        {
            Partner = new BussinessPartnerModel();
            PurchaseOrder = new PurchaseOrderModel();
            PurchaseOrderDetail = new PurchaseOrderDetailModel();
            Warehouse = new WarehouseModel();
        }
        #endregion

        #region Properties
        public BussinessPartnerModel Partner { get; set; }
        public PurchaseOrderModel PurchaseOrder { get; set; }
        public PurchaseOrderDetailModel PurchaseOrderDetail { get; set; }
        public ProductModel Product { get; set; }
        public int? PurchaseInvoiceId { get; set; }
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
