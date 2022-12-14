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
        #endregion
        #region [constructor]
        [ImportingConstructor]
        public MobileAccountPaymentDialogViewModel(IBankAccountManager bankAccountManager)
        {
            _bankAccountManager = bankAccountManager;
        }
        #endregion

        #region [Public Methods]
        protected async override void OnActivate()
        {
            base.OnActivate();
            MobileOperaters = new List<string> { SelectedMobileOperater};
            MobileNumbers = (await _bankAccountManager.GetBankAccountsByBankAsync(SelectedMobileOperater)).ToList();
            if(MobileNumbers == null || MobileNumbers?.Count == 0)
            {
                NotificationManager.Show(new Notifications.Wpf.NotificationContent { Title = "Error", Message = "Please Enter Mobile Account First", Type = Notifications.Wpf.NotificationType.Error });
            }
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
        private List<BankAccountModel> _MobileNumbers;
        /// <summary>
        /// Mobile No List
        /// </summary>
        public List<BankAccountModel> MobileNumbers
        {
            get { return _MobileNumbers; }
            set { _MobileNumbers = value; NotifyOfPropertyChange(nameof(MobileNumbers)); }
        }
        private string _SelectedMobileNumber;
        /// <summary>
        /// Selected Mobile number
        /// </summary>
        public string SelectedMobileNumber
        {
            get { return _SelectedMobileNumber; }
            set { _SelectedMobileNumber = value; NotifyOfPropertyChange(nameof(SelectedMobileNumber)); }
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
