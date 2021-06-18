using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.DateAndTimeUtils;
using SmartSolutions.Util.DictionaryUtils;
using SmartSolutions.Util.LogUtils;
using SmartSolutions.Util.NumericUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Bussiness_Partner
{
    [Export(typeof(IBussinessPartnerManager)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class BussinessPartnerManager : BaseManager, IBussinessPartnerManager
    {
        #region Private Members
        private readonly IRepository Repository;
        #endregion

        #region Costructor
        [ImportingConstructor]
        public BussinessPartnerManager()
        {
            Repository = GetRepository<BussinessPartnerModel>();
        }
        #endregion

        #region Public Methods
        public async Task<bool> AddBussinesPartnerAsync(BussinessPartnerModel partner)
        {
            bool retVal = false;
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                string mobilenumbers = string.Empty;
                parameters["@v_Name"] = partner?.Name;
                parameters["@v_BussinessName"] = partner?.BussinessName;
                parameters["@v_PhoneNumber"] = partner?.PhoneNumber;
                parameters["@v_Address"] = partner?.Address;
                parameters["@v_City"] = partner?.City;
                parameters["@v_IsActive"] = partner.IsActive = true;
                parameters["@v_IsDeleted"] = partner.IsDeleted = false;
                parameters["@v_CreatedAt"] = partner.CreatedAt == null ? DateTime.Now : partner.CreatedAt;
                parameters["@v_CreatedBy"] = partner.CreatedBy == null ? DBNull.Value : (object)partner.CreatedBy;
                parameters["@v_UpdatedAt"] = partner.UpdatedAt == null ? DBNull.Value : (object)partner.UpdatedAt;
                parameters["@v_UpdatedBy"] = partner.UpdatedBy == null ? DBNull.Value : (object)partner.UpdatedBy;
                if (partner?.MobileNumbers?.Count > 0)
                {
                    foreach (var item in partner?.MobileNumbers)
                    {
                        mobilenumbers = mobilenumbers + item + ",";
                    }
                    mobilenumbers = mobilenumbers.Remove(mobilenumbers.Length - 1);
                }
                parameters["@v_MobileNumbers"] = mobilenumbers;
                string query = string.Empty;
                query = @"INSERT INTO BussinessPartner (Name,BussinessName,PhoneNumber,MobileNumber,City,Address,IsActive,IsDeleted,CreatedAt,CreatedBy,UpdatedAt,UpdatedBy)
                                                  VALUES(@v_Name,@v_BussinessName,@v_PhoneNumber,@v_MobileNumbers,@v_City,@v_Address,@v_IsActive,@v_IsDeleted,@v_CreatedAt,@v_CreatedBy,@v_UpdatedAt,@v_UpdatedBy);";
                var result = await Repository.NonQueryAsync(query, parameters: parameters);
                retVal = result > 0 ? true : false;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }
        public async Task<IEnumerable<BussinessPartnerModel>> GetAllBussinessPartnersAsync()
        {
            var partners = new List<BussinessPartnerModel>();
            try
            {
                string query = string.Empty;
                query = @"SELECT * FROM BussinessPartner Where IsActive = 1 AND IsDeleted = 0";
                var values = await Repository.QueryAsync(query);
                if (values != null)
                {
                    foreach (var value in values)
                    {
                        var partner = new BussinessPartnerModel();
                        partner.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                        partner.Name = value?.GetValueFromDictonary("Name")?.ToString();
                        partner.BussinessName = value?.GetValueFromDictonary("BussinessName")?.ToString();
                        partner.City = value?.GetValueFromDictonary("City")?.ToString();
                        partner.PhoneNumber = value?.GetValueFromDictonary("PhoneNumber")?.ToString();
                        partner.Address = value?.GetValueFromDictonary("Address")?.ToString();
                        partner.CreatedAt = value?.GetValueFromDictonary("CreatedAt")?.ToString()?.ToNullableDateTime();
                        partner.CreatedBy = value?.GetValueFromDictonary("CreatedBy")?.ToString();
                        var mobileNumber = value?.GetValueFromDictonary("MobileNumber")?.ToString();
                        if (!string.IsNullOrEmpty(mobileNumber))
                            partner.MobileNumbers = new List<string>(mobileNumber.Split(','));
                        partners.Add(partner);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return partners;
        }
        public async Task<BussinessPartnerModel> GetBussinessPartnerAsync(int? Id)
        {
            var partner = new BussinessPartnerModel();
            try
            {

                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_Id"] = Id;
                string query = string.Empty;
                query = @"SELECT * FROM BussinessPartner WHERE Id = @v_Id AND IsActive = 1 AND IsDeleted = 0";
                var values = await Repository.QueryAsync(query, parameters: parameters);
                var value = values?.FirstOrDefault();

                partner.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                partner.Name = value?.GetValueFromDictonary("Name")?.ToString();
                partner.BussinessName = value?.GetValueFromDictonary("BussinessName")?.ToString();
                partner.City = value?.GetValueFromDictonary("City")?.ToString();
                partner.PhoneNumber = value?.GetValueFromDictonary("PhoneNumber")?.ToString();
                partner.Address = value?.GetValueFromDictonary("Address")?.ToString();
                var mobileNumber = value?.GetValueFromDictonary("MobileNumber")?.ToString();
                partner.MobileNumbers = new List<string>(mobileNumber.Split(','));
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return partner;
        }
        public async Task<BussinessPartnerModel> GetLastAddedPartner()
        {
            BussinessPartnerModel partner = null;
            try
            {
                partner = new BussinessPartnerModel();
                string query = @"SELECT * FROM  BussinessPartner Order by 1 DESC LIMIT 1";
                var values = await Repository.QueryAsync(query);
                var value = values.FirstOrDefault();
                partner.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                partner.Name = value?.GetValueFromDictonary("Name")?.ToString();
                partner.FullName = value?.GetValueFromDictonary("FullName")?.ToString();
                partner.BussinessName = value?.GetValueFromDictonary("BussinessName")?.ToString();
                partner.CreatedAt = value?.GetValueFromDictonary("CreatedAt")?.ToString()?.ToNullableDateTime();
            }
            catch (Exception ex)
            {

                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return partner;
        }
        public async Task<double> GetPartnerCurrentBalanceAsync(int partnerId)
        {
            double retVal = 0;
            try
            {
                string query = string.Empty;
                await Repository.QueryAsync(query);
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }
        public async Task<bool> RemoveBussinessPartnerAsync(int? Id)
        {
            bool retVal = false;
            try
            {
                string query = string.Empty;
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_Id"] = Id;
                query = @"UPDATE BussinessPartner SET IsActive = 0 AND IsDeleted = 1 WHERE Id = @v_Id";
                var result = await Repository.NonQueryAsync(query, parameters: parameters);
                retVal = result > 0 ? true : false;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }
        public async Task<bool> UpdateBussinessPartnerAsync(BussinessPartnerModel partner)
        {
            bool retVal = false;
            try
            {
                string query = string.Empty;
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                string mobilenumbers = string.Empty;
                parameters["@v_Id"] = partner?.Id;
                parameters["@v_Name"] = partner?.FullName;
                parameters["@v_BussinessName"] = partner?.BussinessName;
                parameters["@v_PhoneNumber"] = partner?.PhoneNumber;
                parameters["@v_Address"] = partner?.Address;
                parameters["@v_City"] = partner?.City;
                parameters["@v_IsActive"] = partner.IsActive = true;
                parameters["@v_IsDeleted"] = partner.IsDeleted = false;
                parameters["@v_CreatedBy"] = partner.CreatedBy == null ? DBNull.Value : (object)partner.CreatedBy;
                parameters["@v_UpdatedAt"] = partner.UpdatedAt == null ? DateTime.Now : partner.UpdatedAt;
                parameters["@v_UpdatedBy"] = partner.UpdatedBy == null ? DBNull.Value : (object)partner.UpdatedBy;
                if (partner?.MobileNumbers?.Count > 0)
                {
                    foreach (var item in partner?.MobileNumbers)
                    {
                        mobilenumbers = mobilenumbers + item + ",";
                    }
                    mobilenumbers = mobilenumbers.Remove(mobilenumbers.Length - 1);
                }
                parameters["@v_MobileNumber"] = mobilenumbers;
                query = @"UPDATE BussinessPartner SET Name = @v_Name, BussinessName = @v_BussinessName,PhoneNumber = @v_PhoneNumber
                        ,City = @v_City,Address = @v_Address,MobileNumber = @MobileNumber,IsActive = @v_IsActive
                        ,IsDeleted = @v_IsDeleted,CreatedBy = @v_CreatedBy,UpdatedAt = @v_UpdatedAt
                        ,UpdatedBy = @v_UpdatedBy Where Id = @v_Id";
                var result = await Repository.NonQueryAsync(query, parameters: parameters);
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
