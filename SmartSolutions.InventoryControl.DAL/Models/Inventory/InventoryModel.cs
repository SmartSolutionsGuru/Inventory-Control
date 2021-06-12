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
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public bool IsStockIn { get; set; }
        public bool IsStockOut { get; set; }
        public int StockInHand { get; set; }
        public double Total
        {
            get => Quantity * (double)Price;
            set { value = Quantity * (double)Price;}
        }
        private string _TotalPrice;
        public string TotalPrice
        {
            get { return Total.ToString(); }
            set
            {
                try
                {
                     _TotalPrice = value;
                    Total = Convert.ToDouble(_TotalPrice);
                }
                catch 
                {
                    Total = 0;
                    NotifyOfPropertyChange(nameof(TotalPrice));
                } 
              
            }
        }
        #endregion
    }
}
