using Caliburn.Micro;
using SmartSolutions.InventoryControl.Core.Helpers.SuggestionProvider;
using SmartSolutions.InventoryControl.DAL.Managers.Product;
using SmartSolutions.InventoryControl.DAL.Managers.Product.ProductColor;
using SmartSolutions.InventoryControl.DAL.Managers.Product.ProductSize;
using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using SmartSolutions.InventoryControl.DAL.Models.Inventory;
using SmartSolutions.InventoryControl.DAL.Models.Product;
using SmartSolutions.InventoryControl.DAL.Models.Sales;
using SmartSolutions.InventoryControl.DAL.Models.Stock;
using SmartSolutions.Util.DecimalsUtils;
using SmartSolutions.Util.LogUtils;
using SmartSolutions.Util.NumericUtils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;

namespace SmartSolutions.InventoryControl.Core.ViewModels
{
    [Export(typeof(SalesViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SalesViewModel : BaseViewModel
    {
        #region Private Members
        private readonly IProductColorManager _productColorManager;
        private readonly IProductSizeManager _productSizeManager;
        private readonly IProductManager _productManager;
        private readonly DAL.Managers.Invoice.ISaleInvoiceManager _saleInvoiceManager;
        private readonly DAL.Managers.Bussiness_Partner.IBussinessPartnerManager _bussinessPartnerManager;
        private readonly DAL.Managers.Inventory.IInventoryManager _inventoryManager;
        private readonly DAL.Managers.Bussiness_Partner.IPartnerLedgerManager _partnerLedgerManager;
        private readonly DAL.Managers.Transaction.ITransactionManager _transactionManager;
        private readonly DAL.Managers.Payments.IPaymentTypeManager _paymentTypeManager;
        private readonly DAL.Managers.Stock.StockOut.IStockOutManager _stockOutManager;
        #endregion

        #region Constructor
        public SalesViewModel() { }

        [ImportingConstructor]
        public SalesViewModel(IProductColorManager productColorManager
                              , IProductSizeManager productSizeManager
                              , IProductManager productManager
                              , DAL.Managers.Invoice.ISaleInvoiceManager saleInvoiceManager
                              , DAL.Managers.Bussiness_Partner.IBussinessPartnerManager bussinessPartnerManager
                              , DAL.Managers.Inventory.IInventoryManager inventoryManager
                              , DAL.Managers.Bussiness_Partner.IPartnerLedgerManager partnerLedgerManager
                              , DAL.Managers.Transaction.ITransactionManager transactionManager
                              , DAL.Managers.Payments.IPaymentTypeManager paymentTypeManager)
        {
            _productColorManager = productColorManager;
            _productSizeManager = productSizeManager;
            _productManager = productManager;
            _saleInvoiceManager = saleInvoiceManager;
            _bussinessPartnerManager = bussinessPartnerManager;
            _inventoryManager = inventoryManager;
            _partnerLedgerManager = partnerLedgerManager;
            _transactionManager = transactionManager;
            _paymentTypeManager = paymentTypeManager;
        }
        #endregion

        #region Methods
        protected async override void OnActivate()
        {
            if (Execute.InDesignMode)
            {
                StockOutModel model = new StockOutModel();
                ProductGrid = new ObservableCollection<StockOutModel>();
                ProductGrid.Add(model);
            }
            IsLoading = true;
            LoadingMessage = "Loading...";
            base.OnActivate();
            ProductGrid = new ObservableCollection<StockOutModel>();
            InvoiceTypes = new List<string> { "Sales", "Sales Return" };
            SelectedInvoiceType = InvoiceTypes.Where(x=>x.Equals("Sales")).FirstOrDefault();
            Partners = (await _bussinessPartnerManager.GetAllBussinessPartnersAsync()).OrderBy(x=>x.Name).ToList();
            Products = (await _productManager.GetAllProductsAsync()).ToList();
            ProductSizes = (await _productSizeManager.GetProductAllSizeAsync()).ToList();
            ProductColors = (await _productColorManager.GetProductAllColorsAsync()).ToList();
            SaleInvoice = new SaleInvoiceModel();
            SaleInvoice.PaymentTypes = (await _paymentTypeManager.GetAllPaymentMethodsAsync()).ToList();
            SaleInvoice.InvoiceId = _saleInvoiceManager.GenrateInvoiceNumber("S");
            if (Partners != null || Partners?.Count > 0)
                PartnerSuggetion = new PartnerSuggestionProvider(Partners);
            if (Products != null || Products?.Count > 0)
                ProductSuggetion = new ProductSuggestionProvider(Products);
            StockOutModel inventoryModel = new StockOutModel();
            AutoId = 0;
            AddProduct(inventoryModel);
            SelectedInventoryProduct = inventoryModel;
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
        public async void GetProductAvailableStock(StockOutModel selectedInvetory)
        {
            try
            {
                if (SelectedInventoryProduct != null)
                {
                    if (SelectedProduct == null || SelectedProductColor == null || SelectedProductSize == null) return;
                    var availableStock = await _inventoryManager.GetLastStockInHandAsync(SelectedProduct, SelectedProductColor, SelectedProductSize);
                    AvailableStock = availableStock.StockInHand.Value;
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        public void AddProduct(StockOutModel model)
        {
            try
            {
                ++AutoId;
                CalculateInvoiceTotal();
                StockOutModel newproduct = new StockOutModel();
                //newproduct.InvoiceId = SaleInvoice.InvoiceId;
                //newproduct.InvoiceGuid = SaleInvoice.InvoiceGuid;
                ProductGrid.Add(newproduct);
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        public async void RemoveProduct(StockOutModel product)
        {
            try
            {
                if (AutoId == 1)
                {
                    ProductGrid.Remove(product);
                    product = new StockOutModel();
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
                {
                    //SaleInvoice.TransactionType = SelectedSaleType;
                }
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
                    var productList = new List<StockOutModel>();
                    foreach (var product in ProductGrid)
                    {
                        if (!string.IsNullOrEmpty(product.Product.Name))
                        {
                            //product.IsStockIn = true;
                            productList.Add(product);
                        }
                    }

                    SaleInvoice.SelectedPartner = SelectedPartner;
                    SaleInvoice.PaymentImage = PaymentImage;
                    SaleInvoice.PercentDiscount = PercentDiscount;
                    SaleInvoice.Discount = DiscountPrice;
                    SaleInvoice.InvoiceTotal = InvoiceTotal;
                    SaleInvoice.Payment = Payment;
                    bool invoiceResult = await _saleInvoiceManager.SaveSaleInoiceAsync(SaleInvoice);
                    if (invoiceResult)
                        NotificationManager.Show(new Notifications.Wpf.NotificationContent { Title = "Success", Message = "Invoice Saved Successfully", Type = Notifications.Wpf.NotificationType.Success });
                    else
                        NotificationManager.Show(new Notifications.Wpf.NotificationContent { Title = "Success",Message = "Invoice Not Saved",Type = Notifications.Wpf.NotificationType.Error});
                    if (invoiceResult)
                    {
                        var lastRowId = _saleInvoiceManager.GetLastRowId();
                        if (lastRowId != null)
                        {
                            if (lastRowId > 0)
                            {
                                var resultStock = await _stockOutManager.RemoveBulkStockOutAsync(productList);
                                //var resultInventory = await _inventoryManager.RemoveBulkInventoryAsync(productList);
                                if (resultStock)
                                {
                                    // Now we Create Transaction Object  for saving Transaction
                                    var transaction = new DAL.Models.Transaction.TransactionModel();
                                    transaction.BussinessPartner = SelectedPartner;
                                    //transaction.PartnerLastInvoice = SaleInvoice;
                                    transaction.PaymentImage = SaleInvoice.PaymentImage;
                                    transaction.PaymentMode = "Receivable";
                                    transaction.PaymentType = SaleInvoice.SelectedPaymentType;
                                    var resultTransaction = await _transactionManager.SaveTransactionAsync(transaction);
                                    if (resultTransaction)
                                        NotificationManager.Show(new Notifications.Wpf.NotificationContent{Title = "Success",Message = "Transaction Completed Successfully", Type = Notifications.Wpf.NotificationType.Success });
                                    else
                                        NotificationManager.Show(new Notifications.Wpf.NotificationContent { Title = "Error", Message = "Transaction Not Completed", Type = Notifications.Wpf.NotificationType.Error });
                                    //If transaction is Successfully Completed
                                    if (resultTransaction)
                                    {
                                        var partnerLedger = new BussinessPartnerLedgerModel();
                                        if (Payment > 0)
                                        {
                                            var result = await _partnerLedgerManager.GetPartnerLedgerLastBalanceAsync(SelectedPartner?.Id ?? 0);
                                            if (result != null)
                                            {
                                                //We decicde on this flag either payment is Debit Or credit
                                                if (result.CurrentBalanceType == DAL.Models.PaymentType.DR)
                                                {
                                                    partnerLedger.CurrentBalance = result.CurrentBalance - Payment;
                                                    if (partnerLedger.CurrentBalance < 0)
                                                        partnerLedger.CurrentBalanceType = DAL.Models.PaymentType.CR;
                                                    else
                                                        partnerLedger.CurrentBalanceType = DAL.Models.PaymentType.DR;

                                                }
                                                else
                                                {
                                                    partnerLedger.CurrentBalance = result.CurrentBalance + Payment;
                                                    if (partnerLedger.CurrentBalance < 0)
                                                        partnerLedger.CurrentBalanceType =  DAL.Models.PaymentType.DR;
                                                    else
                                                        partnerLedger.CurrentBalanceType =  DAL.Models.PaymentType.CR;
                                                }
                                            }
                                            await _partnerLedgerManager.AddPartnerBalanceAsync(partnerLedger);
                                        }
                                    }
                                    else
                                    {
                                        NotificationManager.Show(new Notifications.Wpf.NotificationContent {Title = "Error",Message = "Cannot Saved Transaction ", Type= Notifications.Wpf.NotificationType.Error });
                                    }
                                }
                                else
                                {
                                    NotificationManager.Show(new Notifications.Wpf.NotificationContent { Title = "Error", Message = "Cannot Saved Invoice ", Type = Notifications.Wpf.NotificationType.Error });
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
        public async void FilterPartners(string searchText)
        {
            try
            {
                Partners = (await _bussinessPartnerManager.GetAllBussinessPartnersAsync(searchText)).ToList();
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }

        public async void FilterProducts(string search)
        {
            try
            {
                Products = (await _productManager.GetAllProductsAsync(search)).ToList();
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
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
                var resultPartner = await _partnerLedgerManager.GetPartnerLedgerLastBalanceAsync(SelectedPartner.Id.Value);
                if (resultPartner != null)
                    PreviousBalance = resultPartner.CurrentBalance.ToString()?.ToDecimal() ?? 0;
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
                        InvoiceTotal += product.Total ?? 0;
                    }
                }
                GrandTotal = (InvoiceTotal + PreviousBalance);
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        private void OnSelectedInvoiceType(string invoiceType)
        {
            try
            {
                if (string.IsNullOrEmpty(invoiceType)) return;
                if(invoiceType.Equals("Sales Return"))
                {
                    SaleInvoice.InvoiceId = _saleInvoiceManager.GenrateInvoiceNumber("SR");
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        #endregion

        #region Properties
        private decimal _Payment;
        /// <summary>
        /// Payment Amount recived By Customer
        /// </summary>
        public decimal Payment
        {
            get { return _Payment; }
            set { _Payment = value; NotifyOfPropertyChange(nameof(Payment)); }
        }

        private decimal _InvoiceTotal;
        /// <summary>
        /// Current Invoice Total Amount
        /// </summary>
        public decimal InvoiceTotal
        {
            get { return _InvoiceTotal; }
            set { _InvoiceTotal = value; NotifyOfPropertyChange(nameof(InvoiceTotal)); }
        }

        public static int AutoId { get; set; }
        private ObservableCollection<StockOutModel> _ProductGrid;
        /// <summary>
        /// Product Grid for Display Product
        /// </summary>
        public ObservableCollection<StockOutModel> ProductGrid
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
            set { _SelectedInvoiceType = value; NotifyOfPropertyChange(nameof(SelectedInvoiceType));  OnSelectedInvoiceType(SelectedInvoiceType); }
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

        private SaleInvoiceModel _SaleInvoice;
        /// <summary>
        /// Sales Transaction Model
        /// </summary>
        public SaleInvoiceModel SaleInvoice
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
            set { _SelectedProductSize = value; NotifyOfPropertyChange(nameof(SelectedProductSize)); GetProductAvailableStock(SelectedInventoryProduct); }
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
        private decimal _GrandTotal;
        /// <summary>
        /// Grand Total Amount Of Transaction
        /// </summary>
        public decimal GrandTotal
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
        private decimal _PreviousBalance;
        /// <summary>
        /// Privous Balance of that Partner 
        /// </summary>
        public decimal PreviousBalance
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
        private StockOutModel _SelectedInventoryProduct;
        /// <summary>
        /// Selected Incentory from Invoice List
        /// </summary>
        public StockOutModel SelectedInventoryProduct
        {
            get { return _SelectedInventoryProduct; }
            set { _SelectedInventoryProduct = value; NotifyOfPropertyChange(nameof(SelectedInventoryProduct)); }
        }


        #endregion
    }
}
