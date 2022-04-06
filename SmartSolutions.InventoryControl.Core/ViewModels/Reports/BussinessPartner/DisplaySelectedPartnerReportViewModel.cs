using Caliburn.Micro;
using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace SmartSolutions.InventoryControl.Core.ViewModels.Reports.BussinessPartner
{
    [Export(typeof(DisplaySelectedPartnerReportViewModel)), PartCreationPolicy(CreationPolicy.Shared)]
    public class DisplaySelectedPartnerReportViewModel : BaseViewModel, IHandle<object>
    {
        #region [Private Members]
        private readonly IEventAggregator _eventAggregator;
        private readonly DAL.Managers.Bussiness_Partner.IBussinessPartnerManager _bussinessPartnerManager;
        private readonly DAL.Managers.Bussiness_Partner.IPartnerLedgerManager _partnerLedgerManager;

        #endregion

        #region [Constructor]
        public DisplaySelectedPartnerReportViewModel()
        {

        }
        [ImportingConstructor]
        public DisplaySelectedPartnerReportViewModel(IEventAggregator eventAggregator
                                                    , DAL.Managers.Bussiness_Partner.IBussinessPartnerManager bussinessPartnerManager
                                                    , DAL.Managers.Bussiness_Partner.IPartnerLedgerManager partnerLedgerManager)
        {
            _eventAggregator = eventAggregator;
            _bussinessPartnerManager = bussinessPartnerManager;
            _partnerLedgerManager = partnerLedgerManager;
        }
        #endregion

        #region [Methods]
        protected async override void OnActivate()
        {
            base.OnActivate();
            _eventAggregator.Subscribe(this);
            Handle(Partner);
            if (Partner == null) return;
            else
                PartnerLedgers = (await _partnerLedgerManager.GetPartnerBalanceSheetAsync(Partner.Id ?? 0)).ToList();
            if(PartnerLedgers != null || PartnerLedgers?.Count > 0)
            {
                foreach (var ledger in PartnerLedgers)
                {
                    ledger.Partner = Partner;
                }
            }
        }
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            _eventAggregator.Subscribe(this);
            if (Partner == null) return;
        }
        private async void OnSelectedPartner()
        {
            if (Partner == null) return;
            PartnerLedgers = (await _partnerLedgerManager.GetPartnerBalanceSheetAsync(Partner.Id ?? 0)).ToList();
        }

        public void Handle(object message)
        {
            if (message == null) return;
            if (message is BussinessPartnerModel)
                Partner = message as BussinessPartnerModel;
        }
        #endregion

        #region [Properties]
        private List<BussinessPartnerLedgerModel> _PartnerLedgers;
        /// <summary>
        /// Balance sheet Of Selected Partner
        /// </summary>
        public List<BussinessPartnerLedgerModel> PartnerLedgers
        {
            get { return _PartnerLedgers; }
            set { _PartnerLedgers = value; NotifyOfPropertyChange(nameof(PartnerLedgers)); }
        }

        private BussinessPartnerLedgerModel _SelectedTransaction;
        /// <summary>
        /// Selected Transaction Of Selected Partner
        /// </summary>
        public BussinessPartnerLedgerModel SelectedTransaction
        {
            get { return _SelectedTransaction; }
            set { _SelectedTransaction = value; NotifyOfPropertyChange(nameof(SelectedTransaction)); }
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

        private BussinessPartnerModel _Partner;
        /// <summary>
        /// Selected Partner
        /// </summary>
        public BussinessPartnerModel Partner
        {
            get { return _Partner; }
            set { _Partner = value; NotifyOfPropertyChange(nameof(Partner)); OnSelectedPartner(); }
        }

        #endregion
    }
}
