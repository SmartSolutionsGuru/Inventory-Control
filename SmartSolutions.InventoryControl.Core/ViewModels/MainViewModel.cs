using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace SmartSolutions.InventoryControl.Core.ViewModels
{
    [Export(typeof(MainViewModel)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class MainViewModel : BaseViewModel /*Conductor<Screen>*/, IHandle<Screen>
    {
        #region Private Members
        private readonly IEventAggregator _eventAggregator;

        #endregion

        #region Constructor
        [ImportingConstructor]
        public MainViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }
        #endregion

        #region Methods

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
        }
        protected override void OnActivate()
        {
            if(Execute.InDesignMode)
            {
                //IsLoading = false;
            }
            base.OnActivate();
            NowTime = DateTime.Now;
        }
        public void Product()
        {
            IsProductProceeded = true;
            IsPartnerProceeded = false;
            IsPurchaseProceeded = false;
            IsSalesProceeded = false;
            IsReportProceeded = false;
            IsPaymentProceeded = false;
            Handle(IoC.Get<Product.ProductViewModel>());
        }
        public void BussinessPartner()
        {
            IsPartnerProceeded = true;
            IsProductProceeded = false;
            IsPurchaseProceeded = false;
            IsSalesProceeded = false;
            IsReportProceeded = false;
            IsPaymentProceeded = false;
            Handle(IoC.Get<BussinessPartner.BussinessPartnerViewModel>());
        }
        public void Purchase()
        {
            IsPurchaseProceeded = true;
            IsPartnerProceeded = false;
            IsProductProceeded = false;
            IsSalesProceeded = false;
            IsReportProceeded = false;
            IsPaymentProceeded = false;
            Handle(IoC.Get<PurchaseViewModel>());
        }
        public void Sales()
        {
            IsPurchaseProceeded = false;
            IsPartnerProceeded = false;
            IsProductProceeded = false;
            IsReportProceeded = false;
            IsPaymentProceeded = false;
            IsSalesProceeded = true;
            Handle(IoC.Get<SalesViewModel>());
        }
        public void Reports ()
        {
            IsPurchaseProceeded = false;
            IsPartnerProceeded = false;
            IsProductProceeded = false;
            IsReportProceeded = true;
            IsSalesProceeded = false;
            IsPaymentProceeded = false;
        }
        public void Payments()
        {
            IsPaymentProceeded = true;
            IsPurchaseProceeded = false;
            IsPartnerProceeded = false;
            IsProductProceeded = false;
            IsReportProceeded = false;
            IsSalesProceeded = false;
            Handle(IoC.Get<PaymentViewModel>());
        }
        public void Handle(Screen screen)
        {

            if (screen is Product.ProductViewModel || screen is BussinessPartner.BussinessPartnerViewModel || screen is PurchaseViewModel || screen is PaymentViewModel || screen is SalesViewModel)
                {
                    ActivateItem(screen);
                }
            }
        #endregion

        #region Properties
        private bool _IsProductProceeded;
         /// <summary>
         /// For Product Button Pressed
         /// </summary>
        public bool IsProductProceeded
        {
            get { return _IsProductProceeded; }
            set { _IsProductProceeded = value; NotifyOfPropertyChange(nameof(IsProductProceeded)); }
        }
        private bool _IsPartnerProceeded;
        /// <summary>
        /// For Partner Button
        /// </summary>
        public bool IsPartnerProceeded
        {
            get { return _IsPartnerProceeded; }
            set { _IsPartnerProceeded = value; NotifyOfPropertyChange(nameof(IsPartnerProceeded)); }
        }
        private bool _IsPurchaseProceeded;
         /// <summary>
         /// For Purchase Button Pressed
         /// </summary>
        public bool IsPurchaseProceeded
        {
            get { return _IsPurchaseProceeded; }
            set { _IsPurchaseProceeded = value; NotifyOfPropertyChange(nameof(IsPurchaseProceeded)); }
        }
        private bool _IsSalesProceeded;
         /// <summary>
         /// For Sale Button Is Pressed
         /// </summary>
        public bool IsSalesProceeded
        {
            get { return _IsSalesProceeded; }
            set { _IsSalesProceeded = value; NotifyOfPropertyChange(nameof(IsSalesProceeded)); }
        }
        private bool _IsReportProceeded;
         /// <summary>
         /// Report button Selected
         /// </summary>
        public bool IsReportProceeded
        {
            get { return _IsReportProceeded; }
            set { _IsReportProceeded = value; NotifyOfPropertyChange(nameof(IsReportProceeded)); }
        }
        private bool _IsPrinterAvailable;
        /// <summary>
        /// Priner Available Or Not
        /// </summary>
        public bool IsPrinterAvailable
        {
            get { return _IsPrinterAvailable; }
            set { _IsPrinterAvailable = value; NotifyOfPropertyChange(nameof(IsPrinterAvailable)); }
        }
        private bool _IsSettingActive;

        public bool IsSettingActive
        {
            get { return _IsSettingActive; }
            set { _IsSettingActive = value; NotifyOfPropertyChange(nameof(IsSettingActive)); }
        }
        private bool _IsDataSaved;
         /// <summary>
         /// Data backup Updated Or Not
         /// </summary>
        public bool IsDataSaved
        {
            get { return _IsDataSaved; }
            set { _IsDataSaved = value; NotifyOfPropertyChange(nameof(IsDataSaved)); }
        }
        private DateTime _NowTime;

        public DateTime NowTime
        {
            get { return _NowTime; }
            set { _NowTime = value; NotifyOfPropertyChange(nameof(NowTime)); }
        }
        private string _PrinterName;
         /// <summary>
         /// Printer Name if Available
         /// </summary>
        public string PrinterName
        {
            get { return _PrinterName; }
            set { _PrinterName = value; NotifyOfPropertyChange(nameof(PrinterName)); }
        }
        private bool _IsPaymentProceeded;
         /// <summary>
         /// For Payment Options
         /// </summary>
        public bool IsPaymentProceeded
        {
            get { return _IsPaymentProceeded; }
            set { _IsPaymentProceeded = value; NotifyOfPropertyChange(nameof(IsPaymentProceeded)); }
        }

        #endregion
    }
}
