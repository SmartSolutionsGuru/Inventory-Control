using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using SmartSolutions.InventoryControl.DAL.Managers.Bussiness_Partner;
using System.Text;
using System.Linq;
using SmartSolutions.Util.LogUtils;
using Caliburn.Micro;

namespace SmartSolutions.InventoryControl.Core.ViewModels
{
    [Export(typeof(PaymentViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PaymentViewModel : BaseViewModel
    {
        #region Private Members
        private readonly IBussinessPartnerManager _bussinessPartnerManager;
        #endregion
        #region Constructor
        [ImportingConstructor]
        public PaymentViewModel(IBussinessPartnerManager bussinessPartnerManager)
        {
            _bussinessPartnerManager = bussinessPartnerManager;
        }
        #endregion

        #region Methods
        protected async override void OnActivate()
        {
            base.OnActivate();
            Payment = new DAL.Models.PaymentModel();
            BussinessPartners = (await _bussinessPartnerManager.GetAllBussinessPartnersAsync()).ToList();
            PartnerSuggetion = new Helpers.SuggestionProvider.PartnerSuggestionProvider(BussinessPartners);
            IsReceiveAmount = true;
        }
        public void Save()
        {
            try
            {
                //IoC.Get<IDialogManager>().ShowMessageBoxAsync("My Box",options: Dialogs.MessageBoxOptions.Ok);
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
        public async void GetPartnerBalance()
        {
            if (SelectedPartner == null)
                return;
            var balance = await _bussinessPartnerManager.GetPartnerCurrentBalanceAsync(SelectedPartner.Id.Value);

        }
        #endregion

        #region Properties
        private Helpers.SuggestionProvider.PartnerSuggestionProvider _PartnerSuggetion;

        public Helpers.SuggestionProvider.PartnerSuggestionProvider PartnerSuggetion
        {
            get { return _PartnerSuggetion; }
            set { _PartnerSuggetion = value; NotifyOfPropertyChange(nameof(PartnerSuggetion)); }
        }

        private List<DAL.Models.BussinessPartnerModel> _BussinessPartners;
        /// <summary>
        /// List Of Bussiness Partners
        /// </summary>
        public List<DAL.Models.BussinessPartnerModel> BussinessPartners
        {
            get { return _BussinessPartners; }
            set { _BussinessPartners = value; NotifyOfPropertyChange(nameof(BussinessPartners)); }
        }

        private DAL.Models.BussinessPartnerModel _SelectedPartner;
        /// <summary>
        /// Selected Bussiness Partner for Payments
        /// </summary>
        public DAL.Models.BussinessPartnerModel SelectedPartner
        {
            get { return _SelectedPartner; }
            set { _SelectedPartner = value; NotifyOfPropertyChange(nameof(SelectedPartner)); GetPartnerBalance(); }
        }
        private DAL.Models.PaymentModel _Payment;

        public DAL.Models.PaymentModel Payment
        {
            get { return _Payment; }
            set { _Payment = value; NotifyOfPropertyChange(nameof(Payment)); }
        }
        private byte[] _PaymentImage;

        public byte[] PaymentImage
        {
            get { return _PaymentImage; }
            set { _PaymentImage = value; NotifyOfPropertyChange(nameof(PaymentImage)); }
        }
        private double _CurrentPartnerBalance;
        /// <summary>
        /// Current Selected Partner Balance Amount
        /// </summary>
        public double CurrentPartnerBalance
        {
            get { return _CurrentPartnerBalance; }
            set { _CurrentPartnerBalance = value; NotifyOfPropertyChange(nameof(CurrentPartnerBalance)); }
        }

        private bool _IsReceiveAmount;
        /// <summary>
        /// Is Payment Received Or Paid
        /// </summary>
        public bool IsReceiveAmount
        {
            get { return _IsReceiveAmount; }
            set { _IsReceiveAmount = value; NotifyOfPropertyChange(nameof(IsReceiveAmount)); }
        }

        private bool _IsPayAmount;

        public bool IsPayAmount
        {
            get { return _IsPayAmount; }
            set { _IsPayAmount = value; NotifyOfPropertyChange(nameof(IsPayAmount)); }
        }

        #endregion
    }
}
