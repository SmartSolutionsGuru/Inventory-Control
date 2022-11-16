using Caliburn.Micro;
using Notifications.Wpf;
using SmartSolutions.InventoryControl.DAL;
using SmartSolutions.InventoryControl.DAL.Models.Inventory;
using SmartSolutions.InventoryControl.DAL.Models.Product;
using SmartSolutions.InventoryControl.DAL.Models.PurchaseOrder;
using SmartSolutions.InventoryControl.DAL.Models.Stock;
using SmartSolutions.InventoryControl.DAL.Models.Warehouse;
using SmartSolutions.InventoryControl.Plugins.Image;
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
        private readonly DAL.Managers.Invoice.IPurchaseInvoiceManager _purcahaseInvoiceManager;
        private readonly DAL.Managers.Inventory.IInventoryManager _inventoryManager;
        private readonly DAL.Managers.Warehouse.IWarehouseManager _warehouseManager;
        private readonly DAL.Managers.Stock.OpeningStock.IOpeningStockManager _openingStockManager;
        private readonly ICacheImage _cacheImage;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public ProductViewModel(DAL.Managers.Product.ProductType.IProductTypeManager productTypeManager,
                                DAL.Managers.Product.ProductSubType.IProductSubTypeManager productSubTypeManager,
                                DAL.Managers.Product.ProductColor.IProductColorManager productColorManager,
                                DAL.Managers.Product.ProductSize.IProductSizeManager productSizeManager,
                                DAL.Managers.Product.IProductManager productManager,
                                DAL.Managers.Invoice.IPurchaseInvoiceManager purcahseInvoiceManager,
                                DAL.Managers.Inventory.IInventoryManager inventoryManager,
                                DAL.Managers.Warehouse.IWarehouseManager warehouseManager,
                                DAL.Managers.Stock.OpeningStock.IOpeningStockManager openingStockManager,
                                ICacheImage cacheImage)
        {
            IsAddProductPressed = true;
            _productTypeManager = productTypeManager;
            _productSubTypeManager = productSubTypeManager;
            _productColorManager = productColorManager;
            _productSizeManager = productSizeManager;
            _productManager = productManager;
            _purcahaseInvoiceManager = purcahseInvoiceManager;
            _inventoryManager = inventoryManager;
            _warehouseManager = warehouseManager;
            _openingStockManager = openingStockManager;
            _cacheImage = cacheImage;

            // notificationManager = new NotificationManager();
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
            Warehouses = (await _warehouseManager.GetAllWarehousesAsync()).ToList();
            SelectedWarehouse = Warehouses?.FirstOrDefault();
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
                if (!string.IsNullOrEmpty(ProductType))
                {
                    var addProductResult = await _productTypeManager.AddProductTypeAsync(model);
                    if (addProductResult)
                    {
                        NotificationManager.Show(new NotificationContent { Title = "Success", Message = "Product Type Add Successfully", Type = NotificationType.Success });
                        IsAddProduct = false;
                        ProductType = string.Empty;
                        ProductTypes = (await _productTypeManager.GetAllProductsTypesAsync()).ToList();
                        SelectedProductType = ProductTypes.LastOrDefault();

                    }
                    else
                        NotificationManager.Show(new NotificationContent { Title = "Error", Message = "Product Type Not Add", Type = NotificationType.Error });
                }
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
                var isProductsubTypeRemoved = await _productSubTypeManager.RemoveProductSubTypeAsync(SelectedProductSubType?.Id);
                if (isProductsubTypeRemoved)
                    NotificationManager.Show(new NotificationContent { Title = "Success", Message = "Product Type Add Successfully", Type = NotificationType.Success });
                else
                    NotificationManager.Show(new NotificationContent { Title = "Error", Message = "Product Type Not Add", Type = NotificationType.Error });
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
                    if (SelectedProductType != null)
                    {
                        var isAddProductSubType = await _productSubTypeManager.AddProductSubTypeAsync(SelectedProductType?.Id, model);
                        if (isAddProductSubType)
                        {
                            NotificationManager.Show(new NotificationContent { Title = "Success", Message = "Product Sub Type Add Successfully", Type = NotificationType.Success });
                            IsAddSubProduct = false;
                            ProductSubType = string.Empty;
                            ProductSubTypes = (await _productSubTypeManager.GetAllProductSubTypeAsync(SelectedProductType.Id)).ToList();
                            SelectedProductSubType = ProductSubTypes.LastOrDefault();
                        }
                        else
                            NotificationManager.Show(new NotificationContent { Title = "Error", Message = "Product Sub Type Not Add", Type = NotificationType.Error });
                    }
                    else
                    {
                        ProductTypeError = true;
                        IsLoading = false;
                        return;
                    }
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
        public async void RemoveProductColors()
        {
            var isProductRemoved = await _productColorManager.RemoveProductColorAsync(ProductSelectedColor?.Id);
            if (isProductRemoved)
                NotificationManager.Show(new NotificationContent { Title = "Success", Message = "Product Color Removed Successfully", Type = NotificationType.Success });
            else
                NotificationManager.Show(new NotificationContent { Title = "Error", Message = "Product Color Not Removed", Type = NotificationType.Error });

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
                model.CreatedBy = AppSettings.LoggedInUser.DisplayName;
                var isproductColorAdded = await _productColorManager.AddProductColorAsync(model);
                if (isproductColorAdded)
                {
                    NotificationManager.Show(new NotificationContent { Title = "Success", Message = "Product Color Add Successfully", Type = NotificationType.Success });
                    IsAddProductColor = false;
                    ProductColor = string.Empty;
                    ProductColors = (await _productColorManager.GetProductAllColorsAsync()).ToList();
                    ProductSelectedColor = ProductColors.LastOrDefault();
                }
                else
                    NotificationManager.Show(new NotificationContent { Title = "Error", Message = "Product Color Not Added", Type = NotificationType.Error });
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
                var sizeAddResult = await _productSizeManager.AddProductSizeAsync(model);
                if (sizeAddResult)
                {
                    NotificationManager.Show(new Notifications.Wpf.NotificationContent { Title = "Success", Message = "Succesfully Product Size Added", Type = NotificationType.Success });
                    IsAddProductSize = false;
                    ProductSize = string.Empty;
                    ProductSizes = (await _productSizeManager.GetProductAllSizeAsync()).ToList();
                    ProductSelectedSize = ProductSizes.LastOrDefault();
                }
                else
                    NotificationManager.Show(new Notifications.Wpf.NotificationContent { Title = "Error", Message = "Product Size not Added", Type = NotificationType.Error });
                IsLoading = false;
            }
            catch (Exception ex)
            {
                IsLoading = false;
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        #endregion

        public async void AddWarehouse()
        {
            try
            {
                var warehouse = IoC.Get<Warehouse.WarehouseViewModel>();
                await IoC.Get<IDialogManager>().ShowDialogAsync(warehouse);
                Warehouses = (await _warehouseManager.GetAllWarehousesAsync()).ToList();
                SelectedWarehouse = Warehouses?.OrderBy(x => x.CreatedAt).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }

        #region Product
        public async void SaveProduct(ProductModel model)
        {
            try
            {
                IsLoading = true;
                LoadingMessage = "Saving Product...";
                if (model == null)
                    model = new ProductModel();
                bool errorResult = VerifyIsDataFilled();

                if (errorResult == false)
                {
                    model.Name = ProductName;
                    model.ProductType = SelectedProductType;
                    model.ProductSubType = SelectedProductSubType;
                    model.ProductColor = ProductSelectedColor;
                    model.ProductSize = ProductSelectedSize;
                    model.CreatedBy = AppSettings.LoggedInUser.DisplayName;
                    model.CreatedAt = DateTime.Now; 
                    model.Image = ProductImage;
                    model.ImagePath = _cacheImage.SaveImageToDirectory(ProductImage, ImageName);
                    bool result = await _productManager.AddProductAsync(model);
                    if (result)
                    {
                        if (InitialQuantity > -1)
                        {
                            InitialStock = new OpeningStockModel();
                            InitialStock.Product = await _productManager.GetLastAddedProduct();
                            InitialStock.Warehouse = SelectedWarehouse;
                            InitialStock.Quantity = InitialQuantity;



                            InitialStock.Price = EstimatedPrice;
                            InitialStock.Warehouse = SelectedWarehouse;
                            InitialStock.Total = InitialQuantity * EstimatedPrice;
                            InitialStock.Description = $"Initial Stock Of Product {model.Name} At {model.CreatedAt}";
                            InitialStock.CreatedBy = AppSettings.LoggedInUser.DisplayName;
                            var initialStockResult = await _openingStockManager.AddOpeningStockAsync(InitialStock);

                            if (initialStockResult)
                            {
                                //ClearProductDetails();
                                NotificationManager.Show(new NotificationContent { Title = "Success", Message = "Successfully Added Product", Type = NotificationType.Success }, areaName: "WindowArea");
                                ClearProductDetails();
                            }
                            else
                            {
                                await _purcahaseInvoiceManager.RemoveLastPurchaseInvoiceAsync(ProductInitialQuantityInvoice.InvoiceGuid);
                                NotificationManager.Show(new NotificationContent { Title = "Error", Message = "Quantity Of Product Not Added", Type = NotificationType.Error }, areaName: "WindowArea");
                            }
                        }
                        else
                        {
                            NotificationManager.Show(new NotificationContent { Title = "Success", Message = "Successfully Added Product", Type = NotificationType.Success }, areaName: "WindowArea");
                            ClearProductDetails();
                        }
                        IsLoading = false;
                    }
                    else
                    {
                        //Friendly Message Product is Not save Try Again
                        await IoC.Get<IDialogManager>().ShowMessageBoxAsync("Sorry Product is Not Saved Please Try Again", options: Dialogs.MessageBoxOptions.Ok);
                        IsLoading = false;
                    }
                }
                else
                {
                    IsLoading = false;
                    return;
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        private void ClearProductDetails()
        {
            SelectedProductType = null;
            SelectedProductSubType = null;
            ProductSelectedColor = null;
            ProductSelectedSize = null;
            ProductImage = null;
            InitialQuantity = 0;
            EstimatedPrice = 0;
            ProductName = string.Empty;
            ProductNameError = false;
            ProductTypeError = false;
            ProductSubTyprError = false;
            ProductColorError = false;
            ProductSizeError = false;
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
                    return ProductNameError;
                }
                else if (SelectedProductType == null)
                {
                    ProductTypeError = true;
                    return ProductTypeError;
                }
                else if (SelectedProductSubType == null)
                {
                    ProductSubTyprError = true;
                    return ProductSubTyprError;
                }
                else if (ProductSelectedColor == null)
                {
                    ProductColorError = true;
                    return ProductColorError;
                }
                else if (ProductSelectedSize == null)
                {
                    ProductSizeError = true;
                    return ProductSizeError;
                }
                else if (SelectedWarehouse == null)
                {
                    if (InitialQuantity > 0)
                        IsWarehouseNotSelected = true;
                    return IsWarehouseNotSelected;
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
                ProductSubTypes = _productSubTypeManager.GetAllProductSubType(Id).ToList();
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return ProductSubTypes;
        }
        #endregion

        #region Properties
        public string ImageName { get; set; }
        public OpeningStockModel InitialStock { get; set; }
        public PurchaseInvoiceModel ProductInitialQuantityInvoice { get; set; }
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
        private List<WarehouseModel> _Warehouses;

        public List<WarehouseModel> Warehouses
        {
            get { return _Warehouses; }
            set { _Warehouses = value; NotifyOfPropertyChange(nameof(Warehouses)); }
        }
        private WarehouseModel _SelectedWarehouse;
        /// <summary>
        /// Selected warehouse
        /// </summary>
        public WarehouseModel SelectedWarehouse
        {
            get { return _SelectedWarehouse; }
            set { _SelectedWarehouse = value; NotifyOfPropertyChange(nameof(SelectedWarehouse)); }
        }
        private bool _IsWarehouseNotSelected;

        public bool IsWarehouseNotSelected
        {
            get { return _IsWarehouseNotSelected; }
            set { _IsWarehouseNotSelected = value; NotifyOfPropertyChange(nameof(IsWarehouseNotSelected)); }
        }
        private decimal _EstimatedPrice;
        /// <summary>
        /// Ideal Price Of Initial Stock
        /// </summary>
        public decimal EstimatedPrice
        {
            get { return _EstimatedPrice; }
            set { _EstimatedPrice = value; NotifyOfPropertyChange(nameof(EstimatedPrice)); }
        }

        #endregion
    }
}
