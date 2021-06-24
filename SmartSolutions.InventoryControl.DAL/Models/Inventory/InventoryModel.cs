using SmartSolutions.InventoryControl.DAL.Models.Product;
using SmartSolutions.Util.LogUtils;
using System;
using System.ComponentModel.Composition;

namespace SmartSolutions.InventoryControl.DAL.Models.Inventory
{
    [Export(typeof(InventoryModel)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InventoryModel : BaseModel
    {
        private readonly Managers.Inventory.IInventoryManager _inventoryManager;
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
        private async void GetStockInHand()
        {
            try
            {
                if (Product.Id == null || ProductColor.Id == null || ProductSize.Id == null) return;
                var resultStock  = await _inventoryManager.GetLastStockInHandAsync(Product, ProductColor, ProductSize);
                 StockInHand = resultStock.StockInHand;
                ProductLastPrice = resultStock.Price;
                IsProductSizeSelected = true;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
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
            set { _ProductSize = value; NotifyOfPropertyChange(nameof(ProductSize)); GetStockInHand(); }
        }

        private decimal _Price;

        public decimal Price
        {
            get { return _Price; }
            set { _Price = value; NotifyOfPropertyChange(nameof(Price)); OnPriceChange(); }
        }
        private int _Quantity;

        public int Quantity
        {
            get { return _Quantity; }
            set { _Quantity = value; NotifyOfPropertyChange(nameof(Quantity)); StockInHand -= Quantity; }
        }
        private double _Total;

        public double Total
        {
            get { return _Total = Quantity * (double)Price; }
            set { _Total = value = Quantity * (double)Price; NotifyOfPropertyChange(nameof(Total)); }
        }

        public bool IsStockIn { get; set; }
        public bool IsStockOut { get; set; }
        private decimal _ProductLastPrice;

        public decimal ProductLastPrice
        {
            get { return _ProductLastPrice; }
            set { _ProductLastPrice = value;  NotifyOfPropertyChange(nameof(ProductLastPrice)); }
        }


        private int _StockInHand;

        public int StockInHand
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
            Total = Quantity * (double)Price;

        }
        #endregion
    }
}
