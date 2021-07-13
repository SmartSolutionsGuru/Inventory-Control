using SmartSolutions.InventoryControl.DAL.Models.Region;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.LogUtils;
using SmartSolutions.Util.BooleanUtils;
using SmartSolutions.Util.DictionaryUtils;
using SmartSolutions.Util.NumericUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
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
        public async Task<IEnumerable<CityModel>> GetCitiesByCountryId(int? Id)
        {
            if (Id == null || Id == 0) return null;
            List<CityModel> cities = new List<CityModel>();
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_Id"] = Id;
                string query = @"SELECT * FROM City WHERE Id = @v_Id";
                var values = await Repository.QueryAsync(query: query, parameters: parameters);
                if (values != null || values?.Count > 0)
                {
                    foreach (var value  in values)
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

        #endregion
    }
}
