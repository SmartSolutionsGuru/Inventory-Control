using Caliburn.Micro;
using SmartSolutions.InventoryControl.DAL.Models.Inventory;
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
        private readonly DAL.Managers.Invoice.IInvoiceManager _invoiceManager;
        private readonly DAL.Managers.Inventory.IInventoryManager _inventoryManager;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public ProductViewModel(DAL.Managers.Product.ProductType.IProductTypeManager productTypeManager,
                                DAL.Managers.Product.ProductSubType.IProductSubTypeManager productSubTypeManager,
                                DAL.Managers.Product.ProductColor.IProductColorManager productColorManager,
                                DAL.Managers.Product.ProductSize.IProductSizeManager productSizeManager,
                                DAL.Managers.Product.IProductManager productManager,
                                DAL.Managers.Invoice.IInvoiceManager invoiceManager,
                                DAL.Managers.Inventory.IInventoryManager inventoryManager)
        {
            IsAddProductPressed = true;
            _productTypeManager = productTypeManager;
            _productManager = productManager;
            _productSubTypeManager = productSubTypeManager;
            _productSizeManager = productSizeManager;
            _productColorManager = productColorManager;
            _invoiceManager = invoiceManager;
            _inventoryManager = inventoryManager;
        }
        #endregion

        #region Public Methods
        protected async override void OnActivate()
        {
            IsLoading = true;
            LoadingMessage = "Loading...";
            base.OnActivate();
            ProductTypes = (await _productTypeManager.GetAllProductsTypesAsync()).ToList();
            ProductSizes = (await _productSizeManager.GetProductAllSizeAsync()).ToList();
            ProductColors = (await _productColorManager.GetProductAllColorsAsync()).ToList();
            IsLoading = false;
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
                IsLoading = true;
                LoadingMessage = "Saving...";
                ProductTypeModel model = new ProductTypeModel();
                model.Name = ProductType;
                if (ProductType != null)
                    await _productTypeManager.AddProductTypeAsync(model);
                IsAddProduct = false;
                ProductType = string.Empty;
                ProductTypes = (await _productTypeManager.GetAllProductsTypesAsync()).ToList();
                IsLoading = false;

            }
            catch (Exception ex)
            {
                IsLoading = false;
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
                IsLoading = true;
                LoadingMessage = "Saving...";
                if (!string.IsNullOrEmpty(ProductSubType))
                {
                    ProductSubTypeModel model = new ProductSubTypeModel();
                    model.Name = ProductSubType;
                    if(SelectedProductType != null)
                        await _productSubTypeManager.AddProductSubTypeAsync(SelectedProductType?.Id, model);
                    else
                    {
                        ProductTypeError = true;
                        IsLoading = false;
                        return;
                    }
                    IsAddSubProduct = false;
                    ProductSubType = string.Empty;
                    ProductSubTypes = (await _productSubTypeManager.GetAllProductSubTypeAsync(SelectedProductType.Id)).ToList();
                    SelectedProductSubType = ProductSubTypes.LastOrDefault();
                    IsLoading = false;
                }
            }
            catch (Exception ex)
            {
                IsLoading = false;
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
                IsLoading = true;
                LoadingMessage = "Saving...";
                var model = new ProductColorModel();
                if (!string.IsNullOrEmpty(ProductColor))
                {
                    model.Color = ProductColor;
                }
                await _productColorManager.AddProductColorAsync(model);
                IsAddProductColor = false;
                ProductColor = string.Empty;
                ProductColors = (await _productColorManager.GetProductAllColorsAsync()).ToList();
                ProductSelectedColor = ProductColors.LastOrDefault();
                IsLoading = false;
            }
            catch (Exception ex)
            {
                IsLoading = false;
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
                IsLoading = true;
                LoadingMessage = "Saving...";
                if (!string.IsNullOrEmpty(ProductSize))
                    model.Size = ProductSize;
                await _productSizeManager.AddProductSizeAsync(model);
                IsAddProductSize = false;
                ProductSize = string.Empty;
                ProductSizes = (await _productSizeManager.GetProductAllSizeAsync()).ToList();
                ProductSelectedSize = ProductSizes.LastOrDefault();
                IsLoading = false;

            }
            catch (Exception ex)
            {
                IsLoading = false;
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        #endregion

        #region Product
        public async void SaveProduct(ProductModel model)
        {
            try
            {
                IsLoading = true;
                LoadingMessage = "Saving Product...";
                if (model == null)
                    model = new ProductModel();
                bool erroResult = VerifyIsDataFilled();

                if (erroResult == false)
                {
                    model.Name = ProductName;
                    model.ProductType = SelectedProductType;
                    model.ProductSubType = SelectedProductSubType;
                    model.ProductColor = ProductSelectedColor;
                    model.ProductSize = ProductSelectedSize;
                    model.Image = ProductImage;
                    bool result = await _productManager.AddProductAsync(model);
                    if (result)
                    {
                        if (InitialQuantity > 0)
                        {
                            ProductInitialQuantityInvoice = new InvoiceModel();
                            ProductInitialQuantityInvoice.InvoiceId = _invoiceManager.GenrateInvoiceNumber("IQ");
                            ProductInitialQuantityInvoice.TransactionType = "Product Initial Quantity";
                            ProductInitialQuantityInvoice.Description = "Product Initial Quantity Addition";
                            var transactionResult = await _invoiceManager.SaveInoiceAsync(ProductInitialQuantityInvoice);
                            if (transactionResult)
                            {
                                InitialQuantityInventory = new InventoryModel();
                                InitialQuantityInventory.InvoiceId = ProductInitialQuantityInvoice.InvoiceId;
                                InitialQuantityInventory.InvoiceGuid = ProductInitialQuantityInvoice.InvoiceGuid;
                                InitialQuantityInventory.ProductColor = ProductSelectedColor;
                                InitialQuantityInventory.ProductSize = ProductSelectedSize;
                                InitialQuantityInventory.Product = await _productManager.GetLastAddedProduct();
                                InitialQuantityInventory.Quantity = InitialQuantity;
                                var inventoryResult = await _inventoryManager.AddInventoryAsync(InitialQuantityInventory);
                                if(inventoryResult)
                                {
                                    SelectedProductType = null;
                                    SelectedProductSubType = null;
                                    ProductSelectedColor = null;
                                    ProductSelectedSize = null;
                                    ProductImage = null;
                                    ProductName = string.Empty;
                                }
                            }
                        }
                        IsLoading = false;
                    }
                    else
                    {
                        //Friendly Message Product is Not save Try Again
                        await IoC.Get<IDialogManager>().ShowMessageBoxAsync("Sorry Product is Not Saved Please Try Again", options: Dialogs.MessageBoxOptions.Ok);
                    }
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        #endregion

        #region Private Helpers
        private bool VerifyIsDataFilled()
        {
            bool retVal = false;
            try
            {
                if (string.IsNullOrEmpty(ProductName))
                {
                    ProductNameError = true;
                    return retVal = true;
                }
                else if (SelectedProductType == null)
                {
                    ProductTypeError = true;
                    return retVal = true;
                }
                else if (SelectedProductSubType == null)
                {
                    ProductSubTyprError = true;
                    return retVal = true;
                }
                else if (ProductSelectedColor == null)
                {
                    ProductColorError = true;
                    return retVal = true;
                }
                else if (ProductSelectedSize == null)
                {
                    ProductSizeError = true;
                    return retVal = true;
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
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
        public InventoryModel InitialQuantityInventory { get; set; }
        public InvoiceModel ProductInitialQuantityInvoice { get; set; }
        private bool _ProductTypeError;
        /// <summary>
        /// When Product Type is Not Selected
        /// </summary>
        public bool ProductTypeError
        {
            get { return _ProductTypeError; }
            set { _ProductTypeError = value; NotifyOfPropertyChange(nameof(ProductTypeError)); }
        }

        private bool _ProductSubTyprError;
        /// <summary>
        /// When Product Sub Type is Not Selected
        /// </summary>
        public bool ProductSubTyprError
        {
            get { return _ProductSubTyprError; }
            set { _ProductSubTyprError = value; NotifyOfPropertyChange(nameof(ProductSubTyprError)); }
        }
        private bool _ProductNameError;
        /// <summary>
        /// When Product Name is Written
        /// </summary>
        public bool ProductNameError
        {
            get { return _ProductNameError; }
            set { _ProductNameError = value; NotifyOfPropertyChange(nameof(ProductNameError)); }
        }

        private bool _ProductColorError;
        /// <summary>
        /// When Product Color Is Not Selected
        /// </summary>
        public bool ProductColorError
        {
            get { return _ProductColorError; }
            set { _ProductColorError = value; NotifyOfPropertyChange(nameof(ProductColorError)); }
        }
        private bool _ProductSizeError;
        /// <summary>
        /// When Product Size is Not Selected
        /// </summary>
        public bool ProductSizeError
        {
            get { return _ProductSizeError; }
            set { _ProductSizeError = value; NotifyOfPropertyChange(nameof(ProductSizeError)); }
        }

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


        private int _InitialQuantity;
        /// <summary>
        /// Initial Quantity for Stock
        /// </summary>
        public int InitialQuantity
        {
            get { return _InitialQuantity; }
            set { _InitialQuantity = value; NotifyOfPropertyChange(nameof(InitialQuantity)); }
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
            set
            {
                _SelectedProductType = value;
                NotifyOfPropertyChange(nameof(SelectedProductType));
                if (SelectedProductType != null)
                {
                    OnProductTypeSelection(SelectedProductType.Id.Value);
                }
                ProductTypeError = false;
            }
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
            set { _SelectedProductSubType = value; NotifyOfPropertyChange(nameof(SelectedProductSubType)); ProductSubTyprError = false; }
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
            set { _ProductSelectedColor = value; NotifyOfPropertyChange(nameof(ProductSelectedColor)); ProductColorError = false; }
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
