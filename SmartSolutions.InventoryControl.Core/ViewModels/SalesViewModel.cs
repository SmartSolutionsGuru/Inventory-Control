using Caliburn.Micro;
using iText.StyledXmlParser.Jsoup.Helper;
using SmartSolutions.InventoryControl.Core.Helpers.SuggestionProvider;
using SmartSolutions.InventoryControl.Core.ViewModels.Dialogs;
using SmartSolutions.InventoryControl.DAL;
using SmartSolutions.InventoryControl.DAL.Managers.Product;
using SmartSolutions.InventoryControl.DAL.Managers.Product.ProductColor;
using SmartSolutions.InventoryControl.DAL.Managers.Product.ProductSize;
using SmartSolutions.InventoryControl.DAL.Models;
using SmartSolutions.InventoryControl.DAL.Models.Bank;
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
using System.Security.RightsManagement;

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
        private readonly DAL.Managers.Bank.IBankAccountManager _bankAccountManager;
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
                              , DAL.Managers.Payments.IPaymentManager paymentManager
                              , DAL.Managers.Bank.IBankAccountManager bankAccountManager)
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
            _bankAccountManager = bankAccountManager;
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
            PartnerType = new List<int?>() { 2, 3 };
            Partners = (await _bussinessPartnerManager.GetBussinessPartnersByTypeAsync(PartnerType)).OrderBy(x => x.Name).ToList();
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
            Payment = 0;
            InvoiceTotal = 0;
            DiscountPrice = 0;
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
        public void IsPartnerSelected()
        {
            if (SelectedPartner == null)
            {
                IoC.Get<IDialogManager>().ShowMessageBoxAsync("Please Select Vender/Partner First", "smart Solutions", Dialogs.MessageBoxOptions.Ok);
                return;
            }
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
                IsLoading = true;
                var IsAddProduct = CalculateInvoiceTotal(true);
                var resultAddProduct = VerifyEmptyProduct(ProductGrid.LastOrDefault());

                if (!resultAddProduct)
                {
                    if (IsAddProduct || ProductGrid.Count == 0)
                    {
                        ++AutoId;
                        StockOutModel newproduct = new StockOutModel();
                        if (SelectedPartner != null)
                        {
                            newproduct.Partner = SelectedPartner;
                        }
                        ProductGrid.Add(newproduct);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
                IsLoading = false;
            }
            finally { IsLoading = false; }
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
                CalculateInvoiceTotal(false);
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
                                                        payment.PaymentMethod = SelectedPaymentType;
                                                        
                                                        //TODO: Here we Add the Payment Method
                                                        payment.PaymentType = DAL.Models.PaymentType.Payable;
                                                        payment.PaymentImage = PaymentImage;
                                                        payment.PaymentAmount = Payment ?? 0;
                                                        payment.IsPaymentReceived = false;
                                                        if (payment.PaymentType == DAL.Models.PaymentType.Payable)
                                                            payment.Payable = Payment.Value;
                                                        else
                                                            payment.Receivable = Payment.Value;
                                                        payment.Description = $"Payment is Made To {SelectedPartner?.Name} against {productList?.FirstOrDefault()?.SaleInvoiceId} at {DateTime.Now}";
                                                        var resultPayment = await _paymentManager.AddPaymentAsync(payment);
                                                        if (resultPayment)
                                                        {
                                                            //First Effect
                                                            if (SelectedPaymentType != null && SelectedPaymentType.PaymentType == "Bank")
                                                            {
                                                                await _bankAccountManager.AddBankTransactionAsync(SelectedBankAccount);
                                                            }
                                                            else if (SelectedPaymentType != null &&
                                                                (SelectedPaymentType.PaymentType.Equals("Jazz Cash")
                                                                || SelectedPaymentType.PaymentType.Equals("Naya Pay")
                                                                || SelectedPaymentType.PaymentType.Equals("Eassy Paisa")
                                                                || SelectedPaymentType.PaymentType.Equals("UBl Omni")))
                                                            {
                                                                var resultBankTransaction = await _bankAccountManager.AddBankTransactionAsync(SelectedBankAccount);
                                                                if (resultBankTransaction)
                                                                    NotificationManager.Show(new Notifications.Wpf.NotificationContent { Title = "Success", Message = "Successfully Payment Added To Bank", Type = Notifications.Wpf.NotificationType.Success });
                                                                else
                                                                    NotificationManager.Show(new Notifications.Wpf.NotificationContent { Title = "Error", Message = "Payment not Added To Bank", Type = Notifications.Wpf.NotificationType.Error });
                                                            }
                                                            //2nd Effect
                                                            BussinessPartnerLedgerModel partnerPayment = new BussinessPartnerLedgerModel();
                                                            partnerPayment.Partner = SelectedPartner;
                                                            partnerPayment.Payable = Payment.Value;
                                                            var lastPayment = await _paymentManager.GetLastPaymentByPartnerIdAsync(SelectedPartner?.Id ?? 0);
                                                            if (lastPayment != null)
                                                                partnerPayment.Payment.Id = lastPayment.Id;
                                                            partnerPayment.Description = $"Amount of {payment.PaymentAmount} is Recieved To {SelectedPartner.BussinessName} At {DateTime.Now} ";
                                                            await _partnerLedgerManager.AddPartnerBalanceAsync(partnerPayment);
                                                        }
                                                    }
                                                    // here we Update the Partner Ledger Account
                                                    BussinessPartnerLedgerModel partnerLedger = new BussinessPartnerLedgerModel();
                                                    partnerLedger.Partner = SelectedPartner;
                                                    var selectedPartnerBalance = await _partnerLedgerManager.GetPartnerLedgerCurrentBalanceAsync(SelectedPartner?.Id ?? 0);
                                                    partnerLedger.Receivable = InvoiceTotal.Value;
                                                    partnerLedger.InvoiceId = productList?.FirstOrDefault()?.SaleInvoiceId;
                                                    partnerLedger.CurrentBalanceType = partnerLedger?.CurrentBalance > 0 ? DAL.Models.PaymentType.Payable : DAL.Models.PaymentType.Receivable;
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
                Partners = (await _bussinessPartnerManager.GetBussinessPartnersByTypeAsync(PartnerType)).OrderBy(x => x.Name).ToList();
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
                foreach (var product in ProductGrid)
                {
                    product.Partner = SelectedPartner;
                }
                var resultPartner = await _partnerLedgerManager.GetPartnerLedgerCurrentBalanceAsync(SelectedPartner.Id.Value);
                if (resultPartner != null)
                    PreviousBalance = resultPartner.CurrentBalance.ToString()?.ToDecimal() ?? 0;
                if (PreviousBalance < 0)
                {
                    IsValueReceivable = false;
                    PreviousBalance = Math.Abs(PreviousBalance);
                    BalanceType = PaymentType.Payable;
                }
                else
                {
                    IsValueReceivable = true;
                    BalanceType = PaymentType.Receivable;
                }

                GrandTotal = PreviousBalance + InvoiceTotal.Value;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        private bool CalculateInvoiceTotal(bool isShowMessageBox)
        {
            bool retVal = false;
            try
            {
                if (ProductGrid != null || ProductGrid?.Count > 0)
                {
                    if (InvoiceTotal == null || InvoiceTotal > 0)
                        InvoiceTotal = 0;
                    foreach (var product in ProductGrid)
                    {

                        if (product.Quantity > 0 && product.Total == 0)
                        {
                            product.Total = product.Price * product.Quantity;
                        }
                        else
                        {
                            InvoiceTotal += product.Total ?? 0;
                        }
                        retVal = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            CalculateGrandtotal();
            return retVal;
        }
        public void ClearInvoice()
        {
            Clear();
        }
        private void CalculateGrandtotal()
        {
            if (BalanceType == PaymentType.None) return;
            if (BalanceType == PaymentType.Receivable)
            {
                if (PreviousBalance == 0)
                {
                    GrandTotal = InvoiceTotal.Value;
                }
                else
                {
                    var currentBalance = PreviousBalance + InvoiceTotal.Value;
                    if(currentBalance > 0)
                    {
                        currentBalance = currentBalance + Payment ?? 0;
                        GrandTotal = currentBalance + DiscountPrice;
                    }
                    else
                    {
                        //GrandTotal = PreviousBalance + InvoiceTotal.Value;
                    }
                   
                }
            }
            else
            {
                var currentBalance = PreviousBalance + InvoiceTotal ?? 0;
                currentBalance = currentBalance - Payment ?? 0;
                GrandTotal = currentBalance - DiscountPrice;
                //GrandTotal = PreviousBalance - InvoiceTotal.Value;
            }
        }
        private bool VerifyEmptyProduct(StockOutModel product)
        {
            //null guard
            if (product == null) return false;
            bool retVal = false;
            if ((product.Price == null || product.Price == 0) || (product.Quantity == null || product.Quantity == 0))
            {
                IoC.Get<IDialogManager>().ShowMessageBoxAsync("Please complete the entry first", "Smart Solutions", MessageBoxOptions.Ok);
                retVal = true;
                return retVal;
            }
            return retVal;
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
        private void OnPaymentRecieved()
        {
            CalculateInvoiceTotal(false);
            if (BalanceType == PaymentType.Receivable)
            {
                GrandTotal = (PreviousBalance + InvoiceTotal ?? 0) - (Payment ?? 0 + DiscountPrice);
            }
            else
            {
                var totalPayment = Payment + DiscountPrice;
                var totalBill = PreviousBalance - InvoiceTotal;
                if (totalBill < 0)
                    GrandTotal = totalBill + totalPayment ?? 0;
                else
                    GrandTotal = totalBill - totalPayment ?? 0;
            }
            if (SelectedPaymentType == null)
                SelectedPaymentType = SaleInvoice.PaymentTypes.FirstOrDefault();
        }
        /// <summary>
        /// Calculate discount Price for Invoice
        /// </summary>
        /// <param name="discountAmount"></param>
        public void CalculateDiscount(decimal discountAmount)
        {
            if (discountAmount != 0)
            {
                CalculateInvoiceTotal(false);
                //if (BalanceType == PaymentType.Receivable)
                //{
                //    var totalPayment = discountAmount + Payment;
                //    var totalBill = InvoiceTotal + PreviousBalance;
                //    GrandTotal = totalBill - totalPayment ?? 0;
                //}
                //else
                //{
                //    var totalPayment = discountAmount + Payment;
                //    var totalBill = InvoiceTotal - PreviousBalance;
                //    GrandTotal = totalBill - totalPayment ?? 0;
                //}

            }
        }
        public async void OnSelectingPaymentType(PaymentTypeModel paymentType)
        {
            // null guard
            if (paymentType == null) return;
            switch (paymentType.Name)
            {
                case "Cash":
                    break;
                case "Bank":
                    var dlg = IoC.Get<BankPaymentDialogViewModel>();
                    await IoC.Get<IDialogManager>().ShowDialogAsync(dlg);
                    SelectedBankAccount = dlg.SelectedBankAccount;
                    SelectedBankAccount.Payable = Payment.Value;
                    SelectedBankAccount.Receivable = 0;
                    SelectedBankAccount.Description = $"Amount Of {Payment} to Account No {SelectedBankAccount.AccountNumber} of Bank {SelectedBankAccount.Branch.Bank.Name} Branch {SelectedBankAccount.Branch.Name} Transfer At {DateTime.Now}";
                    break;
                case "Jazz Cash":
                    var jazzDlg = IoC.Get<MobileAccountPaymentDialogViewModel>();
                    jazzDlg.SelectedMobileOperater = paymentType.Name;
                    jazzDlg.Amount = Payment ?? 0;
                    await IoC.Get<IDialogManager>().ShowDialogAsync(jazzDlg);
                    SelectedBankAccount = jazzDlg.SelectedMobileAccount;
                    SelectedBankAccount.Payable = Payment.Value;
                    SelectedBankAccount.Receivable = 0;
                    SelectedBankAccount.Description = $"Amount Of {Payment} to Account No {SelectedBankAccount.AccountNumber} of Bank {SelectedBankAccount.Branch.Bank.Name} Branch {SelectedBankAccount.Branch.Name} Transfer At {DateTime.Now}";

                    break;
                case "Ubl Omni":
                    var ublDlg = IoC.Get<MobileAccountPaymentDialogViewModel>();
                    ublDlg.SelectedMobileOperater = paymentType.Name;
                    ublDlg.Amount = Payment ?? 0;
                    await IoC.Get<IDialogManager>().ShowDialogAsync(ublDlg);
                    SelectedBankAccount = ublDlg.SelectedMobileAccount;
                    SelectedBankAccount.Payable = Payment.Value;
                    SelectedBankAccount.Receivable = 0;
                    SelectedBankAccount.Description = $"Amount Of {Payment} to Account No {SelectedBankAccount.AccountNumber} of Bank {SelectedBankAccount.Branch.Bank.Name} Branch {SelectedBankAccount.Branch.Name} Transfer At {DateTime.Now}";
                    break;
                case "Easy paisa":
                    var easyDlg = IoC.Get<MobileAccountPaymentDialogViewModel>();
                    easyDlg.SelectedMobileOperater = paymentType?.Name;
                    easyDlg.Amount = Payment ?? 0;
                    await IoC.Get<IDialogManager>().ShowDialogAsync(easyDlg);
                    SelectedBankAccount = easyDlg.SelectedMobileAccount;
                    SelectedBankAccount.Payable = Payment.Value;
                    SelectedBankAccount.Receivable = 0;
                    SelectedBankAccount.Description = $"Amount Of {Payment} to Account No {SelectedBankAccount.AccountNumber} of Bank {SelectedBankAccount.Branch.Bank.Name} Branch {SelectedBankAccount.Branch.Name} Transfer At {DateTime.Now}";
                    break;
                case "Partial":
                    break;
                case "Credit":
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Properties
        public BankAccountModel SelectedBankAccount { get; set; }
        public List<int?> PartnerType { get; set; }

        private bool _IsValueCredit;
        /// <summary>
        /// Property That is Used for changing Color On basis of DR and CR
        /// </summary>
        public bool IsValueReceivable
        {
            get { return _IsValueCredit; }
            set { _IsValueCredit = value; NotifyOfPropertyChange(nameof(IsValueReceivable)); }
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
            set { _Payment = value; NotifyOfPropertyChange(nameof(Payment)); OnPaymentRecieved(); }
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
        private PaymentTypeModel _SelectedPaymentType;

        public PaymentTypeModel SelectedPaymentType
        {
            get { return _SelectedPaymentType; }
            set { _SelectedPaymentType = value; NotifyOfPropertyChange(nameof(SelectedPaymentType)); OnSelectingPaymentType(SelectedPaymentType); }
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
            set { _DiscountPrice = value; NotifyOfPropertyChange(nameof(DiscountPrice)); CalculateDiscount(DiscountPrice); }
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
        private DAL.Models.PaymentType _BalanceType;
        /// <summary>
        /// IS it Reciveable or Payable
        /// </summary>
        public DAL.Models.PaymentType BalanceType
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
