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
    public class DisplaySelectedPartnerReportViewModel : BaseViewModel,IHandle<object>
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
            //Handle(Partner);
            _eventAggregator.Subscribe(this);
            Partners = (await _bussinessPartnerManager.GetAllBussinessPartnersAsync()).ToList();
            if(Partners != null)
            {
                if (Partner == null)
                    Partner = Partners.FirstOrDefault();
                PartnerLedger = (await _partnerLedgerManager.GetPartnerBalanceSheetAsync(Partner.Id ?? 0)).ToList();
            }
            //if (Partner == null) return;
            //PartnerLedger = (await _partnerLedgerManager.GetPartnerBalanceSheetAsync(Partner.Id ?? 0)).ToList();
        }
        private async void OnSelectedPartner()
        {
            if (Partner == null) return;
            PartnerLedger = (await _partnerLedgerManager.GetPartnerBalanceSheetAsync(Partner.Id ?? 0)).ToList();
        }

        public void Handle(object message)
        {
            if(message == null) return;
            if(message is BussinessPartnerModel)
                Partner = message as BussinessPartnerModel;
        }
        #endregion

        #region [Properties]
        private List<BussinessPartnerLedgerModel> _PartnerLedger;
        /// <summary>
        /// Balance sheet Of Selected Partner
        /// </summary>
        public List<BussinessPartnerLedgerModel> PartnerLedger
        {
            get { return _PartnerLedger; }
            set { _PartnerLedger = value; NotifyOfPropertyChange(nameof(PartnerLedger)); }
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
