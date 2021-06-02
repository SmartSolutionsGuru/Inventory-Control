using SmartSolutions.InventoryControl.DAL.Models.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSolutions.InventoryControl.DAL.Models
{
    public class PurchaseModel : BaseModel
    {
        public PurchaseModel()
        {
            PaymentMode = new List<string> {"Cash","Bank","JazzCash", "Partial","U Paias","Easy Paisa","Other"};
        }
        public ProductModel Product { get; set; }
        public BussinessPartnerModel Vender { get; set; }
        public ProductColorModel ProductColor { get; set; }
        public ProductSizeModel ProductSize { get; set; }
        public List<string> PaymentMode { get; set; }
        public string SelectedPaymentMode { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }

        private byte[] _PaymentImage;

        public byte[] PaymentImage
        {
            get { return _PaymentImage; }
            set { _PaymentImage = value; NotifyOfPropertyChange(nameof(PaymentImage)); }
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
        public int Quantity { get; set; }
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
        public double Total
        {
            get => Quantity * Price;
            set { value = Quantity * Price;}
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
    }
    public enum PaymentMode
    {
        None = 0,
        Cah = 1,
        Bank =2,
        JazzCash =3,
        Partial = 4,
        Other = 5
    }
}
