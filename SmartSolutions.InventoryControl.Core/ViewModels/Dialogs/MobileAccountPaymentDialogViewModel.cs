using SmartSolutions.InventoryControl.DAL.Managers.Bank;
using SmartSolutions.InventoryControl.DAL.Models.Bank;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace SmartSolutions.InventoryControl.Core.ViewModels.Dialogs
{
    [Export(typeof(MobileAccountPaymentDialogViewModel)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class MobileAccountPaymentDialogViewModel:BaseViewModel
    {
        #region [Private Members]
        private readonly IBankAccountManager _bankAccountManager;
        private readonly IBankBranchManager _bankBranchManager;
        #endregion

        #region [constructor]
        [ImportingConstructor]
        public MobileAccountPaymentDialogViewModel(IBankAccountManager bankAccountManager, IBankBranchManager bankBranchManager)
        {
            _bankAccountManager = bankAccountManager;
            _bankBranchManager = bankBranchManager;
        }
        #endregion

        #region [Public Methods]
        protected async override void OnActivate()
        {
            base.OnActivate();
            MobileOperaters = new List<string> { SelectedMobileOperater};
            var branch = (await _bankBranchManager.GetBankBranchesByBankNameAsync(SelectedMobileOperater)).ToList();
            MobileAccounts = (await _bankAccountManager.GetBankAccountsByBankAsync(SelectedMobileOperater)).ToList();
          
            if(MobileAccounts == null || MobileAccounts?.Count == 0)
            {
                NotificationManager.Show(new Notifications.Wpf.NotificationContent { Title = "Error", Message = "Please Enter Mobile Account First", Type = Notifications.Wpf.NotificationType.Error });
            }
            else
            {
                MobileAccounts.FirstOrDefault().Branch = branch.FirstOrDefault();
                SelectedMobileAccount = MobileAccounts.FirstOrDefault();
            }
        }

        public void Submit()
        {

            SelectedMobileAccount.Branch.Bank.Name = SelectedMobileOperater;
            TryClose();
        }
        public void Close()
        {
            TryClose();
        }
        public void Cancel()
        {
            TryClose(true);
        }
        #endregion

        #region [Properties]
        private List<string> _MobileOperaters;
        /// <summary>
        /// Name of Operaters
        /// </summary>
        public List<string> MobileOperaters
        {
            get { return _MobileOperaters; }
            set { _MobileOperaters = value; NotifyOfPropertyChange(nameof(MobileOperaters)); }
        }
        private string _SelectedMobileOperater;
        /// <summary>
        /// Selected Mobile Operater
        /// </summary>
        public string SelectedMobileOperater
        {
            get { return _SelectedMobileOperater; }
            set { _SelectedMobileOperater = value; NotifyOfPropertyChange(nameof(SelectedMobileOperater)); }
        }
        private List<BankAccountModel> _MobileAccounts;
        /// <summary>
        /// Mobile No List
        /// </summary>
        public List<BankAccountModel> MobileAccounts
        {
            get { return _MobileAccounts; }
            set { _MobileAccounts = value; NotifyOfPropertyChange(nameof(MobileAccounts)); }
        }
        private BankAccountModel _SelectedMobileAccount;
        /// <summary>
        /// Selected Mobile number
        /// </summary>
        public BankAccountModel SelectedMobileAccount
        {
            get { return _SelectedMobileAccount; }
            set { _SelectedMobileAccount = value; NotifyOfPropertyChange(nameof(SelectedMobileAccount)); }
        }
        private decimal _Amount;
        /// <summary>
        /// amount of Payment
        /// </summary>
        public decimal Amount
        {
            get { return _Amount; }
            set { _Amount = value; NotifyOfPropertyChange(nameof(Amount)); }
        }

        #endregion
    }
}
