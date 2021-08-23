using SmartSolutions.InventoryControl.DAL.Models;
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

namespace SmartSolutions.InventoryControl.DAL.Managers.Settings
{
    [Export(typeof(ISystemSettingManager)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SystemSettingManager : BaseManager, ISystemSettingManager
    {
        #region Private Members
        private readonly IRepository Repository;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public SystemSettingManager()
        {
            Repository = GetRepository<SystemSettingModel>();
        }
        #endregion

        #region Public Methods
        public async Task<SystemSettingModel> GetSystemSettingById(int? Id)
        {
            if (Id == null || Id.Value == 0) return null;
            SystemSettingModel setting = new SystemSettingModel();
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_settingId"] = Id;
                string query = "SELECT * FROM SystemSettings WHERE SettingKey = @v_settingId";
                var values = await Repository.QueryAsync(query: query, parameters: parameters);
                if (values != null || values?.Count > 0)
                {
                    var value = values?.FirstOrDefault();
                    setting.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                    setting.Name = value?.GetValueFromDictonary("Name")?.ToString();
                    setting.SettingKey = value?.GetValueFromDictonary("SettingKey")?.ToString();
                    setting.SettingValue = value?.GetValueFromDictonary("SettingValue")?.ToString()?.ToNullableInt() ?? 0;
                    setting.Value = value?.GetValueFromDictonary("Value")?.ToString();
                    setting.DefaultValue = value?.GetValueFromDictonary("DefaultValue")?.ToString()?.ToNullableBoolean() ?? false;
                    setting.Description = value?.GetValueFromDictonary("Description")?.ToString();
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return setting;
        }
        public async Task<SystemSettingModel> GetsystemSettingByKeyAsync(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;
            SystemSettingModel setting = new SystemSettingModel();
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_settingKey"] = key;
                string query = "SELECT * FROM SystemSettings WHERE SettingKey = @v_settingKey";
                var values = await Repository.QueryAsync(query: query, parameters: parameters);
                if(values != null || values?.Count > 0)
                {
                    var value = values?.FirstOrDefault();
                    setting.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                    setting.Name = value?.GetValueFromDictonary("Name")?.ToString();
                    setting.SettingKey = value?.GetValueFromDictonary("SettingKey")?.ToString();
                    setting.SettingValue = value?.GetValueFromDictonary("SettingValue")?.ToString()?.ToNullableInt() ?? 0;
                    setting.Value = value?.GetValueFromDictonary("Value")?.ToString();
                    setting.DefaultValue = value?.GetValueFromDictonary("DefaultValue")?.ToString()?.ToNullableBoolean() ?? false;
                    setting.Description = value?.GetValueFromDictonary("Description")?.ToString();
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return setting;
        }
        public async Task<bool> SaveSettingAsync(SystemSettingModel setting)
        {
            if (setting == null) return false;
            bool retVal = false;
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_Name"] = setting.Name;
                parameters["@v_SettingKey"] = setting.SettingKey;
                parameters["@v_SettingValue"] = setting.SettingValue;
                parameters["@v_Value"] = setting.Value == null ? DBNull.Value : (object)setting.Value;
                parameters["@v_DefaultValue"] = setting.DefaultValue == null ? DBNull.Value : (object)setting.DefaultValue;
                parameters["@v_Description"] = setting.Description == null ? DBNull.Value : (object)setting.Description;
                parameters["@v_IsActive"] = setting.IsActive = true;
                parameters["@v_CreatedAt"] = setting.CreatedAt == null ? DateTime.Now : setting.CreatedAt;
                parameters["@v_CreatedBy"] = setting.CreatedBy == null ? DBNull.Value : (object)setting.CreatedBy;
                parameters["@v_UpdatedAt"] = setting.UpdatedAt == null ? DBNull.Value : (object)setting.UpdatedAt;
                parameters["@v_UpdatedBy"] = setting.UpdatedBy == null ? DBNull.Value : (object)setting.UpdatedBy;
                string query = @"INSERT INTO SystemSettings(Name,SettingKey,SettingValue,Value,DefaultValue,Description,CreatedAt,CreatedBy,UpdatedAt,UpdatedBy)
                                                    VALUES(@v_Name,@v_SettingKey,@v_SettingValue,@v_Value,@v_DefaultValue,@v_Description,@v_CreatedAt,@v_CreatedBy,@v_UpdatedAt,@v_UpdatedBy);";
               var result =  await Repository.NonQueryAsync(query:query,parameters:parameters);
                retVal = result > 0 ? true : false;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }
        #endregion
    }
}
