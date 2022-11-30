using Caliburn.Micro;
using SmartSolutions.InventoryControl.DAL;
using SmartSolutions.InventoryControl.DAL.Managers.Bank;
using SmartSolutions.InventoryControl.DAL.Models.Bank;
using SmartSolutions.Util.LogUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace SmartSolutions.InventoryControl.Core.ViewModels.Bank
{
    [Export(typeof(BankAccountViewModel)), PartCreationPolicy(CreationPolicy.Shared)]
    public class BankAccountViewModel : BaseViewModel
    {
        //TODO: Update And Delete Bank Account is Not Implemented
        #region Private Members
        private readonly IBankManager _bankManager;
        private readonly IBankBranchManager _bankBranchManager;
        private readonly IBankAccountManager _bankAccountManager;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public BankAccountViewModel(IBankManager bankManager, IBankBranchManager bankBranchManager, IBankAccountManager bankAccountManager)
        {
            _bankManager = bankManager;
            _bankBranchManager = bankBranchManager;
            _bankAccountManager = bankAccountManager;
        }
        #endregion

        #region Methods
        protected async override void OnActivate()
        {
            base.OnActivate();
            IsAddBankPressed = true;
            Banks = (await _bankManager.GetAllBanksAsync()).ToList();
            AccountTypes = new List<string> { "Current", "Saving", "Default" };
            AccountStatus = new List<string> { "Working", "Close", "Transition", "Foreign Currency", "Other" };
        }

        public async void AddBank()
        {
            try
            {
                var dlg = IoC.Get<BankViewModel>();
                await IoC.Get<IDialogManager>().ShowDialogAsync(dlg);
                Banks = (await _bankManager.GetAllBanksAsync()).ToList();
                SelectedBank = Banks?.LastOrDefault();

            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        public async void AddBranch()
        {
            try
            {
                var dlg = IoC.Get<BankBranchViewModel>();
                await IoC.Get<IDialogManager>().ShowDialogAsync(dlg);
                BankBranches = (await _bankBranchManager.GetBankBrachesByBankIdAsync(SelectedBank?.Id)).ToList();
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        private async void GetBranches(int? id)
        {
            if (id == null) return;
            try
            {
                BankBranches = (await _bankBranchManager.GetBankBrachesByBankIdAsync(id)).ToList();
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        public async void AddAccount()
        {
            try
            {

                var bankAccount = new BankAccountModel();
                bankAccount.Branch = SelectedBranch;
                bankAccount.AccountStatus = SelectedAccountStatus;
                bankAccount.AccountType = SelectedAccountType;
                bankAccount.AccountNumber = BankAccountNumber;
                bankAccount.OpeningDate = AccountOpeningDate;
                bankAccount.OpeningBalance = InitialBalance;
                bankAccount.Description = Description;
                bankAccount.CreatedBy = AppSettings.LoggedInUser.DisplayName;
                var verificationResult = await _bankAccountManager.IsAccountAlreadyExist(SelectedBranch?.Id, BankAccountNumber);
                if (verificationResult == false)
                {
                    var result = await _bankAccountManager.AddBankAccountAsync(bankAccount);
                    if (result)
                    {
                        NotificationManager.Show(new Notifications.Wpf.NotificationContent { Title = "Success", Message = "Successfully Added Bank Account", Type = Notifications.Wpf.NotificationType.Success });
                        SelectedAccountStatus = string.Empty;
                        SelectedAccountStatus = string.Empty;
                        BankAccountNumber = string.Empty;
                        InitialBalance = null;
                        Description = string.Empty;
                    }
                    else
                    {
                        NotificationManager.Show(new Notifications.Wpf.NotificationContent { Title = "Error", Message = "SorryCannot Add Bank Account", Type = Notifications.Wpf.NotificationType.Error });
                    }

                }
                else
                    NotificationManager.Show(new Notifications.Wpf.NotificationContent { Title = "Error", Message = "Sorry Bank Account Already Exist", Type = Notifications.Wpf.NotificationType.Error });
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

        #endregion

        #region Properties
        private List<BankModel> _Banks;
        /// <summary>
        /// List of All Banks
        /// </summary>
        public List<BankModel> Banks
        {
            get { return _Banks; }
            set { _Banks = value; NotifyOfPropertyChange(nameof(Banks)); }
        }

        private BankModel _SelectedBank;

        public BankModel SelectedBank
        {
            get { return _SelectedBank; }
            set { _SelectedBank = value; NotifyOfPropertyChange(nameof(SelectedBank)); GetBranches(SelectedBank?.Id); }
        }
        private List<BankBranchModel> _BankBranches;
        /// <summary>
        /// List Of All Bank Branches
        /// </summary>
        public List<BankBranchModel> BankBranches
        {
            get { return _BankBranches; }
            set { _BankBranches = value; NotifyOfPropertyChange(nameof(BankBranches)); }
        }

        private BankBranchModel _SelectedBranch;

        public BankBranchModel SelectedBranch
        {
            get { return _SelectedBranch; }
            set { _SelectedBranch = value; NotifyOfPropertyChange(nameof(SelectedBranch)); }
        }

        private bool _IsAddBankPressed;

        public bool IsAddBankPressed
        {
            get { return _IsAddBankPressed; }
            set { _IsAddBankPressed = value; NotifyOfPropertyChange(nameof(IsAddBankPressed)); }
        }
        private bool _IsUpdateBankPressed;

        public bool IsUpdateBankPressed
        {
            get { return _IsUpdateBankPressed; }
            set { _IsUpdateBankPressed = value; NotifyOfPropertyChange(nameof(IsUpdateBankPressed)); }
        }
        private bool _IsRemoveBankPressed;

        public bool IsRemoveBankPressed
        {
            get { return _IsRemoveBankPressed; }
            set { _IsRemoveBankPressed = value; NotifyOfPropertyChange(nameof(IsRemoveBankPressed)); }
        }
        private List<string> _AccountTypes;
        /// <summary>
        /// Account Type like Current,saving,Default etc...
        /// </summary>
        public List<string> AccountTypes
        {
            get { return _AccountTypes; }
            set { _AccountTypes = value; NotifyOfPropertyChange(nameof(AccountTypes)); }
        }
        private string _SelectedAccountType;

        public string SelectedAccountType
        {
            get { return _SelectedAccountType; }
            set { _SelectedAccountType = value; NotifyOfPropertyChange(nameof(SelectedAccountType)); }
        }

        private List<string> _AccountStatus;
        /// <summary>
        /// Account status like Open,Close,suspended,Transation etc...
        /// </summary>
        public List<string> AccountStatus
        {
            get { return _AccountStatus; }
            set { _AccountStatus = value; NotifyOfPropertyChange(nameof(AccountStatus)); }
        }
        private string _SelectedAccountStatus;

        public string SelectedAccountStatus
        {
            get { return _SelectedAccountStatus; }
            set { _SelectedAccountStatus = value; NotifyOfPropertyChange(nameof(SelectedAccountStatus)); }
        }

        private string _BankAccountNumber;

        public string BankAccountNumber
        {
            get { return _BankAccountNumber; }
            set { _BankAccountNumber = value; NotifyOfPropertyChange(nameof(BankAccountNumber)); }
        }

        private decimal? _InitialBalance;

        public decimal? InitialBalance
        {
            get { return _InitialBalance; }
            set { _InitialBalance = value; NotifyOfPropertyChange(nameof(InitialBalance)); }
        }

        private DateTime? _AccountOpeningDate;

        public DateTime? AccountOpeningDate
        {
            get { return _AccountOpeningDate; }
            set { _AccountOpeningDate = value; NotifyOfPropertyChange(nameof(AccountOpeningDate)); }
        }
        private string _Description;

        public string Description
        {
            get { return _Description; }
            set { _Description = value; NotifyOfPropertyChange(nameof(Description)); }
        }

        #endregion
    }
}
