using Caliburn.Micro;
using SmartSolutions.InventoryControl.DAL.Managers.Bussiness_Partner;
using SmartSolutions.InventoryControl.DAL.Models.Bank;
using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using SmartSolutions.InventoryControl.DAL.Models.Payments;
using SmartSolutions.Util.LogUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace SmartSolutions.InventoryControl.Core.ViewModels
{
    [Export(typeof(PaymentViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PaymentViewModel : BaseViewModel, IHandle<BankAccountModel>
    {
        #region Private Members
        private readonly IEventAggregator _eventAggregator;
        private readonly IBussinessPartnerManager _bussinessPartnerManager;
        private readonly IPartnerLedgerManager _partnerLedgerManager;
        private readonly DAL.Managers.Payments.IPaymentManager _paymentManager;
        private readonly DAL.Managers.Payments.IPaymentTypeManager _paymentTypeManager;
        private readonly DAL.Managers.Bank.IBankAccountManager _bankAccountManager;
        #endregion

        #region Constructor
        public PaymentViewModel() { }

        [ImportingConstructor]
        public PaymentViewModel(IEventAggregator eventAggregator
                                , IBussinessPartnerManager bussinessPartnerManager
                                , DAL.Managers.Payments.IPaymentTypeManager paymentTypeManager
                                , DAL.Managers.Payments.IPaymentManager paymentManager
                                , IPartnerLedgerManager partnerLedgerManager, DAL.Managers.Bank.IBankAccountManager bankAccountManager)
        {
            _eventAggregator = eventAggregator;
            _bussinessPartnerManager = bussinessPartnerManager;
            _paymentTypeManager = paymentTypeManager;
            _paymentManager = paymentManager;
            _partnerLedgerManager = partnerLedgerManager;
            _bankAccountManager = bankAccountManager;
        }
        #endregion

        #region Methods
        protected async override void OnActivate()
        {
            base.OnActivate();
            PaymentTypes = (await _paymentTypeManager.GetAllPaymentMethodsAsync()).ToList();
            Payment = new PaymentModel();
            BussinessPartners = (await _bussinessPartnerManager.GetAllBussinessPartnersAsync()).ToList();
            PartnerSuggetion = new Helpers.SuggestionProvider.PartnerSuggestionProvider(BussinessPartners);
            IsReceiveAmount = true;
            _eventAggregator.Subscribe(this);
        }
        public async void SearchPartner(string searchText)
        {
            try
            {
                BussinessPartners = (await _bussinessPartnerManager.GetAllBussinessPartnersAsync(searchText)).ToList();
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        public async void Save()
        {
            try
            {
                if (Payment == null) return;
                Payment.PaymentAmount = Amount;
                Payment.PaymentMethod = SelectedPaymentType;
                PaymentImage = PaymentImage;
                Payment.Partner = SelectedPartner;

                //If we recive the Payment then it is CR or Payable vice versa 
                Payment.PaymentType = IsReceiveAmount == true ? DAL.Models.PaymentType.Payable : DAL.Models.PaymentType.Receivable;
                if (Payment.PaymentType == DAL.Models.PaymentType.Receivable)
                    Payment.Receivable = Amount;
                else
                    Payment.Payable = Amount;
                //If Bank is Selected the Enter Amount into Bank
                if (SelectedPaymentType != null && SelectedPaymentType.Id == 6)
                {
                    if (Amount == 0 || Amount < 0)
                        await IoC.Get<IDialogManager>().ShowMessageBoxAsync("Please Enter Amount Of Payment",options: Dialogs.MessageBoxOptions.Ok);
                    if (BankAccount != null)
                    {
                        BankAccount.Receivable = Payment.Receivable;
                        BankAccount.Payable = Payment.Payable;
                        BankAccount.Description = $"{SelectedPartner?.Name} Payment is {Payment.PaymentType} At {DateTime.Now} Throught {BankAccount?.Branch?.Name} in {BankAccount.AccountNumber}";
                        await _bankAccountManager.AddBankTransactionAsync(BankAccount); 
                    }
                }
                Payment.Description = string.IsNullOrEmpty(Description) ? $"Payment of {Amount} is made through {SelectedPaymentType.PaymentType} At {DateTime.Now}" : Description;
                var paymentResult = await _paymentManager.AddPaymentAsync(payment: Payment);
                if (paymentResult)
                {
                    NotificationManager.Show(new Notifications.Wpf.NotificationContent { Title = "Success", Message = "Payment Added Successfully", Type = Notifications.Wpf.NotificationType.Success });
                    var partnerLedger = new BussinessPartnerLedgerModel();
                    partnerLedger.Partner = SelectedPartner;
                    partnerLedger.Payment = await _paymentManager.GetLastPaymentByPartnerIdAsync(SelectedPartner?.Id ?? 0);
                    var partnerCurrentStatus = new BussinessPartnerLedgerModel();
                    partnerCurrentStatus = CalculateCurrentBalanceAndBalnceType(partnerLedger);
                    partnerLedger.Receivable = Payment.Receivable;
                    partnerLedger.Payable = Payment.Payable;
                    partnerLedger.Description = $"{SelectedPartner.BussinessName} Made Payment {Payment.PaymentType} Amount Of {Amount} through {Payment.PaymentMethod.PaymentType}";
                    partnerLedger.CurrentBalance = partnerCurrentStatus.CurrentBalance;//partnerLedger?.Payment?.PaymentType == DAL.Models.PaymentType.DR ? partnerLedger.CurrentBalance - partnerLedger.Payment.PaymentAmount : partnerLedger.CurrentBalance + partnerLedger.Payment.PaymentAmount;
                    partnerLedger.CurrentBalanceType = partnerCurrentStatus.CurrentBalanceType;//partnerLedger.Payment.PaymentType;
                    var ledgerResult = await _partnerLedgerManager.UpdatePartnerCurrentBalanceAsync(partnerLedger);
                    if (ledgerResult)
                    {
                        NotificationManager.Show(new Notifications.Wpf.NotificationContent { Title = "Success", Message = "Partner Ledger Updated Successfully ", Type = Notifications.Wpf.NotificationType.Success });
                        ClearPaymentData();
                    }
                    else
                        NotificationManager.Show(new Notifications.Wpf.NotificationContent { Title = "Error", Message = "Partner Ledger Not Updated ", Type = Notifications.Wpf.NotificationType.Error });
                }
                else
                {
                    NotificationManager.Show(new Notifications.Wpf.NotificationContent { Title = "Error", Message = "Payment Cannot Added ", Type = Notifications.Wpf.NotificationType.Error });
                }

            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }

        private BussinessPartnerLedgerModel CalculateCurrentBalanceAndBalnceType(BussinessPartnerLedgerModel partnerLedger)
        {
            var retVal = new BussinessPartnerLedgerModel();
            try
            {
                //It Means We are Receiving Money and it is our Liablity
                if (partnerLedger.CurrentBalance == 0 && IsReceiveAmount)
                {
                    retVal.CurrentBalance = Amount;
                    retVal.CurrentBalanceType = DAL.Models.PaymentType.Payable;

                }
                //It Means We are Paying Money and it is our Assets
                else if (partnerLedger.CurrentBalance == 0 && IsPayAmount)
                {
                    retVal.CurrentBalance = Amount;
                    retVal.CurrentBalanceType = DAL.Models.PaymentType.Receivable;
                }
                // We Assume that Partner 
                if (partnerLedger.CurrentBalance > 0
                    && IsReceiveAmount
                    && (partnerLedger.CurrentBalanceType == DAL.Models.PaymentType.Receivable))
                {
                    var result = partnerLedger.CurrentBalance - Amount;
                    //we assume amount recived from Partner is Less then his balance
                    if (result > 0)
                    {
                        partnerLedger.CurrentBalance = result;
                        partnerLedger.CurrentBalanceType = DAL.Models.PaymentType.Receivable;
                    }
                    //we assume amount received from partner is greater then his Balance
                    else
                    {
                        partnerLedger.CurrentBalance = Math.Abs(result);
                        partnerLedger.CurrentBalanceType = DAL.Models.PaymentType.Payable;
                    }

                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }

        private void ClearPaymentData()
        {
            SelectedPartner = null;// new BussinessPartnerModel();
            Payment = new PaymentModel();
            CurrentPartnerBalance = 0;
            Amount = 0;
            PaymentImage = null;
            IsValueCredit = false;
        }

        public void Cancel()
        {
            TryClose();
        }
        public async void GetPartnerBalance()
        {
            if (SelectedPartner == null)
                return;
            var partnerLedger = await _partnerLedgerManager.GetPartnerLedgerCurrentBalanceAsync(SelectedPartner?.Id.Value ?? 0);
            if (partnerLedger != null)
            {
                if (partnerLedger.CurrentBalanceType == DAL.Models.PaymentType.Receivable)
                    IsValueCredit = true;
                else
                    IsValueCredit = false;
                CurrentPartnerBalance = partnerLedger.CurrentBalance;
                AmountType = partnerLedger?.CurrentBalanceType.ToString();
                IsAmountAvailable = true;
            }

        }

        public async void GetPartnerBalanceSheet()
        {
            try
            {
                var dlg = IoC.Get<BussinessPartner.PartnerBalanceViewModel>();
                if (SelectedPartner != null)
                {
                    dlg.SelectedPartner = SelectedPartner;
                    dlg.GetPartnerBalanceSheet(SelectedPartner);
                    await IoC.Get<IDialogManager>().ShowDialogAsync(dlg);
                }
                else
                {
                    NotificationManager.Show(new Notifications.Wpf.NotificationContent { Title = "Information", Message = "Please Select Partner First", Type = Notifications.Wpf.NotificationType.Information });
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        private async void OnPaymentSelectedType(PaymentTypeModel selectedPaymentType)
        {
            try
            {
                // Null guard
                if (selectedPaymentType == null) return;
                if (SelectedPartner != null)
                {
                    if (selectedPaymentType.PaymentType.Equals("Bank"))
                    {
                        _eventAggregator.Subscribe(this);
                        var dlg = IoC.Get<Dialogs.BankPaymentDialogViewModel>();
                        await IoC.Get<IDialogManager>().ShowDialogAsync(dlg);
                        Handle(dlg.SelectedBankAccount);
                    }

                }
                else
                {
                    await IoC.Get<IDialogManager>().ShowMessageBoxAsync("Please Select Partner First", options: Dialogs.MessageBoxOptions.Ok);
                }

            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }

        public void Handle(BankAccountModel message)
        {
            BankAccount = message;
        }
        #endregion

        #region Properties
        private BankAccountModel _BankAccount;
        /// <summary>
        /// Get the Account Number If Bank is Selected
        /// </summary>
        public BankAccountModel BankAccount
        {
            get { return _BankAccount; }
            set { _BankAccount = value; }
        }

        private bool _IsValueCredit;
        /// <summary>
        /// Change background on Value if Credit Or Not
        /// </summary>
        public bool IsValueCredit
        {
            get { return _IsValueCredit; }
            set { _IsValueCredit = value; NotifyOfPropertyChange(nameof(IsValueCredit)); }
        }

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
            get { return _PartnerSuggetion ?? new Helpers.SuggestionProvider.PartnerSuggestionProvider(BussinessPartners); }
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
        private PaymentModel _Payment;

        public PaymentModel Payment
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
        private decimal _CurrentPartnerBalance;
        /// <summary>
        /// Current Selected Partner Balance Amount
        /// </summary>
        public decimal CurrentPartnerBalance
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
        private List<PaymentTypeModel> _PaymentTypes;
        /// <summary>
        /// List Of Payment Types Like Cash, JazzCash etc...
        /// </summary>
        public List<PaymentTypeModel> PaymentTypes
        {
            get { return _PaymentTypes; }
            set { _PaymentTypes = value; NotifyOfPropertyChange(nameof(PaymentTypes)); }
        }
        private PaymentTypeModel _SelectedPaymentType;
        /// <summary>
        /// Selected Type Of Payment Like JazzCash etc...
        /// </summary>
        public PaymentTypeModel SelectedPaymentType
        {
            get { return _SelectedPaymentType; }
            set { _SelectedPaymentType = value; NotifyOfPropertyChange(nameof(SelectedPaymentType)); OnPaymentSelectedType(SelectedPaymentType); }
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
