using Caliburn.Micro;
using SmartSolutions.InventoryControl.Core.Helpers.SuggestionProvider;
using SmartSolutions.InventoryControl.DAL;
using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using SmartSolutions.InventoryControl.DAL.Models.Inventory;
using SmartSolutions.InventoryControl.DAL.Models.Payments;
using SmartSolutions.InventoryControl.DAL.Models.Product;
using SmartSolutions.InventoryControl.DAL.Models.PurchaseOrder;
using SmartSolutions.InventoryControl.DAL.Models.Stock;
using SmartSolutions.InventoryControl.DAL.Models.Warehouse;
using SmartSolutions.Util.LogUtils;
using SmartSolutions.Util.NumericUtils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;

namespace SmartSolutions.InventoryControl.Core.ViewModels
{
    [Export(typeof(PurchaseViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PurchaseViewModel : BaseViewModel
    {
        #region Private Members
        private readonly DAL.Managers.Stock.StockIn.IStockInManager _stockInManager;
        private readonly DAL.Managers.Inventory.IInventoryManager _inventoryManager;
        private readonly DAL.Managers.Product.IProductManager _productManager;
        private readonly DAL.Managers.Bussiness_Partner.IBussinessPartnerManager _bussinessPartnerManager;
        private readonly DAL.Managers.Product.ProductSize.IProductSizeManager _productSizeManager;
        private readonly DAL.Managers.Product.ProductColor.IProductColorManager _productColorManager;
        private readonly DAL.Managers.Invoice.IPurchaseInvoiceManager _purchaseInvoiceManager;
        private readonly DAL.Managers.Bussiness_Partner.IPartnerLedgerManager _partnerLedgerManager;
        private readonly DAL.Managers.Warehouse.IWarehouseManager _warehouseManager;
        private readonly DAL.Managers.Payments.IPaymentTypeManager _paymentTypeManager;
        private readonly DAL.Managers.Purchase.IPurchaseOrderManager _purchaseOrderManager;
        private readonly DAL.Managers.Purchase.IPurchaseOrderDetailManager _purchaseOrderDetailManager;
        #endregion

        #region Constructor
        public PurchaseViewModel() { }

        [ImportingConstructor]
        public PurchaseViewModel(DAL.Managers.Stock.StockIn.IStockInManager stockInManager
                                , DAL.Managers.Inventory.IInventoryManager inventoryManager
                                , DAL.Managers.Product.IProductManager productManager
                                , DAL.Managers.Bussiness_Partner.IBussinessPartnerManager bussinessPartnerManager
                                , DAL.Managers.Product.ProductColor.IProductColorManager productColorManager
                                , DAL.Managers.Product.ProductSize.IProductSizeManager productSizeManager
                                , DAL.Managers.Invoice.IPurchaseInvoiceManager purchaseInvoiceManager
                                , DAL.Managers.Bussiness_Partner.IPartnerLedgerManager partnerLedgerManager
                                , DAL.Managers.Warehouse.IWarehouseManager warehouseManager
                                , DAL.Managers.Payments.IPaymentTypeManager paymentTypeManager
                                , DAL.Managers.Purchase.IPurchaseOrderManager purchaseOrderManager
                                , DAL.Managers.Purchase.IPurchaseOrderDetailManager purchaseOrderDetailManager)
        {
            _stockInManager = stockInManager;
            _inventoryManager = inventoryManager;
            _productManager = productManager;
            _bussinessPartnerManager = bussinessPartnerManager;
            _productColorManager = productColorManager;
            _productSizeManager = productSizeManager;
            _purchaseInvoiceManager = purchaseInvoiceManager;
            _partnerLedgerManager = partnerLedgerManager;
            _warehouseManager = warehouseManager;
            _paymentTypeManager = paymentTypeManager;
            _purchaseOrderManager = purchaseOrderManager;
            _purchaseOrderDetailManager = purchaseOrderDetailManager;
        }
        #endregion

        #region Public Methods
        protected async override void OnActivate()
        {
            if (Execute.InDesignMode)
            {
                ProductGrid = new ObservableCollection<StockInModel>();
                var model = new StockInModel();
                AutoId = 0;
                AddProduct(model);
            }
            try
            {
                IsLoading = true;
                LoadingMessage = "Loading...";

                base.OnActivate();
                PurchaseTypes = new List<string> { "Purchase", "Purchase Return" };
                SelectedPurchaseType = PurchaseTypes.Where(x => x.Equals("Purchase")).FirstOrDefault();
                Products = (await _productManager.GetAllProductsAsync()).ToList();
                Venders = (await _bussinessPartnerManager.GetAllBussinessPartnersAsync()).OrderBy(x => x.Name).ToList();
                ProductSizes = (await _productSizeManager.GetProductAllSizeAsync()).ToList();
                ProductColors = (await _productColorManager.GetProductAllColorsAsync()).ToList();
                Warehouses = (await _warehouseManager.GetAllWarehousesAsync()).ToList();
                PurchaseOrder = new PurchaseOrderModel();
                PurchaseOrderDetails = new List<PurchaseOrderDetailModel>();
                PurchaseInvoice = new PurchaseInvoiceModel();
                PurchaseInvoice.PaymentTypes = (await _paymentTypeManager.GetAllPaymentMethodsAsync()).ToList();
                PurchaseInvoice.InvoiceId = _purchaseInvoiceManager.GenrateInvoiceNumber("P");
                ProductGrid = new ObservableCollection<StockInModel>();
                var model = new StockInModel();
                AutoId = 0;
                AddProduct(model);
                IsLoading = false;

            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        public void AddProduct(StockInModel product)
        {
            try
            {
                ++AutoId;
                var newProduct = new StockInModel();
                CalculateInvoiceTotal();
                ProductGrid.Add(newProduct);
                SelectedProduct = new ProductModel();
                SelectedWarehouse = new WarehouseModel();
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        public async void RemoveProduct(StockInModel product)
        {
            try
            {
                if (AutoId == 1)
                {
                    ProductGrid.Remove(product);
                    product = new StockInModel();
                    ProductGrid.Add(product);
                    ProductSizes = (await _productSizeManager.GetProductAllSizeAsync()).ToList();
                    ProductColors = (await _productColorManager.GetProductAllColorsAsync()).ToList();
                }
                else
                {
                    --AutoId;
                    ProductGrid.Remove(product);
                }
                //TODO: Here we remove the PurchaseOrderDetail Values  in PurchaseOrderDetail List 
                InvoiceTotal = ReCalculateInvoicePrice();
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
                if (!string.IsNullOrEmpty(SelectedPurchaseType))
                {
                    //PurchaseInvoice.TransactionType = SelectedPurchaseType;
                }
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
                if(Payment != null || Payment > 0)
                {
                    
                }
                #endregion
                //We Assume that if User Selected Purchase Return Then Perform This Section
                //Other wise Go for Purchase
                if (SelectedPurchaseType.Equals("Purchase Return"))
                {
                    #region Genrate  Purchase Return Order 
                    var purchasereturnInvoice = new PurchaseReturnInvoiceModel();
                    purchasereturnInvoice.Partner = SelectedPartner;
                    #endregion
                }
                else
                {
                    //Purchase Section
                    #region Gerating PO And Filling Details
                    PurchaseOrder.Partner.Id = SelectedPartner?.Id;
                    PurchaseOrder.Status = PurchaseOrderModel.OrderStatus.New;
                    PurchaseOrder.Description = "Order Placed";
                    PurchaseOrder.SubTotal = PurchaseInvoice.InvoiceTotal;
                    PurchaseOrder.Discount = PurchaseInvoice.Discount;
                    PurchaseOrder.GrandTotal = PurchaseInvoice.InvoiceTotal;
                    PurchaseOrder.IsActive = true;
                    PurchaseOrder.CreatedAt = DateTime.Now;
                    PurchaseOrder.CreatedBy = AppSettings.LoggedInUser.DisplayName;
                    var orderResult = await _purchaseOrderManager.CreatePurchaseOrderAsync(PurchaseOrder);
                    if (orderResult)
                    {
                        int? purchaseOrderId = await _purchaseOrderManager.GetLastPurchaseOrderIdAsync();
                        if (purchaseOrderId > 0)
                        {
                            if (PurchaseOrderDetails != null || PurchaseOrderDetails?.Count > 0)
                            {
                                foreach (var product in ProductGrid)
                                {
                                    if (product.Product != null)
                                    {
                                        // Here we Only Update the the PO Id
                                        var purchaseOrderDetail = new PurchaseOrderDetailModel();
                                        purchaseOrderDetail.PurchaseOrder.Id = purchaseOrderId;
                                        PurchaseOrder.Id = purchaseOrderId;
                                        purchaseOrderDetail.PurchaseOrder = PurchaseOrder;
                                        purchaseOrderDetail.Product = product.Product;
                                        purchaseOrderDetail.ProductColor = product.Product?.ProductColor;
                                        purchaseOrderDetail.ProductSize = product.Product?.ProductSize;
                                        purchaseOrderDetail.Total = product.Total.Value;
                                        purchaseOrderDetail.Price = product.Price.Value;
                                        purchaseOrderDetail.Quantity = product.Quantity.Value;
                                        purchaseOrderDetail.Warehouse = product.Warehouse;
                                        purchaseOrderDetail.IsActive = true;
                                        purchaseOrderDetail.Discount = PurchaseInvoice?.Discount ?? 0;
                                        purchaseOrderDetail.CreatedAt = DateTime.Now;
                                        purchaseOrderDetail.CreatedBy = AppSettings.LoggedInUser?.DisplayName;
                                        PurchaseOrderDetails.Add(purchaseOrderDetail);
                                    }
                                }
                                //Fetching Order Detail Id,s
                                var resultOrderDetail = await _purchaseOrderDetailManager.AddPurchaseOrderBulkDetailAsync(PurchaseOrderDetails);
                                if (resultOrderDetail)
                                {
                                    var orderdetailIds = await _purchaseOrderDetailManager.GetOrderDetailIdByOrderIdAsync(purchaseOrderId);
                                    for (int i = 0; i < orderdetailIds.Count; i++)
                                    {
                                        //Assigning Id,s of OrderDetails To Order details Object
                                        PurchaseOrderDetails.ElementAtOrDefault(i).Id = Convert.ToInt32(orderdetailIds.ElementAtOrDefault(i));
                                    }
                                    #region Creating  Purchase Invoice
                                    //if (Payment == null || Payment == 0)
                                    //{
                                    //  Here we assume that Payment is not Added
                                    if (ProductGrid != null || ProductGrid?.Count > 0)
                                    {
                                        var productList = new List<StockInModel>();
                                        foreach (var stock in ProductGrid)
                                        {
                                            var itemNo = ProductGrid.IndexOf(stock);
                                            var selectedOrderDetail = PurchaseOrderDetails.ElementAtOrDefault(itemNo);

                                            if (selectedOrderDetail != null)
                                            {
                                                stock.Partner = SelectedPartner;
                                                stock.PurchaseOrder = PurchaseOrder;
                                                stock.Product = selectedOrderDetail?.Product;
                                                stock.PurchaseOrderDetail.Id = selectedOrderDetail.Id;
                                                stock.Description = $"Purchase Product {selectedOrderDetail?.Product?.Name} From {SelectedPartner?.Name} In Price {selectedOrderDetail?.Price} Quantity {selectedOrderDetail?.Quantity} At {selectedOrderDetail?.CreatedAt}";
                                                stock.Warehouse = selectedOrderDetail.Warehouse;
                                                if (!string.IsNullOrEmpty(stock?.Product?.Name))
                                                {
                                                    productList.Add(stock);
                                                }
                                            }
                                        }
                                        PurchaseInvoice.SelectedPartner = SelectedPartner;
                                        PurchaseInvoice.PaymentImage = PaymentImage;
                                        PurchaseInvoice.PercentDiscount = PercentDiscount;
                                        PurchaseInvoice.Discount = DiscountPrice ?? 0;
                                        PurchaseInvoice.InvoiceTotal = InvoiceTotal.Value;
                                        PurchaseInvoice.Payment = new PaymentModel { Id = 0, PaymentAmount = Payment ?? 0 };
                                        bool invoiceResult = await _purchaseInvoiceManager.SavePurchaseInoiceAsync(PurchaseInvoice);
                                        if (invoiceResult)
                                        {
                                            var lastRowId = _purchaseInvoiceManager.GetLastRowId();
                                            if (lastRowId != null)
                                            {
                                                if (lastRowId > 0)
                                                {
                                                    foreach (var item in productList)
                                                    {
                                                        item.PurchaseInvoiceId = lastRowId;
                                                    }
                                                    var resultStockIn = await _stockInManager.AddBulkStockInAsync(productList);
                                                    if (resultStockIn)
                                                    {
                                                        // here we Update the Partner Ledger Account
                                                        BussinessPartnerLedgerModel partnerLedger = new BussinessPartnerLedgerModel();
                                                        partnerLedger.Partner = SelectedPartner;
                                                        var selectedPArtnerBalance = await _partnerLedgerManager.GetPartnerLedgerLastBalanceAsync(SelectedPartner?.Id ?? 0);
                                                        partnerLedger.CR = InvoiceTotal.Value;
                                                        partnerLedger.InvoiceId = productList?.FirstOrDefault()?.PurchaseInvoiceId;
                                                        partnerLedger.CurrentBalance = partnerLedger.CurrentBalance + InvoiceTotal.Value;
                                                        partnerLedger.CurrentBalanceType = DAL.Models.PaymentType.CR;
                                                        partnerLedger.Description = $"{SelectedPartner.Name} Sells Stock Amount Of {InvoiceTotal.Value} Invoice Of{PurchaseInvoice.InvoiceId} At {PurchaseInvoice.CreatedAt}";
                                                        var result = await _partnerLedgerManager.UpdatePartnerCurrentBalanceAsync(partnerLedger);
                                                        if (result)
                                                        {
                                                            NotificationManager.Show(new Notifications.Wpf.NotificationContent { Title = "Success", Message = "Successfully Stock Added", Type = Notifications.Wpf.NotificationType.Success });
                                                            PurchaseInvoice.InvoiceId = _purchaseInvoiceManager.GenrateInvoiceNumber("P");
                                                            Clear();
                                                            if(Payment != null || Payment > 0)
                                                            {
                                                                // Here Payment is made
                                                                var payment = new PaymentModel();
                                                                payment.Partner = SelectedPartner;
                                                                payment.PaymentMethod = PurchaseInvoice?.SelectedPaymentType;
                                                                payment.PaymentType = DAL.Models.PaymentType.CR;
                                                                payment.PaymentImage = PaymentImage;
                                                                payment.PaymentAmount = Payment ?? 0;
                                                                payment.IsPaymentReceived = false;
                                                                payment.Description = $"Payment is Made To {SelectedPartner?.Name} against {productList?.FirstOrDefault()?.PurchaseInvoiceId} at {DateTime.Now}";
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        IsLoading = false;
                                    }
                                    #endregion
                                }
                            }
                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        private void Clear()
        {
            try
            {
                SelectedPurchaseType = string.Empty;
                SelectedPartner = new BussinessPartnerModel();
                ProductGrid = new ObservableCollection<StockInModel>();
                var newProduct = new StockInModel();
                ProductGrid.Add(newProduct);
                InvoiceTotal = 0;
                PercentDiscount = 0;
                DiscountPrice = 0;
                Payment = 0;
                PaymentImage = null;


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
        private async void OnSelectingPartner()
        {
            try
            {
                if (SelectedPartner == null) return;
                var selectedPartnerLedger = await _partnerLedgerManager.GetPartnerLedgerLastBalanceAsync(SelectedPartner.Id.Value);
              
                if (selectedPartnerLedger != null)
                {
                    PreviousBalance = selectedPartnerLedger.CurrentBalance;
                    BalanceType = selectedPartnerLedger.CurrentBalanceType;
                    GrandTotal = PreviousBalance + InvoiceTotal;
                }
                else
                {
                    PreviousBalance = 0;
                }
                if (SelectedPurchaseType.Equals("Purchase Return"))
                {
                    //Here we Get Only those Product Which are sell by that Specific Partner
                    Products = (await _productManager.GetAllProductsPurchasedByPartnerAsync(SelectedPartner?.Id)).ToList();
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        private void CalculateInvoiceTotal()
        {
            if (ProductGrid != null || ProductGrid?.Count > 0)
            {
                InvoiceTotal = 0;
                foreach (var product in ProductGrid)
                {
                    InvoiceTotal += product.Total ?? 0;
                }
                GrandTotal = InvoiceTotal + PreviousBalance;
            }
        }
        public void CalculateDiscountPrice(int percentDiscount, decimal discountAmount)
        {
            if (percentDiscount != 0)
            {
                decimal newInvoiceTotal = ReCalculateInvoicePrice();
                if (newInvoiceTotal > 0 && newInvoiceTotal > InvoiceTotal)
                {
                    InvoiceTotal = newInvoiceTotal;
                }
                DiscountPrice = InvoiceTotal * PercentDiscount / 100;
                InvoiceTotal = InvoiceTotal - DiscountPrice;
                GrandTotal = InvoiceTotal + PreviousBalance;
            }
            else if (discountAmount != 0)
            {
                InvoiceTotal = InvoiceTotal - discountAmount;
                GrandTotal = InvoiceTotal + PreviousBalance;
            }
        }
        private decimal ReCalculateInvoicePrice()
        {
            decimal newInvoiceTotal = 0;
            if (ProductGrid != null || ProductGrid?.Count > 0)
            {
                foreach (var product in ProductGrid)
                {
                    newInvoiceTotal += product.Total.Value;
                }
                if (PercentDiscount > 0)
                {
                    newInvoiceTotal = InvoiceTotal.Value - DiscountPrice.Value;
                }
                else if (DiscountPrice > 0)
                {
                    newInvoiceTotal = InvoiceTotal.Value - DiscountPrice.Value;
                }
            }

            return newInvoiceTotal;
        }
        public void OnPaymentRecived()
        {
            GrandTotal = PreviousBalance + (InvoiceTotal - Payment);
        }
        public async void FilterVenders(string searchText)
        {
            try
            {
                Venders = (await _bussinessPartnerManager.GetAllBussinessPartnersAsync(searchText)).ToList();
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
        private void OnSelectedPurchaseType(string purchaseType)
        {
            if (string.IsNullOrEmpty(purchaseType)) return;
            try
            {
                if(purchaseType.Equals("Purchase Return"))
                {
                    PurchaseInvoice.InvoiceId = _purchaseInvoiceManager.GenrateInvoiceNumber("PR");
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }

        private void OnSelectedProduct(ProductModel selectedProduct)
        {
            if (selectedProduct == null) return;
            try
            {
                if(SelectedPurchaseType.Equals("Purchase Return"))
                {
                    
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        #endregion

        #region Properties
        private bool _PaymentModeError;

        public bool PaymentModeError
        {
            get { return _PaymentModeError; }
            set { _PaymentModeError = value;  NotifyOfPropertyChange(nameof(PaymentModeError)); }
        }

        public PurchaseOrderModel PurchaseOrder { get; set; }
        public List<PurchaseOrderDetailModel> PurchaseOrderDetails { get; set; }
        public StockInModel StockIn { get; set; }
        private List<WarehouseModel> _Warehouses;
        /// <summary>
        /// List Of Warehouses whicha are avaiable
        /// </summary>
        public List<WarehouseModel> Warehouses
        {
            get { return _Warehouses; }
            set { _Warehouses = value; NotifyOfPropertyChange(nameof(Warehouses)); }
        }
        private WarehouseModel _SelectedWarehouse;
        /// <summary>
        /// Selected Warehouse In Which Stock Is Store
        /// </summary>
        public WarehouseModel SelectedWarehouse
        {
            get { return _SelectedWarehouse; }
            set { _SelectedWarehouse = value; NotifyOfPropertyChange(nameof(SelectedWarehouse)); }
        }

        private DAL.Models.PaymentType _BalanceType;
        /// <summary>
        /// IS it Reciveable or Payable
        /// </summary>
        public DAL.Models.PaymentType BalanceType
        {
            get { return _BalanceType; }
            set { _BalanceType = value; NotifyOfPropertyChange(nameof(BalanceType)); }
        }

        private decimal _PreviousBalance;
        /// <summary>
        /// Previous Balance Of Selected Partner
        /// </summary>
        public decimal PreviousBalance
        {
            get { return _PreviousBalance; }
            set { _PreviousBalance = value; NotifyOfPropertyChange(nameof(PreviousBalance)); }
        }

        public static int AutoId { get; set; }

        private PurchaseInvoiceModel _PurchaseInvoice;
        /// <summary>
        /// Transaction Object for Purchase Transaction
        /// </summary>
        public PurchaseInvoiceModel PurchaseInvoice
        {
            get { return _PurchaseInvoice; }
            set { _PurchaseInvoice = value; NotifyOfPropertyChange(nameof(PurchaseInvoice)); }
        }


        private BussinessPartnerModel _SelectedPartner;
        public BussinessPartnerModel SelectedPartner
        {
            get { return _SelectedPartner; }
            set { _SelectedPartner = value; NotifyOfPropertyChange(nameof(SelectedPartner)); OnSelectingPartner(); }
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
            set { _SelectedPurchaseType = value; NotifyOfPropertyChange(nameof(SelectedPurchaseType)); OnSelectedPurchaseType(SelectedPurchaseType); }
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
        private List<ProductColorModel> _ProductColors;
        /// <summary>
        /// List of Selected Product Colors
        /// </summary>
        public List<ProductColorModel> ProductColors
        {
            get { return _ProductColors; }
            set { _ProductColors = value; NotifyOfPropertyChange(nameof(ProductColors)); }
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

        public ProductModel SelectedProduct
        {
            get { return _SelectedProduct; }
            set { _SelectedProduct = value; NotifyOfPropertyChange(nameof(SelectedProduct)); OnSelectedProduct(SelectedProduct); }
        }


        private List<BussinessPartnerModel> _Venders;
        public List<BussinessPartnerModel> Venders
        {
            get { return _Venders; }
            set { _Venders = value; NotifyOfPropertyChange(nameof(Venders)); }
        }
        private List<BussinessPartnerModel> _DisplayVenders;

        public List<BussinessPartnerModel> DisplayVenders
        {
            get { return _DisplayVenders; }
            set { _DisplayVenders = value; NotifyOfPropertyChange(nameof(DisplayVenders)); }
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
            set { _PercentDiscount = value; NotifyOfPropertyChange(nameof(PercentDiscount)); /*CalculateDiscountPrice();*/ }
        }

        private decimal? _DiscountPrice;
        /// <summary>
        /// Price After Calculating Discount
        /// </summary>
        public decimal? DiscountPrice
        {
            get { return _DiscountPrice; }
            set { _DiscountPrice = value; NotifyOfPropertyChange(nameof(DiscountPrice)); /*CalculateDiscountPrice(0, DiscountPrice);*/ }
        }

        private decimal? _InvoiceTotal;
        /// <summary>
        /// Invoice Grand Total 
        /// </summary>
        public decimal? InvoiceTotal
        {
            get { return _InvoiceTotal; }
            set { _InvoiceTotal = value; NotifyOfPropertyChange(nameof(InvoiceTotal)); }
        }


        private decimal? _GrandTotal;
        /// <summary>
        /// Grand Total Amount Of Transaction
        /// </summary>
        public decimal? GrandTotal
        {
            get { return _GrandTotal; }
            set { _GrandTotal = value; NotifyOfPropertyChange(nameof(GrandTotal)); }
        }

        private decimal? _Payment;
        /// <summary>
        /// Payment Which is Made to Supply Partner
        /// </summary>
        public decimal? Payment
        {
            get { return _Payment; }
            set { _Payment = value; NotifyOfPropertyChange(nameof(Payment)); OnPaymentRecived(); }
        }
        private ObservableCollection<StockInModel> _ProductGrid;
        /// <summary>
        /// Grid for Display Products
        /// </summary>
        public ObservableCollection<StockInModel> ProductGrid
        {
            get { return _ProductGrid; }
            set { _ProductGrid = value; NotifyOfPropertyChange(nameof(ProductGrid)); }
        }
        private StockInModel _SelectedInventory;
        /// <summary>
        /// Selected Row of Inventory
        /// </summary>
        public StockInModel SelectedInventory
        {
            get { return _SelectedInventory; }
            set { _SelectedInventory = value; NotifyOfPropertyChange(nameof(SelectedInventory)); }
        }
        #endregion
    }
}
