using Caliburn.Micro;
using SmartSolutions.InventoryControl.Core.Helpers.SuggestionProvider;
using SmartSolutions.InventoryControl.DAL;
using SmartSolutions.InventoryControl.DAL.Managers.Bussiness_Partner;
using SmartSolutions.InventoryControl.DAL.Managers.Region;
using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using SmartSolutions.InventoryControl.DAL.Models.Inventory;
using SmartSolutions.InventoryControl.DAL.Models.Payments;
using SmartSolutions.InventoryControl.DAL.Models.PurchaseOrder;
using SmartSolutions.InventoryControl.DAL.Models.Region;
using SmartSolutions.InventoryControl.DAL.Models.Sales;
using SmartSolutions.Util.LogUtils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.Core.ViewModels.BussinessPartner
{
    [Export(typeof(BussinessPartnerViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class BussinessPartnerViewModel : BaseViewModel
    {
        #region Private Members
        private readonly IBussinessPartnerManager _bussinessPartnerManager;
        private readonly DAL.Managers.Invoice.IPurchaseInvoiceManager _purchaseInvoiceManager;
        private readonly DAL.Managers.Invoice.ISaleInvoiceManager _saleInvoiceManager;
        private readonly IPartnerLedgerManager _partnerLedgerManager;
        private readonly IPartnerTypeManager _partnerTypeManager;
        private readonly IPartnerCategoryManager _partnerCategoryManager;
        private readonly DAL.Managers.Payments.IPaymentTypeManager _paymentTypeManager;
        private readonly DAL.Managers.Region.ICityManager _cityManager;
        private readonly DAL.Managers.Payments.IPaymentManager _paymentManager;

        #endregion

        #region Constructor
        [ImportingConstructor]
        public BussinessPartnerViewModel(IBussinessPartnerManager bussinessPartnerManager
                                        , DAL.Managers.Invoice.IPurchaseInvoiceManager purchaseInvoiceManager
                                        , DAL.Managers.Invoice.ISaleInvoiceManager saleInvoiceManager
                                        , IPartnerLedgerManager partnerLedgerManager
                                        , IPartnerTypeManager partnerTypeManager
                                        , IPartnerCategoryManager partnerCategoryManager
                                        , DAL.Managers.Payments.IPaymentTypeManager paymentTypeManager
                                        , DAL.Managers.Region.ICityManager cityManager
                                        , DAL.Managers.Payments.IPaymentManager paymentManager)
        {
            _bussinessPartnerManager = bussinessPartnerManager;
            _purchaseInvoiceManager = purchaseInvoiceManager;
            _saleInvoiceManager = saleInvoiceManager;
            _partnerLedgerManager = partnerLedgerManager;
            _partnerTypeManager = partnerTypeManager;
            _partnerCategoryManager = partnerCategoryManager;
            _paymentTypeManager = paymentTypeManager;
            _cityManager = cityManager;
            _paymentManager = paymentManager;

        }
        #endregion

        #region Public Methods
        public void AddPartner()
        {
            try
            {
                IsUpdatePartner = false;
                IsRemoveParnter = false;
                IsAddPartner = true;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        public async void UpdatePartner()
        {
            try
            {
                NewBussinessPartner = null;
                IsUpdatePartner = true;
                IsAddPartner = false;
                IsRemoveParnter = false;
                if (SelectedBussinessPartner == null)
                {
                    BussinessPartners = (await _bussinessPartnerManager.GetAllBussinessPartnersAsync()).ToList();
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        public void RemovePartner()
        {
            try
            {
                NewBussinessPartner = null;
                //_bussinessPartnerManager.RemoveBussinessPartner(SelectedBussinessPartner?.Id);
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        public void AddMobileNo(BussinessPartnerModel model)
        {
            if (!string.IsNullOrEmpty(PartnerMobileNumber))
            {
                NewBussinessPartner.MobileNumbers.Add(PartnerMobileNumber);
                PartnerMobileNumber = string.Empty;
                MobileNumbers.Add(PartnerMobileNumber);
                MobileMessage = "Add Another Mobile No";
            }
            else
            {
                MobileNumbers.Add(string.Empty);
            }
        }
        public void RemoveMobileNo(BussinessPartnerModel model)
        {

        }
        public void Cancel()
        {
            TryClose();
        }
        public async void SavePartner()
        {
            try
            {
                IsLoading = true;
                LoadingMessage = "Saving...";
                if (!string.IsNullOrEmpty(PartnerMobileNumber))
                {
                    NewBussinessPartner?.MobileNumbers.Add(PartnerMobileNumber);
                }
                if (string.IsNullOrEmpty(NewBussinessPartner?.BussinessName))
                {
                    BussinessNameError = true;
                    return;
                }
                else if (string.IsNullOrEmpty(NewBussinessPartner?.FullName))
                {
                    FullNameError = true;
                    return;
                }
                else if (NewBussinessPartner?.MobileNumbers.Count == 0)
                {
                    MobileNoError = true;
                    return;
                }

                if (NewBussinessPartner != null)
                {
                    NewBussinessPartner.PartnerCategory = SelectedPartnerCategory;
                    NewBussinessPartner.PartnerType = SelectedPartnerType;
                    NewBussinessPartner.City = SelectedCity;
                    NewBussinessPartner.CreatedBy = AppSettings.LoggedInUser.DisplayName;
                    var resultPartner = await _bussinessPartnerManager.AddBussinesPartnerAsync(NewBussinessPartner);
                    if (resultPartner)
                        NotificationManager.Show(new Notifications.Wpf.NotificationContent { Title = "Success", Message = "Partner Added Successfully", Type = Notifications.Wpf.NotificationType.Success }, areaName: "WindowArea");
                    else
                        NotificationManager.Show(new Notifications.Wpf.NotificationContent { Title = "Error", Message = "Sorry Partner Not Added", Type = Notifications.Wpf.NotificationType.Error });
                    if (InitialAmount > 0)
                    {
                        if (resultPartner)
                        {
                            // here we Create and Fill the Payment Object
                            var payment = new PaymentModel();
                            payment.Partner = await _bussinessPartnerManager.GetLastAddedPartner();
                            payment.PaymentType = SelectedAmountType.Equals("DR (Receivable)") == true ? DAL.Models.PaymentType.DR : DAL.Models.PaymentType.CR;
                            payment.PaymentAmount = InitialAmount;
                            payment.IsPaymentReceived = SelectedAmountType.Equals("DR (Receivable)") ? true : false;
                            payment.CreatedBy = AppSettings.LoggedInUser.DisplayName;
                            payment.PaymentMethod = await _paymentTypeManager.GetPaymentMethodByIdAsync(1);
                            var paymentResult = await _paymentManager.AddPaymentAsync(payment);
                            if (paymentResult)
                            {
                                NotificationManager.Show(new Notifications.Wpf.NotificationContent { Title = "Success", Message = "Inital Amount Added Successfully", Type = Notifications.Wpf.NotificationType.Success });
                                //Here we Update the Partner Ledger Account
                                var partnerLedger = new BussinessPartnerLedgerModel();
                                partnerLedger.Partner = payment.Partner ?? await _bussinessPartnerManager.GetLastAddedPartner();
                                partnerLedger.Payment = await _paymentManager.GetLastPaymentByPartnerId(payment?.Partner.Id);
                                partnerLedger.CurrentBalance = payment.PaymentAmount;
                                partnerLedger.CurrentBalanceType = payment.PaymentType;
                                partnerLedger.Description = "Initial Deposit / Balance";
                                partnerLedger.CreatedBy = AppSettings.LoggedInUser.DisplayName;
                                var partnerResult = await _partnerLedgerManager.AddPartnerBalance(partnerLedger);
                                //Display user Friendly Toast
                                if (partnerResult)
                                {
                                    NotificationManager.Show(new Notifications.Wpf.NotificationContent { Title = "Success", Message = "Partner Ledger Updated Successfully", Type = Notifications.Wpf.NotificationType.Success });
                                    ClearPartnerDetails();
                                }
                                else
                                    NotificationManager.Show(new Notifications.Wpf.NotificationContent { Title = "Error", Message = "Partner Ledger Not Updated", Type = Notifications.Wpf.NotificationType.Error });
                            }
                            else
                                NotificationManager.Show(new Notifications.Wpf.NotificationContent { Title = "Error", Message = "Inital Amount Not Added ", Type = Notifications.Wpf.NotificationType.Error });
                        }
                    }
                    else
                    {
                        ClearPartnerDetails();
                    }
                }
                IsLoading = false;
            }
            catch (Exception ex)
            {
                IsLoading = false;
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        private void ClearPartnerDetails()
        {
            PartnerMobileNumber = string.Empty;
            InitialAmount = 0;
            NewBussinessPartner = new BussinessPartnerModel();
            SelectedAmountType = string.Empty;
            SelectedPartnerCategory = new BussinessPartnerCategoryModel();
            SelectedPartnerType = new BussinessPartnerTypeModel();

        }
        public async void UpdatePartnerProfile()
        {
            try
            {
                if (SelectedBussinessPartner != null)
                {
                    await _bussinessPartnerManager.UpdateBussinessPartnerAsync(SelectedBussinessPartner);
                    SelectedBussinessPartner = null;
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        #endregion

        #region Private Methods
      
        #endregion

        #region Protected Methods
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
        }
        protected async override void OnActivate()
        {
            if (Execute.InDesignMode)
            {
                IsRemoveParnter = false;
                IsAddPartner = false;
                IsUpdatePartner = true;
            }
            IsLoading = true;
            base.OnActivate();
            AddPartner();
            AmountType = new List<string> { "DR (Receivable)", " CR (Payable)" };
            PaymentMethods = (await _paymentTypeManager.GetAllPaymentMethodsAsync()).ToList();
            SelectedPaymentMethod = PaymentMethods?.Where(p => p.Name == "Unkown").FirstOrDefault();
            NewBussinessPartner = new BussinessPartnerModel();
            NewBussinessPartner.MobileNumbers = new List<string>();
            MobileMessage = "Enter Mobile Number";
            PartnerTypes = (await _partnerTypeManager.GetPartnerTypesAsync()).ToList();
            PartnerCategories = (await _partnerCategoryManager.GetPartnerCategoriesAsync()).ToList();
            //TODO: Here we HardCode the Pakistan Id But have to Change that
            Cities = (await _cityManager.GetCitiesByCountryId(162)).ToList();
            CitySuggetion = new CitySuggetionProvider(Cities);
            IsLoading = false;
        }
        protected override void OnInitialize()
        {
            base.OnInitialize();
        }
        #endregion

        #region Properties
        private List<PaymentTypeModel> _PaymentMethods;
        /// <summary>
        /// Payment Method Like Jazz Cash ,Bank,Cash etc...
        /// </summary>
        public List<PaymentTypeModel> PaymentMethods
        {
            get { return _PaymentMethods; }
            set { _PaymentMethods = value; NotifyOfPropertyChange(nameof(PaymentMethods)); }
        }

        private PaymentTypeModel _SelectedPaymentMethod;
        /// <summary>
        /// Selected Payment Method Like Jazz Cash ,Bank,Cash etc...
        /// </summary>
        public PaymentTypeModel SelectedPaymentMethod
        {
            get { return _SelectedPaymentMethod; }
            set { _SelectedPaymentMethod = value; NotifyOfPropertyChange(nameof(SelectedPaymentMethod)); }
        }

        private List<BussinessPartnerTypeModel> _PartnerTypes;
        /// <summary>
        /// List Of Partner Types Like Vender Customer etc...
        /// </summary>
        public List<BussinessPartnerTypeModel> PartnerTypes
        {
            get { return _PartnerTypes; }
            set { _PartnerTypes = value; NotifyOfPropertyChange(nameof(PartnerTypes)); }
        }
        private BussinessPartnerTypeModel _SelectedPartnerType;
        /// <summary>
        /// Selected Partner Type Like Vender Customer etc...
        /// </summary>
        public BussinessPartnerTypeModel SelectedPartnerType
        {
            get { return _SelectedPartnerType; }
            set { _SelectedPartnerType = value; NotifyOfPropertyChange(nameof(SelectedPartnerType)); }
        }

        private List<BussinessPartnerCategoryModel> _PartnerCategories;
        /// <summary>
        /// PartnerCategory Like SoleProperiter,Company,Factory etc...
        /// </summary>
        public List<BussinessPartnerCategoryModel> PartnerCategories
        {
            get { return _PartnerCategories; }
            set { _PartnerCategories = value; NotifyOfPropertyChange(nameof(PartnerCategories)); }
        }

        private BussinessPartnerCategoryModel _SelectedPartnerCategory;
        /// <summary>
        /// Selected Partner Category Like SoleProperiter,Company,Factory etc..
        /// </summary>
        public BussinessPartnerCategoryModel SelectedPartnerCategory
        {
            get { return _SelectedPartnerCategory; }
            set { _SelectedPartnerCategory = value; NotifyOfPropertyChange(nameof(SelectedPartnerCategory)); }
        }

        public BussinessPartnerLedgerModel PartnerLedger { get; set; }
        private List<string> _AmountType;
        /// <summary>
        /// Amount Type for Initial Amount Like Payable Or Receviable
        /// </summary>
        public List<string> AmountType
        {
            get { return _AmountType; }
            set { _AmountType = value; NotifyOfPropertyChange(nameof(AmountType)); }
        }
        private string _SelectedAmountType;
        /// <summary>
        /// Selected AmountType
        /// </summary>
        public string SelectedAmountType
        {
            get { return _SelectedAmountType; }
            set { _SelectedAmountType = value; NotifyOfPropertyChange(nameof(SelectedAmountType)); }
        }

        private decimal _InitialAmount;
        /// <summary>
        /// Initial Amount for Adding Partner Account
        /// </summary>
        public decimal InitialAmount
        {
            get { return _InitialAmount; }
            set { _InitialAmount = value; NotifyOfPropertyChange(nameof(InitialAmount)); }
        }

        private bool _MobileNoError;

        public bool MobileNoError
        {
            get { return _MobileNoError; }
            set { _MobileNoError = value; NotifyOfPropertyChange(nameof(MobileNoError)); }
        }

        private bool _FullNameError;

        public bool FullNameError
        {
            get { return _FullNameError; }
            set { _FullNameError = value; NotifyOfPropertyChange(nameof(FullNameError)); }
        }

        private bool _BussinessNameError;

        public bool BussinessNameError
        {
            get { return _BussinessNameError; }
            set { _BussinessNameError = value; NotifyOfPropertyChange(nameof(BussinessNameError)); }
        }

        private string _MobileMessage;
        /// <summary>
        /// Message for Mobile TextBlock
        /// </summary>
        public string MobileMessage
        {
            get { return _MobileMessage; }
            set { _MobileMessage = value; NotifyOfPropertyChange(nameof(MobileMessage)); }
        }

        private ObservableCollection<string> _MobileNumbers;

        public ObservableCollection<string> MobileNumbers
        {
            get { return _MobileNumbers ?? new ObservableCollection<string>(); }
            set { _MobileNumbers = value; NotifyOfPropertyChange(nameof(MobileNumbers)); }
        }

        private string _PartnerMobileNumber;

        public string PartnerMobileNumber
        {
            get { return _PartnerMobileNumber; }
            set { _PartnerMobileNumber = value; NotifyOfPropertyChange(nameof(PartnerMobileNumber)); }
        }

        private bool _IsAddPartner;
        /// <summary>
        /// Adding Partner
        /// </summary>
        public bool IsAddPartner
        {
            get { return _IsAddPartner; }
            set { _IsAddPartner = value; NotifyOfPropertyChange(nameof(IsAddPartner)); }
        }
        private bool _IsUpdatePartner;
        /// <summary>
        /// Updating Partner
        /// </summary>
        public bool IsUpdatePartner
        {
            get { return _IsUpdatePartner; }
            set { _IsUpdatePartner = value; NotifyOfPropertyChange(nameof(IsUpdatePartner)); }
        }
        private bool _IsRemoveParnter;
        /// <summary>
        /// Removing Partner
        /// </summary>
        public bool IsRemoveParnter
        {
            get { return _IsRemoveParnter; }
            set { _IsRemoveParnter = value; NotifyOfPropertyChange(nameof(IsRemoveParnter)); }
        }
        private BussinessPartnerModel _NewBussinessPartner;
        /// <summary>
        /// 
        /// </summary>
        public BussinessPartnerModel NewBussinessPartner
        {
            get { return _NewBussinessPartner; }
            set { _NewBussinessPartner = value; NotifyOfPropertyChange(nameof(NewBussinessPartner)); }
        }

        private List<BussinessPartnerModel> _BussinessPartners;
        /// <summary>
        /// List Of Bussiness Partner
        /// </summary>
        public List<BussinessPartnerModel> BussinessPartners
        {
            get { return _BussinessPartners; }
            set { _BussinessPartners = value; NotifyOfPropertyChange(nameof(BussinessPartners)); }
        }

        private BussinessPartnerModel _SelectedBussinessPartner;
        /// <summary>
        /// Selected Bussiness Partner
        /// </summary>
        public BussinessPartnerModel SelectedBussinessPartner
        {
            get { return _SelectedBussinessPartner; }
            set { _SelectedBussinessPartner = value; NotifyOfPropertyChange(nameof(SelectedBussinessPartner)); UpdatePartner(); }
        }
        private List<CityModel> _Cities;

        public List<CityModel> Cities
        {
            get { return _Cities; }
            set { _Cities = value; NotifyOfPropertyChange(nameof(Cities)); }
        }
        private CitySuggetionProvider _CitySuggetion;

        public CitySuggetionProvider CitySuggetion
        {
            get { return _CitySuggetion; }
            set { _CitySuggetion = value; NotifyOfPropertyChange(nameof(CitySuggetion)); }
        }

        private CityModel _SelectedCity;
        /// <summary>
        /// Selected City from User
        /// </summary>
        public CityModel SelectedCity
        {
            get { return _SelectedCity; }
            set { _SelectedCity = value; NotifyOfPropertyChange(nameof(SelectedCity)); }
        }

        #endregion
    }
}
