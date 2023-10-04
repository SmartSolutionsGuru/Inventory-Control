using SmartSolutions.InventoryControl.DAL.Managers.Region;
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
        private readonly ICityManager _cityManager;
        private readonly IPartnerTypeManager _partnerTypeManager;
        private readonly IPartnerCategoryManager _partnerCategoryManager;
        #endregion

        #region Costructor
        [ImportingConstructor]
        public BussinessPartnerManager(ICityManager cityManager
                                        , IPartnerTypeManager partnerTypeManager
                                        , IPartnerCategoryManager partnerCategoryManager)
        {
            Repository = GetRepository<BussinessPartnerModel>();
            _cityManager = cityManager;
            _partnerTypeManager = partnerTypeManager;
            _partnerCategoryManager = partnerCategoryManager;
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
                parameters["@v_WhatsAppNumber"] = partner?.WhatsAppNumber == null ? DBNull.Value:(object)partner?.WhatsAppNumber;
                string query = string.Empty;
                query = @"INSERT INTO BussinessPartner (PartnerTypeId,PartnerCategoryId,Name,BussinessName,PhoneNumber,MobileNumber,CityId,Address,IsActive,CreatedAt,CreatedBy,UpdatedAt,UpdatedBy,WhatsAppNumber)
                                                  VALUES(@v_PartnerTypeId,@v_PartnerCategoryId,@v_Name,@v_BussinessName,@v_PhoneNumber,@v_MobileNumbers,@v_CityId,@v_Address,@v_IsActive,@v_CreatedAt,@v_CreatedBy,@v_UpdatedAt,@v_UpdatedBy,@v_WhatsAppNumber);";
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
        public async Task<bool> IsPartnerAlreadyExist(string bussinessName, string mobileNumnber)
        {
            bool retVal = false;    
            try
            {
                //null guard
                if(string.IsNullOrEmpty(bussinessName) || string.IsNullOrEmpty(mobileNumnber)) return false; 
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_bussinessName"] = bussinessName;
                parameters["@v_mobileNumber"] = mobileNumnber;
                //
                string query = @"SELECT * FROM BussinessPartner WHERE BussinessName LIKE @v_bussinessName AND WhatsAppNumber LIKE @v_mobileNumber";
                var values = await Repository.QueryAsync(query, parameters: parameters);
                if(values != null)
                    retVal = true;
                else
                    retVal = false;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }
        public async Task<IEnumerable<BussinessPartnerModel>> GetAllBussinessPartnersAsync(string search = null)
        {
            var partners = new List<BussinessPartnerModel>();
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_searchText"] = search == null ? search = string.Empty : search;
                string query = string.Empty;
                //query = @"SELECT * FROM BussinessPartner Where Name LIKE @v_searchText + '%' AND IsActive = 1";
                //query = @"SELECT * FROM BussinessPartner Where Name LIKE @v_searchText + '%' AND IsActive = 1";
                query = @"SELECT * FROM BussinessPartner Where BussinessName LIKE @v_searchText + '%' AND IsActive = 1";
                var values = await Repository.QueryAsync(query, parameters: parameters);
                if (values != null)
                {
                    foreach (var value in values)
                    {
                        var partner = new BussinessPartnerModel();
                        partner.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                        partner.Name = value?.GetValueFromDictonary("Name")?.ToString();
                        partner.PartnerType = new BussinessPartnerTypeModel { Id = value?.GetValueFromDictonary("PartnerTypeId")?.ToString()?.ToInt() };
                        partner.PartnerType = await _partnerTypeManager.GetPartnerTypeByIdAsync(partner?.PartnerType?.Id);
                        partner.PartnerCategory = new BussinessPartnerCategoryModel { Id = value?.GetValueFromDictonary("PartnerCategoryId")?.ToString()?.ToInt() };
                        partner.PartnerCategory = await _partnerCategoryManager.GetPartnerCategoryByIdAsync(partner?.PartnerCategory?.Id);
                        partner.BussinessName = value?.GetValueFromDictonary("BussinessName")?.ToString();
                        partner.City = new Models.Region.CityModel
                        {
                            Id = value?.GetValueFromDictonary("CityId")?.ToString().ToNullableInt(),
                        };
                        partner.City = await _cityManager.GetCityFromIdAsync(partner.City?.Id);
                        partner.PhoneNumber = value?.GetValueFromDictonary("PhoneNumber")?.ToString();
                        partner.Address = value?.GetValueFromDictonary("Address")?.ToString();
                        var mobileNumber = value?.GetValueFromDictonary("MobileNumber")?.ToString();
                        if (!string.IsNullOrEmpty(mobileNumber))
                            partner.MobileNumbers = new List<string>(mobileNumber.Split(','));
                        //partner.CreatedAt = value?.GetValueFromDictonary("CreatedAt")?.ToString()?.ToNullableDateTime();
                        partner.CreatedBy = value?.GetValueFromDictonary("CreatedBy")?.ToString();
                        partners.Add(partner);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return partners.OrderBy(p=>p.BussinessName);
        }

        public async Task<IEnumerable<BussinessPartnerLedgerModel>> GetAllBussinessPartnersWithBalanceAsync()
        {
            var bussinessPartners = new List<BussinessPartnerLedgerModel>();
            try
            {
                //string query = @"SELECT Name,BussinessName,PartnerTypeId,PhoneNumber,MobileNumber,CityId,CurrentBalance,CurrentBalanceType FROM dbo.BussinessPartner bp
                //                INNER JOIN PartnerLedgerAccounts pl ON bp.Id = pl.PartnerId";
                string query = @"SELECT Name,BussinessName,PartnerTypeId,PhoneNumber,MobileNumber,CityId,
	                            pl.CurrentBalance FROM dbo.BussinessPartner bp
	                            INNER JOIN (SELECT PartnerId, (SUM(DR) - SUM(CR)) as CurrentBalance FROM PartnerLedgerAccounts 
                                Group BY PartnerLedgerAccounts.PartnerId)pl
                                ON bp.Id = pl.PartnerId";
                var values = await Repository.QueryAsync(query);
                if (values != null || values?.Count > 0)
                {
                    foreach (var value in values)
                    {
                        var partnerLedger = new BussinessPartnerLedgerModel();
                        partnerLedger.Partner = new BussinessPartnerModel();

                        partnerLedger.Partner.Name = value?.GetValueFromDictonary("Name").ToString();
                        partnerLedger.Partner.BussinessName = value?.GetValueFromDictonary("BussinessName")?.ToString();
                        partnerLedger.Partner.PhoneNumber = value?.GetValueFromDictonary("PhoneNumber").ToString();
                        partnerLedger.CurrentBalance = Convert.ToDecimal(value?.GetValueFromDictonary("CurrentBalance").ToString());
                        if (partnerLedger.CurrentBalance < 0)
                            partnerLedger.CurrentBalanceType = PaymentType.Payable;
                        else
                            partnerLedger.CurrentBalanceType= PaymentType.Receivable;
                        //partnerLedger.CurrentBalanceType = value?.GetValueFromDictonary("CurrentBalanceType").ToString().ToEnum<PaymentType>() ?? PaymentType.None;
                        var mobileNumber = value?.GetValueFromDictonary("MobileNumber")?.ToString();
                        if (!string.IsNullOrEmpty(mobileNumber))
                            partnerLedger.Partner.MobileNumbers = new List<string>(mobileNumber.Split(','));
                        int? partnerTypeId = value?.GetValueFromDictonary("PartnerTypeId")?.ToString()?.ToInt();
                        partnerLedger.Partner.PartnerType = await _partnerTypeManager.GetPartnerTypeByIdAsync(partnerTypeId);
                        int? cityId =  value?.GetValueFromDictonary("CityId").ToString()?.ToInt();
                        partnerLedger.Partner.City = await _cityManager.GetCityFromIdAsync(cityId);
                        bussinessPartners.Add(partnerLedger);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return bussinessPartners;
        }

        public async Task<BussinessPartnerModel> GetBussinessPartnerByIdAsync(int? Id)
        {
            if (Id == null || Id == 0) return null;
            var partner = new BussinessPartnerModel();
            try
            {

                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_Id"] = Id;
                string query = string.Empty;
                query = @"SELECT * FROM BussinessPartner WHERE Id = @v_Id AND IsActive = 1";
                var values = await Repository.QueryAsync(query, parameters: parameters);
                if(values != null && values?.Count > 0) 
                {
                    var value = values?.FirstOrDefault();
                    partner.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                    partner.Name = value?.GetValueFromDictonary("Name")?.ToString();
                    partner.BussinessName = value?.GetValueFromDictonary("BussinessName")?.ToString();
                    partner.City = new Models.Region.CityModel { Id = value?.GetValueFromDictonary("City")?.ToString().ToNullableInt() };
                    partner.PhoneNumber = value?.GetValueFromDictonary("PhoneNumber")?.ToString();
                    partner.Address = value?.GetValueFromDictonary("Address")?.ToString();
                    var mobileNumber = value?.GetValueFromDictonary("MobileNumber")?.ToString();
                    if (mobileNumber != null)
                    {
                        partner.MobileNumbers = new List<string>(mobileNumber.Split(','));
                    }
                }
                   
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return partner;
        }

        public async Task<IEnumerable<BussinessPartnerModel>> GetBussinessPartnersByTypeAsync(List<int?> typeId)
        {
            if (typeId == null || typeId?.Count == 0) return null;
            List<BussinessPartnerModel> partners = new List<BussinessPartnerModel>();
            try
            {
                var Id = string.Empty;
                foreach (var item in typeId)
                {
                    Id = item.ToString();
                    Dictionary<string, object> parameters = new Dictionary<string, object>();
                    parameters["@v_Id"] = Id;
                    string query = @"SELECT * FROM BussinessPartner WHERE PartnerTypeId = @v_Id";
                    var values = await Repository.QueryAsync(query, parameters: parameters);
                    if (values != null || values?.Count > 0)
                    {
                        foreach (var value in values)
                        {
                            var partner = new BussinessPartnerModel();
                            partner.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                            partner.Name = value?.GetValueFromDictonary("Name")?.ToString();
                            partner.PartnerType = new BussinessPartnerTypeModel { Id = value?.GetValueFromDictonary("PartnerTypeId")?.ToString()?.ToInt() };
                            partner.PartnerType = await _partnerTypeManager.GetPartnerTypeByIdAsync(partner?.PartnerType?.Id);
                            partner.PartnerCategory = new BussinessPartnerCategoryModel { Id = value?.GetValueFromDictonary("PartnerCategoryId")?.ToString()?.ToInt() };
                            partner.PartnerCategory = await _partnerCategoryManager.GetPartnerCategoryByIdAsync(partner?.PartnerCategory?.Id);
                            partner.BussinessName = value?.GetValueFromDictonary("BussinessName")?.ToString();
                            partner.City = new Models.Region.CityModel
                            {
                                Id = value?.GetValueFromDictonary("CityId")?.ToString().ToNullableInt(),
                            };
                            partner.City = await _cityManager.GetCityFromIdAsync(partner.City?.Id);
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
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return partners;
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
                parameters["@v_CityId"] = partner?.City?.Id;
                parameters["@v_IsActive"] = partner.IsActive = true;
                parameters["@v_CreatedBy"] = partner.CreatedBy == null ? DBNull.Value : (object)partner.CreatedBy;
                parameters["@v_UpdatedAt"] = partner.UpdatedAt = DateTime.Now;
                parameters["@v_UpdatedBy"] = partner.UpdatedBy == null ? partner.CreatedBy : partner.UpdatedBy;
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
                        ,CityID = @v_CityId,Address = @v_Address,MobileNumber = @v_MobileNumber,IsActive = @v_IsActive
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
