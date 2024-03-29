﻿using Caliburn.Micro;
using SmartSolutions.InventoryControl.Core.Helpers.SuggestionProvider;
using SmartSolutions.InventoryControl.DAL;
using SmartSolutions.InventoryControl.DAL.Managers.Product;
using SmartSolutions.InventoryControl.DAL.Managers.Product.ProductColor;
using SmartSolutions.InventoryControl.DAL.Managers.Product.ProductSize;
using SmartSolutions.InventoryControl.DAL.Models;
using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using SmartSolutions.InventoryControl.DAL.Models.Inventory;
using SmartSolutions.InventoryControl.DAL.Models.Payments;
using SmartSolutions.InventoryControl.DAL.Models.Product;
using SmartSolutions.InventoryControl.DAL.Models.Sales;
using SmartSolutions.InventoryControl.DAL.Models.Warehouse;
using SmartSolutions.Util.DecimalsUtils;
using SmartSolutions.Util.LogUtils;
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
        private readonly DAL.Managers.Stock.StockIn.IStockInManager _stockInManager;
        private readonly DAL.Managers.Warehouse.IWarehouseManager _warehouseManager;
        private readonly DAL.Managers.Sale.ISaleOrderManager _saleOrderManager;
        private readonly DAL.Managers.Sale.ISaleOrderDetailManager _saleOrderDetailManager;
        private readonly DAL.Managers.Payments.IPaymentManager _paymentManager;
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
                              , DAL.Managers.Payments.IPaymentTypeManager paymentTypeManager
                              , DAL.Managers.Stock.StockIn.IStockInManager stockInManager
                              , DAL.Managers.Stock.StockOut.IStockOutManager stockOutManager
                              , DAL.Managers.Warehouse.IWarehouseManager warehouseManager
                              , DAL.Managers.Sale.ISaleOrderManager saleOrderManager
                              , DAL.Managers.Sale.ISaleOrderDetailManager saleOrderDetailManager
                              , DAL.Managers.Payments.IPaymentManager paymentManager)
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
            _stockOutManager = stockOutManager;
            _stockInManager = stockInManager;
            _warehouseManager = warehouseManager;
            _saleOrderManager = saleOrderManager;
            _saleOrderDetailManager = saleOrderDetailManager;
            _paymentManager = paymentManager;
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
            SelectedInvoiceType = InvoiceTypes.Where(x => x.Equals("Sales")).FirstOrDefault();
            var partnerType = new List<int?>() { 2, 3 };
            Partners = (await _bussinessPartnerManager.GetBussinessPartnersByTypeAsync(partnerType)).OrderBy(x => x.Name).ToList();
            //Partners = (await _bussinessPartnerManager.GetAllBussinessPartnersAsync()).OrderBy(x => x.Name).ToList();
            //Products = (await _productManager.GetAllProductsAsync()).ToList();
            //Warehouses = (await _warehouseManager.GetAllWarehousesAsync()).ToList();
            Products = (await _productManager.GetAllProductsWithColorAndSize(string.Empty)).ToList();
            ProductSizes = (await _productSizeManager.GetProductAllSizeAsync()).ToList();
            ProductColors = (await _productColorManager.GetProductAllColorsAsync()).ToList();
            SaleOrderDetail = new List<SaleOrderDetailModel>();
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
        public async void GetProductAvailableStock(int? productId)
        {
            try
            {
                if (productId == null || productId == 0) return;
                var availableStock = await _stockOutManager.GetStockInHandAsync(productId);
                SelectedInventoryProduct.StockInHand = availableStock.Value;
                var resultStockIn = _stockInManager.GetStockInProduct(productId);
                Warehouses = (await _warehouseManager.GetAllWarehouseByProductId(productId ?? 0)).ToList();
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
                GrandTotal = InvoiceTotal.Value + PreviousBalance;
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
                if (string.IsNullOrEmpty(SelectedInvoiceType))
                {
                    SelectedInvoiceType = InvoiceTypes.FirstOrDefault();
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
                        if (!string.IsNullOrEmpty(product?.Product?.Name))
                        {
                            //product.IsStockIn = true;
                            productList.Add(product);
                        }
                    }

                    #region Sale Order And Sale Detail Order Generation                    
                    SaleOrder = new SaleOrderModel();
                    SaleOrder.SalePartner.Id = SelectedPartner?.Id;
                    SaleOrder.Status = SaleOrderModel.OrderStatus.New;
                    SaleOrder.Description = " Sale Order Placed";
                    SaleOrder.SubTotal = SaleInvoice.InvoiceTotal;
                    SaleOrder.Discount = SaleInvoice.Discount;
                    SaleOrder.GrandTotal = SaleInvoice.InvoiceTotal;
                    SaleOrder.IsActive = true;
                    SaleOrder.CreatedAt = DateTime.Now;
                    SaleOrder.CreatedBy = AppSettings.LoggedInUser.DisplayName;
                    var saleOrderResult = await _saleOrderManager.CreateSaleOrderAsync(SaleOrder);
                    //Sale Order is Created
                    if (saleOrderResult)
                    {
                        int? saleOrderId = await _saleOrderManager.GetLastSaleOrderIdAsync();
                        if (saleOrderId > 0)
                        {
                            if (SaleOrderDetail != null || SaleOrderDetail?.Count > 0)
                            {
                                foreach (var product in ProductGrid)
                                {
                                    if (product.Product != null && !string.IsNullOrEmpty(product?.Product?.Name))
                                    {
                                        // Here we Only Update the PO Id
                                        var saleOrderDetail = new SaleOrderDetailModel();
                                        saleOrderDetail.SaleOrder.Id = saleOrderId;
                                        SaleOrder.Id = saleOrderId;
                                        saleOrderDetail.SaleOrder = SaleOrder;
                                        saleOrderDetail.Product = product?.Product;
                                        saleOrderDetail.Product.ProductColor = product?.Product?.ProductColor;
                                        saleOrderDetail.Product.ProductSize = product?.Product?.ProductSize;
                                        saleOrderDetail.Total = product?.Total.Value;
                                        saleOrderDetail.Price = product?.Price.Value;
                                        saleOrderDetail.Quantity = product?.Quantity.Value;
                                        saleOrderDetail.Warehouse = product?.SelectedWarehouse;
                                        saleOrderDetail.IsActive = true;
                                        saleOrderDetail.Discount = SaleInvoice?.Discount ?? 0;
                                        saleOrderDetail.CreatedAt = DateTime.Now;
                                        saleOrderDetail.CreatedBy = AppSettings.LoggedInUser?.DisplayName;
                                        SaleOrderDetail.Add(saleOrderDetail);
                                    }
                                }
                                //Fetching Order Detail Id,s
                                var resultOrderDetail = await _saleOrderDetailManager.AddSaleOrderBulkDetailAsync(SaleOrderDetail);
                                if (resultOrderDetail)
                                {
                                    var orderdetailIds = await _saleOrderDetailManager.GetOrderDetailIdByOrderIdAsync(saleOrderId);
                                    for (int i = 0; i < orderdetailIds.Count; i++)
                                    {
                                        //Assigning Id,s of OrderDetails To Order details Object
                                        SaleOrderDetail.ElementAtOrDefault(i).Id = Convert.ToInt32(orderdetailIds.ElementAtOrDefault(i));
                                    }
                                }
                                else
                                {
                                    NotificationManager.Show(new Notifications.Wpf.NotificationContent { Title = "Error", Message = "Cannot Create Sale Order Details", Type = Notifications.Wpf.NotificationType.Error });
                                }
                                #endregion

                                #region Creating Sale Invoice
                                //  Here we assume that Payment is not Added
                                var stockOutProductList = new List<StockOutModel>();
                                if (ProductGrid != null || ProductGrid?.Count > 0)
                                {
                                    foreach (var item in ProductGrid)
                                    {
                                        var itemNo = ProductGrid.IndexOf(item);
                                        var selectedOrderDetail = SaleOrderDetail.ElementAtOrDefault(itemNo);

                                        if (selectedOrderDetail != null)
                                        {
                                            item.Partner = SelectedPartner;
                                            item.SaleOrder = SaleOrder;
                                            item.Product = selectedOrderDetail?.Product;
                                            item.SaleOrderDetail.Id = selectedOrderDetail.Id;
                                            item.Description = $"Sale Product {selectedOrderDetail?.Product?.Name} From {SelectedPartner?.Name} In Price {selectedOrderDetail?.Price} Quantity {selectedOrderDetail?.Quantity} At {selectedOrderDetail?.CreatedAt}";
                                            item.SelectedWarehouse = selectedOrderDetail.Warehouse;
                                            if (!string.IsNullOrEmpty(item?.Product?.Name))
                                            {
                                                stockOutProductList.Add(item);
                                            }
                                        }
                                    }
                                    SaleInvoice.SelectedPartner = SelectedPartner;
                                    SaleInvoice.PaymentImage = PaymentImage;
                                    SaleInvoice.PercentDiscount = PercentDiscount;
                                    SaleInvoice.Discount = DiscountPrice;
                                    SaleInvoice.InvoiceTotal = InvoiceTotal.Value;
                                    SaleInvoice.Payment = new PaymentModel { Id = 0, PaymentAmount = Payment ?? 0 };
                                    bool invoiceResult = await _saleInvoiceManager.SaveSaleInoiceAsync(SaleInvoice);
                                    if (invoiceResult)
                                    {
                                        var lastRowId = _saleInvoiceManager.GetLastRowId();
                                        if (lastRowId != null)
                                        {
                                            if (lastRowId > 0)
                                            {
                                                foreach (var item in stockOutProductList)
                                                {
                                                    item.SaleInvoiceId = lastRowId;
                                                }
                                                var resultStockOut = await _stockOutManager.AddBulkStockOutAsync(stockOutProductList);
                                                if (resultStockOut)
                                                {
                                                    if (Payment != null || Payment > 0)
                                                    {
                                                        // Here Payment is made
                                                        var payment = new PaymentModel();
                                                        payment.Partner = SelectedPartner;
                                                        payment.PaymentMethod = SaleInvoice?.SelectedPaymentType;
                                                        payment.PaymentType = DAL.Models.PaymentType.CR;
                                                        payment.PaymentImage = PaymentImage;
                                                        payment.PaymentAmount = Payment ?? 0;
                                                        payment.IsPaymentReceived = false;
                                                        if (payment.PaymentType == DAL.Models.PaymentType.CR)
                                                            payment.CR = Payment.Value;
                                                        else
                                                            payment.DR = Payment.Value;
                                                        payment.Description = $"Payment is Made To {SelectedPartner?.Name} against {productList?.FirstOrDefault()?.SaleInvoiceId} at {DateTime.Now}";
                                                        var resultPayment = await _paymentManager.AddPaymentAsync(payment);
                                                    }
                                                    // here we Update the Partner Ledger Account
                                                    BussinessPartnerLedgerModel partnerLedger = new BussinessPartnerLedgerModel();
                                                    partnerLedger.Partner = SelectedPartner;
                                                    var selectedPartnerBalance = await _partnerLedgerManager.GetPartnerLedgerLastBalanceAsync(SelectedPartner?.Id ?? 0);
                                                    partnerLedger.DR = InvoiceTotal.Value;
                                                    partnerLedger.InvoiceId = productList?.FirstOrDefault()?.SaleInvoiceId;
                                                    if (Payment != null || Payment > 0)
                                                    {
                                                        partnerLedger.Payment = await _paymentManager.GetLastPaymentByPartnerIdAsync(SelectedPartner?.Id);
                                                        partnerLedger.CurrentBalance = selectedPartnerBalance.CurrentBalance + InvoiceTotal.Value - partnerLedger.Payment.PaymentAmount;
                                                        partnerLedger.DR = partnerLedger.Payment.PaymentAmount;
                                                    }
                                                    else
                                                    {
                                                        partnerLedger.CurrentBalance = selectedPartnerBalance.CurrentBalance + InvoiceTotal.Value;
                                                    }
                                                    partnerLedger.CurrentBalanceType = partnerLedger?.CurrentBalance > 0 ? DAL.Models.PaymentType.CR : DAL.Models.PaymentType.DR;
                                                    partnerLedger.Description = $"{SelectedPartner.Name} Sells Stock Amount Of {InvoiceTotal.Value} Invoice Of{SaleInvoice.InvoiceId} At {SaleInvoice.CreatedAt}";
                                                    var result = await _partnerLedgerManager.UpdatePartnerCurrentBalanceAsync(partnerLedger);
                                                    if (result)
                                                    {
                                                        NotificationManager.Show(new Notifications.Wpf.NotificationContent { Title = "Success", Message = "Successfully Stock Added", Type = Notifications.Wpf.NotificationType.Success });
                                                        SaleInvoice.InvoiceId = _saleInvoiceManager.GenrateInvoiceNumber("S");

                                                    }
                                                }
                                            }
                                        }
                                    }
                                    Clear();
                                    IsLoading = false;
                                }
                                #endregion
                            }
                        }
                    }
                    else
                    {
                        NotificationManager.Show(new Notifications.Wpf.NotificationContent { Title = "Error", Message = "Cannot Create Sale Order", Type = Notifications.Wpf.NotificationType.Error });
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }

        private async void Clear()
        {
            try
            {
                SelectedSaleType = string.Empty;
                Partners = (await _bussinessPartnerManager.GetAllBussinessPartnersAsync()).OrderBy(x => x.Name).ToList();
                SelectedPartner = new BussinessPartnerModel();
                ProductGrid = new ObservableCollection<StockOutModel>();
                var newProduct = new StockOutModel();
                AutoId = 0;
                AddProduct(newProduct);
                InvoiceTotal = 0;
                PercentDiscount = 0;
                DiscountPrice = 0;
                Payment = 0;
                PaymentImage = null;
                PreviousBalance = 0;
                GrandTotal = 0;


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
                if (PreviousBalance < 0)
                {
                    IsValueCredit = true;
                    PreviousBalance = Math.Abs(PreviousBalance);
                    BalanceType = PaymentType.DR.ToString();
                }
                else
                {
                    IsValueCredit = false;
                    BalanceType = PaymentType.CR.ToString();
                }

                GrandTotal = PreviousBalance + InvoiceTotal.Value;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        private void CalculateInvoiceTotal()
        {
            try
            {
                if (ProductGrid != null || ProductGrid?.Count > 0)
                {
                    if (InvoiceTotal == null)
                        InvoiceTotal = 0;
                    foreach (var product in ProductGrid)
                    {

                        InvoiceTotal += product.Total ?? 0;
                    }
                }
                if (PreviousBalance > 0)
                    GrandTotal = (InvoiceTotal.Value + PreviousBalance);
                else
                    GrandTotal = (InvoiceTotal.Value - PreviousBalance);
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
                if (invoiceType.Equals("Sales Return"))
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
        private bool _IsValueCredit;
        /// <summary>
        /// Property That is Used for changing Color On basis of DR and CR
        /// </summary>
        public bool IsValueCredit
        {
            get { return _IsValueCredit; }
            set { _IsValueCredit = value; NotifyOfPropertyChange(nameof(IsValueCredit)); }
        }

        public SaleOrderModel SaleOrder { get; set; }
        public List<SaleOrderDetailModel> SaleOrderDetail { get; set; }
        private WarehouseModel _SelectedWarehouse;
        /// <summary>
        /// Selected Warehouse For Product
        /// </summary>
        public WarehouseModel SelectedWarehouse
        {
            get { return _SelectedWarehouse; }
            set { _SelectedWarehouse = value; }
        }

        private List<WarehouseModel> _Warehouses;
        /// <summary>
        /// List Of Warehouse
        /// </summary>
        public List<WarehouseModel> Warehouses
        {
            get { return _Warehouses; }
            set { _Warehouses = value; NotifyOfPropertyChange(nameof(_Warehouses)); }
        }

        private decimal? _Payment;
        /// <summary>
        /// Payment Amount received By Customer
        /// </summary>
        public decimal? Payment
        {
            get { return _Payment; }
            set { _Payment = value; NotifyOfPropertyChange(nameof(Payment)); }
        }

        private decimal? _InvoiceTotal;
        /// <summary>
        /// Current Invoice Total Amount
        /// </summary>
        public decimal? InvoiceTotal
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
            set { _SelectedInvoiceType = value; NotifyOfPropertyChange(nameof(SelectedInvoiceType)); OnSelectedInvoiceType(SelectedInvoiceType); }
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
        /// Selected business Partner
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
        /// List of Product Suggestions
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
            set { _SelectedProduct = value; NotifyOfPropertyChange(nameof(SelectedProduct)); }
        }
        private decimal? _ProductLastPrice;
        /// <summary>
        /// Product Last Price On Which Buyer Purchase
        /// </summary>
        public decimal? ProductLastPrice
        {
            get { return _ProductLastPrice; }
            set { _ProductLastPrice = value; NotifyOfPropertyChange(nameof(ProductLastPrice)); }
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
            set { _SelectedProductSize = value; NotifyOfPropertyChange(nameof(SelectedProductSize)); }
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
        private decimal _DiscountPrice;
        /// <summary>
        /// Price After Calculating Discount
        /// </summary>
        public decimal DiscountPrice
        {
            get { return _DiscountPrice; }
            set { _DiscountPrice = value; NotifyOfPropertyChange(nameof(DiscountPrice)); }
        }
        private decimal _PreviousBalance;
        /// <summary>
        /// Perievous Balance of that Partner 
        /// </summary>
        public decimal PreviousBalance
        {
            get { return _PreviousBalance; }
            set { _PreviousBalance = value; NotifyOfPropertyChange(nameof(PreviousBalance)); }
        }
        private string _BalanceType;
        /// <summary>
        /// Balance Type of like Payable or Receivable
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
        /// Selected Inventory from Invoice List
        /// </summary>
        public StockOutModel SelectedInventoryProduct
        {
            get { return _SelectedInventoryProduct; }
            set { _SelectedInventoryProduct = value; NotifyOfPropertyChange(nameof(SelectedInventoryProduct)); OnSelectedProduct(); }
        }

        private void OnSelectedProduct()
        {
            if (SelectedInventoryProduct != null && !string.IsNullOrEmpty(SelectedInventoryProduct?.Product?.Name))
            {

            }
        }


        #endregion
    }
}
