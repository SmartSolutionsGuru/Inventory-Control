using SmartSolutions.InventoryControl.DAL.Managers.Region;
using SmartSolutions.InventoryControl.DAL.Models.Region;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using SmartSolutions.Util.LogUtils;
using SmartSolutions.InventoryControl.DAL;
using System.Diagnostics.Metrics;
using SmartSolutions.Util.StrUtils;

namespace SmartSolutions.InventoryControl.Core.ViewModels.Dialogs
{
    /// <summary>
    /// Class for Adding new City
    /// </summary>
    [Export(typeof(AddCityDialogViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class AddCityDialogViewModel : BaseViewModel
    {
        #region [Private Members]
        private readonly DAL.Managers.Region.ICityManager _cityManager;
        private readonly DAL.Managers.Region.ICountryManager _countryManager;
        private readonly DAL.Managers.Region.IProvinceManager _provinceManager;
        #endregion

        #region [Constructor]
        [ImportingConstructor]
        public AddCityDialogViewModel(ICountryManager countryManager,
                                    IProvinceManager provinceManager,
                                    ICityManager cityManager)
        {
            _countryManager = countryManager;
            _provinceManager = provinceManager;
            _cityManager = cityManager;
        }
        #endregion
        protected async override void OnActivate()
        {
            base.OnActivate();
            Countries = (await _countryManager.GetCountriesAsync()).ToList();
            SelectedCountry = Countries.FirstOrDefault(x => x.Name.ToUpper().Equals("Pakistan".ToUpper()));
            Provinces = (await _provinceManager.GetProvinceByCountryIdAsync(SelectedCountry.Id)).ToList();
            SelectedProvince = Provinces?.FirstOrDefault();
        }

        public void Close()
        {
            TryClose();
        }
        public async void Submit()
        {
            IsLoading = true;
            try
            {
                if (!string.IsNullOrEmpty(CityName))
                {
                    //Verify city Already exist in list or not
                    var resultCity = await _cityManager.GetCityByNameAsync(CityName);
                    if (string.IsNullOrEmpty(resultCity.CityName))
                    {
                        City = new CityModel()
                        {
                            Name = CityName,
                            Country = SelectedCountry,
                            Province = SelectedProvince,
                            PhoneCode = Convert.ToInt32(CityCode),
                            CreatedAt = DateTime.Now,
                            CreatedBy = AppSettings.LoggedInUser.DisplayName,
                            IsActive = true,
                            IsDeleted = false,
                            Description = $"City {CityName} With {SelectedCountry.Name} and {SelectedProvince.Name} is Added At {DateTime.Now} With {AppSettings.LoggedInUser.Name}"

                        };
                        var result = await _cityManager.AddCityAsync(City);
                        if (result)
                        {
                            NotificationManager.Show(new Notifications.Wpf.NotificationContent { Title = "Success", Message = "City Added Successfully", Type = Notifications.Wpf.NotificationType.Success });
                        }
                        else
                        {
                            NotificationManager.Show(new Notifications.Wpf.NotificationContent { Title = "Error", Message = "City not  Added", Type = Notifications.Wpf.NotificationType.Error });
                        }
                    }
                    else
                    {
                        NotificationManager.Show(new Notifications.Wpf.NotificationContent { Title = "Error", Message = "City Already  Exist", Type = Notifications.Wpf.NotificationType.Error });
                    }

                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            finally
            {
                IsLoading = false;
                TryClose();
            }
        }
        public void Cancel()
        {
            TryClose();
        }



        #region [Properties]

        public CityModel City { get; set; }

        private List<DAL.Models.Region.CountryModel> _Countries;
        /// <summary>
        /// gets or set List Of Countries
        /// </summary>
        public List<DAL.Models.Region.CountryModel> Countries
        {
            get { return _Countries; }
            set { _Countries = value; NotifyOfPropertyChange(nameof(Countries)); }
        }
        private DAL.Models.Region.CountryModel _SelectedCountry;
        /// <summary>
        /// gets or set Selected Country
        /// </summary>
        public DAL.Models.Region.CountryModel SelectedCountry
        {
            get { return _SelectedCountry; }
            set { _SelectedCountry = value; NotifyOfPropertyChange(nameof(SelectedCountry)); }
        }

        private List<DAL.Models.Region.ProvinceModel> _Provinces;
        /// <summary>
        /// gets or set List of Provice
        /// </summary>
        public List<DAL.Models.Region.ProvinceModel> Provinces
        {
            get { return _Provinces; }
            set { _Provinces = value; NotifyOfPropertyChange(nameof(Provinces)); }
        }
        private DAL.Models.Region.ProvinceModel _SelectedProvince;

        public DAL.Models.Region.ProvinceModel SelectedProvince
        {
            get { return _SelectedProvince; }
            set { _SelectedProvince = value; NotifyOfPropertyChange(nameof(SelectedProvince)); }
        }
        private string _CityName;
        /// <summary>
        /// gets or set City Name
        /// </summary>
        public string CityName
        {
            get { return _CityName; }
            set 
            {
                if(!string.IsNullOrEmpty(value))
                {
                    _CityName = value;
                    _CityName = _CityName.CapitalizeFirstLetter();
                }              
                NotifyOfPropertyChange(nameof(CityName));
            }
        }
        private string _CityCode;
        /// <summary>
        /// gets or set City Code
        /// </summary>
        public string CityCode
        {
            get { return _CityCode; }
            set { _CityCode = value; NotifyOfPropertyChange(nameof(CityCode)); }
        }

        #endregion
    }
}
