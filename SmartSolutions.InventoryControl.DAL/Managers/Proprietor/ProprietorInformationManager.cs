using SmartSolutions.InventoryControl.DAL.Models;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.DictionaryUtils;
using SmartSolutions.Util.LogUtils;
using SmartSolutions.Util.NumericUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Proprietor
{
    [Export(typeof(IProprietorInformationManager)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ProprietorInformationManager : BaseManager, IProprietorInformationManager
    {
        #region Private Members
        private readonly IRepository Repository;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public ProprietorInformationManager()
        {
            Repository = GetRepository<ProprietorInformationModel>();
        }
        #endregion
        public async Task<bool> AddProprietorInfoAsync(ProprietorInformationModel ProprietorInfo)
        {
            if (ProprietorInfo == null) return false;
            bool retVal = false;
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_ProprietorName"] = ProprietorInfo.ProprietorName;
                parameters["@v_BussinessName"] = ProprietorInfo.BussinessName;
                parameters["@v_BussinessTypeId"] = ProprietorInfo?.BussinessType?.Id;
                parameters["@v_BussinessCategoryId"] = ProprietorInfo?.BussinessCategory?.Id;
                parameters["@v_CityId"] = ProprietorInfo?.City?.Id;
                parameters["@v_LandLineNumber"] = ProprietorInfo?.LandLineNumber == null ? DBNull.Value : (object)ProprietorInfo?.LandLineNumber;
                parameters["@v_LandLineNumber1"] = ProprietorInfo?.LandLineNumber1 == null ? DBNull.Value : (object)ProprietorInfo?.LandLineNumber1;
                parameters["@v_MobileNumber"] = ProprietorInfo.MobileNumber == null ? DBNull.Value : (object)ProprietorInfo?.MobileNumber;
                parameters["@v_MobileNumber1"] = ProprietorInfo.MobileNumber1 == null ? DBNull.Value : (object)ProprietorInfo?.MobileNumber1;
                parameters["@v_WhatsAppNumber"] = ProprietorInfo.WhatsAppNumber == null ? DBNull.Value : (object)ProprietorInfo?.WhatsAppNumber;
                parameters["@v_BussinessAddress"] = ProprietorInfo.BussinessAddress == null ? DBNull.Value : (object)ProprietorInfo?.BussinessAddress;
                parameters["@v_HomeAddress"] = ProprietorInfo.HomeAddress == null ? DBNull.Value : (object)ProprietorInfo?.HomeAddress;
                parameters["@v_Description"] = ProprietorInfo.Description == null ? DBNull.Value : (object)ProprietorInfo?.Description;
                parameters["@v_IsActive"] = ProprietorInfo.IsActive = true;
                parameters["@v_CreatedAt"] = ProprietorInfo.CreatedAt == null ? DateTime.Now : ProprietorInfo.CreatedAt;
                parameters["@v_CreatedBy"] = ProprietorInfo.CreatedBy == null ? DBNull.Value : (object)ProprietorInfo.CreatedBy;
                parameters["@v_UpdatedAt"] = ProprietorInfo.UpdatedAt == null ? DBNull.Value : (object)ProprietorInfo.UpdatedAt;
                parameters["@v_UpdatedBy"] = ProprietorInfo.UpdatedBy == null ? DBNull.Value : (object)ProprietorInfo.UpdatedBy;
                string query = @"INSERT INTO ProprietorInfo (ProprietorName,BussinessName,BussinessTypeId,BussinessCategoryId,CityId,LandLineNumber,LandLineNumber1,MobileNumber,MobileNumber1,WhatsAppNumber,BussinessAddress,HomeAddress,Description,IsActive,CreatedAt,CreatedBy,UpdatedAt,UpdatedBy)
                                                        VALUES(@v_ProprietorName,@v_BussinessName,@v_BussinessTypeId,@v_BussinessCategoryId,@v_CityId,@v_LandLineNumber,@v_LandLineNumber1,@v_MobileNumber,@v_MobileNumber1,@v_WhatsAppNumber,@v_BussinessAddress,@v_HomeAddress,@v_Description,@v_IsActive,@v_CreatedAt,@v_CreatedBy, @v_UpdatedAt,@v_UpdatedBy)";
                var result = await Repository.NonQueryAsync(query, parameters: parameters);
                retVal = result > 0 ? true : false;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }

        public async Task<ProprietorInformationModel> GetProprietorInfoAsync()
        {
            var proprietorInfo = new ProprietorInformationModel();
            try
            {
                string query = @"SELECT * FROM ProprietorInfo WHERE IsActive = 1";
                var values = await Repository.QueryAsync(query);
                if(values != null || values.Count > 0)
                {
                    var value = values.FirstOrDefault();
                    proprietorInfo.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                    proprietorInfo.ProprietorName = value?.GetValueFromDictonary("ProprietorName")?.ToString();
                    proprietorInfo.BussinessName = value?.GetValueFromDictonary("BussinessName")?.ToString();
                    proprietorInfo.LandLineNumber = value?.GetValueFromDictonary("LandLineNumber")?.ToString();
                    proprietorInfo.LandLineNumber1 = value?.GetValueFromDictonary("LandLineNumber1")?.ToString();
                    proprietorInfo.MobileNumber = value?.GetValueFromDictonary("MobileNumber")?.ToString();
                    proprietorInfo.MobileNumber1 = value?.GetValueFromDictonary("MobileNumber1")?.ToString();
                    proprietorInfo.WhatsAppNumber = value?.GetValueFromDictonary("WhatsAppNumber")?.ToString();
                    proprietorInfo.BussinessAddress = value?.GetValueFromDictonary("BussinessAddress")?.ToString();
                    proprietorInfo.HomeAddress = value?.GetValueFromDictonary("HomeAddress")?.ToString();
                    proprietorInfo.Description = value?.GetValueFromDictonary("Description")?.ToString();
                    proprietorInfo.BussinessType = new Models.BussinessPartner.BussinessPartnerTypeModel { Id = value.GetValueFromDictonary("Id").ToString()?.ToInt()};
                    proprietorInfo.BussinessCategory = new Models.BussinessPartner.BussinessPartnerCategoryModel { Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt()};
                    proprietorInfo.City = new Models.Region.CityModel { Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt()};
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return proprietorInfo;
        }
    }
}
