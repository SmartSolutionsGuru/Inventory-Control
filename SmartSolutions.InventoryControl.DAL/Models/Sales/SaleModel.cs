using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSolutions.InventoryControl.DAL.Models.Sales
{
    public class SaleModel : BaseModel
    {
        #region Constructor
        public SaleModel()
        {
            Product = new Product.ProductModel();
            ProductColor = new Product.ProductColorModel();
            ProductSize = new Product.ProductSizeModel();
        }
        #endregion

        #region Properties
        public Models.Product.ProductModel Product { get; set; }
        public Product.ProductColorModel ProductColor { get; set; }
        public Product.ProductSizeModel ProductSize { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double Total { get; set; }
        #endregion
    }
}
