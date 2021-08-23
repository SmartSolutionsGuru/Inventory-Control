using Caliburn.Micro;
using SmartSolutions.InventoryControl.DAL.Models;
using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using SmartSolutions.InventoryControl.DAL.Models.Region;
using SmartSolutions.Util.LogUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace SmartSolutions.InventoryControl.Core.ViewModels
{
    [Export(typeof(ProprietorInformationViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ProprietorInformationViewModel : BaseViewModel
    {
        #region Private Members
        private readonly DAL.Managers.Bussiness_Partner.IPartnerTypeManager _partnerTypeManager;
        private readonly DAL.Managers.Bussiness_Partner.IPartnerCategoryManager _partnerCategoryManager;
        private readonly DAL.Managers.Region.ICityManager _cityManager;
        private readonly DAL.Managers.Proprietor.IProprietorInformationManager _proprietorInformationManager;
        private readonly DAL.Managers.Settings.ISystemSettingManager _systemSettingManager;
        private readonly IEventAggregator _eventAggregator;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public ProprietorInformationViewModel(DAL.Managers.Bussiness_Partner.IPartnerTypeManager partnerTypeManager
                                              , DAL.Managers.Bussiness_Partner.IPartnerCategoryManager partnerCategoryManager
                                              , DAL.Managers.Region.ICityManager cityManager
                                              , DAL.Managers.Proprietor.IProprietorInformationManager proprietorInformationManager
                                              , DAL.Managers.Settings.ISystemSettingManager systemSettingManager
                                               , IEventAggregator eventAggregator)
        {
            _partnerTypeManager = partnerTypeManager;
            _partnerCategoryManager = partnerCategoryManager;
            _cityManager = cityManager;
            _proprietorInformationManager = proprietorInformationManager;
            _systemSettingManager = systemSettingManager;
            _eventAggregator = eventAggregator;
        }
        #endregion

        #region Public Methods
        protected async override void OnActivate()
        {
            base.OnActivate();
            ProprietorInformation = new ProprietorInformationModel();
            Cities = (await _cityManager.GetCitiesByCountryNameAsync("Pakistan")).ToList();
            BussinessTypes = (await _partnerTypeManager.GetPartnerTypesAsync()).ToList();
            BussinessCwtegories = (await _partnerCategoryManager.GetPartnerCategoriesAsync()).ToList();

        }

        public void Cancel()
        {
            TryClose();
        }
        public async void Submit()
        {
            try
            {
                IsLoading = true;
                LoadingMessage = "Saving Information...";
                ProprietorInformation.ProprietorName = ProprietorName;
                ProprietorInformation.BussinessName = CompanyName;
                ProprietorInformation.BussinessType = SelectedBussinessType;
                ProprietorInformation.BussinessCategory = SelectedBussinessCategory;
                ProprietorInformation.City = SelectedCity;
                ProprietorInformation.LandLineNumber = LandLineNumber;
                ProprietorInformation.LandLineNumber1 = LandLineNumber1;
                ProprietorInformation.MobileNumber = MobileNumber;
                ProprietorInformation.MobileNumber1 = MobileNumber;
                ProprietorInformation.WhatsAppNumber = WhatsAppNumber;
                ProprietorInformation.BussinessAddress = BussinessAddress;
                ProprietorInformation.HomeAddress = HomeAddress;
                ProprietorInformation.Description = Description;
                var result = await _proprietorInformationManager.AddProprietorInfoAsync(ProprietorInformation);
                if (result)
                {
                    NotificationManager.Show(new Notifications.Wpf.NotificationContent { Title = "Success", Message = "Successfully Add Info", Type = Notifications.Wpf.NotificationType.Information });
                    var systemSetting = new SystemSettingModel();
                    systemSetting.SettingKey = "IsProprietorAvailable";
                    systemSetting.Name = systemSetting.SettingKey;
                    systemSetting.SettingValue = 1;
                    systemSetting.DefaultValue = false;
                    systemSetting.Description = "Proprietor Info has been Saved";
                    systemSetting.CreatedAt = DateTime.Now;
                    var resultSaved = await _systemSettingManager.SaveSettingAsync(systemSetting);
                    if (resultSaved)
                        _eventAggregator.PublishOnCurrentThread(IoC.Get<Login.LoginViewModel>());
                }
                IsLoading = false;
            }
            catch (Exception ex)
            {
                IsLoading = false;
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        #endregion

        #region Properties
        private string _CompanyName;
        /// <summary>
        /// Company Name Of Proprietor
        /// </summary>
        public string CompanyName
        {
            get { return _CompanyName; }
            set { _CompanyName = value; NotifyOfPropertyChange(nameof(CompanyName)); }
        }
        private string _ProprietorName;
        /// <summary>
        /// Proprietor Name
        /// </summary>
        public string ProprietorName
        {
            get { return _ProprietorName; }
            set { _ProprietorName = value; NotifyOfPropertyChange(nameof(ProprietorName)); }
        }

        private ProprietorInformationModel _ProprietorInformation;
        /// <summary>
        /// Basic Information Of Propeiter
        /// </summary>
        public ProprietorInformationModel ProprietorInformation
        {
            get { return _ProprietorInformation; }
            set { _ProprietorInformation = value; NotifyOfPropertyChange(nameof(ProprietorInformation)); }
        }

        private List<CityModel> _Cities;

        public List<CityModel> Cities
        {
            get { return _Cities; }
            set { _Cities = value; NotifyOfPropertyChange(nameof(Cities)); }
        }

        private List<BussinessPartnerTypeModel> _BussinessTypes;

        public List<BussinessPartnerTypeModel> BussinessTypes
        {
            get { return _BussinessTypes; }
            set { _BussinessTypes = value; NotifyOfPropertyChange(nameof(BussinessTypes)); }
        }
        private BussinessPartnerTypeModel _SelectedBussinessType;

        public BussinessPartnerTypeModel SelectedBussinessType
        {
            get { return _SelectedBussinessType; }
            set { _SelectedBussinessType = value; NotifyOfPropertyChange(nameof(SelectedBussinessType)); }
        }

        private List<BussinessPartnerCategoryModel> _BussinessCwtegories;

        public List<BussinessPartnerCategoryModel> BussinessCwtegories
        {
            get { return _BussinessCwtegories; }
            set { _BussinessCwtegories = value; NotifyOfPropertyChange(nameof(BussinessCwtegories)); }
        }
        private BussinessPartnerCategoryModel _SelectedBussinessCategory;

        public BussinessPartnerCategoryModel SelectedBussinessCategory
        {
            get { return _SelectedBussinessCategory; }
            set { _SelectedBussinessCategory = value; NotifyOfPropertyChange(nameof(SelectedBussinessCategory)); }
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
            set { _LandLineNumber1 = value; NotifyOfPropertyChange(nameof(LandLineNumber)); }
        }

        private CityModel _SelectedCity;
        /// <summary>
        /// Selected City
        /// </summary>
        public CityModel SelectedCity
        {
            get { return _SelectedCity; }
            set { _SelectedCity = value; NotifyOfPropertyChange(nameof(SelectedCity)); }
        }

        private string _MobileNumber;

        public string MobileNumber
        {
            get { return _MobileNumber; }
            set { _MobileNumber = value; NotifyOfPropertyChange(nameof(MobileNumber)); }
        }
        private string _MobileNumber1;

        public string MobileNumber1
        {
            get { return _MobileNumber1; }
            set { _MobileNumber1 = value; NotifyOfPropertyChange(nameof(MobileNumber1)); }
        }
        private string _WhatsAppNumber;

        public string WhatsAppNumber
        {
            get { return _WhatsAppNumber; }
            set { _WhatsAppNumber = value; NotifyOfPropertyChange(nameof(WhatsAppNumber)); }
        }
        private string _BussinessAddress;

        public string BussinessAddress
        {
            get { return _BussinessAddress; }
            set { _BussinessAddress = value; NotifyOfPropertyChange(nameof(BussinessAddress)); }
        }
        private string _HomeAddress;

        public string HomeAddress
        {
            get { return _HomeAddress; }
            set { _HomeAddress = value; NotifyOfPropertyChange(nameof(HomeAddress)); }
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
