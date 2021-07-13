using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.DictionaryUtils;
using SmartSolutions.Util.LogUtils;
using SmartSolutions.Util.NumericUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Bussiness_Partner
{
    [Export(typeof(IPartnerTypeManager)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class PartnerTypeManager : BaseManager, IPartnerTypeManager
    {
        #region Private Members
        private readonly IRepository Repository;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public PartnerTypeManager()
        {
            Repository = GetRepository<BussinessPartnerTypeModel>();
        }
        #endregion

        #region GET Methods
        public async Task<IEnumerable<BussinessPartnerTypeModel>> GetPartnerType()
        {
            var partnerTypes = new List<BussinessPartnerTypeModel>();
            try
            {
                string query = @"SELECT * FROM PartnerType WHERE IsActive = 1";
                var values = await Repository.QueryAsync(query:query);
                if (values != null || values?.Count > 0)
                {
                    foreach (var value in values)
                    {
                        var partnerType = new BussinessPartnerTypeModel();
                        partnerType.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                        partnerType.Name = value?.GetValueFromDictonary("Name")?.ToString();
                        partnerType.Description = value?.GetValueFromDictonary("Description")?.ToString();
                        partnerTypes.Add(partnerType);
                    } 
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return partnerTypes;
        }
        #endregion
    }
}
