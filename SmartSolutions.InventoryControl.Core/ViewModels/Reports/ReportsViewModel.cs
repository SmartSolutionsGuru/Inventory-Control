using Caliburn.Micro;
using SmartSolutions.InventoryControl.Core.ViewModels.Reports.BussinessPartner;
using SmartSolutions.InventoryControl.Core.ViewModels.Reports.Product;
using SmartSolutions.InventoryControl.Core.ViewModels.Reports.Purchase;
using SmartSolutions.InventoryControl.DAL.Managers.Product;
using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using SmartSolutions.Util.LogUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace SmartSolutions.InventoryControl.Core.ViewModels.Reports
{
    [Export(typeof(ReportsViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ReportsViewModel : BaseViewModel, IHandle<Screen>, IHandle<string>
    {

        #region Private Members
        private readonly IEventAggregator _eventAggregator;
        private readonly IProductManager _productManager;
        private readonly DAL.Managers.Bussiness_Partner.IBussinessPartnerManager _bussinessPartnerManager;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public ReportsViewModel(IEventAggregator eventAggregator
                                , IProductManager productManager
                                , DAL.Managers.Bussiness_Partner.IBussinessPartnerManager bussinessPartnerManager)
        {
            _eventAggregator = eventAggregator;
            _productManager = productManager;
            _bussinessPartnerManager = bussinessPartnerManager;
        }
        #endregion

        #region Methods
        protected override void OnActivate()
        {
            base.OnActivate();
            ReportsCategory = new List<string>
            {
                "Business Partners",
                "Purchase",
                "Products",
                "Sales",
                "Payments",
                "Bank Accounts"
            };
            BissinessPartnerReports = new List<string>
            {
                "All Business Partners",
                "Business Partner By Vendor",
                "Business Partner By Seller",
                "Business Partner By City",
                "Business Partner By DR Balance",
                "Business Partner By CR Balance",
                "Business Partner Balance sheet"
            };
            ProductReports = new List<string>
            {
                "All Products",
                "Product By Name",
                "Product By Size",
                "Product By Color"
            };
            PurchaseReports = new List<string>
            {
                "All Purchase",
                "Purchase Of Current Month",
                "Purchase By Starting Date",
                "Purchase Of Specific Date",
                "Purchase By Specific Vendor"
            };
            SaleReport = new List<string>
            {
                "All Sales",
                "Sale Of Current Month",
                "Sales By Starting Date",
                "Sale By Ending Date",
                "Sale of Specific Partner"
            };

        }

        public void Handle(Screen screen)
        {
            if (screen is DisplayAllPartnersViewModel
                || screen is AllProductReportViewModel
                || screen is DisplaySelectedPartnerReportViewModel
                || screen is PurchaseReportViewModel)
            {
                if (screen is DisplayAllPartnersViewModel)
                    _eventAggregator.PublishOnCurrentThread(SelectedReportSubCategory);
                if (screen is DisplaySelectedPartnerReportViewModel)
                    _eventAggregator.PublishOnBackgroundThread(this.SelectedPartner);
                ActiveItem = screen;
                ActivateItem(screen);
            }
        }
        public void Handle(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                _eventAggregator.PublishOnUIThread(message);
            }
        }
        #endregion

        #region Private Methods
        private void OnSelectedCategory(string selectedReportCategory)
        {
            if (!string.IsNullOrEmpty(selectedReportCategory))
            {
                switch (selectedReportCategory)
                {
                    case "Business Partners":
                        ReportsSubCategory = BissinessPartnerReports;
                        break;
                    case "Purchase":
                        ReportsSubCategory = PurchaseReports;
                        break;
                    case "Products":
                        ReportsSubCategory = ProductReports;
                        break;
                    case "Sales":
                        ReportsSubCategory = SaleReport;
                        break;
                    case "Payments":
                        ReportsSubCategory = PaymentReport;
                        break;
                    case "Bank Accounts":
                        ReportsSubCategory = BankAccount;
                        break;
                }
            }
        }

        private void OnSelectedSubCategory(string selectedReportSubCategory)
        {
            switch (selectedReportSubCategory)
            {
                #region Business Partner
                case "All Business Partners":
                    IsDisplayCombo = false;
                    IsDatePickerVisible = false;
                    IsComboBoxVisible = false;
                    Handle(selectedReportSubCategory);
                    Handle(IoC.Get<DisplayAllPartnersViewModel>());
                    break;
                case "Business Partner By Vendor":
                    IsDisplayCombo = false;
                    IsComboBoxVisible = false;
                    Handle(selectedReportSubCategory);
                    Handle(IoC.Get<DisplayAllPartnersViewModel>());
                    break;
                case "Business Partner By Seller":
                    IsDisplayCombo = false;
                    IsComboBoxVisible = false;
                    Handle(selectedReportSubCategory);
                    Handle(IoC.Get<DisplayAllPartnersViewModel>());
                    break;
                case "Business Partner By City":
                    IsDisplayCombo = false;
                    IsComboBoxVisible = false;
                    Handle(selectedReportSubCategory);
                    Handle(IoC.Get<DisplayAllPartnersViewModel>());
                    break;
                case "Business Partner By DR Balance":
                    IsDisplayCombo = false;
                    IsComboBoxVisible = false;
                    Handle(selectedReportSubCategory);
                    Handle(IoC.Get<DisplayAllPartnersViewModel>());
                    break;
                case "Business Partner By CR Balance":
                    IsDisplayCombo = false;
                    IsComboBoxVisible = false;
                    Handle(selectedReportSubCategory);
                    Handle(IoC.Get<DisplayAllPartnersViewModel>());
                    break;
                case "Business Partner Balance sheet":
                    IsDisplayCombo = true;
                    IsComboBoxVisible = true;
                    OnSelectingBalanceSheet();
                    break;
                #endregion

                #region Products
                case "All Products":
                    Handle(IoC.Get<AllProductReportViewModel>());
                    break;
                case "Product By Name":
                    Handle(IoC.Get<AllProductReportViewModel>());
                    break;
                case "Product By Size":
                    Handle(IoC.Get<AllProductReportViewModel>());
                    break;
                case "Product By Color":
                    Handle(IoC.Get<AllProductReportViewModel>());
                    break;
                #endregion

                #region Purchase
                //Includes All Vendors and  from start date
                case "All Purchase":
                    IsDatePickerVisible = false;
                    Handle(selectedReportSubCategory);
                    Handle(IoC.Get<PurchaseReportViewModel>());
                    break;
                case "Purchase Of Current Month":
                    IsDatePickerVisible = false;
                    Handle(selectedReportSubCategory);
                    Handle(IoC.Get<PurchaseReportViewModel>());
                    break;
                case "Purchase By Starting Date":
                    IsDisplayCombo = true;
                    IsDatePickerVisible = true;
                    Handle(selectedReportSubCategory);
                    Handle(IoC.Get<PurchaseReportViewModel>());
                    break;
                case "Purchase By Ending Date":
                    IsDatePickerVisible = false;
                    Handle(selectedReportSubCategory);
                    Handle(IoC.Get<PurchaseReportViewModel>());
                    break;
                case "Purchase Of Specific Date":
                    IsDatePickerVisible = false;
                    Handle(selectedReportSubCategory);
                    Handle(IoC.Get<PurchaseReportViewModel>());
                    break;
                case "Purchase By Specific Vendor":
                    IsDatePickerVisible = false;
                    Handle(selectedReportSubCategory);
                    Handle(IoC.Get<PurchaseReportViewModel>());
                    break;
                #endregion

                #region Sales
                case "All Sales":
                    break;
                case "Sale Of Current Month":
                    break;
                case "Sales By Starting Date":
                    break;
                case "Sale By Ending Date":
                    break;
                case "Sale of Specific Partner":
                    break;
                #endregion

                #region Payments
                case "All Payments":
                    break;
                case "Payment Of Partner":
                    break;
                case "Payment Of Partner By Starting Date":
                    break;
                case "Payment Of Partner By Ending Date":
                    break;
                case "Payment Of Partner By Start End Date":
                    break;
                #endregion

                #region Bank Accounts

                #endregion
                default:
                    break;
            }
        }

        private async void OnSelectingBalanceSheet()
        {
            Partners = (await _bussinessPartnerManager.GetAllBussinessPartnersAsync()).ToList();

        }
        private void OnSelectingPartner()
        {
            if (SelectedPartner == null) return;
            Handle(IoC.Get<DisplaySelectedPartnerReportViewModel>());

        }

        public void OnGettingText(string searchText)
        {
            if (string.IsNullOrEmpty(searchText)) return;
            try
            {
                //IsBusy = true;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            finally
            {
                //IsBusy = false;
            }
        }
        #endregion

        #region Properties
        private bool _IsComboBoxVisible;

        public bool IsComboBoxVisible
        {
            get { return _IsComboBoxVisible; }
            set { _IsComboBoxVisible = value; NotifyOfPropertyChange(nameof(IsComboBoxVisible)); }
        }
        private bool _IsDatePickerVisible;

        public bool IsDatePickerVisible
        {
            get { return _IsDatePickerVisible; }
            set { _IsDatePickerVisible = value; NotifyOfPropertyChange(nameof(IsDatePickerVisible)); }
        }
        private DateTime _StartDate;

        public DateTime StartDate
        {
            get { return _StartDate; }
            set { _StartDate = value; NotifyOfPropertyChange(nameof(StartDate)); }
        }

        private DateTime _EndDate;

        public DateTime EndDate
        {
            get { return _EndDate; }
            set { _EndDate = value; NotifyOfPropertyChange(nameof(EndDate)); }
        }
        private DateTime _SelectedStartDate;
        /// <summary>
        /// Selected Start Date For 
        /// Purchase By Starting Date
        /// </summary>
        public DateTime SelectedStartDate
        {
            get { return _SelectedStartDate; }
            set { _SelectedStartDate = value; NotifyOfPropertyChange(nameof(SelectedStartDate)); }
        }

        private bool _IsDisplayCombo;
        /// <summary>
        /// Used For Displaying Third Combo Box
        /// </summary>
        public bool IsDisplayCombo
        {
            get { return _IsDisplayCombo; }
            set { _IsDisplayCombo = value; NotifyOfPropertyChange(nameof(IsDisplayCombo)); }
        }

        private List<string> _ReportsCategory;
        /// <summary>
        /// Reports List For Displaying Report
        /// </summary>
        public List<string> ReportsCategory
        {
            get { return _ReportsCategory; }
            set { _ReportsCategory = value; NotifyOfPropertyChange(nameof(ReportsCategory)); }
        }

        private string _SelectedReportCategory;
        /// <summary>
        /// Selected Report
        /// </summary>
        public string SelectedReportCategory
        {
            get { return _SelectedReportCategory; }
            set { _SelectedReportCategory = value; NotifyOfPropertyChange(nameof(SelectedReportCategory)); OnSelectedCategory(SelectedReportCategory); }
        }

        private List<string> _ReportsSubCategory;
        /// <summary>
        /// Selected Reports By Sub Category Like Vendor Partners, Sale Partners etc...
        /// </summary>
        public List<string> ReportsSubCategory
        {
            get { return _ReportsSubCategory; }
            set { _ReportsSubCategory = value; NotifyOfPropertyChange(nameof(ReportsSubCategory)); }
        }
        private string _SelectedReportSubCategory;
        /// <summary>
        /// Selected Report By Sub Category Like Vendor Partners, Sale Partners etc...
        /// </summary>
        public string SelectedReportSubCategory
        {
            get { return _SelectedReportSubCategory; }
            set { _SelectedReportSubCategory = value; NotifyOfPropertyChange(nameof(SelectedReportSubCategory)); OnSelectedSubCategory(SelectedReportSubCategory); }
        }

        private List<string> _BissinessPartnerReports;

        public List<string> BissinessPartnerReports
        {
            get { return _BissinessPartnerReports; }
            set { _BissinessPartnerReports = value; NotifyOfPropertyChange(nameof(BissinessPartnerReports)); }
        }
        private List<string> _PurchaseReports;
        /// <summary>
        /// Reports of Purchase
        /// </summary>
        public List<string> PurchaseReports
        {
            get { return _PurchaseReports; }
            set { _PurchaseReports = value; NotifyOfPropertyChange(nameof(_PurchaseReports)); }
        }

        private List<string> _ProductReports;

        public List<string> ProductReports
        {
            get { return _ProductReports; }
            set { _ProductReports = value; NotifyOfPropertyChange(nameof(ProductReports)); }
        }

        private List<string> _SaleReport;

        public List<string> SaleReport
        {
            get { return _SaleReport; }
            set { _SaleReport = value; NotifyOfPropertyChange(nameof(SaleReport)); }
        }
        private List<string> _PaymentReport;

        public List<string> PaymentReport
        {
            get { return _PaymentReport; }
            set { _PaymentReport = value; NotifyOfPropertyChange(nameof(PaymentReport)); }
        }

        private List<string> _BankAccount;

        public List<string> BankAccount
        {
            get { return _BankAccount; }
            set { _BankAccount = value; NotifyOfPropertyChange(nameof(BankAccount)); }
        }

        private string _SearchText;

        public string SearchText
        {
            get { return _SearchText; }
            set { _SearchText = value; NotifyOfPropertyChange(nameof(SearchText)); OnGettingText(SearchText); }
        }
        private List<BussinessPartnerModel> _Partners;
        /// <summary>
        /// List Of Partners
        /// </summary>
        public List<BussinessPartnerModel> Partners
        {
            get { return _Partners; }
            set { _Partners = value; NotifyOfPropertyChange(nameof(Partners)); }
        }
        private BussinessPartnerModel _SelectedPartner;
        /// <summary>
        /// Selected Partner From List
        /// </summary>
        public BussinessPartnerModel SelectedPartner
        {
            get { return _SelectedPartner; }
            set { _SelectedPartner = value; NotifyOfPropertyChange(nameof(SelectedPartner)); OnSelectingPartner(); }
        }




        #endregion
    }
}
