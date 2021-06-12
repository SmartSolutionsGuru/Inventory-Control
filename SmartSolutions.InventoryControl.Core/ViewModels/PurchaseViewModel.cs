using SmartSolutions.InventoryControl.Core.Helpers.SuggestionProvider;
using SmartSolutions.InventoryControl.DAL.Models.Inventory;
using SmartSolutions.InventoryControl.DAL.Models;
using SmartSolutions.InventoryControl.DAL.Models.Product;
using SmartSolutions.Util.LogUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace SmartSolutions.InventoryControl.Core.ViewModels
{
    [Export(typeof(PurchaseViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PurchaseViewModel : BaseViewModel 
    {
        #region Private Members
        private readonly DAL.Managers.Inventory.IInventoryManager _inventoryManager;
        private readonly DAL.Managers.Product.IProductManager _productManager;
        private readonly DAL.Managers.Bussiness_Partner.IBussinessPartnerManager _bussinessPartnerManager;
        private readonly DAL.Managers.Product.ProductSize.IProductSizeManager _productSizeManager;
        private readonly DAL.Managers.Product.ProductColor.IProductColorManager _productColorManager;
        private readonly DAL.Managers.Invoice.IInvoiceManager _InvoiceManager;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public PurchaseViewModel(DAL.Managers.Inventory.IInventoryManager inventoryManager
                                , DAL.Managers.Product.IProductManager productManager
                                , DAL.Managers.Bussiness_Partner.IBussinessPartnerManager bussinessPartnerManager
                                , DAL.Managers.Product.ProductColor.IProductColorManager productColorManager
                                , DAL.Managers.Product.ProductSize.IProductSizeManager productSizeManager
                                , DAL.Managers.Invoice.IInvoiceManager invoiceManager)
        {
            _inventoryManager = inventoryManager;
            _productManager = productManager;
            _bussinessPartnerManager = bussinessPartnerManager;
            _productColorManager = productColorManager;
            _productSizeManager = productSizeManager;
            _InvoiceManager = invoiceManager;
        }
        #endregion

        #region Public Methods
        protected async override void OnActivate()
        {
            try
            {
                IsLoading = true;
                LoadingMessage = "Loading...";
               
                base.OnActivate();
                PurchaseTypes = new List<string> { "Purchase", "Purchase Return" };
                Products = (await _productManager.GetAllProductsAsync()).ToList();
                Venders = (await _bussinessPartnerManager.GetAllBussinessPartnersAsync()).ToList();
                ProductSizes = (await _productSizeManager.GetProductAllSizeAsync()).ToList();
                ProductColors = (await _productColorManager.GetProductAllColorsAsync()).ToList();
                PurchaseInvoice = new InvoiceModel();
                PurchaseInvoice.InvoiceId = _InvoiceManager.GenrateInvoiceNumber("p");
                ProductGrid = new ObservableCollection<InventoryModel>();
                //CurrentItem = new PurchaseModel();
                AddProduct();
                int Id = await _inventoryManager.GetLastTransationIdAsync();
               if (Venders != null)
                    PartnerSuggetion = new PartnerSuggestionProvider(Venders);
                if (Products != null)              
                    ProductSuggetion = new ProductSuggestionProvider(Products);
                IsLoading = false;
                                                                
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        public void OnGettingPrice()
        {
            if (Quantity > 0 && Price > 0)
            {
                TotalPrice = Quantity * (double)Price;
                NotifyOfPropertyChange(nameof(TotalPrice));
            }
            else
                TotalPrice = 0;
        }
        public void AddProduct()
        {
            try
            {
                //if (CurrentItem != null)
                //{
                //CurrentItem.Product = SelectedProduct;
                //CurrentItem.ProductColor = SelectedProductColor;
                //CurrentItem.ProductSize = SelectedProductSize;
                //CurrentItem.Price = Price;
                //CurrentItem.Quantity = Quantity;
                //CurrentItem.Total = TotalPrice;
                //PurchaseTransaction?.Products?.Add(CurrentItem);
                //if (InvoiceTotal == 0)
                //    InvoiceTotal = TotalPrice;
                //else
                //    InvoiceTotal += TotalPrice;
                //ClearProduct();
                //CurrentItem = new PurchaseModel();
                ++AutoId;
                var newProduct = new InventoryModel();
                newProduct.InvoiceGuid = PurchaseInvoice.InvoiceGuid;
                newProduct.InvoiceId = PurchaseInvoice.InvoiceId;
               
                ProductGrid.Add(newProduct);


                // }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        private void ClearProduct()
        {
            SelectedProduct = new ProductModel();
            SelectedProductColor = new ProductColorModel();
            SelectedProductSize = new ProductSizeModel();
            Price = 0;
            Quantity = 0;
            TotalPrice = 0;


        }
        public void RemoveProduct(ProductModel product)
        {
            try
            {
                --AutoId;
                ProductGrid.Remove(CurrentItem);
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        public async void SaveTransaction()
        {
            try
            {
                IsLoading = true;
                LoadingMessage = "Saving...";

                #region Error Checking
                if (!string.IsNullOrEmpty(SelectedPurchaseType))
                    PurchaseInvoice.TransactionType = SelectedPurchaseType;
                else
                {
                    PurchaseTypeError = true;
                    IsLoading = false;
                    return;
                }

                if (SelectedPartner != null)
                    PurchaseInvoice.SelectedPartner = SelectedPartner;
                else
                {
                    VenderError = true;
                    IsLoading = false;
                    return;
                }
                if (SelectedProduct != null)
                    CurrentItem.Product = SelectedProduct;
                else
                {
                    ProductError = true;
                    IsLoading = false;
                    return;
                }
                if (SelectedProductColor != null)
                    CurrentItem.ProductColor = SelectedProductColor;
                else
                {
                    ProductColorError = true;
                    IsLoading = false;
                    return;
                }
                if (SelectedProductSize != null)
                    CurrentItem.ProductSize = SelectedProductSize;
                else
                {
                    ProductSizeError = true;
                    IsLoading = false;
                    return;
                }
                if (Quantity > 0)
                    CurrentItem.Quantity = Quantity;
                else
                {
                    QuantityError = true;
                    IsLoading = false;
                    return;
                }
                if (Price > 0)
                    CurrentItem.Price = Price;
                else
                {
                    PriceError = true;
                    IsLoading = false;
                    return;
                }
                #endregion

                PurchaseInvoice.IsPurchaseInvoice = true;
                PurchaseInvoice.SelectedPartner = SelectedPartner;
                PurchaseInvoice.PaymentImage = PaymentImage;
                PurchaseInvoice.PercentDiscount = PercentDiscount;
                PurchaseInvoice.Discount = DiscountPrice;
                PurchaseInvoice.InvoiceTotal = InvoiceTotal;
                PurchaseInvoice.Payment = Payment;
                PurchaseInvoice.IsAmountRecived = true;
                PurchaseInvoice.IsAmountPaid = false;
                PurchaseInvoice.TransactionType = SelectedPurchaseType;
                bool transactionResult = await _InvoiceManager.SaveInoiceAsync(PurchaseInvoice);
                if(transactionResult)
                {
                    var lastRowId = _InvoiceManager.GetLastRowId();
                    if(lastRowId != null)
                    {
                        if(lastRowId > 0)
                        {
                          var resultInventory =  await  _inventoryManager.AddBulkInventoryAsync(ProductGrid.ToList());
                        }
                    }
                }



            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        public void Cancel()
        {
            TryClose();
        }

        #endregion

        #region Properties
        public static int AutoId { get; set; }

        private bool _QuantityError;
        /// <summary>
        /// Error Quantity 
        /// </summary>
        public bool QuantityError
        {
            get { return _QuantityError; }
            set { _QuantityError = value; NotifyOfPropertyChange(nameof(QuantityError)); }
        }

        private bool _PriceError;
        /// <summary>
        /// Price Error If wrong Value
        /// </summary>
        public bool PriceError
        {
            get { return _PriceError; }
            set { _PriceError = value; NotifyOfPropertyChange(nameof(PriceError)); }
        }

        private int _Quantity;
        /// <summary>
        /// Quantity Of Product
        /// </summary>
        public int Quantity
        {
            get { return _Quantity; }
            set { _Quantity = value; NotifyOfPropertyChange(nameof(Quantity)); }
        }
        private decimal _Price;
        /// <summary>
        /// Price Of Product
        /// </summary>
        public decimal Price
        {
            get { return _Price; }
            set { _Price = value; NotifyOfPropertyChange(nameof(Price)); OnGettingPrice(); }
        }

        private double _TotalPrice;

        public double TotalPrice
        {
            get { return _TotalPrice; }
            set { _TotalPrice = value; NotifyOfPropertyChange(nameof(TotalPrice)); }
        }
        private double _LastBalance;
        /// <summary>
        /// Last Balance Of Partner
        /// </summary>
        public double LastBalance
        {
            get { return _LastBalance; }
            set { _LastBalance = value; NotifyOfPropertyChange(nameof(LastBalance)); }
        }

        private InvoiceModel _PurchaseInvoice;
        /// <summary>
        /// Transaction Object for Purchase Transaction
        /// </summary>
        public InvoiceModel PurchaseInvoice
        {
            get { return _PurchaseInvoice; }
            set { _PurchaseInvoice = value; NotifyOfPropertyChange(nameof(PurchaseInvoice)); }
        }

        private PartnerSuggestionProvider _PartnerSuggetion;
        /// <summary>
        /// Peoperty for Partner Suggetion
        /// </summary>
        public PartnerSuggestionProvider PartnerSuggetion
        {
            get { return _PartnerSuggetion; }
            set { _PartnerSuggetion = value; NotifyOfPropertyChange(nameof(PartnerSuggetion)); }
        }
        private BussinessPartnerModel _SelectedPartner;
        public BussinessPartnerModel SelectedPartner
        {
            get { return _SelectedPartner; }
            set { _SelectedPartner = value; NotifyOfPropertyChange(nameof(SelectedPartner)); }
        }
        private ProductSuggestionProvider _ProductSuggetion;
        /// <summary>
        /// List of Product Suggetions
        /// </summary>
        public ProductSuggestionProvider ProductSuggetion
        {
            get { return _ProductSuggetion; }
            set { _ProductSuggetion = value; NotifyOfPropertyChange(nameof(ProductSuggetion)); }
        }
        private ProductModel _SelectedProduct;
        /// <summary>
        /// SElected Product 
        /// </summary>
        public ProductModel SelectedProduct
        {
            get { return _SelectedProduct; }
            set { _SelectedProduct = value; NotifyOfPropertyChange(nameof(SelectedProduct)); ProductSuggetion = new ProductSuggestionProvider(Products); }
        }

        private bool _ProductSizeError;
        /// <summary>
        /// If Product Size is Not Selected
        /// </summary>
        public bool ProductSizeError
        {
            get { return _ProductSizeError; }
            set { _ProductSizeError = value; NotifyOfPropertyChange(nameof(ProductSizeError)); }
        }

        private bool _ProductColorError;
        /// <summary>
        /// If Product Color is Not Selected
        /// </summary>
        public bool ProductColorError
        {
            get { return _ProductColorError; }
            set { _ProductColorError = value; NotifyOfPropertyChange(nameof(ProductColorError)); }
        }

        private bool _VenderError;
        /// <summary>
        /// If Vender is not Selected
        /// </summary>
        public bool VenderError
        {
            get { return _VenderError; }
            set { _VenderError = value; NotifyOfPropertyChange(nameof(VenderError)); }
        }

        private bool _ProductError;
        /// <summary>
        /// Product Selection Error
        /// </summary>
        public bool ProductError
        {
            get { return _ProductError; }
            set { _ProductError = value; NotifyOfPropertyChange(nameof(ProductError)); }
        }

        private bool _PurchaseTypeError;
        /// <summary>
        /// Error For Purchase Type
        /// </summary>
        public bool PurchaseTypeError
        {
            get { return _PurchaseTypeError; }
            set { _PurchaseTypeError = value; NotifyOfPropertyChange(nameof(PurchaseTypeError)); }
        }

        private InventoryModel _CurrentItem;
        /// <summary>
        /// Current Item For Adding Product
        /// </summary>
        public InventoryModel CurrentItem
        {
            get { return _CurrentItem; }
            set { _CurrentItem = value; NotifyOfPropertyChange(nameof(CurrentItem)); }
        }


        private List<string> _PurchaseTypes;
        /// <summary>
        /// Purchase Types Like Return etc..
        /// </summary>
        public List<string> PurchaseTypes
        {
            get { return _PurchaseTypes; }
            set { _PurchaseTypes = value; NotifyOfPropertyChange(nameof(PurchaseTypes)); }
        }
        private string _SelectedPurchaseType;

        public string SelectedPurchaseType
        {
            get { return _SelectedPurchaseType; }
            set { _SelectedPurchaseType = value; NotifyOfPropertyChange(nameof(SelectedPurchaseType)); }
        }
        private List<ProductSizeModel> _ProductSizes;
        /// <summary>
        /// Size List Of Selected Product
        /// </summary>
        public List<ProductSizeModel> ProductSizes
        {
            get { return _ProductSizes; }
            set { _ProductSizes = value; NotifyOfPropertyChange(nameof(ProductSizes)); }
        }
        private ProductSizeModel _SelectedProductSize;
        /// <summary>
        /// Selected Product Size
        /// </summary>
        public ProductSizeModel SelectedProductSize
        {
            get { return _SelectedProductSize; }
            set { _SelectedProductSize = value; NotifyOfPropertyChange(nameof(SelectedProductSize)); }
        }

        private List<ProductColorModel> _ProductColors;
        /// <summary>
        /// List of Selected Product Colors
        /// </summary>
        public List<ProductColorModel> ProductColors
        {
            get { return _ProductColors; }
            set { _ProductColors = value; NotifyOfPropertyChange(nameof(ProductColors)); }
        }
        private ProductColorModel _SelectedProductColor;
        /// <summary>
        /// Selected Product Color
        /// </summary>
        public ProductColorModel SelectedProductColor
        {
            get { return _SelectedProductColor; }
            set { _SelectedProductColor = value; NotifyOfPropertyChange(nameof(SelectedProductColor)); }
        }
        private List<ProductModel> _Products;
        /// <summary>
        /// List Of Products
        /// </summary>
        public List<ProductModel> Products
        {
            get { return _Products; }
            set { _Products = value; NotifyOfPropertyChange(nameof(Products)); }
        }
        private List<BussinessPartnerModel> _Venders;
        public List<BussinessPartnerModel> Venders
        {
            get { return _Venders; }
            set { _Venders = value; NotifyOfPropertyChange(nameof(Venders)); }
        }
        private BussinessPartnerModel _SelectedVender;
        public BussinessPartnerModel SelectedVender
        {
            get { return _SelectedVender; }
            set { _SelectedVender = value; NotifyOfPropertyChange(nameof(SelectedVender)); }
        }
        private byte[] _PaymentImage;
        /// <summary>
        /// Payment Image 
        /// </summary>
        public byte[] PaymentImage
        {
            get { return _PaymentImage; }
            set { _PaymentImage = value; NotifyOfPropertyChange(nameof(PaymentImage)); }
        }
        private int _PercentDiscount;
        /// <summary>
        /// Discount In Percentage
        /// </summary>
        public int PercentDiscount
        {
            get { return _PercentDiscount; }
            set { _PercentDiscount = value; NotifyOfPropertyChange(nameof(PercentDiscount)); }
        }

        private double _DiscountPrice;
        /// <summary>
        /// Price After Calculating Discount
        /// </summary>
        public double DiscountPrice
        {
            get { return _DiscountPrice; }
            set { _DiscountPrice = value; NotifyOfPropertyChange(nameof(DiscountPrice)); }
        }

        private double _InvoiceTotal;
        /// <summary>
        /// Invoice Grand Total 
        /// </summary>
        public double InvoiceTotal
        {
            get { return _InvoiceTotal; }
            set { _InvoiceTotal = value; NotifyOfPropertyChange(nameof(InvoiceTotal)); }
        }


        private double _GrandTotal;
        /// <summary>
        /// Grand Total Amount Of Transaction
        /// </summary>
        public double GrandTotal
        {
            get { return _GrandTotal; }
            set { _GrandTotal = value; NotifyOfPropertyChange(nameof(GrandTotal)); }
        }

        private double _Payment;
        /// <summary>
        /// Payment Which is Made to Supply Partner
        /// </summary>
        public double Payment
        {
            get { return _Payment; }
            set { _Payment = value; NotifyOfPropertyChange(nameof(Payment)); }
        }

        private ObservableCollection<InventoryModel> _ProductGrid;

        public ObservableCollection<InventoryModel> ProductGrid
        {
            get { return _ProductGrid; }
            set { _ProductGrid = value; NotifyOfPropertyChange(nameof(ProductGrid)); }
        }



        #endregion
    }
}
