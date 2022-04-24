using Caliburn.Micro;
using Microsoft.SqlServer.Management.Smo;
using Notifications.Wpf;
using SmartSolutions.InventoryControl.Core.ViewModels.Dialogs;
using SmartSolutions.InventoryControl.Core.ViewModels.Login;
using SmartSolutions.InventoryControl.Core.ViewModels.Settings;
using SmartSolutions.InventoryControl.DAL;
using SmartSolutions.InventoryControl.DAL.Models;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.LogUtils;
using System;
using System.ComponentModel.Composition;

namespace SmartSolutions.InventoryControl.Core.ViewModels
{
    [Export(typeof(MainViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class MainViewModel : BaseViewModel, IHandle<Screen>
    {
        #region Private Members
        private readonly IEventAggregator _eventAggregator;
        private readonly DAL.Managers.BackUp.IDatabaseBackupManager _databaseBackupManager;
        private readonly DAL.Managers.Settings.ISystemSettingManager _systemSettingManager;
        private readonly NotificationManager notificationManager;
        #endregion

        #region Constructor
        public MainViewModel() { }

        [ImportingConstructor]
        public MainViewModel(IEventAggregator eventAggregator
                            , DAL.Managers.Settings.ISystemSettingManager systemSettingManager
                            , DAL.Managers.BackUp.IDatabaseBackupManager databaseBackupManager)
        {
            _eventAggregator = eventAggregator;
            _systemSettingManager = systemSettingManager;
            _databaseBackupManager = databaseBackupManager;
             notificationManager = new NotificationManager();
            ProprietorInfo = AppSettings.Proprietor;
            NotifyOfPropertyChange(nameof(ProprietorInfo.BussinessName));
        }
        #endregion

        #region Methods
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
        }
        protected override void OnActivate()
        {
            if (Execute.InDesignMode)
            {
                //IsLoading = false;
            }
            base.OnActivate();
            NowTime = DateTime.Now;
            CurrentUser = AppSettings.LoggedInUser;
            TransactionCounter = Counter;
        }
        public void LogOut()
        {
            try
            {
                _eventAggregator.PublishOnCurrentThread(IoC.Get<LoginViewModel>());
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Fatal);
            }
        }
        /// <summary>
        /// Get the Product Form
        /// </summary>
        public void Product()
        {
            IsBankProceeded = false;
            IsProductProceeded = true;
            IsPartnerProceeded = false;
            IsPurchaseProceeded = false;
            IsSalesProceeded = false;
            IsReportProceeded = false;
            IsPaymentProceeded = false;
            Handle(IoC.Get<Product.ProductViewModel>());
        }
        /// <summary>
        /// Get the BusinessPartner Form
        /// </summary>
        public void BussinessPartner()
        {
            IsBankProceeded = false;
            IsPartnerProceeded = true;
            IsProductProceeded = false;
            IsPurchaseProceeded = false;
            IsSalesProceeded = false;
            IsReportProceeded = false;
            IsPaymentProceeded = false;
            Handle(IoC.Get<BussinessPartner.BussinessPartnerViewModel>());
        }
        public void Purchase()
        {
            IsBankProceeded = false;
            IsPurchaseProceeded = true;
            IsPartnerProceeded = false;
            IsProductProceeded = false;
            IsSalesProceeded = false;
            IsReportProceeded = false;
            IsPaymentProceeded = false;
            Handle(IoC.Get<PurchaseViewModel>());
        }
        public void Sales()
        {
            IsBankProceeded = false;
            IsPurchaseProceeded = false;
            IsPartnerProceeded = false;
            IsProductProceeded = false;
            IsReportProceeded = false;
            IsPaymentProceeded = false;
            IsSalesProceeded = true;
            Handle(IoC.Get<SalesViewModel>());
        }
        public void Reports()
        {
            IsBankProceeded = false;
            IsPurchaseProceeded = false;
            IsPartnerProceeded = false;
            IsProductProceeded = false;
            IsSalesProceeded = false;
            IsPaymentProceeded = false;
            IsReportProceeded = true;
            Handle(IoC.Get<Reports.ReportsViewModel>());
        }
        public void Payments()
        {
            IsPaymentProceeded = true;
            IsPurchaseProceeded = false;
            IsPartnerProceeded = false;
            IsProductProceeded = false;
            IsReportProceeded = false;
            IsSalesProceeded = false;
            IsBankProceeded = false;
            Handle(IoC.Get<PaymentViewModel>());
        }
        public void Bank()
        {
            try
            {
                IsBankProceeded = true;
                IsPaymentProceeded = false;
                IsPurchaseProceeded = false;
                IsPartnerProceeded = false;
                IsProductProceeded = false;
                IsReportProceeded = false;
                IsSalesProceeded = false;
                Handle(IoC.Get<Bank.BankAccountViewModel>());
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        public void Handle(Screen screen)
        {

            if (screen is Product.ProductViewModel
                || screen is BussinessPartner.BussinessPartnerViewModel
                || screen is PurchaseViewModel
                || screen is PaymentViewModel
                || screen is SalesViewModel
                || screen is Reports.ReportsViewModel
                || screen is Bank.BankAccountViewModel)
            {
                ActiveItem = screen;
                ActivateItem(screen);
            }
        }
        public async void SMS()
        {
            await IoC.Get<IDialogManager>().ShowMessageBoxAsync("Sorry This Feature is Not Available Yet", options: Dialogs.MessageBoxOptions.Ok);
        }
        public async void CreateUser()
        {
            try
            {
                var dlg = IoC.Get<UserCreationViewModel>();
                await IoC.Get<IDialogManager>().ShowDialogAsync(dlg);
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        public async void UpdateDbBackup()
        {
            try
            {
                var resultSetting = await _systemSettingManager.GetsystemSettingByKeyAsync("Is_DbPath_Inserted");
                if (resultSetting != null)
                {
                    if (resultSetting.SettingValue == 0)
                    {
                        var dlg = IoC.Get<PathInsertionViewModel>();
                        await IoC.Get<IDialogManager>().ShowDialogAsync(dlg);

                    }
                    else
                    {
                        Server myServer = new Server();
                        Database myDatabase = myServer.Databases[ConnectionInfo.Instance.Database];
                        var result = await _databaseBackupManager.CreateDifferentialBackupAsync(myServer, myDatabase, resultSetting.Description);
                       //var result =  await _databaseBackupManager.CreateBackupAsync(ConnectionInfo.Instance.Database, resultSetting.Description);
                        if(result)
                            NotificationManager.Show(new Notifications.Wpf.NotificationContent { Title = "Success", Message = "BackUp Successfully", Type = Notifications.Wpf.NotificationType.Success });
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        public void PrintDocument()
        {
            try
            {

            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }

        #endregion

        #region Properties
        //public static int Counter { get; set; }
        private static int _Counter;

        public static int Counter
        {
            get
            {
                return _Counter;
            }
            set
            {
                _Counter = value;               
            }
        }

        private bool _IsBankProceeded;

        public bool IsBankProceeded
        {
            get { return _IsBankProceeded; }
            set { _IsBankProceeded = value; NotifyOfPropertyChange(nameof(IsBankProceeded)); }
        }

        private bool _IsProductProceeded;
        /// <summary>
        /// For Product Button Pressed
        /// </summary>
        public bool IsProductProceeded
        {
            get { return _IsProductProceeded; }
            set { _IsProductProceeded = value; NotifyOfPropertyChange(nameof(IsProductProceeded)); }
        }
        private bool _IsPartnerProceeded;
        /// <summary>
        /// For Partner Button
        /// </summary>
        public bool IsPartnerProceeded
        {
            get { return _IsPartnerProceeded; }
            set { _IsPartnerProceeded = value; NotifyOfPropertyChange(nameof(IsPartnerProceeded)); }
        }
        private bool _IsPurchaseProceeded;
        /// <summary>
        /// For Purchase Button Pressed
        /// </summary>
        public bool IsPurchaseProceeded
        {
            get { return _IsPurchaseProceeded; }
            set { _IsPurchaseProceeded = value; NotifyOfPropertyChange(nameof(IsPurchaseProceeded)); }
        }
        private bool _IsSalesProceeded;
        /// <summary>
        /// For Sale Button Is Pressed
        /// </summary>
        public bool IsSalesProceeded
        {
            get { return _IsSalesProceeded; }
            set { _IsSalesProceeded = value; NotifyOfPropertyChange(nameof(IsSalesProceeded)); }
        }
        private bool _IsReportProceeded;
        /// <summary>
        /// Report button Selected
        /// </summary>
        public bool IsReportProceeded
        {
            get { return _IsReportProceeded; }
            set { _IsReportProceeded = value; NotifyOfPropertyChange(nameof(IsReportProceeded)); }
        }
        private bool _IsPrinterAvailable;
        /// <summary>
        /// Printer Available Or Not
        /// </summary>
        public bool IsPrinterAvailable
        {
            get { return _IsPrinterAvailable; }
            set { _IsPrinterAvailable = value; NotifyOfPropertyChange(nameof(IsPrinterAvailable)); }
        }
        private bool _IsSettingActive;

        public bool IsSettingActive
        {
            get { return _IsSettingActive; }
            set { _IsSettingActive = value; NotifyOfPropertyChange(nameof(IsSettingActive)); }
        }
        private bool _IsDataSaved;
        /// <summary>
        /// Data backup Updated Or Not
        /// </summary>
        public bool IsDataSaved
        {
            get { return _IsDataSaved; }
            set { _IsDataSaved = value; NotifyOfPropertyChange(nameof(IsDataSaved)); }
        }
        private DateTime _NowTime;

        public DateTime NowTime
        {
            get { return _NowTime; }
            set { _NowTime = value; NotifyOfPropertyChange(nameof(NowTime)); }
        }
        private string _PrinterName;
        /// <summary>
        /// Printer Name if Available
        /// </summary>
        public string PrinterName
        {
            get { return _PrinterName; }
            set { _PrinterName = value; NotifyOfPropertyChange(nameof(PrinterName)); }
        }
        private bool _IsPaymentProceeded;
        /// <summary>
        /// For Payment Options
        /// </summary>
        public bool IsPaymentProceeded
        {
            get { return _IsPaymentProceeded; }
            set { _IsPaymentProceeded = value; NotifyOfPropertyChange(nameof(IsPaymentProceeded)); }
        }
        private DAL.Models.Authentication.IdentityUserModel _CurrentUser;

        public DAL.Models.Authentication.IdentityUserModel CurrentUser
        {
            get { return _CurrentUser; }
            set { _CurrentUser = value; NotifyOfPropertyChange(nameof(CurrentUser)); }
        }
        private  int _TransactionCounter;
        /// <summary>
        /// Counter for Keeping Record of Logging Transaction
        /// </summary>
        public  int TransactionCounter
        {
            get { return _TransactionCounter; }
            set { _TransactionCounter = value; NotifyOfPropertyChange(nameof(TransactionCounter)); }
        }
        private ProprietorInformationModel _ProprietorInfo;
        /// <summary>
        /// Basic Info Of Proprietor
        /// </summary>
        public ProprietorInformationModel ProprietorInfo
        {
            get { return _ProprietorInfo; }
            set { _ProprietorInfo = value; NotifyOfPropertyChange(nameof(ProprietorInfo)); }
        }

        #endregion
    }
}
