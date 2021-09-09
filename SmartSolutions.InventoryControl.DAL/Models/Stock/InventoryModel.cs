using SmartSolutions.InventoryControl.DAL.Models.Product;
using SmartSolutions.Util.LogUtils;
using System;
using System.ComponentModel.Composition;

namespace SmartSolutions.InventoryControl.DAL.Models.Inventory
{
    /// <summary>
    /// Class For Creation Inventory 
    /// </summary>
    [Export(typeof(InventoryModel)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InventoryModel : BaseModel
    {
        #region PrivateMembers
        private readonly Managers.Inventory.IInventoryManager _inventoryManager;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public InventoryModel()
        {

            Product = new ProductModel();
            ProductColor = new ProductColorModel();
            ProductSize = new ProductSizeModel();
            _inventoryManager = new Managers.Inventory.InventoryManager();

        }
        #endregion

        #region Private Methods
      
        #endregion

        #region Properties
        public string InvoiceId { get; set; }
        public Guid InvoiceGuid { get; set; }
        public ProductModel Product { get; set; }
        public ProductColorModel ProductColor { get; set; }

        private ProductSizeModel _ProductSize;

        public ProductSizeModel ProductSize
        {
            get { return _ProductSize; }
            set { _ProductSize = value; NotifyOfPropertyChange(nameof(ProductSize));}
        }

        private decimal? _Price;

        public decimal? Price
        {
            get { return _Price; }
            set { _Price = value; NotifyOfPropertyChange(nameof(Price)); OnPriceChange(); }
        }
        private int? _Quantity;

        public int? Quantity
        {
            get { return _Quantity; }
            set { _Quantity = value; NotifyOfPropertyChange(nameof(Quantity)); StockInHand -= Quantity; }
        }
        private decimal? _Total;

        public decimal? Total
        {
            get { return _Total = Quantity  * (decimal)Price.Value == null ? 0 : Price.Value; }
            set { _Total = value = Quantity ?? 0 * (decimal)Price; NotifyOfPropertyChange(nameof(Total)); }
        }

        public bool IsStockIn { get; set; }
        public bool IsStockOut { get; set; }
        private decimal _ProductLastPrice;

        public decimal ProductLastPrice
        {
            get { return _ProductLastPrice; }
            set { _ProductLastPrice = value;  NotifyOfPropertyChange(nameof(ProductLastPrice)); }
        }


        private int? _StockInHand;

        public int? StockInHand
        {
            get { return _StockInHand; }
            set { _StockInHand = value; NotifyOfPropertyChange(nameof(StockInHand)); }
        }

        private bool _IsProductSizeSelected;

        public bool IsProductSizeSelected
        {
            get { return _IsProductSizeSelected; }
            set { _IsProductSizeSelected = value; NotifyOfPropertyChange(nameof(IsProductSizeSelected)); }
        }

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
