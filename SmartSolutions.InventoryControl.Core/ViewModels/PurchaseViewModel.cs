using Caliburn.Micro;
using iText.StyledXmlParser.Jsoup.Helper;
using SmartSolutions.InventoryControl.Core.Helpers.SuggestionProvider;
using SmartSolutions.InventoryControl.Core.ViewModels.Dialogs;
using SmartSolutions.InventoryControl.DAL;
using SmartSolutions.InventoryControl.DAL.Models.Bank;
using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using SmartSolutions.InventoryControl.DAL.Models.Inventory;
using SmartSolutions.InventoryControl.DAL.Models.Payments;
using SmartSolutions.InventoryControl.DAL.Models.Product;
using SmartSolutions.InventoryControl.DAL.Models.PurchaseOrder;
using SmartSolutions.InventoryControl.DAL.Models.Stock;
using SmartSolutions.InventoryControl.DAL.Models.Warehouse;
using SmartSolutions.Util.LogUtils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.Core.ViewModels
{
    [Export(typeof(PurchaseViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PurchaseViewModel : BaseViewModel
    {
        #region [Private Members]
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
        private readonly DAL.Managers.Payments.IPaymentManager _paymentManager;
        private readonly DAL.Managers.Purchase.IPurchaseReturnManager _purchaseReturnManager;
        private readonly DAL.Managers.Bank.IBankAccountManager _bankAccountManager;
        #endregion

        #region [Constructor]
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
                                , DAL.Managers.Purchase.IPurchaseOrderDetailManager purchaseOrderDetailManager
                                , DAL.Managers.Payments.IPaymentManager paymentManager
                                , DAL.Managers.Purchase.IPurchaseReturnManager purchaseReturnManager
                                , DAL.Managers.Bank.IBankAccountManager bankAccountManager)
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
            _paymentManager = paymentManager;
            _purchaseReturnManager = purchaseReturnManager;
            _bankAccountManager = bankAccountManager;
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
                Products = (await _productManager.GetAllProductsWithColorAndSize(string.Empty)).ToList();
                var partnerType = new List<int?> { 1, 3 };
                Venders = (await _bussinessPartnerManager.GetBussinessPartnersByTypeAsync(partnerType)).OrderBy(x => x.Name).ToList();
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
                Payment = 0;
                DiscountPrice = 0;

            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        public async void AddProduct(StockInModel product)
        {
            try
            {
                IsLoading = true;
                bool IsAddProduct = CalculateInvoiceTotal();
                // Indicates wheather product is Filled or Not
                var resultIndicator = VerifyEmptyProduct(ProductGrid.LastOrDefault());
                if(!resultIndicator)
                {
                    var newProduct = new StockInModel();
                    if (SelectedPartner != null)
                    {
                        newProduct.Partner = SelectedPartner;
                        newProduct.Warehouse = Warehouses.FirstOrDefault();
                    }
                    else
                    {
                        newProduct.Warehouse = Warehouses.FirstOrDefault();
                    }
                    if (IsAddProduct || ProductGrid.Count == 0)
                    {

                        if (Products != null && Products?.Count > 0)
                        {
                            ++AutoId;
                            ProductSuggetion = new ProductSuggestionProvider(Products);
                        }
                        else
                        {
                            await IoC.Get<IDialogManager>().ShowMessageBoxAsync($"There are No Products {Environment.NewLine}Please add Products To Proceed");
                        }
                        ProductGrid.Add(newProduct);
                        SelectedProduct = new ProductModel();
                        SelectedWarehouse = new WarehouseModel();

                    }
                }               
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }
        public void RemoveProduct(StockInModel product)
        {
            try
            {
                if (AutoId == 1)
                {
                    ProductGrid.Remove(product);
                    product = new StockInModel();
                    ProductGrid.Add(product);
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
                if (Payment != null || Payment > 0)
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
                    #region Generating PO And Filling Details
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
                                        purchaseOrderDetail.Product = product?.Product;
                                        purchaseOrderDetail.ProductColor = product?.Product?.ProductColor;
                                        purchaseOrderDetail.ProductSize = product?.Product?.ProductSize;
                                        purchaseOrderDetail.Total = product?.Total.Value;
                                        purchaseOrderDetail.Price = (decimal)product?.Price.Value;
                                        purchaseOrderDetail.Quantity = product?.Quantity.Value;
                                        purchaseOrderDetail.Warehouse = product?.Warehouse;
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
                                                        if (Payment != null && Payment > 0)
                                                        {
                                                            // Here Payment is made
                                                            var payment = new PaymentModel();
                                                            payment.Partner = SelectedPartner;
                                                            payment.PaymentMethod = SelectedPaymentType;
                                                            payment.PaymentType = DAL.Models.PaymentType.Receivable;
                                                            payment.PaymentImage = PaymentImage;
                                                            payment.PaymentAmount = Payment ?? 0;
                                                            payment.IsPaymentReceived = false;
                                                            if (payment.PaymentType == DAL.Models.PaymentType.Receivable)
                                                                payment.Receivable = Payment.Value;
                                                            else
                                                                payment.Payable = Payment.Value;
                                                            payment.Description = $"Payment is Made To {SelectedPartner?.Name} against {productList?.FirstOrDefault()?.PurchaseInvoiceId} at {DateTime.Now}";
                                                            var resultPayment = await _paymentManager.AddPaymentAsync(payment);
                                                            if(resultPayment)
                                                            {
                                                                //First Effect
                                                                if (SelectedPaymentType != null && SelectedPaymentType.PaymentType == "Bank")
                                                                {
                                                                    SelectedBankAccount.Payable = Payment.Value;
                                                                    SelectedBankAccount.Receivable = 0;
                                                                    SelectedBankAccount.Description = $"Payment Of {Payment.Value} Paid To {SelectedPartner.BussinessName} At {DateTime.Now}";
                                                                   await  _bankAccountManager.AddBankTransactionAsync(SelectedBankAccount);
                                                                }
                                                                else if(SelectedPaymentType != null && 
                                                                    (SelectedPaymentType.PaymentType.Equals("JazzCash") 
                                                                    || SelectedPaymentType.PaymentType.Equals("EassyPaisa") 
                                                                    || SelectedPaymentType.PaymentType.Equals("JazzCash")))
                                                                {

                                                                }
                                                                //2nd Effect
                                                                BussinessPartnerLedgerModel partnerPayment = new BussinessPartnerLedgerModel();
                                                                partnerPayment.Partner = SelectedPartner;
                                                                partnerPayment.Receivable = Payment.Value;
                                                                var lastPayment =await  _paymentManager.GetLastPaymentByPartnerIdAsync(SelectedPartner?.Id ?? 0);
                                                                if (lastPayment != null)
                                                                    partnerPayment.Payment.Id = lastPayment.Id;
                                                                partnerPayment.Description = $"Amount of {payment.PaymentAmount} is Paid To {SelectedPartner.BussinessName} At {DateTime.Now} ";
                                                                await _partnerLedgerManager.AddPartnerBalanceAsync(partnerPayment);
                                                            }

                                                        }
                                                        // here we Update the Partner Ledger Account
                                                        BussinessPartnerLedgerModel partnerLedger = new BussinessPartnerLedgerModel();
                                                        partnerLedger.Partner = SelectedPartner;
                                                        //var selectedPartnerBalance = await _partnerLedgerManager.GetPartnerLedgerCurrentBalanceAsync(SelectedPartner?.Id ?? 0);
                                                        partnerLedger.Payable = InvoiceTotal.Value;
                                                        partnerLedger.InvoiceId = productList?.FirstOrDefault()?.PurchaseInvoiceId;
                                                        partnerLedger.Description = $"{SelectedPartner.Name} Sells Stock Amount Of {InvoiceTotal.Value} Invoice Of{PurchaseInvoice.InvoiceId} At {PurchaseInvoice.CreatedAt}";
                                                        var result = await _partnerLedgerManager.UpdatePartnerCurrentBalanceAsync(partnerLedger);
                                                        if (result)
                                                        {
                                                            NotificationManager.Show(new Notifications.Wpf.NotificationContent { Title = "Success", Message = "Successfully Stock Added", Type = Notifications.Wpf.NotificationType.Success });
                                                            PurchaseInvoice.InvoiceId = _purchaseInvoiceManager.GenrateInvoiceNumber("P");

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
                    }
                    #endregion
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
                SelectedPurchaseType = "Purchase";
                Venders = (await _bussinessPartnerManager.GetAllBussinessPartnersAsync()).OrderBy(x => x.Name).ToList();
                SelectedPartner = new BussinessPartnerModel();
                ProductGrid = new ObservableCollection<StockInModel>();
                var newProduct = new StockInModel();
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
        private async void OnSelectingPartner()
        {
            try
            {
                if (SelectedPartner == null) return;
                foreach (var product in ProductGrid)
                {
                    product.Partner = SelectedPartner;
                }
                var selectedPartnerLedger = await _partnerLedgerManager.GetPartnerLedgerCurrentBalanceAsync(SelectedPartner.Id.Value);

                if (selectedPartnerLedger != null)
                {
                    PreviousBalance = selectedPartnerLedger.CurrentBalance;
                    //Becouse it is vender so it will be Payable
                    if (PreviousBalance < 0)
                    {
                        IsValueReceivable = false;
                        PreviousBalance = Math.Abs(PreviousBalance);
                        BalanceType = DAL.Models.PaymentType.Payable;

                    }
                    else
                    {
                        IsValueReceivable = true;
                        BalanceType = DAL.Models.PaymentType.Receivable;
                    }
                    selectedPartnerLedger.CurrentBalanceType = BalanceType;
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
                    ProductSuggetion = new ProductSuggestionProvider(Products);

                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        private bool CalculateInvoiceTotal()
        {
            bool retVal = false;
            if (ProductGrid != null && ProductGrid?.Count > 0)
            {
                InvoiceTotal = 0;
                foreach (var product in ProductGrid)
                {

                    if (product.Quantity > 0 && product.Total == 0)
                    {
                        product.Total = (decimal)product.Price * product.Quantity;
                    }
                    else
                    {
                        InvoiceTotal += product.Total ?? 0;
                    }
                    retVal = true;
                }
                if (BalanceType == DAL.Models.PaymentType.Receivable)
                {
                    if (PreviousBalance == 0)
                        GrandTotal = InvoiceTotal;
                    else
                        GrandTotal = PreviousBalance - InvoiceTotal;
                }
                else
                    GrandTotal = PreviousBalance + InvoiceTotal;
            }
            return retVal;
        }
        private bool VerifyEmptyProduct(StockInModel product)
        {
            //null guard
            if (product == null) return false;
            bool retVal = false;
            if ((product.Price == null || product.Price == 0)
                && (product.Quantity == null || product.Quantity == 0))
            {
                IoC.Get<IDialogManager>().ShowMessageBoxAsync("Please complete the entry first", "Smart Solutions", Dialogs.MessageBoxOptions.Ok);
                retVal = true;
                return retVal;
            }
            return retVal;
        }
        public void VerifySelectedPartner()
        {
            if (SelectedPartner == null)
            {
                IoC.Get<IDialogManager>().ShowMessageBoxAsync("Please Select Vender/Partner First", "smart Solutions", Dialogs.MessageBoxOptions.Ok);
                return;
            }
        }
        /// <summary>
        /// Calculate discount Price for Invoice
        /// </summary>
        /// <param name="discountAmount"></param>
        public void CalculateDiscountPrice(decimal discountAmount)
        {

            if (discountAmount != 0)
            {
                CalculateInvoiceTotal();
                InvoiceTotal = InvoiceTotal - (discountAmount + Payment);
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
            CalculateInvoiceTotal();
            GrandTotal = PreviousBalance + (InvoiceTotal - Payment - DiscountPrice);
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
                if (purchaseType.Equals("Purchase Return"))
                {
                    PurchaseInvoice.InvoiceId = _purchaseInvoiceManager.GenrateInvoiceNumber("PR");
                    IsPurchaseSelected = false;
                }
                else
                {
                    if (PurchaseInvoice != null)
                        PurchaseInvoice.InvoiceId = _purchaseInvoiceManager.GenrateInvoiceNumber("P");
                    IsPurchaseSelected = true;
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }

        private async void OnSelectedProduct(ProductModel selectedProduct)
        {
            if (selectedProduct == null || SelectedPartner == null) return;

            try
            {
                if (SelectedPurchaseType.Equals("Purchase Return"))
                {
                    Products = (await _productManager.GetAllProductsPurchasedByPartnerAsync(SelectedPartner?.Id)).ToList();
                    ProductSuggetion = new ProductSuggestionProvider(Products);
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        private void OnGettingSuggetions()
        {
            if (ProductSuggetion == null || ProductSuggetion.SuggestedProducts.Count == 0)
            {
                ProductSuggetion = new ProductSuggestionProvider(Products);
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
                     SelectedBankAccount = new BankAccountModel();
                    SelectedBankAccount.Branch = dlg.SelectedBranch;
                    SelectedBankAccount.Branch.Bank = dlg.SelectedBank;
                    SelectedBankAccount.AccountNumber = dlg.SelectedBankAccount.AccountNumber;
                    break;
                case "Jazz Cash":
                    var jazzDlg = IoC.Get<MobileAccountPaymentDialogViewModel>();
                    jazzDlg.SelectedMobileOperater = paymentType.Name;
                    jazzDlg.Amount = Payment ?? 0;
                    await IoC.Get<IDialogManager>().ShowDialogAsync(jazzDlg);
                    break;
                case "Ubl Omni":
                    var ublDlg = IoC.Get<MobileAccountPaymentDialogViewModel>();
                    ublDlg.SelectedMobileOperater = paymentType.Name;
                    ublDlg.Amount = Payment ?? 0;
                    await IoC.Get<IDialogManager>().ShowDialogAsync(ublDlg);
                    break;
                case "Easy paisa":
                    var easyDlg = IoC.Get<MobileAccountPaymentDialogViewModel>();
                    easyDlg.SelectedMobileOperater = paymentType.Name;
                    easyDlg.Amount = Payment ?? 0;
                    await IoC.Get<IDialogManager>().ShowDialogAsync(easyDlg);
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
        private bool _IsPurchaseSelected = true;
        /// <summary>
        /// Purchase Selected Or Purchase Return Selected
        /// </summary>
        public bool IsPurchaseSelected
        {
            get { return _IsPurchaseSelected; }
            set { _IsPurchaseSelected = value; NotifyOfPropertyChange(nameof(IsPurchaseSelected)); }
        }

        private string _ToolTip;

        public string ToolTip
        {
            get { return _ToolTip; }
            set { _ToolTip = value; NotifyOfPropertyChange(nameof(ToolTip)); }
        }

        private bool _IsValueCredit;
        /// <summary>
        /// Property That is Used for changing Color On basis of DR and CR
        /// </summary>
        public bool IsValueReceivable
        {
            get { return _IsValueCredit; }
            set { _IsValueCredit = value; NotifyOfPropertyChange(nameof(IsValueReceivable)); }
        }

        private ProductSuggestionProvider _ProductSuggetion;
        /// <summary> 
        /// List of Product Suggestions 
        /// </summary> 
        public ProductSuggestionProvider ProductSuggetion
        {
            get
            {
                return _ProductSuggetion;
            }
            set { _ProductSuggetion = value; NotifyOfPropertyChange(nameof(ProductSuggetion)); OnGettingSuggetions(); }
        }
        private bool _PaymentModeError;
        public bool PaymentModeError
        {
            get { return _PaymentModeError; }
            set { _PaymentModeError = value; NotifyOfPropertyChange(nameof(PaymentModeError)); }
        }

        public PurchaseOrderModel PurchaseOrder { get; set; }
        public List<PurchaseOrderDetailModel> PurchaseOrderDetails { get; set; }
        public StockInModel StockIn { get; set; }
        private List<WarehouseModel> _Warehouses;
        /// <summary>
        /// List Of Warehouses which are available
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

        private PaymentTypeModel _SelectedPaymentType;
        /// <summary>
        /// Selected Payment Type
        /// </summary>
        public PaymentTypeModel SelectedPaymentType
        {
            get { return _SelectedPaymentType; }
            set { _SelectedPaymentType = value; NotifyOfPropertyChange(nameof(SelectedPaymentType)); OnSelectingPaymentType(SelectedPaymentType); }
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
            set { _DiscountPrice = value; NotifyOfPropertyChange(nameof(DiscountPrice)); CalculateDiscountPrice(DiscountPrice.Value); }
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
