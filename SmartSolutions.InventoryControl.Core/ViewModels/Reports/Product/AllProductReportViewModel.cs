using SmartSolutions.InventoryControl.DAL.Managers.Product.ProductType;
using SmartSolutions.InventoryControl.DAL.Models.Product;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace SmartSolutions.InventoryControl.Core.ViewModels.Reports.Product
{
    [Export(typeof(AllProductReportViewModel)), PartCreationPolicy(CreationPolicy.Shared)]
    public class AllProductReportViewModel : BaseViewModel
    {
        #region [Private Members]
        private readonly IProductTypeManager _productTypeManager;
        private readonly DAL.Managers.Product.ProductSubType.IProductSubTypeManager _productSubTypeManager;
        private readonly DAL.Managers.Product.ProductColor.IProductColorManager _productColorManager;
        private readonly DAL.Managers.Product.ProductSize.IProductSizeManager _productSizeManager;
        private readonly DAL.Managers.Product.IProductManager _productManager;
        private readonly DAL.Managers.Stock.StockOut.IStockOutManager _stockOutManager;
        #endregion

        #region [Constructor]
        public AllProductReportViewModel()
        {

        }
        [ImportingConstructor]
        public AllProductReportViewModel(IProductTypeManager productTypeManager
                                        , DAL.Managers.Product.ProductSubType.IProductSubTypeManager productSubTypeManager
                                        , DAL.Managers.Product.ProductColor.IProductColorManager productColorManager
                                        , DAL.Managers.Product.ProductSize.IProductSizeManager productSizeManager
                                        , DAL.Managers.Product.IProductManager productManager
                                        , DAL.Managers.Stock.StockOut.IStockOutManager stockOutManager)
        {
            _productTypeManager = productTypeManager;
            _productSubTypeManager = productSubTypeManager;
            _productColorManager = productColorManager;
            _productSizeManager = productSizeManager;
            _productManager = productManager;
            _stockOutManager = stockOutManager;
            Products = new ObservableCollection<DisplayProduct>();
        }
        #endregion

        #region [Methods]
        protected override async void OnActivate()
        {
            base.OnActivate();
            Products.Clear();

            var resultProducts = (await _productManager.GetAllProductsAsync()).ToList();
            if (resultProducts != null && resultProducts.Count > 0)
            {
                foreach (var product in resultProducts)
                {
                    product.ProductColor = await _productColorManager.GetProductColorByIdAsync(product?.ProductColor?.Id.Value ?? 0);
                    product.ProductSize = await _productSizeManager.GetProductSizeByIdAsync(product?.ProductSize?.Id.Value ?? 0);
                    product.ProductType = await _productTypeManager.GetProductTypeByIdAsync(product?.ProductType?.Id.Value ?? 0);
                    product.ProductSubType = await _productSubTypeManager.GetProductSubTypeByIdAsync(product?.ProductSubType?.Id.Value ?? 0);
                    var displayProduct = new DisplayProduct
                    {
                        ProductName = product.Name,
                        ProductColor = product.ProductColor.Color,
                        ProductSize = product.ProductSize.Size,
                        ProductType = product.ProductType.Name,
                        ProductSubType = product.ProductSubType.Name
                    };
                    var resultQuantity = await _stockOutManager.GetStockInHandAsync(product.Id ?? 0);
                    if(resultQuantity < 0)
                        displayProduct.IsQuantityNegitive = true;
                    displayProduct.Quantity = resultQuantity ?? 0;
                    Products.Add(displayProduct);
                }
            }
        }
        #endregion

        #region [Properties]
        private ObservableCollection<DisplayProduct> _Products;
        /// <summary>
        /// List Of Products To Be Displayed
        /// </summary>
        public ObservableCollection<DisplayProduct> Products
        {
            get { return _Products; }
            set { _Products = value; NotifyOfPropertyChange(nameof(Products)); }
        }
        private DisplayProduct _SelectedProduct;
        /// <summary>
        /// Selected Product from the List
        /// </summary>
        public DisplayProduct SelectedProduct
        {
            get { return _SelectedProduct; }
            set { _SelectedProduct = value; NotifyOfPropertyChange(nameof(SelectedProduct)); }
        }

        #endregion

    }
    public class DisplayProduct
    {
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public string ProductSubType { get; set; }
        public string ProductColor { get; set; }
        public string ProductSize { get; set; }
        public bool IsQuantityNegitive { get; set; }
        public int Quantity { get; set; }
    }
}
