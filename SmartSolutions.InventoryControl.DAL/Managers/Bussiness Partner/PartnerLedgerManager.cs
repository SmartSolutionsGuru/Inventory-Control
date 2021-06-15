using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using SmartSolutions.InventoryControl.DAL.Models.Inventory;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.BooleanUtils;
using SmartSolutions.Util.DictionaryUtils;
using SmartSolutions.Util.LogUtils;
using SmartSolutions.Util.NumericUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Bussiness_Partner
{
    [Export(typeof(IPartnerLedgerManager)),PartCreationPolicy(CreationPolicy.Shared)]
    public class PartnerLedgerManager : BaseManager, IPartnerLedgerManager
    {
        #region Private Members
        private readonly IRepository Repository;
        private readonly IBussinessPartnerManager _bussinessPartnerManager;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public PartnerLedgerManager(IBussinessPartnerManager bussinessPartnerManager)
        {
            Repository = GetRepository<BussinessPartnerLedgerModel>();
            _bussinessPartnerManager = bussinessPartnerManager;
        }

        #endregion

        #region Public Methods
        public async Task<bool> AddPartnerBalance(BussinessPartnerLedgerModel partnerLedger)
        {
            bool retVal = false;
            try
            {
                if (partnerLedger == null) return false;
                string query = string.Empty;
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_PartnerId"] = partnerLedger.Partner?.Id;
                parameters["@v_InvoiceId"] = partnerLedger.InvoiceId;
                parameters["@v_InvoiceGuid"] = partnerLedger.InvoiceGuid;
                parameters["@v_IsBalancePayable"] = partnerLedger.IsBalancePayable;
                parameters["@v_AmountRecivable"] = partnerLedger.AmountReciveable;
                parameters["@v_AmountPayable"] = partnerLedger.AmountPayable;
                parameters["@v_BalanceAmount"] = partnerLedger.BalanceAmount;
                parameters["@v_Description"] = partnerLedger.Description;
                parameters["@v_IsActive"] = partnerLedger.IsActive = true;
                parameters["@v_IsDeleted"] = partnerLedger.IsDeleted = false;
                parameters["@v_CreatedAt"] = partnerLedger.CreatedAt == null ? DateTime.Now : partnerLedger.CreatedAt;
                parameters["@v_CreatedBy"] = partnerLedger.CreatedBy == null ? DBNull.Value : (object)partnerLedger.CreatedBy;
                parameters["@v_UpdatedAt"] = partnerLedger.UpdatedAt == null ? DBNull.Value : (object)partnerLedger.UpdatedAt;
                parameters["@v_UpdatedBy"] = partnerLedger.UpdatedBy == null ? DBNull.Value : (object)partnerLedger.UpdatedBy;

                query = @"INSERT INTO PartnerLedger(PartnerId,InvoiceId,InvoiceGuid,IsBalancePayable,AmountReciveable,AmountPayable,BalanceAmount,Description,IsActive,IsDeleted,CreatedAt,CreatedBy,UpdatedAt,UpdatedBy)
                                    VALUES(@v_PartnerId,@v_InvoiceId,@v_InvoiceGuid,@v_IsBalancePayable,@v_AmountReciveable,@v_AmountPayable,@v_BalanceAmount,@v_Description,@v_IsActive,@v_IsDeleted,@v_CreatedAt,@v_CreatedBy,@v_UpdatedAt,@v_UpdatedBy);";
               var result =  await Repository.NonQueryAsync(query: query, parameters: parameters);
                retVal = result > 0 ?  true : false;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }

        public async Task<BussinessPartnerLedgerModel> GetPartnerLedgerLastBalance(int partnerId)
        {
            BussinessPartnerLedgerModel retVal = null;
            try
            {
                if (partnerId == 0) return null;
                string query = string.Empty;
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_PartnerId"] = partnerId;
                query = @"SELECT * FROM PartnerLedger WHERE PartnerId = @v_PartnerId Order By 1 DESC LIMIT 1";
                var values = await Repository.QueryAsync(query: query,parameters:parameters);

                if (values != null || values?.Count > 0)
                {
                    var value = values?.FirstOrDefault();
                    var partnerLedger = new BussinessPartnerLedgerModel();
                    partnerLedger.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                    partnerLedger.AmountPayable = Convert.ToDouble(value?.GetValueFromDictonary("AmountPayable")?.ToString()?.ToInt());
                    partnerLedger.AmountReciveable = Convert.ToDouble(value?.GetValueFromDictonary("AmountReciveable")?.ToString()?.ToInt());
                    partnerLedger.BalanceAmount = Convert.ToDouble(value?.GetValueFromDictonary("BalanceAmount")?.ToString()?.ToInt());
                    partnerLedger.IsBalancePayable = value?.GetValueFromDictonary("IsBalancePayable")?.ToString()?.ToNullableBoolean() ?? false;
                    retVal = partnerLedger; 
                }
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
