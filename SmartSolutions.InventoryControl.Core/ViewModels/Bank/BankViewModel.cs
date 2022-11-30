using SmartSolutions.InventoryControl.DAL.Managers.Bank;
using SmartSolutions.InventoryControl.DAL.Models.Bank;
using SmartSolutions.Util.LogUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace SmartSolutions.InventoryControl.Core.ViewModels.Bank
{
    [Export(typeof(BankViewModel)), PartCreationPolicy(CreationPolicy.Shared)]
    public class BankViewModel : BaseViewModel
    {
        #region Private Member
        private readonly IBankManager _bankManager;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public BankViewModel(IBankManager bankManager)
        {
            _bankManager = bankManager;
        }
        #endregion

        #region Methods
        protected override void OnActivate()
        {
            base.OnActivate();
            IsBankAdded = true;
        }

        public async void AddBank()
        {
            try
            {
                if (!string.IsNullOrEmpty(BankName))
                {
                    var bank = new BankModel();
                    bank.Name = BankName;
                    bank.Description = Description;
                   var result =  await _bankManager.AddBankAsync(bank);
                    if(result)
                    {
                        NotificationManager.Show(new Notifications.Wpf.NotificationContent { Title = "Success", Message = "Successfully Bank Addedd", Type = Notifications.Wpf.NotificationType.Success });
                        BankName = string.Empty;
                        TryClose();
                    }
                    else
                    {
                        NotificationManager.Show(new Notifications.Wpf.NotificationContent { Title = "Error", Message = "Cannot Add Bank ", Type = Notifications.Wpf.NotificationType.Error });
                    }
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
        public void Close()
        {
            TryClose();
        }
        #endregion

        #region Properties
        private string _BankName;

        public string BankName
        {
            get { return _BankName; }
            set { _BankName = value; NotifyOfPropertyChange(nameof(BankName)); }
        }
        private string _Description;

        public string Description
        {
            get { return _Description; }
            set { _Description = value; NotifyOfPropertyChange(nameof(Description)); }
        }
        private bool _IsBankUpdated;

        public bool IsBankUpdated
        {
            get { return _IsBankUpdated; }
            set { _IsBankUpdated = value; NotifyOfPropertyChange(nameof(IsBankUpdated)); }
        }
        private bool _IsBankAdded;

        public bool IsBankAdded
        {
            get { return _IsBankAdded; }
            set { _IsBankAdded = value; NotifyOfPropertyChange(nameof(IsBankAdded)); }
        }
        private bool _IsBankRemoved;

        public bool IsBankRemoved
        {
            get { return _IsBankRemoved; }
            set { _IsBankRemoved = value; NotifyOfPropertyChange(nameof(IsBankRemoved)); }
        }

        #endregion
    }
}