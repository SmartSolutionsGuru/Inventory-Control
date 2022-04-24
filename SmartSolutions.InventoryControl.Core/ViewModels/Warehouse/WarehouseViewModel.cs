using Caliburn.Micro;
using SmartSolutions.InventoryControl.DAL.Models.Region;
using SmartSolutions.InventoryControl.DAL.Models.Warehouse;
using SmartSolutions.Util.LogUtils;
using SmartSolutions.Util.ValidUtils;
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
        private readonly DAL.Managers.Warehouse.IWarehouseManager _warehouseManager;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public WarehouseViewModel(DAL.Managers.Region.ICountryManager countryManager
                                 , DAL.Managers.Region.ICityManager cityManager
                                 , DAL.Managers.Warehouse.IWarehouseManager warehouseManager)
        {
            _countryManager = countryManager;
            _cityManager = cityManager;
            _warehouseManager = warehouseManager;
        }
        #endregion

        #region Public Methods
        public async void SaveWarehouse()
        {
            try
            {
                IsLoading = true;
                LoadingMessage = "Saving Details...";
                Warehouse = new WarehouseModel();
                if (!string.IsNullOrEmpty(WarehouseName))
                    Warehouse.Name = WarehouseName;
                if (!string.IsNullOrEmpty(PhoneNumber))
                {
                    if (ValidationUtil.IsValidPhoneNumber(PhoneNumber))
                        Warehouse.PhoneNumber = PhoneNumber;
                }
                if (!string.IsNullOrEmpty(MobileNumber))
                {
                    if (ValidationUtil.IsValidMobileNumber(MobileNumber))
                        Warehouse.MobileNumber = MobileNumber;
                }
                if (!string.IsNullOrEmpty(Address))
                {
                    Warehouse.Address = Address;
                }
                if (SelectedCountry == null || string.IsNullOrEmpty(SelectedCountry?.Name))
                {
                    SelectedCountry = Countries?.Where(x => x.Id == 162).FirstOrDefault();
                }
                Warehouse.Country = SelectedCountry;
                if (SelectedCity == null || string.IsNullOrEmpty(SelectedCity?.Name))
                {
                    if (Cities == null || Cities?.Count() == 0)
                    {
                        Cities = (await _cityManager.GetCitiesByCountryIdAsync(SelectedCountry?.Id)).ToList();
                    }
                    SelectedCity = Cities?.FirstOrDefault(x => x.Id == 274);
                }
                Warehouse.City = SelectedCity;
                var result = await _warehouseManager.SaveWarehouseAsync(Warehouse);
                if (result)
                {
                    IsLoading = false;
                    Close();
                }
                else
                {
                    // User Friendly Message for Not Saving warehouse
                    await IoC.Get<IDialogManager>().ShowMessageBoxAsync("Sorry Cannot Save Warehouse Try Again", options: Dialogs.MessageBoxOptions.Ok);
                }
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
        public void Close()
        {
            TryClose();
        }
        public async void GetCitiesBySelectedCountry(int? countryId)
        {
            try
            {
                Cities = (await _cityManager.GetCitiesByCountryIdAsync(countryId)).ToList();
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
            IsAddWarehouse = true;
            Cities = new List<CityModel>();
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
        private string _WarehouseName;

        public string WarehouseName
        {
            get { return _WarehouseName; }
            set { _WarehouseName = value; NotifyOfPropertyChange(nameof(WarehouseName)); }
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
        //public WarehouseModel Warehouse { get; set; }
        private WarehouseModel _Warehouse;

        public WarehouseModel Warehouse
        {
            get { return _Warehouse; }
            set { _Warehouse = value; NotifyOfPropertyChange(nameof(Warehouse)); }
        }

        private bool _IsAddWarehouse;
        /// <summary>
        /// Add warehouse is Selected
        /// </summary>
        public bool IsAddWarehouse
        {
            get { return _IsAddWarehouse; }
            set { _IsAddWarehouse = value; NotifyOfPropertyChange(nameof(IsAddWarehouse)); }
        }
        private bool _IsUpdateWarehouse;
        /// <summary>
        /// Update Warehouse
        /// </summary>
        public bool IsUpdateWarehouse
        {
            get { return _IsUpdateWarehouse; }
            set { _IsUpdateWarehouse = value; NotifyOfPropertyChange(nameof(IsUpdateWarehouse)); }
        }

        private bool _IsRemoveWarehouse;

        public bool IsRemoveWarehouse
        {
            get { return _IsRemoveWarehouse; }
            set { _IsRemoveWarehouse = value; NotifyOfPropertyChange(nameof(IsRemoveWarehouse)); }
        }

        #endregion
    }
}
