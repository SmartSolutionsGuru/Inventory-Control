using SmartSolutions.InventoryControl.DAL.Models.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSolutions.InventoryControl.DAL.Models.Inventory
{
    public class InventoryModel : BaseModel
    {
        #region Constructor
        public InventoryModel()
        {

            Product = new ProductModel();
            ProductColor = new ProductColorModel();
            ProductSize = new ProductSizeModel();
        }
        #endregion                                                  

        #region Properties
        public string InvoiceId { get; set; }
        public Guid InvoiceGuid { get; set; }
        public ProductModel Product { get; set; }
        public ProductColorModel ProductColor { get; set; }
        public ProductSizeModel ProductSize { get; set; }
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
            set { _Quantity = value; NotifyOfPropertyChange(nameof(Quantity)); }
        }
        private double _Total;

        public double Total
        {
            get { return _Total = Quantity * (double)Price; }
            set { _Total = value = Quantity * (double)Price; NotifyOfPropertyChange(nameof(Total)); }
        }

        public bool IsStockIn { get; set; }
        public bool IsStockOut { get; set; }
        public int StockInHand { get; set; }
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
