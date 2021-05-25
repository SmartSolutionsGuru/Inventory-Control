using SmartSolutions.InventoryControl.DAL.Models;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.LogUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
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
        public async Task<bool> AddBussinesPartner(BussinessPartnerModel partner)
        {
            bool retVal = false;
            try
            {
                string query = string.Empty;

                await Repository.NonQueryAsync(query);
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }
        public async Task<IEnumerable<BussinessPartnerModel>> GetAllBussinessPartners()
        {
            var partners = new List<BussinessPartnerModel>();
            try
            {
                string query = string.Empty;
                await Repository.QueryAsync(query);
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return partners;
        }
        public async Task<BussinessPartnerModel> GetBussinessPartner(int? Id)
        {
            var partner = new BussinessPartnerModel();
            try
            {
                string query = string.Empty;
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_Id"] = Id;
                await Repository.QueryAsync(query, parameters: parameters);
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return partner;
        }
        public async Task<bool> RemoveBussinessPartner(int? Id)
        {
            bool retVal = false;
            try
            {
                string query = string.Empty;
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_Id"] = Id;
                var result = await Repository.NonQueryAsync(query, parameters: parameters);
                retVal = result > 0 ? true : false;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }
        public async Task<bool> UpdateBussinessPartner(int? Id)
        {
            bool retVal = false;
            try
            {
                string query = string.Empty;
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_Id"] = Id;
                var result = await Repository.NonQueryAsync(query,parameters:parameters);
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
