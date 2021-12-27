using Caliburn.Micro;
using SmartSolutions.InventoryControl.Core.ViewModels.Reports.BussinessPartner;
using SmartSolutions.InventoryControl.DAL.Managers.Product;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace SmartSolutions.InventoryControl.Core.ViewModels.Reports
{
    [Export(typeof(ReportsViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ReportsViewModel : BaseViewModel, IHandle<Screen>
    {

        #region Private Members
        private readonly IProductManager _productManager;
        #endregion

        #region Costructor
        [ImportingConstructor]
        public ReportsViewModel(IProductManager productManager)
        {
            _productManager = productManager;
        }
        #endregion

        #region Methods
        protected override void OnActivate()
        {
            base.OnActivate();
            ReportsCategory = new List<string>
            {
                "Bussiness Partners",
                "Purchase",
                "Products",
                "Sales",
                "Payments",
                "Bank Accounts"
            };
            BissinessPartnerReports = new List<string>
            {
                "All Bussiness Partners",
                "Bussiness Partner By Vender",
                "Bussiness Partner By Seller",
                "Bussiness Partner By City",
                "Bussiness Partner By DR Balance",
                "Bussiness Partner By CR Balance"
            };
            ProductReports = new List<string> 
            {
                "All Products",
                "Product By Name",
                "Produt By Size",
                "Product By Color"
            };
        }

        public void Handle(Screen screen)
        {
            if (screen is DisplayAllPartnersViewModel)
            {
                ActiveItem = screen;
                ActivateItem(screen);
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
                    case "Bussiness Partners":
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
                #region Bussiness Partner
                case "All Bussiness Partners":
                    Handle(IoC.Get<DisplayAllPartnersViewModel>());
                    break;
                case "Bussiness Partner By Vender":
                    Handle(IoC.Get<DisplayAllPartnersViewModel>());
                    break;
                case "Bussiness Partner By Seller":
                    Handle(IoC.Get<DisplayAllPartnersViewModel>());
                    break;
                case "Bussiness Partner By City":
                    Handle(IoC.Get<DisplayAllPartnersViewModel>());
                    break;
                case "Bussiness Partner By DR Balance":
                    Handle(IoC.Get<DisplayAllPartnersViewModel>());
                    break;
                case "Bussiness Partner By CR Balance":
                    Handle(IoC.Get<DisplayAllPartnersViewModel>());
                    break;
                #endregion
                #region Products
                case "All Products":
                    Handle(IoC.Get<DisplayAllPartnersViewModel>());
                    break;
                case "Product By Name":
                    Handle(IoC.Get<DisplayAllPartnersViewModel>());
                    break;
                case "Produt By Size":
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
                #region Bnk Accounts

                #endregion
                default:
                    break;
            }
        }
        #endregion

        #region Properties
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
        /// Selected Reports By Sub Category Like Vender Partners, Sale Partners etc...
        /// </summary>
        public List<string> ReportsSubCategory
        {
            get { return _ReportsSubCategory; }
            set { _ReportsSubCategory = value; NotifyOfPropertyChange(nameof(ReportsSubCategory)); }
        }
        private string _SelectedReportSubCategory;
        /// <summary>
        /// Selected Report By Sub Category Like Vender Partners, Sale Partners etc...
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


        #endregion
    }
}
