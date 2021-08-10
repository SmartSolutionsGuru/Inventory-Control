using SmartSolutions.InventoryControl.DAL.Models;
using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.BooleanUtils;
using SmartSolutions.Util.DateAndTimeUtils;
using SmartSolutions.Util.DictionaryUtils;
using SmartSolutions.Util.EnumUtils;
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

        #region ADD
        public async Task<bool> AddBussinesPartnerAsync(BussinessPartnerModel partner)
        {
            bool retVal = false;
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                string mobilenumbers = string.Empty;
                parameters["@v_PartnerTypeId"] = partner?.PartnerType?.Id;
                parameters["@v_PartnerCategoryId"] = partner?.PartnerCategory?.Id;
                parameters["@v_Name"] = partner?.Name;
                parameters["@v_BussinessName"] = partner?.BussinessName;
                parameters["@v_PhoneNumber"] = partner?.PhoneNumber;
                if (partner?.MobileNumbers?.Count > 0)
                {
                    foreach (var item in partner?.MobileNumbers)
                    {
                        mobilenumbers = mobilenumbers + item + ",";
                    }
                    mobilenumbers = mobilenumbers.Remove(mobilenumbers.Length - 1);
                }
                parameters["@v_MobileNumbers"] = mobilenumbers;
                parameters["@v_Address"] = partner?.Address == null ? DBNull.Value : (object)partner.Address;
                parameters["@v_CityId"] = partner?.City?.Id;
                parameters["@v_IsActive"] = partner.IsActive = true;
                parameters["@v_CreatedAt"] = partner.CreatedAt == null ? DateTime.Now : partner.CreatedAt;
                parameters["@v_CreatedBy"] = partner.CreatedBy == null ? DBNull.Value : (object)partner.CreatedBy;
                parameters["@v_UpdatedAt"] = partner.UpdatedAt == null ? DBNull.Value : (object)partner.UpdatedAt;
                parameters["@v_UpdatedBy"] = partner.UpdatedBy == null ? DBNull.Value : (object)partner.UpdatedBy;
                string query = string.Empty;
                query = @"INSERT INTO BussinessPartner (PartnerTypeId,PartnerCategoryId,Name,BussinessName,PhoneNumber,MobileNumber,CityId,Address,IsActive,CreatedAt,CreatedBy,UpdatedAt,UpdatedBy)
                                                  VALUES(@v_PartnerTypeId,@v_PartnerCategoryId,@v_Name,@v_BussinessName,@v_PhoneNumber,@v_MobileNumbers,@v_CityId,@v_Address,@v_IsActive,@v_CreatedAt,@v_CreatedBy,@v_UpdatedAt,@v_UpdatedBy);";
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

        #region GET
        public async Task<IEnumerable<BussinessPartnerModel>> GetAllBussinessPartnersAsync()
        {
            var partners = new List<BussinessPartnerModel>();
            try
            {
                string query = string.Empty;
                query = @"SELECT * FROM BussinessPartner Where IsActive = 1";
                var values = await Repository.QueryAsync(query);
                if (values != null)
                {
                    foreach (var value in values)
                    {
                        var partner = new BussinessPartnerModel();
                        partner.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                        partner.Name = value?.GetValueFromDictonary("Name")?.ToString();
                        partner.BussinessName = value?.GetValueFromDictonary("BussinessName")?.ToString();
                        partner.City = new Models.Region.CityModel
                        {
                            Id = value?.GetValueFromDictonary("CityId")?.ToString().ToNullableInt(),
                        };
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
                query = @"SELECT * FROM BussinessPartner WHERE Id = @v_Id AND IsActive = 1";
                var values = await Repository.QueryAsync(query, parameters: parameters);
                var value = values?.FirstOrDefault();

                partner.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                partner.Name = value?.GetValueFromDictonary("Name")?.ToString();
                partner.BussinessName = value?.GetValueFromDictonary("BussinessName")?.ToString();
                partner.City = new Models.Region.CityModel { Id = value?.GetValueFromDictonary("City")?.ToString().ToNullableInt() };
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
                string query = string.Empty;
                partner = new BussinessPartnerModel();
                if (Repository.Type == DBTypes.SQLITE)
                    query = @"SELECT * FROM  BussinessPartner Order by 1 DESC LIMIT 1";
                else if (Repository.Type == DBTypes.SQLServer)
                    query = @"SELECT * FROM BussinessPartner WHERE Id = (SELECT MAX(Id) FROM BussinessPartner);";

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
        /// <summary>
        /// Method That Will return the Current balance of Partner 
        /// </summary>
        /// <param name="partnerId"></param>
        /// <returns></returns>
        public async Task<BussinessPartnerLedgerModel> GetPartnerCurrentBalanceAsync(int partnerId)
        {
            BussinessPartnerLedgerModel partnerLedger = new BussinessPartnerLedgerModel();
            try
            {
                if (partnerId > 0)
                {
                    string query = string.Empty;
                    Dictionary<string, object> parameters = new Dictionary<string, object>();
                    parameters["@v_PartnerId"] = partnerId;
                    query = @"SELECT * FROM PartnerLedger WHERE PartnerId = @v_PartnerId AND IsActive =1  ORDER BY 1 DESC LIMIT 1";
                    var values = await Repository.QueryAsync(query, parameters: parameters);
                    if (values != null)
                    {
                        var value = values.FirstOrDefault();
                        partnerLedger.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                        partnerLedger.Partner.Id = value?.GetValueFromDictonary("PartnerId")?.ToString()?.ToInt();
                        partnerLedger.CurrentBalance = value?.GetValueFromDictonary("CurrentBalance")?.ToString()?.ToNullableInt() ?? 0;
                        partnerLedger.IsActive = value?.GetValueFromDictonary("IsActive")?.ToString()?.ToNullableBoolean() ?? false;
                        partnerLedger.CurrentBalanceType = value?.GetValueFromDictonary("CurrentBalanceType")?.ToString()?.ToEnum<PaymentType>() ?? PaymentType.None;
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return partnerLedger;
        }
        #endregion

        #region REMOVE
        public async Task<bool> RemoveBussinessPartnerAsync(int? Id)
        {
            bool retVal = false;
            try
            {
                string query = string.Empty;
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_Id"] = Id;
                query = @"UPDATE BussinessPartner SET IsActive = 0  WHERE Id = @v_Id";
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

        #region UPDATE
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
                        ,CreatedBy = @v_CreatedBy,UpdatedAt = @v_UpdatedAt
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
        #endregion
    }
}
