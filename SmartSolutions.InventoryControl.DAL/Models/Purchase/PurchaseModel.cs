using SmartSolutions.InventoryControl.DAL.Models.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSolutions.InventoryControl.DAL.Models.Purchase
{
    public class PurchaseModel : BaseModel
    {
        #region Constructor
        public PurchaseModel()
        {
            Product = new ProductModel();
            ProductColor = new ProductColorModel();
            ProductSize = new ProductSizeModel();
        }
        #endregion

        #region Properties
        //public ProductModel Product { get; set; }
        private ProductModel _Product;

        public ProductModel Product
        {
            get { return _Product; }
            set { _Product = value; NotifyOfPropertyChange(nameof(Product)); }
        }

        public ProductColorModel ProductColor { get; set; }
        public ProductSizeModel ProductSize { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public double Total
        {
            get => Quantity * Price;
            set { value = Quantity * Price;}
        }
        public string ProductPrice 
        {
            get => Price.ToString();
            set
            {
                try
                {
                    Price = double.Parse(value);
                }
                catch 
                {
                    Price = 0;
                }
               
            }
        }
        public string ProductQuantity 
        {
            get => Quantity.ToString();
            set 
            {
                try
                {
                    Quantity = Int32.Parse(value);
                }
                catch 
                {
                    Quantity = 0;
                }
               
            }
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
