using SmartSolutions.InventoryControl.DAL.Models.Region;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.DictionaryUtils;
using SmartSolutions.Util.LogUtils;
using SmartSolutions.Util.NumericUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Region
{
    [Export(typeof(IProvinceManager)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ProvinceManager : BaseManager, IProvinceManager
    {
        #region Private Members
        private readonly IRepository Repository;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public ProvinceManager()
        {
            Repository = GetRepository<ProvinceModel>();
        }
        #endregion

        #region Public Methods

        public async Task<IEnumerable<ProvinceModel>> GetProvinceByCountryIdAsync(int? countryId)
        {
            List<ProvinceModel> provinces = new List<ProvinceModel>();
            try
            {
                if (countryId == null || countryId == 0) return null;
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_countryId"] = countryId;
                string query = "SELECT * FROM Province WHERE CountryId = @countryId";
                var values = await Repository.QueryAsync(query, parameters: parameters);
                if (values != null || values?.Count > 0)
                {
                    foreach (var value in values)
                    {
                        var province = new ProvinceModel();
                        province.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                        province.Name = value?.GetValueFromDictonary("Name")?.ToString();
                        provinces.Add(province);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return provinces;
        }
        #endregion
    }
}
