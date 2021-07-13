using SmartSolutions.InventoryControl.DAL.Models.Region;
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
    [Export(typeof(ICountryManager)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class CountryManager : BaseManager, ICountryManager
    {
        #region Private Members
        private readonly IRepository Repository;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public CountryManager()
        {
            Repository = GetRepository<CountryModel>();
        }
        #endregion

        #region GET Methods
        public async Task<IEnumerable<CountryModel>> GetCountriesAsync()
        {
            List<CountryModel> countries = new List<CountryModel>();
            try
            {
                string query = @"SELECT * FROM Country";
                var values = await Repository.QueryAsync(query: query);
                if (values != null || values?.Count > 0)
                {
                    foreach (var value in values)
                    {
                        var country = new CountryModel();
                        country.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                        country.Name = value?.GetValueFromDictonary("Name")?.ToString();
                        country.NiceName = value?.GetValueFromDictonary("NiceName")?.ToString();
                        country.Iso = value?.GetValueFromDictonary("Iso")?.ToString();
                        country.Iso3 = value?.GetValueFromDictonary("Iso3")?.ToString();
                        country.IsActive = value?.GetValueFromDictonary("IsActive")?.ToString()?.ToNullableBoolean();
                        country.PhoneCode = value?.GetValueFromDictonary("PhoneCode")?.ToString()?.ToNullableInt() ?? 0;
                        countries.Add(country);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return countries;
        }

        public async Task<CountryModel> GetCountryByIdAsync(int? Id)
        {
            var country = new CountryModel();
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_Id"] = Id;
                string query = @"SELECT * FROM Country WHERE Id = @v_Id";
                var values = await Repository.QueryAsync(query: query, parameters: parameters);
                if (values != null || values?.Count > 0)
                {
                    var value = values.FirstOrDefault();
                    country.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                    country.Name = value?.GetValueFromDictonary("Name")?.ToString();
                    country.NiceName = value?.GetValueFromDictonary("NiceName")?.ToString();
                    country.Iso = value?.GetValueFromDictonary("Iso")?.ToString();
                    country.Iso3 = value?.GetValueFromDictonary("Iso3")?.ToString();
                    country.IsActive = value?.GetValueFromDictonary("IsActive")?.ToString()?.ToNullableBoolean();
                    country.PhoneCode = value?.GetValueFromDictonary("PhoneCode")?.ToString()?.ToNullableInt() ?? 0;
                }
            }
            catch (Exception ex)
            {

                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return country;
        }
        #endregion
    }
}
