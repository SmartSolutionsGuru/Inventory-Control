using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using SmartSolutions.InventoryControl.DAL.Managers.Bussiness_Partner;
using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using System.Text;
using System.Linq;
using SmartSolutions.Util.LogUtils;
using Caliburn.Micro;
using SmartSolutions.InventoryControl.DAL.Models.Transaction;
using SmartSolutions.InventoryControl.DAL.Managers.Invoice;
using SmartSolutions.InventoryControl.DAL.Managers.Transaction;

namespace SmartSolutions.InventoryControl.Core.ViewModels
{
    [Export(typeof(PaymentViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PaymentViewModel : BaseViewModel
    {
        #region Private Members
        private readonly IBussinessPartnerManager _bussinessPartnerManager;
        private readonly IInvoiceManager _invoiceManager;
        private readonly ITransactionManager _transactionManager;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public PaymentViewModel(IBussinessPartnerManager bussinessPartnerManager
                                , IInvoiceManager invoiceManager
                                , ITransactionManager transactionManager)
        {
            _bussinessPartnerManager = bussinessPartnerManager;
            _invoiceManager = invoiceManager;
            _transactionManager = transactionManager;
        }
        #endregion

        #region Methods
        protected async override void OnActivate()
        {
            base.OnActivate();
            PaymentTypes = new List<string> { "Unknown", "Cash", "Bank", "JazzCash", "Easy Paisa", "U Paisa", "Partial", "Other" };
            Payment = new DAL.Models.PaymentModel();
            BussinessPartners = (await _bussinessPartnerManager.GetAllBussinessPartnersAsync()).ToList();
            PartnerSuggetion = new Helpers.SuggestionProvider.PartnerSuggestionProvider(BussinessPartners);
            IsReceiveAmount = true;
        }
        public async void Save()
        {
            try
            {
                TransactionModel transaction = new TransactionModel();
                transaction.PaymentMode = IsPayAmount == true ? "Payable" : "Reciveable";
                transaction.BussinessPartner = SelectedPartner;
                transaction.PartnerLastInvoice = await _invoiceManager.GetPartnerLastInvoice(SelectedPartner?.Id);
                transaction.PaymentImage = PaymentImage;
                transaction.PaymentType = SelectedPaymentType;
                var resultTransaction = await _transactionManager.SaveTransactionAsync(transaction);
                if (resultTransaction)
                {
                    var partnerLedger = new BussinessPartnerLedgerModel();
                    //partnerLedger.
                }

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
            var partnerLedger = await _bussinessPartnerManager.GetPartnerCurrentBalanceAsync(SelectedPartner.Id.Value);
            if (partnerLedger != null)
            {
                CurrentPartnerBalance = partnerLedger.BalanceAmount;
                AmountType = partnerLedger?.IsBalancePayable == true ? "Payable" : "Receivable";
                IsAmountAvailable = true;
            }

        }
        #endregion

        #region Properties
        private string _AmountType;
        /// <summary>
        /// Last Balance Of Partner is Payable or Reciveable
        /// </summary>
        public string AmountType
        {
            get { return _AmountType; }
            set { _AmountType = value; NotifyOfPropertyChange(nameof(AmountType)); }
        }

        private bool _IsAmountAvailable;
        /// <summary>
        /// Flag for verifing Amount is Filled or Not
        /// </summary>
        public bool IsAmountAvailable
        {
            get { return _IsAmountAvailable; }
            set { _IsAmountAvailable = value; NotifyOfPropertyChange(nameof(IsAmountAvailable)); }
        }

        private Helpers.SuggestionProvider.PartnerSuggestionProvider _PartnerSuggetion;

        public Helpers.SuggestionProvider.PartnerSuggestionProvider PartnerSuggetion
        {
            get { return _PartnerSuggetion; }
            set { _PartnerSuggetion = value; NotifyOfPropertyChange(nameof(PartnerSuggetion)); }
        }

        private List<BussinessPartnerModel> _BussinessPartners;
        /// <summary>
        /// List Of Bussiness Partners
        /// </summary>
        public List<BussinessPartnerModel> BussinessPartners
        {
            get { return _BussinessPartners; }
            set { _BussinessPartners = value; NotifyOfPropertyChange(nameof(BussinessPartners)); }
        }

        private BussinessPartnerModel _SelectedPartner;
        /// <summary>
        /// Selected Bussiness Partner for Payments
        /// </summary>
        public BussinessPartnerModel SelectedPartner
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
        private decimal _Amount;
        /// <summary>
        /// Amount which is paid or Recieved
        /// </summary>
        public decimal Amount
        {
            get { return _Amount; }
            set { _Amount = value; NotifyOfPropertyChange(nameof(AmountType)); }
        }
        private List<string> _PaymentTypes;
        /// <summary>
        /// List Of Payment Types Like Cash, JazzCash etc...
        /// </summary>
        public List<string> PaymentTypes
        {
            get { return _PaymentTypes; }
            set { _PaymentTypes = value; NotifyOfPropertyChange(nameof(PaymentTypes)); }
        }
        private string _SelectedPaymentType;
        /// <summary>
        /// Selected Type Of Payment Like JazzCash etc...
        /// </summary>
        public string SelectedPaymentType
        {
            get { return _SelectedPaymentType; }
            set { _SelectedPaymentType = value; NotifyOfPropertyChange(nameof(SelectedPaymentType)); }
        }
        private string _Description;
        /// <summary>
        /// DEscription Of Payment Type
        /// </summary>
        public string Description
        {
            get { return _Description; }
            set { _Description = value; NotifyOfPropertyChange(nameof(Description)); }
        }


        #endregion
    }
}
