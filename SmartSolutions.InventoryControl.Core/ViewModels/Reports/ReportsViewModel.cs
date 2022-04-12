using Caliburn.Micro;
using SmartSolutions.InventoryControl.Core.ViewModels.Reports.BussinessPartner;
using SmartSolutions.InventoryControl.Core.ViewModels.Reports.Product;
using SmartSolutions.InventoryControl.DAL.Managers.Product;
using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
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
        }

        public void Handle(Screen screen)
        {
            if (screen is DisplayAllPartnersViewModel
                || screen is AllProductReportViewModel
                || screen is DisplaySelectedPartnerReportViewModel)
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
                    default:
                    case "Sales":
                        ReportsSubCategory = BissinessPartnerReports;
                        break;
                    case "Payments":
                        ReportsSubCategory = PurchaseReports;
                        break;
                    case "Bank Accounts":
                        ReportsSubCategory = ProductReports;
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
                    Handle(selectedReportSubCategory);
                    Handle(IoC.Get<DisplayAllPartnersViewModel>());
                    break;
                case "Business Partner By Vendor":
                    Handle(selectedReportSubCategory);
                    Handle(IoC.Get<DisplayAllPartnersViewModel>());
                    break;
                case "Business Partner By Seller":
                    Handle(selectedReportSubCategory);
                    Handle(IoC.Get<DisplayAllPartnersViewModel>());
                    break;
                case "Business Partner By City":
                    Handle(selectedReportSubCategory);
                    Handle(IoC.Get<DisplayAllPartnersViewModel>());
                    break;
                case "Business Partner By DR Balance":
                    Handle(selectedReportSubCategory);
                    Handle(IoC.Get<DisplayAllPartnersViewModel>());
                    break;
                case "Business Partner By CR Balance":
                    Handle(selectedReportSubCategory);
                    Handle(IoC.Get<DisplayAllPartnersViewModel>());
                    break;
                case "Business Partner Balance sheet":
                    IsDisplayCombo = true;
                    OnSelectingBalanceSheet();

                    break;
                #endregion

                #region Products
                case "All Products":
                    Handle(IoC.Get<AllProductReportViewModel>());
                    break;
                case "Product By Name":
                    Handle(IoC.Get<DisplayAllPartnersViewModel>());
                    break;
                case "Product By Size":
                    Handle(IoC.Get<DisplayAllPartnersViewModel>());
                    break;
                case "Product By Color":
                    Handle(IoC.Get<DisplayAllPartnersViewModel>());
                    break;
                #endregion

                #region Purchase

                #endregion

                #region Sales

                #endregion

                #region Payments

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


        #endregion

        #region Properties
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

        private string _SearchText;

        public string SearchText
        {
            get { return _SearchText; }
            set { _SearchText = value; NotifyOfPropertyChange(nameof(SearchText)); }
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
