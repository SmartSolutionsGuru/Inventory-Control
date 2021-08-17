using SmartSolutions.InventoryControl.DAL.Managers.Bussiness_Partner;
using SmartSolutions.InventoryControl.DAL.Managers.Payments;
using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using SmartSolutions.Util.LogUtils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace SmartSolutions.InventoryControl.Core.ViewModels.BussinessPartner
{
    [Export(typeof(PartnerBalanceViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PartnerBalanceViewModel : BaseViewModel
    {
        #region Private Members
        private readonly IPartnerLedgerManager _partnerLedgerManager;
        private readonly IPaymentTypeManager _paymentTypeManager;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public PartnerBalanceViewModel(IPartnerLedgerManager partnerLedgerManager, IPaymentTypeManager paymentTypeManager)
        {
            _partnerLedgerManager = partnerLedgerManager;
            _paymentTypeManager = paymentTypeManager;
        }
        #endregion

        #region Methods
        protected override void OnActivate()
        {
            base.OnActivate();

        }
        public async void GetPartnerBalanceSheet(BussinessPartnerModel selectedPartner)
        {
            try
            {
                if (SelectedPartner != null)
                {
                    var partnerBalnceSheetResult  = (await _partnerLedgerManager.GetPartnerBalanceSheetAsync(SelectedPartner?.Id)).ToList();
                    foreach (var partnersheet in partnerBalnceSheetResult)
                    {
                        partnersheet.Partner = SelectedPartner;
                    }
                    PartnerBalanceSheet = new ObservableCollection<BussinessPartnerLedgerModel>(partnerBalnceSheetResult);
                   
                    HeadingText = $"{SelectedPartner.Name} Balance Sheet";
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        public void Close()
        {
            TryClose();
        }
        #endregion

        #region Properties
        private BussinessPartnerModel _SelectedPartner;
        /// <summary>
        /// Selected Partner 
        /// </summary>
        public BussinessPartnerModel SelectedPartner
        {
            get { return _SelectedPartner; }
            set { _SelectedPartner = value; NotifyOfPropertyChange(nameof(SelectedPartner)); }
        }

        private ObservableCollection<BussinessPartnerLedgerModel> _PartnerBalanceSheet;
        /// <summary>
        /// List for Displaying Partner Balance
        /// </summary>
        public ObservableCollection<BussinessPartnerLedgerModel> PartnerBalanceSheet
        {
            get { return _PartnerBalanceSheet; }
            set { _PartnerBalanceSheet = value; NotifyOfPropertyChange(nameof(PartnerBalanceSheet)); }
        }

        private BussinessPartnerLedgerModel _PartnerSelectedBalance;
        /// <summary>
        /// Partner  Selected  Balance 
        /// </summary>
        public BussinessPartnerLedgerModel PartnerSelectedBalance
        {
            get { return _PartnerSelectedBalance; }
            set { _PartnerSelectedBalance = value; NotifyOfPropertyChange(nameof(PartnerSelectedBalance)); }
        }

        private string _HeadingText;
         /// <summary>
         /// Heading Text With Partner Name
         /// </summary>
        public string HeadingText
        {
            get { return _HeadingText; }
            set { _HeadingText = value; NotifyOfPropertyChange(nameof(HeadingText)); }
        }

        #endregion
    }
}
