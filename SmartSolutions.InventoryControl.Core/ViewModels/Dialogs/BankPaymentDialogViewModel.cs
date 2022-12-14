using Caliburn.Micro;
using SmartSolutions.InventoryControl.DAL.Models.Bank;
using SmartSolutions.Util.LogUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace SmartSolutions.InventoryControl.Core.ViewModels.Dialogs
{
    [Export(typeof(BankPaymentDialogViewModel)), PartCreationPolicy(CreationPolicy.Shared)]
    public class BankPaymentDialogViewModel : BaseViewModel
    {
        #region Private Members
        private readonly IEventAggregator _eventAggregator;
        private readonly DAL.Managers.Bank.IBankManager _bankManager;
        private readonly DAL.Managers.Bank.IBankBranchManager _bankBranchManager;
        private readonly DAL.Managers.Bank.IBankAccountManager _bankAccountManager;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public BankPaymentDialogViewModel(IEventAggregator eventAggregator
                                         , DAL.Managers.Bank.IBankManager bankManager
                                         , DAL.Managers.Bank.IBankBranchManager bankBranchManager
                                         , DAL.Managers.Bank.IBankAccountManager bankAccountManager)
        {
            _eventAggregator = eventAggregator;
            _bankManager = bankManager;
            _bankBranchManager = bankBranchManager;
            _bankAccountManager = bankAccountManager;
        }
        #endregion

        #region Methods
        protected override async void OnActivate()
        {
            base.OnActivate();
            Banks = (await _bankManager.GetAllBanksAsync()).ToList();

        }
        private async void OnSelectedBank(BankModel selectedBank)
        {
            try
            {
                if (selectedBank == null) return;
                Branches = (await _bankBranchManager.GetBankBrachesByBankIdAsync(selectedBank.Id ?? 0)).ToList();
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        private async void OnSelectedBranch(BankBranchModel selectedBranch)
        {
            if (selectedBranch == null) return;
            BankAccounts = (await _bankAccountManager.GetAllBankAccountByBranchAsync(SelectedBranch?.Id)).ToList();
        }
        public void Close()
        {
            TryClose();
        }
        public void Cancel()
        {
            TryClose();
        }
        public async void Submit()
        {
            try
            {
                if (SelectedBankAccount == null)
                    NotificationManager.Show(new Notifications.Wpf.NotificationContent { Title = "Error", Message = "Please Select Bank Account", Type = Notifications.Wpf.NotificationType.Error });
                else
                {
                    SelectedBankAccount.Branch = SelectedBranch;
                    SelectedBankAccount.Branch.Bank = SelectedBank;
                    //await _bankAccountManager.AddBankTransactionAsync(SelectedBankAccount);
                    _eventAggregator.PublishOnBackgroundThread(this);
                    TryClose();
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }

        }
        #endregion

        #region Properties
        private List<BankModel> _Banks;
        /// <summary>
        /// List Of Banks 
        /// </summary>
        public List<BankModel> Banks
        {
            get { return _Banks; }
            set { _Banks = value; NotifyOfPropertyChange(nameof(Banks)); }
        }

        private BankModel _SelectedBank;
        /// <summary>
        /// Selected Bank 
        /// </summary>
        public BankModel SelectedBank
        {
            get { return _SelectedBank; }
            set { _SelectedBank = value; NotifyOfPropertyChange(nameof(SelectedBank)); OnSelectedBank(SelectedBank); }
        }


        private List<BankBranchModel> _Branches;
        /// <summary>
        /// List Of Selected Bank Branches
        /// </summary>
        public List<BankBranchModel> Branches
        {
            get { return _Branches; }
            set { _Branches = value; NotifyOfPropertyChange(nameof(Branches)); }
        }
        private BankBranchModel _SelectedBranch;
        /// <summary>
        /// Selected Branch Of Selected Bank
        /// </summary>
        public BankBranchModel SelectedBranch
        {
            get { return _SelectedBranch; }
            set { _SelectedBranch = value; NotifyOfPropertyChange(nameof(SelectedBranch)); OnSelectedBranch(SelectedBranch); }
        }



        private List<BankAccountModel> _BankAccounts;
        /// <summary>
        /// Bank Accounts From Specifc Bank And Branch
        /// </summary>
        public List<BankAccountModel> BankAccounts
        {
            get { return _BankAccounts; }
            set { _BankAccounts = value; NotifyOfPropertyChange(nameof(BankAccounts)); }
        }
        private BankAccountModel _SelectedBankAccount;
        /// <summary>
        /// finnaly Selected Account to Transfer Amount
        /// </summary>
        public BankAccountModel SelectedBankAccount
        {
            get { return _SelectedBankAccount; }
            set { _SelectedBankAccount = value; NotifyOfPropertyChange(nameof(SelectedBankAccount)); }
        }

        #endregion
    }
}
