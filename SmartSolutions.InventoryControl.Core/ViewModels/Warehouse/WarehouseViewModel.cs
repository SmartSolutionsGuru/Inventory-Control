using SmartSolutions.InventoryControl.DAL.Models.Region;
using SmartSolutions.InventoryControl.DAL.Models.Warehouse;
using SmartSolutions.Util.LogUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace SmartSolutions.InventoryControl.Core.ViewModels.Warehouse
{
    [Export(typeof(WarehouseViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class WarehouseViewModel : BaseViewModel
    {
        #region Private Members
        private readonly DAL.Managers.Region.ICountryManager _countryManager;
        private readonly DAL.Managers.Region.ICityManager _cityManager;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public WarehouseViewModel(DAL.Managers.Region.ICountryManager countryManager
                                 , DAL.Managers.Region.ICityManager cityManager)
        {
            _countryManager = countryManager;
            _cityManager = cityManager;
        }
        #endregion

        #region Public Methods
        public void SaveWarehouse()
        {
            try
            {

            }
            catch (Exception ex)
            {

                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        public void Clear()
        {
            TryClose();
        }
        public async void GetCitiesBySelectedCountry(int? countryId)
        {
            try
            {
               Cities = (await _cityManager.GetCitiesByCountryId(countryId)).ToList();
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
        }
        #endregion

        #region Protected Methods
        protected async override void OnActivate()
        {
            base.OnActivate();
            Countries = (await _countryManager.GetCountriesAsync()).ToList();
        }
        #endregion

        #region Properties
        private List<CountryModel> _Countries;
        /// <summary>
        /// List Of All Countries 
        /// </summary>
        public List<CountryModel> Countries
        {
            get { return _Countries; }
            set { _Countries = value; NotifyOfPropertyChange(nameof(Countries)); }
        }
        private CountryModel _SelectedCountry;

        public CountryModel SelectedCountry
        {
            get { return _SelectedCountry; }
            set { _SelectedCountry = value; NotifyOfPropertyChange(nameof(SelectedCountry)); GetCitiesBySelectedCountry(SelectedCountry?.Id); }
        }

        private List<CityModel> _Cities;
        /// <summary>
        /// List Of Cities Related To the Country
        /// </summary>
        public List<CityModel> Cities
        {
            get { return _Cities; }
            set { _Cities = value; NotifyOfPropertyChange(nameof(Cities)); }
        }
        private CityModel _SelectedCity;
        /// <summary>
        /// Selectd City
        /// </summary>
        public CityModel SelectedCity
        {
            get { return _SelectedCity; }
            set { _SelectedCity = value; NotifyOfPropertyChange(nameof(SelectedCity)); }
        }
        private string _PhoneNumber;
        /// <summary>
        /// Phone Number Of Warehouse
        /// </summary>
        public string PhoneNumber
        {
            get { return _PhoneNumber; }
            set { _PhoneNumber = value; NotifyOfPropertyChange(nameof(PhoneNumber)); }
        }

        private string _MobileNumber;
        /// <summary>
        /// Mobile Number for Warehouse
        /// </summary>
        public string MobileNumber
        {
            get { return _MobileNumber; }
            set { _MobileNumber = value; NotifyOfPropertyChange(nameof(MobileNumber)); }
        }
        private string _Address;
        /// <summary>
        /// Warehouse Address
        /// </summary>
        public string Address
        {
            get { return _Address; }
            set { _Address = value; NotifyOfPropertyChange(nameof(Address)); }
        }
        public WarehouseModel Warehouse { get; set; }
        #endregion
    }
}
