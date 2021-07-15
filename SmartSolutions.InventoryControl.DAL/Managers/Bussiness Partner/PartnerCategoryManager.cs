using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.DictionaryUtils;
using SmartSolutions.Util.LogUtils;
using SmartSolutions.Util.NumericUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Bussiness_Partner
{
    [Export(typeof(IPartnerCategoryManager)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class PartnerCategoryManager : BaseManager, IPartnerCategoryManager
    {
        #region Private Members
        private readonly IRepository Repository;
        #endregion

        #region Constructor
        public PartnerCategoryManager()
        {
            Repository = GetRepository<BussinessPartnerCategoryModel>();
        }
        #endregion

        #region GET Methods
        public async Task<IEnumerable<BussinessPartnerCategoryModel>> GetPartnerCategoriesAsync()
        {
            var partnerCategories = new List<BussinessPartnerCategoryModel>();
            try
            {
                string query = @"SELECT * FROM PartnerCategory WHERE IsActive = 1";
                var values = await Repository.QueryAsync(query: query);
                if(values != null || values?.Count > 0)
                {
                    foreach (var value in values)
                    {
                        var partnerCategory = new BussinessPartnerCategoryModel();
                        partnerCategory.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt() ?? 0;
                        partnerCategory.Name = value?.GetValueFromDictonary("Name")?.ToString();
                        partnerCategory.Description = value?.GetValueFromDictonary("Description")?.ToString();
                        partnerCategories.Add(partnerCategory);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return partnerCategories;
        }
        #endregion
    }
}
