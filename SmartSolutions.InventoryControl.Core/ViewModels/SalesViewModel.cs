using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using SmartSolutions.InventoryControl.DAL.Managers.Product.ProductColor;
using SmartSolutions.InventoryControl.DAL.Managers.Product.ProductSize;
using SmartSolutions.InventoryControl.DAL.Managers.Product;
using System.Text;
using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using System.Linq;
using SmartSolutions.InventoryControl.DAL.Models.Product;
using SmartSolutions.InventoryControl.Core.Helpers.SuggestionProvider;
using SmartSolutions.InventoryControl.DAL.Models.Inventory;
using SmartSolutions.Util.LogUtils;
using SmartSolutions.Util.NumericUtils;
using Caliburn.Micro;
using System.Collections.ObjectModel;

namespace SmartSolutions.InventoryControl.Core.ViewModels
{
    [Export(typeof(SalesViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SalesViewModel : BaseViewModel
    {
        #region Private Members
        private readonly IProductColorManager _productColorManager;
        private readonly IProductSizeManager _productSizeManager;
        private readonly IProductManager _productManager;
        private readonly DAL.Managers.Invoice.IInvoiceManager _invoiceManager;
        private readonly DAL.Managers.Bussiness_Partner.IBussinessPartnerManager _bussinessPartnerManager;
        private readonly DAL.Managers.Inventory.IInventoryManager _inventoryManager;
        private readonly DAL.Managers.Bussiness_Partner.IPartnerLedgerManager _partnerLedgerManager;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public SalesViewModel(IProductColorManager productColorManager
                              , IProductSizeManager productSizeManager
                              , IProductManager productManager
                              , DAL.Managers.Invoice.IInvoiceManager invoiceManager
                              , DAL.Managers.Bussiness_Partner.IBussinessPartnerManager bussinessPartnerManager
                              , DAL.Managers.Inventory.IInventoryManager inventoryManager
                              , DAL.Managers.Bussiness_Partner.IPartnerLedgerManager partnerLedgerManager)
        {
            _productColorManager = productColorManager;
            _productSizeManager = productSizeManager;
            _productManager = productManager;
            _invoiceManager = invoiceManager;
            _bussinessPartnerManager = bussinessPartnerManager;
            _inventoryManager = inventoryManager;
            _partnerLedgerManager = partnerLedgerManager;
        }
        #endregion

        #region Methods
        protected async override void OnActivate()
        {
            if (Execute.InDesignMode)
            {
                InventoryModel model = new InventoryModel();
                ProductGrid = new ObservableCollection<InventoryModel>();
                ProductGrid.Add(model);
            }
            IsLoading = true;
            LoadingMessage = "Loading...";
            base.OnActivate();
            ProductGrid = new ObservableCollection<InventoryModel>();
            InvoiceTypes = new List<string> { "Sales", "Sales Return" };
            Partners = (await _bussinessPartnerManager.GetAllBussinessPartnersAsync()).ToList();
            Products = (await _productManager.GetAllProductsAsync()).ToList();
            ProductSizes = (await _productSizeManager.GetProductAllSizeAsync()).ToList();
            ProductColors = (await _productColorManager.GetProductAllColorsAsync()).ToList();
            SaleInvoice = new InvoiceModel();
            SaleInvoice.InvoiceId = _invoiceManager.GenrateInvoiceNumber("S");
            if (Partners != null || Partners?.Count > 0)
                PartnerSuggetion = new PartnerSuggestionProvider(Partners);
            if (Products != null || Products?.Count > 0)
                ProductSuggetion = new ProductSuggestionProvider(Products);
            InventoryModel inventoryModel = new InventoryModel();
            AutoId = 0;
            AddProduct(inventoryModel);
            IsLoading = false;
        }
        public void OnGettingPrice()
        {
            if (Quantity > 0 && Price > 0)
            {
                TotalPrice = Quantity * Price;
                NotifyOfPropertyChange(nameof(TotalPrice));
            }
            else
                TotalPrice = 0;
        }
        public async void GetProductAvailableStock()
        {
            try
            {
                if (SelectedProduct == null || SelectedProductColor == null || SelectedProductSize == null) return;
                var availableStock = await _inventoryManager.GetLastStockInHandAsync(SelectedProduct, SelectedProductColor, SelectedProductSize);
                AvailableStock = availableStock.StockInHand;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        public void AddProduct(InventoryModel model)
        {
            try
            {
                ++AutoId;
                CalculateInvoiceTotal();
                InventoryModel newproduct = new InventoryModel();
                newproduct.InvoiceId = SaleInvoice.InvoiceId;
                newproduct.InvoiceGuid = SaleInvoice.InvoiceGuid;
                ProductGrid.Add(newproduct);
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        public async void RemoveProduct(InventoryModel product)
        {
            try
            {
                if (AutoId == 1)
                {
                    ProductGrid.Remove(product);
                    product = new InventoryModel();
                    ProductGrid.Add(product);
                    ProductSizes = (await _productSizeManager.GetProductAllSizeAsync()).ToList();
                    ProductColors = (await _productColorManager.GetProductAllColorsAsync()).ToList();
                }
                else
                {
                    --AutoId;
                    ProductGrid.Remove(product);
                }
                CalculateInvoiceTotal();
                GrandTotal = InvoiceTotal + PreviousBalance;
            }
            catch (Exception ex)
            {

                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        public async void SaveInvoice()
        {
            try
            {
                IsLoading = true;
                LoadingMessage = "Saving...";

                #region Error Checking
                if (!string.IsNullOrEmpty(SelectedSaleType))
                    SaleInvoice.TransactionType = SelectedSaleType;
                else
                {
                    SaleTypeError = true;
                    IsLoading = false;
                    return;
                }

                if (SelectedPartner != null)
                    SaleInvoice.SelectedPartner = SelectedPartner;
                else
                {
                    CustomerError = true;
                    IsLoading = false;
                    return;
                }
                #endregion

                if (ProductGrid != null || ProductGrid?.Count > 0)
                {
                    var productList = new List<InventoryModel>();
                    foreach (var product in ProductGrid)
                    {
                        if (!string.IsNullOrEmpty(product.Product.Name))
                        {
                            product.IsStockIn = true;
                            productList.Add(product);
                        }
                    }
                    SaleInvoice.IsPurchaseInvoice = true;
                    SaleInvoice.SelectedPartner = SelectedPartner;
                    SaleInvoice.PaymentImage = PaymentImage;
                    SaleInvoice.PercentDiscount = PercentDiscount;
                    SaleInvoice.Discount = DiscountPrice;
                    SaleInvoice.InvoiceTotal = InvoiceTotal;
                    SaleInvoice.Payment = Payment;
                    SaleInvoice.IsAmountPaid = false;
                    SaleInvoice.IsAmountRecived = true;
                    SaleInvoice.IsPurchaseInvoice = false;
                    SaleInvoice.TransactionType = SelectedSaleType;
                    bool transactionResult = await _invoiceManager.SaveInoiceAsync(SaleInvoice);
                    if (transactionResult)
                    {
                        var lastRowId = _invoiceManager.GetLastRowId();
                        if (lastRowId != null)
                        {
                            if (lastRowId > 0)
                            {
                                var resultInventory = await _inventoryManager.RemoveBulkInventoryAsync(productList);
                                if (resultInventory)
                                {

                                }
                            }
                        }
                    }
                    IsLoading = false;
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
        private async void OnSelectedPartner()
        {
            try
            {
                if (SelectedPartner == null)
                {
                    PartnerSuggetion = new PartnerSuggestionProvider(Partners);
                    PreviousBalance = 0;
                    return;
                }
                var resultPartner = await _partnerLedgerManager.GetPartnerLedgerLastBalance(SelectedPartner.Id.Value);
                if (resultPartner != null)
                    PreviousBalance = resultPartner.BalanceAmount.ToString()?.ToInt() ?? 0;
                GrandTotal = PreviousBalance + InvoiceTotal;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        private void OnProductSelection()
        {
            if (SelectedProduct == null)
            {
                ProductSuggetion = new ProductSuggestionProvider(Products);
                return;
            }
        }
        private void CalculateInvoiceTotal()
        {
            try
            {
                if (ProductGrid != null || ProductGrid?.Count > 0)
                {
                    foreach (var product in ProductGrid)
                    {
                        InvoiceTotal += product.Total;
                    }
                }
                GrandTotal = (InvoiceTotal + PreviousBalance);
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        #endregion

        #region Properties
        private double _Payment;

        public double Payment
        {
            get { return _Payment; }
            set { _Payment = value; NotifyOfPropertyChange(nameof(Payment)); }
        }

        private double _InvoiceTotal;
        /// <summary>
        /// Current Invoice Total Amount
        /// </summary>
        public double InvoiceTotal
        {
            get { return _InvoiceTotal; }
            set { _InvoiceTotal = value; NotifyOfPropertyChange(nameof(InvoiceTotal)); }
        }

        public static int AutoId { get; set; }
        private ObservableCollection<InventoryModel> _ProductGrid;
        /// <summary>
        /// Product Grid for Display Product
        /// </summary>
        public ObservableCollection<InventoryModel> ProductGrid
        {
            get { return _ProductGrid; }
            set { _ProductGrid = value; NotifyOfPropertyChange(nameof(ProductGrid)); }
        }

        private bool _CustomerError;
        /// <summary>
        /// if Customer is Not Selected
        /// </summary>
        public bool CustomerError
        {
            get { return _CustomerError; }
            set { _CustomerError = value; NotifyOfPropertyChange(nameof(CustomerError)); }
        }

        private bool _SaleTypeError;
        /// <summary>
        /// if Sale type is Not Selected
        /// </summary>
        public bool SaleTypeError
        {
            get { return _SaleTypeError; }
            set { _SaleTypeError = value; NotifyOfPropertyChange(nameof(SaleTypeError)); }
        }

        private int _AvailableStock;
        /// <summary>
        /// Stock available for Sale
        /// </summary>
        public int AvailableStock
        {
            get { return _AvailableStock; }
            set { _AvailableStock = value; NotifyOfPropertyChange(nameof(AvailableStock)); }
        }

        private List<string> _InvoiceTypes;

        public List<string> InvoiceTypes
        {
            get { return _InvoiceTypes; }
            set { _InvoiceTypes = value; NotifyOfPropertyChange(nameof(InvoiceTypes)); }
        }

        private string _SelectedInvoiceType;
        /// <summary>
        /// Selected Invoice
        /// </summary>
        public string SelectedInvoiceType
        {
            get { return _SelectedInvoiceType; }
            set { _SelectedInvoiceType = value; NotifyOfPropertyChange(nameof(SelectedInvoiceType)); }
        }

        private List<BussinessPartnerModel> _Partners;

        public List<BussinessPartnerModel> Partners
        {
            get { return _Partners; }
            set { _Partners = value; NotifyOfPropertyChange(nameof(Partners)); }
        }

        private PartnerSuggestionProvider _PartnerSuggetion;

        public PartnerSuggestionProvider PartnerSuggetion
        {
            get { return _PartnerSuggetion; }
            set { _PartnerSuggetion = value; NotifyOfPropertyChange(nameof(PartnerSuggetion)); }
        }

        private BussinessPartnerModel _SelectedPartner;
        /// <summary>
        /// Selected bussiness Partner
        /// </summary>
        public BussinessPartnerModel SelectedPartner
        {
            get { return _SelectedPartner; }
            set { _SelectedPartner = value; NotifyOfPropertyChange(nameof(SelectedPartner)); OnSelectedPartner(); }
        }

        private InvoiceModel _SaleInvoice;
        /// <summary>
        /// Sales Transaction Model
        /// </summary>
        public InvoiceModel SaleInvoice
        {
            get { return _SaleInvoice; }
            set { _SaleInvoice = value; NotifyOfPropertyChange(nameof(SaleInvoice)); }
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
        private List<ProductModel> _Products;
        /// <summary>
        /// List Of Products
        /// </summary>
        public List<ProductModel> Products
        {
            get { return _Products; }
            set { _Products = value; NotifyOfPropertyChange(nameof(Products)); }
        }
        private ProductModel _SelectedProduct;
        /// <summary>
        /// SElected Product 
        /// </summary>
        public ProductModel SelectedProduct
        {
            get { return _SelectedProduct; }
            set { _SelectedProduct = value; NotifyOfPropertyChange(nameof(SelectedProduct)); OnProductSelection(); }
        }
        private string _SelectedSaleType;

        public string SelectedSaleType
        {
            get { return _SelectedSaleType; }
            set { _SelectedSaleType = value; NotifyOfPropertyChange(nameof(SelectedSaleType)); }
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
            set { _SelectedProductSize = value; NotifyOfPropertyChange(nameof(SelectedProductSize)); GetProductAvailableStock(); }
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
        private double _Price;
        /// <summary>
        /// Price Of Product
        /// </summary>
        public double Price
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
        private int _PercentDiscount;
        /// <summary>
        /// Discount In Percentage
        /// </summary>
        public int PercentDiscount
        {
            get { return _PercentDiscount; }
            set { _PercentDiscount = value; NotifyOfPropertyChange(nameof(PercentDiscount)); }
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
        private byte[] _PaymentImage;
        /// <summary>
        /// Payment Image 
        /// </summary>
        public byte[] PaymentImage
        {
            get { return _PaymentImage; }
            set { _PaymentImage = value; NotifyOfPropertyChange(nameof(PaymentImage)); }
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
        private int _PreviousBalance;
        /// <summary>
        /// Privous Balance of that Partner 
        /// </summary>
        public int PreviousBalance
        {
            get { return _PreviousBalance; }
            set { _PreviousBalance = value; NotifyOfPropertyChange(nameof(PreviousBalance)); }
        }
        private string _BalanceType;
        /// <summary>
        /// Balance Type of like Payable or Reciveable
        /// </summary>
        public string BalanceType
        {
            get { return _BalanceType; }
            set { _BalanceType = value; NotifyOfPropertyChange(nameof(BalanceType)); }
        }
        private bool _IsProductSizeSelected;

        public bool IsProductSizeSelected
        {
            get { return _IsProductSizeSelected; }
            set { _IsProductSizeSelected = value; NotifyOfPropertyChange(nameof(IsProductSizeSelected)); }
        }
        private InventoryModel _SelectedInventoryProduct;
         /// <summary>
         /// Selected Incentory from Invoice List
         /// </summary>
        public InventoryModel SelectedInventoryProduct
        {
            get { return _SelectedInventoryProduct; }
            set { _SelectedInventoryProduct = value; NotifyOfPropertyChange(nameof(SelectedInventoryProduct)); }
        }


        #endregion
    }
}
