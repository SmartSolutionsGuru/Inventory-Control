using Caliburn.Micro;
using SmartSolutions.InventoryControl.Core.Helpers.SuggestionProvider;
using SmartSolutions.InventoryControl.DAL;
using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using SmartSolutions.InventoryControl.DAL.Models.Inventory;
using SmartSolutions.InventoryControl.DAL.Models.Product;
using SmartSolutions.InventoryControl.DAL.Models.PurchaseOrder;
using SmartSolutions.InventoryControl.DAL.Models.Stock;
using SmartSolutions.Util.LogUtils;
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
                Products = (await _productManager.GetAllProductsAsync()).ToList();
                Venders = (await _bussinessPartnerManager.GetAllBussinessPartnersAsync()).ToList();
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
        public void AddProduct(StockInModel product)
        {
            try
            {
                ++AutoId;
                var purchaseOrderDetail = new PurchaseOrderDetailModel();
                purchaseOrderDetail.Product = StockIn?.Product;
                var newProduct = new StockInModel();
                CalculateInvoiceTotal();
                ProductGrid.Add(newProduct);
                //TODO: Here we add the PurchaseOrderDetail Values  in PurchaseOrderDetail List 
                ProductSuggetion = new ProductSuggestionProvider(Products);
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
                #endregion

                #region Gerating PO And Filling Details
                PurchaseOrder.Partner = new BussinessPartnerModel { Id = SelectedPartner?.Id};
                PurchaseOrder.Status = PurchaseOrderModel.OrderStatus.New;
                PurchaseOrder.Description = "Order Placed";
                PurchaseOrder.SubTotal = PurchaseInvoice.InvoiceTotal;
                PurchaseOrder.Discount = PurchaseInvoice.Discount;
                PurchaseOrder.GrandTotal = PurchaseInvoice.InvoiceTotal;
                PurchaseOrder.IsActive = true;
                PurchaseOrder.CreatedAt = DateTime.Now;
                PurchaseOrder.CreatedBy = AppSettings.LoggedInUser.DisplayName;
                var orderResult = await _purchaseOrderManager.CreatePurchaseOrderAsync(PurchaseOrder);
                if(orderResult)
                {
                   int?  purchaseOrderId = await _purchaseOrderManager.GetLastPurchaseOrderIdAsync();
                    if (purchaseOrderId > 0)
                    {
                        if (ProductGrid != null || ProductGrid?.Count > 0)
                        {
                            foreach (var product in ProductGrid)
                            {
                                var orderDetail = new PurchaseOrderDetailModel();
                                orderDetail.PurchaseOrder = PurchaseOrder;
                                orderDetail.Product = product.Product;
                                orderDetail.ProductColor = product.Product?.ProductColor;
                                orderDetail.ProductSize = product.Product?.ProductSize;
                                orderDetail.Total = product.Total;
                                orderDetail.Price = product.Price;
                                orderDetail.Quantity = product.Quantity;
                                orderDetail.IsActive = true;
                                orderDetail.CreatedAt = DateTime.Now;
                                orderDetail.CreatedBy = AppSettings.LoggedInUser?.DisplayName;
                                PurchaseOrderDetails.Add(orderDetail);
                            }
                            await _purchaseOrderDetailManager.AddPurchaseOrderBulkDetailAsync(PurchaseOrderDetails);
                        }
                    }
                }
                #endregion

                #region Creating  Purchase Invoice
                if (ProductGrid != null || ProductGrid?.Count > 0)
                {
                    var productList = new List<StockInModel>();
                    foreach (var stock in ProductGrid)
                    {
                        if (!string.IsNullOrEmpty(stock.Product.Name))
                        {
                            productList.Add(stock);
                        }
                    }
                    PurchaseInvoice.SelectedPartner = SelectedPartner;
                    PurchaseInvoice.PaymentImage = PaymentImage;
                    PurchaseInvoice.PercentDiscount = PercentDiscount;
                    PurchaseInvoice.Discount = DiscountPrice;
                    PurchaseInvoice.InvoiceTotal = InvoiceTotal;
                    PurchaseInvoice.Payment = Payment;
                    bool invoiceResult = await _purchaseInvoiceManager.SavePurchaseInoiceAsync(PurchaseInvoice);
                    if (invoiceResult)
                    {
                        var lastRowId = _purchaseInvoiceManager.GetLastRowId();
                        if (lastRowId != null)
                        {
                            if (lastRowId > 0)
                            {
                                //var resultInventory = await _inventoryManager.AddBulkInventoryAsync(productList);
                                var resultStockIn = await _stockInManager.AddBulkStockInAsync(productList);
                                if (resultStockIn)
                                {

                                }
                            }
                        }
                    }
                    IsLoading = false;
                }
                #endregion
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
                var selectedPartnerLedger = await _partnerLedgerManager.GetPartnerLedgerLastBalance(SelectedPartner.Id.Value);
                if (selectedPartnerLedger != null)
                {
                    PreviousBalance = selectedPartnerLedger.CurrentBalance;
                    BalanceType = selectedPartnerLedger.CurrentBalanceType.ToString();
                    GrandTotal = PreviousBalance + InvoiceTotal;
                }
                else
                {
                    PreviousBalance = 0;
                    BalanceType = "Receivable";
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
                    InvoiceTotal += product.Total;
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
                    newInvoiceTotal += product.Total;
                }
                if (PercentDiscount > 0)
                {
                    newInvoiceTotal = InvoiceTotal - DiscountPrice;
                }
                else if (DiscountPrice > 0)
                {
                    newInvoiceTotal = InvoiceTotal - DiscountPrice;
                }
            }

            return newInvoiceTotal;
        }
        public void OnPaymentRecived()
        {
            GrandTotal = PreviousBalance + (InvoiceTotal - Payment);
        }
        #endregion

        #region Properties
        public PurchaseOrderModel PurchaseOrder { get; set;}
        public List<PurchaseOrderDetailModel> PurchaseOrderDetails { get; set; }
        public StockInModel StockIn { get; set; }
        private List<DAL.Models.Warehouse.WarehouseModel> _Warehouses;
        /// <summary>
        /// List Of Warehouses whicha are avaiable
        /// </summary>
        public List<DAL.Models.Warehouse.WarehouseModel> Warehouses
        {
            get { return _Warehouses; }
            set { _Warehouses = value; NotifyOfPropertyChange(nameof(Warehouses)); }
        }
        private DAL.Models.Warehouse.WarehouseModel _SelectedWarehouse;
        /// <summary>
        /// Selected Warehouse In Which Stock Is Store
        /// </summary>
        public DAL.Models.Warehouse.WarehouseModel SelectedWarehouse
        {
            get { return _SelectedWarehouse; }
            set { _SelectedWarehouse = value; NotifyOfPropertyChange(nameof(SelectedWarehouse)); }
        }

        private string _BalanceType;
        /// <summary>
        /// IS it Reciveable or Payable
        /// </summary>
        public string BalanceType
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
            set { _SelectedPartner = value; NotifyOfPropertyChange(nameof(SelectedPartner)); OnSelectingPartner(); PartnerSuggetion = new PartnerSuggestionProvider(Venders); }
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
        private List<BussinessPartnerModel> _Venders;
        public List<BussinessPartnerModel> Venders
        {
            get { return _Venders; }
            set { _Venders = value; NotifyOfPropertyChange(nameof(Venders)); }
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

        private decimal _DiscountPrice;
        /// <summary>
        /// Price After Calculating Discount
        /// </summary>
        public decimal DiscountPrice
        {
            get { return _DiscountPrice; }
            set { _DiscountPrice = value; NotifyOfPropertyChange(nameof(DiscountPrice)); /*CalculateDiscountPrice(0, DiscountPrice);*/ }
        }

        private decimal _InvoiceTotal;
        /// <summary>
        /// Invoice Grand Total 
        /// </summary>
        public decimal InvoiceTotal
        {
            get { return _InvoiceTotal; }
            set { _InvoiceTotal = value; NotifyOfPropertyChange(nameof(InvoiceTotal)); }
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

        private decimal _Payment;
        /// <summary>
        /// Payment Which is Made to Supply Partner
        /// </summary>
        public decimal Payment
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
        private InventoryModel _SelectedInventory;
        /// <summary>
        /// Selected Row of Inventory
        /// </summary>
        public InventoryModel SelectedInventory
        {
            get { return _SelectedInventory; }
            set { _SelectedInventory = value; NotifyOfPropertyChange(nameof(SelectedInventory)); }
        }

        #endregion
    }
}
