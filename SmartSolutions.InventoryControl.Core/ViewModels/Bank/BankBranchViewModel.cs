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
    [Export(typeof(BankBranchViewModel)), PartCreationPolicy(CreationPolicy.Shared)]
    public class BankBranchViewModel : BaseViewModel
    {
        #region Private Members
        private readonly IBankBranchManager _branchManager;
        private readonly IBankManager _bankManager;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public BankBranchViewModel(IBankBranchManager branchManager, IBankManager bankManager)
        {
            _branchManager = branchManager;
            _bankManager = bankManager;
        }
        #endregion

        #region Methods
        protected async override void OnActivate()
        {
            base.OnActivate();
            IsBranchAdded = true;
            Banks = (await _bankManager.GetAllBanksAsync()).ToList();
        }
        public async void AddBranch()
        {
            try
            {
                //TODO: Validations checking
                var branch = new BankBranchModel();
                branch.Name = BranchName;
                branch.Bank = SelectedBank;
                branch.BussinessPhone = LandLineNumber;
                branch.BussinessPhone1 = LandLineNumber1;
                branch.MobilePhone = MobileNumber;
                branch.Email = Email;
                branch.Address = Address;
                branch.Description = Description;
                var result = await _branchManager.AddBankBranchAsync(branch);
                if(result)
                {
                    NotificationManager.Show(new Notifications.Wpf.NotificationContent { Title="Success",Message = "Branch Addedd Succesfully", Type= Notifications.Wpf.NotificationType.Success});
                    BranchName = string.Empty;
                    LandLineNumber = string.Empty;
                    LandLineNumber1 = string.Empty;
                    MobileNumber = string.Empty;
                    Email = string.Empty;
                    Description = string.Empty;
                    Address = string.Empty;
                    TryClose();
                }
                else
                {
                    NotificationManager.Show(new Notifications.Wpf.NotificationContent { Title = "Error",Message = "Cannot Add Branch",Type = Notifications.Wpf.NotificationType.Error});
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
        private bool _IsBranchAdded;

        public bool IsBranchAdded
        {
            get { return _IsBranchAdded; }
            set { _IsBranchAdded = value; NotifyOfPropertyChange(nameof(IsBranchAdded)); }
        }
        private bool _IsBranchUpdated;

        public bool IsBranchUpdated
        {
            get { return _IsBranchUpdated; }
            set { _IsBranchUpdated = value; NotifyOfPropertyChange(nameof(IsBranchUpdated)); }
        }
        private bool _IsBranchRemoved;

        public bool IsBranchRemoved
        {
            get { return _IsBranchRemoved; }
            set { _IsBranchRemoved = value; NotifyOfPropertyChange(nameof(IsBranchRemoved)); }
        }

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

        public BankModel SelectedBank
        {
            get { return _SelectedBank; }
            set { _SelectedBank = value; NotifyOfPropertyChange(nameof(SelectedBank)); }
        }
        private string _BranchName;

        public string BranchName
        {
            get { return _BranchName; }
            set { _BranchName = value; NotifyOfPropertyChange(nameof(BranchName)); }
        }
        private string _LandLineNumber;

        public string LandLineNumber
        {
            get { return _LandLineNumber; }
            set { _LandLineNumber = value; NotifyOfPropertyChange(nameof(LandLineNumber)); }
        }
        private string _LandLineNumber1;

        public string LandLineNumber1
        {
            get { return _LandLineNumber1; }
            set { _LandLineNumber1 = value; NotifyOfPropertyChange(nameof(LandLineNumber1)); }
        }
        private string _MobileNumber;

        public string MobileNumber
        {
            get { return _MobileNumber; }
            set { _MobileNumber = value; NotifyOfPropertyChange(nameof(MobileNumber)); }
        }
        private string _Email;

        public string Email
        {
            get { return _Email; }
            set { _Email = value; NotifyOfPropertyChange(nameof(Email)); }
        }

        private string _Description;

        public string Description
        {
            get { return _Description; }
            set { _Description = value; NotifyOfPropertyChange(nameof(Description)); }
        }
        private string _Address;

        public string Address
        {
            get { return _Address; }
            set { _Address = value; NotifyOfPropertyChange(nameof(Address)); }
        }

        #endregion
    }
}
