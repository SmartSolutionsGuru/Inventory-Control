﻿using SmartSolutions.InventoryControl.DAL.Models.Region;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.BooleanUtils;
using SmartSolutions.Util.DictionaryUtils;
using SmartSolutions.Util.LogUtils;
using SmartSolutions.Util.NumericUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Region
{
    [Export(typeof(ICityManager)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class CityManager : BaseManager, ICityManager
    {
        #region Private Members
        private readonly IRepository Repository;
        #endregion

        #region Costructor
        [ImportingConstructor]
        public CityManager()
        {
            Repository = GetRepository<CityModel>();
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Get the All Cities From Country Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>List Of CityModels</returns>
        public async Task<IEnumerable<CityModel>> GetCitiesByCountryIdAsync(int? Id)
        {
            if (Id == null || Id == 0) return null;
            List<CityModel> cities = new List<CityModel>();
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_Id"] = Id;
                string query = @"SELECT * FROM City WHERE CountryId = @v_Id";
                var values = await Repository.QueryAsync(query: query, parameters: parameters);
                if (values != null || values?.Count > 0)
                {
                    foreach (var value in values)
                    {
                        var city = new CityModel();
                        city.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                        city.Name = value?.GetValueFromDictonary("Name")?.ToString();
                        city.CityName = value?.GetValueFromDictonary("Name")?.ToString();
                        city.Province.Id = value?.GetValueFromDictonary("ProvinceId")?.ToString()?.ToNullableInt() ?? 0;
                        city.Country.Id = value?.GetValueFromDictonary("CountrId")?.ToString()?.ToNullableInt() ?? 0;
                        city.IsActive = value?.GetValueFromDictonary("IsActive")?.ToString()?.ToNullableBoolean();
                        cities.Add(city);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return cities;
        }
        /// <summary>
        /// Get the All Cities Of Country From Country Name
        /// </summary>
        /// <param name="countryName"></param>
        /// <returns>List Of CityModels</returns>
        public async Task<IEnumerable<CityModel>> GetCitiesByCountryNameAsync(string countryName)
        {
            if (string.IsNullOrEmpty(countryName)) return null;
            List<CityModel> cities = new List<CityModel>();
            try
            {

                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_CountryName"] = countryName;
                string query = @"SELECT * FROM City WHERE CountryId = (SELECT Id FROM Country WHERE NiceName = @v_CountryName);";
                var values = await Repository.QueryAsync(query: query, parameters: parameters);
                if (values != null || values?.Count > 0)
                {
                    foreach (var value in values)
                    {
                        var city = new CityModel();
                        city.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                        city.Name = value?.GetValueFromDictonary("Name")?.ToString();
                        city.Province.Id = value?.GetValueFromDictonary("ProvinceId")?.ToString()?.ToNullableInt() ?? 0;
                        city.Country.Id = value?.GetValueFromDictonary("CountrId")?.ToString()?.ToNullableInt() ?? 0;
                        city.IsActive = value?.GetValueFromDictonary("IsActive")?.ToString()?.ToNullableBoolean();
                        cities.Add(city);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return cities;
        }
        /// <summary>
        /// Get the City Detail From City Id
        /// </summary>
        /// <param name="cityId"> </param>
        /// <returns> List Of City Models</returns>
        public async Task<CityModel> GetCityFromIdAsync(int? cityId)
        {
            if (cityId == null || cityId == 0) return null;
            var city = new CityModel();
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_Id"] = cityId;
                string query = @"SELECT * FROM City WHERE Id = @v_Id";
                var values = await Repository.QueryAsync(query: query, parameters: parameters);
                if (values != null || values.Count > 0)
                {
                    var resultCity = values.FirstOrDefault();
                    city.Id = resultCity.GetValueFromDictonary("Id")?.ToString()?.ToInt() ?? 0;
                    city.Name = resultCity.GetValueFromDictonary("Name")?.ToString();
                    city.CityName = resultCity.GetValueFromDictonary("Name")?.ToString();
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return city;
        }
        public async Task<IEnumerable<CityModel>> GetCitiesAsync(string searchText)
        {
            List<CityModel> cities = new List<CityModel>();
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_searchText"] = searchText;
                string query = @"SELECT * FROM dbo.City WHERE Name LIKE @v_searchText + '%' AND IsActive = 1;";
                var values = await Repository.QueryAsync(query: query, parameters: parameters);
                if(values != null && values?.Count > 0)
                {
                    foreach (var value in values)
                    {
                        var city = new CityModel();
                        city.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                        city.CityName = city.Name = value?.GetValueFromDictonary("Name")?.ToString();
                        city.PhoneCode = value?.GetValueFromDictonary("CityCode")?.ToString().ToInt() ?? 0;
                        city.Province.Id = value?.GetValueFromDictonary("ProvinceId")?.ToString()?.ToNullableInt() ?? 0;
                        city.Country.Id = value?.GetValueFromDictonary("CountrId")?.ToString()?.ToNullableInt() ?? 0;
                        cities.Add(city);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return cities;
        }

        public async Task<bool> AddCityAsync(CityModel city)
        {
            bool retVal = false;
            try
            {
                //null guard
                if (city == null) return retVal;
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@_Name"] = city.Name;
                parameters["@v_CountryId"] = city.Country.Id;
                parameters["@v_ProvinceId"] = city.Province.Id;
                parameters["@v_PhoneCode"] = city.PhoneCode  == 0 ? DBNull.Value : (object)city.PhoneCode;
                parameters["@v_Description"] = city.Description;
                parameters["@v_IsActive"] = city.IsActive;
                parameters["@v_IsDeleted"] = city.IsDeleted;
                parameters["@v_CreatedAt"] = city.CreatedAt;
                parameters["@v_CreatedBy"] = city.CreatedBy;
                string query = @"INSERT INTO City (Name,CountryId,ProvinceId,PhoneCode,Description,IsActive,IsDeleted,CreatedAt,CreatedBy) 
                                           VALUES(@_Name,@v_CountryId,@v_ProvinceId,@v_PhoneCode,@v_Description,@v_IsActive,@v_IsDeleted,@v_CreatedAt,@v_CreatedBy)";
                var result = await Repository.NonQueryAsync(query,parameters:parameters);
                retVal = result > 0 ? true : false;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }
        /// <summary>
        /// get the City By Name
        /// </summary>
        /// <param name="cityName">city Name</param>
        /// <returns>Return City object</returns>
        public async Task<CityModel> GetCityByNameAsync(string cityName)
        {
            CityModel city = new CityModel();
            if (string.IsNullOrEmpty(cityName)) return null;
            try
            {
                Dictionary<string,object> parameters = new Dictionary<string, object>();
                parameters["@v_Name"] = cityName;
                string query = @"SELECT * FROM City WHERE Name = @v_Name";
               var values = await Repository.QueryAsync(query, parameters: parameters);
                if(values.Any())
                {
                    foreach (var value in values) 
                    {
                        city.Id = value?.GetValueFromDictonary("Id")?.ToString().ToInt();
                        city.Name = city.CityName = value?.GetValueFromDictonary("Name")?.ToString();
                        city.PhoneCode = value?.GetValueFromDictonary("PhoneCode")?.ToString()?.ToNullableInt() ?? 0;
                        city.Country = new CountryModel { Id = value?.GetValueFromDictonary("CountryCode")?.ToString()?.ToInt() };
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return city;
        }
        #endregion
    }
}
