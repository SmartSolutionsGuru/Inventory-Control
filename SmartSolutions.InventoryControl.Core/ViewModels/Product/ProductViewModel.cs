using Caliburn.Micro;
using SmartSolutions.InventoryControl.DAL.Models.Product;
using SmartSolutions.Util.LogUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace SmartSolutions.InventoryControl.Core.ViewModels.Product
{
    [Export(typeof(ProductViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ProductViewModel : BaseViewModel
    {

        #region Private Members
        private readonly DAL.Managers.Product.ProductType.IProductTypeManager _productTypeManager;
        private readonly DAL.Managers.Product.ProductSubType.IProductSubTypeManager _productSubTypeManager;
        private readonly DAL.Managers.Product.ProductColor.IProductColorManager _productColorManager;
        private readonly DAL.Managers.Product.ProductSize.IProductSizeManager _productSizeManager;
        private readonly DAL.Managers.Product.IProductManager _productManager;

        #endregion

        #region Constructor
        [ImportingConstructor]
        public ProductViewModel(DAL.Managers.Product.ProductType.IProductTypeManager productTypeManager,
                                DAL.Managers.Product.ProductSubType.IProductSubTypeManager productSubTypeManager,
                                DAL.Managers.Product.ProductColor.IProductColorManager productColorManager,
                                DAL.Managers.Product.ProductSize.IProductSizeManager productSizeManager,
                                DAL.Managers.Product.IProductManager productManager)
        {
            IsAddProductPressed = true;
            _productTypeManager = productTypeManager;
            _productManager = productManager;
            _productSubTypeManager = productSubTypeManager;
            _productSizeManager = productSizeManager;
            _productColorManager = productColorManager;
        }
        #endregion

        #region Public Methods
        protected async override void OnActivate()
        {
            base.OnActivate();
            ProductTypes = (await _productTypeManager.GetAllProductsTypesAsync()).ToList();
            ProductSizes = (await _productSizeManager.GetProductAllSizeAsync()).ToList();
            ProductColors = (await _productColorManager.GetProductAllColorsAsync()).ToList();
        }

        #region Product Type
        public void OpenAddProductType()
        {
            IsAddProduct = true;
        }
        public void RemoveProductType()
        {
            try
            {

            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        public async void AddProductType()
        {
            try
            {
                ProductTypeModel model = new ProductTypeModel();
                model.Name = ProductType;
                if (ProductType != null)
                    await _productTypeManager.AddProductTypeAsync(model);
                IsAddProduct = false;
                ProductType = string.Empty;
                ProductTypes = (await _productTypeManager.GetAllProductsTypesAsync()).ToList();

            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        public void CloseAddProductType()
        {
            IsAddProduct = false;
        }
        #endregion

        #region Product Sub Type
        public void OpenAddProductSubType()
        {
            try
            {
                IsAddSubProduct = true;
                if (SelectedProductType != null)
                    ProductSubTypeFooter = "This Product Sub Type is Child of Selected Product Type";
                else
                    ProductSubTypeFooter = "Please Select Product Type First for Adding SubType";
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        public async void RemoveProductSubType()
        {
            try
            {
                await _productSubTypeManager.RemoveProductSubTypeAsync(SelectedProductSubType.Id);
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        public async void AddProductSubType()
        {
            try
            {
                if (!string.IsNullOrEmpty(ProductSubType))
                {
                    ProductSubTypeModel model = new ProductSubTypeModel();
                    model.Name = ProductSubType;
                    await _productSubTypeManager.AddProductSubTypeAsync(SelectedProductType.Id, model);
                    IsAddSubProduct = false;
                    ProductSubType = string.Empty;
                    ProductSubTypes = (await _productSubTypeManager.GetAllProductSubTypeAsync(SelectedProductType.Id)).ToList();
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        public void CloseAddProductSubType()
        {
            IsAddSubProduct = false;
        }
        #endregion

        #region Product Color
        public void OpenAddColors()
        {
            IsAddProductColor = true;
        }
        public void RemoveProductColors()
        {
            _productColorManager.RemoveProductColorAsync(ProductSelectedColor?.Id);
        }
        public async void AddProductColors()
        {
            try
            {
                var model = new ProductColorModel();
                if (!string.IsNullOrEmpty(ProductColor))
                {
                    model.Color = ProductColor;
                }
                await _productColorManager.AddProductColorAsync(model);
                IsAddProductColor = false;
                ProductColors = (await _productColorManager.GetProductAllColorsAsync()).ToList();
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        #endregion

        #region Product Size
        public void OpenAddProductSize()
        {
            IsAddProductSize = true;
        }
        public void RemoveProductSize()
        {
            _productSizeManager.RemoveProductSizeAsync(ProductSelectedSize.Id);
        }
        public async void AddProductSize()
        {
            var model = new ProductSizeModel();
            try
            {
                if (!string.IsNullOrEmpty(ProductSize))
                    model.Size = ProductSize;
                await _productSizeManager.AddProductSizeAsync(model);
                IsAddProductSize = false;
                ProductSizes = (await _productSizeManager.GetProductAllSizeAsync()).ToList();

            }
            catch (Exception ex)
            {

                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        #endregion

        #region Product
        public async void SaveProduct(ProductModel model)
        {
            try
            {
                if (model == null)
                    model = new ProductModel();
                VerifyIsDataFilled();
                model.Name = ProductName;
                model.ProductType = SelectedProductType;
                model.ProductSubType = SelectedProductSubType;
                model.ProductColor = ProductSelectedColor;
                model.ProductSize = ProductSelectedSize;
                model.Image = ProductImage;
                bool result = await _productManager.AddProductAsync(model);
                if (result)
                {
                    SelectedProductType = null;
                    SelectedProductSubType = null;
                    ProductSelectedColor = null;
                    ProductSelectedSize = null;
                    ProductImage = null;
                    ProductName = string.Empty;

                }
                else
                {
                    //Friendly Message Product is Not save Try Again
                    await IoC.Get<IDialogManager>().ShowMessageBoxAsync("Sorry Product is Not Saved Please Try Again", options: Dialogs.MessageBoxOptions.Ok);
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        #endregion

        #region Private Helpers
        private void VerifyIsDataFilled()
        {
            try
            {
                if (string.IsNullOrEmpty(ProductName))
                    IoC.Get<IDialogManager>().ShowMessageBoxAsync("Please Enter the Product Name", options: Dialogs.MessageBoxOptions.Ok);
                if(SelectedProductType == null)
                    IoC.Get<IDialogManager>().ShowMessageBoxAsync("Please Select the Product Type", options: Dialogs.MessageBoxOptions.Ok);
                if (SelectedProductSubType == null)
                    IoC.Get<IDialogManager>().ShowMessageBoxAsync("Please Select the Product Sub Type", options: Dialogs.MessageBoxOptions.Ok);
                if (ProductSelectedColor == null)
                    IoC.Get<IDialogManager>().ShowMessageBoxAsync("Please Select the Product Color", options: Dialogs.MessageBoxOptions.Ok);
                if (ProductSelectedSize == null)
                    IoC.Get<IDialogManager>().ShowMessageBoxAsync("Please Select the Product Size", options: Dialogs.MessageBoxOptions.Ok);

            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        #endregion
        public void Clear()
        {
            TryClose();
        }

        public List<ProductSubTypeModel> OnProductTypeSelection(int Id)
        {
            try
            {
                ProductSubTypes = (_productSubTypeManager.GetAllProductSubType(Id)).ToList();
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return ProductSubTypes;
        }
        #endregion

        #region Properties
        private bool _IsAddProductPressed;
        /// <summary>
        /// Verify this button is Pressed
        /// </summary>
        public bool IsAddProductPressed
        {
            get { return _IsAddProductPressed; }
            set { _IsAddProductPressed = value; NotifyOfPropertyChange(nameof(IsAddProductPressed)); }
        }
        private bool _IsUpdateProduct;
        /// <summary>
        /// Product Update Button Clicked
        /// </summary>
        public bool IsUpdateProduct
        {
            get { return _IsUpdateProduct; }
            set { _IsUpdateProduct = value; NotifyOfPropertyChange(nameof(IsUpdateProduct)); }
        }
        private bool _IsRemoveProduct;
        /// <summary>
        /// Remove Product Button Clicked
        /// </summary>
        public bool IsRemoveProduct
        {
            get { return _IsRemoveProduct; }
            set { _IsRemoveProduct = value; NotifyOfPropertyChange(nameof(IsRemoveProduct)); }
        }


        private string _ProductSize;
        /// <summary>
        /// New Product Size Addition
        /// </summary>
        public string ProductSize
        {
            get { return _ProductSize; }
            set { _ProductSize = value; NotifyOfPropertyChange(nameof(ProductSize)); }
        }

        private string _ProductColor;
        /// <summary>
        /// Adding The Product Color
        /// </summary>
        public string ProductColor
        {
            get { return _ProductColor; }
            set { _ProductColor = value; NotifyOfPropertyChange(nameof(ProductColor)); }
        }

        private string _ProductSubTypeFooter;
        /// <summary>
        /// Product sub Type Footer for Information
        /// </summary>
        public string ProductSubTypeFooter
        {
            get { return _ProductSubTypeFooter; }
            set { _ProductSubTypeFooter = value; NotifyOfPropertyChange(nameof(ProductSubTypeFooter)); }
        }



        private List<ProductTypeModel> _ProductTypes;
        /// <summary>
        /// List of Product types
        /// </summary>
        public List<ProductTypeModel> ProductTypes
        {
            get { return _ProductTypes; }
            set { _ProductTypes = value; NotifyOfPropertyChange(nameof(ProductTypes)); }
        }

        private ProductTypeModel _SelectedProductType;
        /// <summary>
        /// Selected Product Type
        /// </summary>
        public ProductTypeModel SelectedProductType
        {
            get { return _SelectedProductType; }
            set { _SelectedProductType = value; NotifyOfPropertyChange(nameof(SelectedProductType)); OnProductTypeSelection(SelectedProductType.Id.Value); }
        }

        private List<ProductSubTypeModel> _ProductSubTypes;
        /// <summary>
        /// SubTypes for Product
        /// </summary>
        public List<ProductSubTypeModel> ProductSubTypes
        {
            get { return _ProductSubTypes; }
            set { _ProductSubTypes = value; NotifyOfPropertyChange(nameof(ProductSubTypes)); }
        }

        private ProductSubTypeModel _SelectedProductSubType;
        /// <summary>
        /// Selected SubType of Product
        /// </summary>
        public ProductSubTypeModel SelectedProductSubType
        {
            get { return _SelectedProductSubType; }
            set { _SelectedProductSubType = value; NotifyOfPropertyChange(nameof(SelectedProductSubType)); }
        }

        private string _ProductSubType;
        /// <summary>
        /// Text for Product Sub Type
        /// </summary>
        public string ProductSubType
        {
            get { return _ProductSubType; }
            set { _ProductSubType = value; NotifyOfPropertyChange(nameof(ProductSubType)); }
        }

        private string _ProductName;
        /// <summary>
        /// Product Name 
        /// </summary>
        public string ProductName
        {
            get { return _ProductName; }
            set { _ProductName = value; NotifyOfPropertyChange(nameof(ProductName)); }
        }
        private List<ProductColorModel> _ProductColors;
        /// <summary>
        /// Colors for Product
        /// </summary>
        public List<ProductColorModel> ProductColors
        {
            get { return _ProductColors; }
            set { _ProductColors = value; NotifyOfPropertyChange(nameof(ProductColors)); }
        }
        private ProductColorModel _ProductSelectedColor;
        /// <summary>
        /// Product Selected Color
        /// </summary>
        public ProductColorModel ProductSelectedColor
        {
            get { return _ProductSelectedColor; }
            set { _ProductSelectedColor = value; NotifyOfPropertyChange(nameof(ProductSelectedColor)); }
        }
        private List<ProductSizeModel> _ProductSizes;
        /// <summary>
        /// Product size List
        /// </summary>
        public List<ProductSizeModel> ProductSizes
        {
            get { return _ProductSizes; }
            set { _ProductSizes = value; NotifyOfPropertyChange(nameof(ProductSizes)); }
        }

        private ProductSizeModel _ProductSelectedSize;
        /// <summary>
        /// Selected Product Size
        /// </summary>
        public ProductSizeModel ProductSelectedSize
        {
            get { return _ProductSelectedSize; }
            set { _ProductSelectedSize = value; NotifyOfPropertyChange(nameof(ProductSelectedSize)); }
        }

        private string _ProductType;
        /// <summary>
        /// Add new Product Type
        /// </summary>
        public string ProductType
        {
            get { return _ProductType; }
            set { _ProductType = value; NotifyOfPropertyChange(nameof(ProductType)); }
        }
        private bool _IsAddProduct;
        /// <summary>
        /// make it true and display the textbox 
        /// </summary>
        public bool IsAddProduct
        {
            get { return _IsAddProduct; }
            set { _IsAddProduct = value; NotifyOfPropertyChange(nameof(IsAddProduct)); }
        }
        private bool _IsAddSubProduct;
        /// <summary>
        /// for Adding Sub Product
        /// </summary>
        public bool IsAddSubProduct
        {
            get { return _IsAddSubProduct; }
            set { _IsAddSubProduct = value; NotifyOfPropertyChange(nameof(IsAddSubProduct)); }
        }

        private bool _IsAddProductColor;
        /// <summary>
        /// for Adding Product Colors
        /// </summary>
        public bool IsAddProductColor
        {
            get { return _IsAddProductColor; }
            set { _IsAddProductColor = value; NotifyOfPropertyChange(nameof(IsAddProductColor)); }
        }

        private bool _IsAddProductSize;
        /// <summary>
        /// is Product Size adding
        /// </summary>
        public bool IsAddProductSize
        {
            get { return _IsAddProductSize; }
            set { _IsAddProductSize = value; NotifyOfPropertyChange(nameof(IsAddProductSize)); }
        }

        private byte[] _ProductImage;
        /// <summary>
        /// Path for Product Image
        /// </summary>
        public byte[] ProductImage
        {
            get { return _ProductImage; }
            set { _ProductImage = value; NotifyOfPropertyChange(nameof(ProductImage)); }
        }
        private ProductModel _Product;
        /// <summary>
        /// Property Adding New Product
        /// </summary>
        public ProductModel Product
        {
            get { return _Product; }
            set { _Product = value; NotifyOfPropertyChange(nameof(Product)); }
        }

        #endregion
    }
}
