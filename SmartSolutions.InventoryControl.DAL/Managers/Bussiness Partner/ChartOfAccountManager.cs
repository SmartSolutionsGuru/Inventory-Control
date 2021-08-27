using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.DictionaryUtils;
using SmartSolutions.Util.EnumUtils;
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
    [Export(typeof(IChartOfAccountManager)),PartCreationPolicy(CreationPolicy.Shared)]
    public class ChartOfAccountManager : BaseManager, IChartOfAccountManager
    {
        #region Private Members
        private readonly IRepository Repository;
        #endregion

        #region Constructor
        public ChartOfAccountManager()
        {
            Repository = GetRepository<ChartOfAccountModel>();
        }
        #endregion

        #region Public Methods
        public async Task<IEnumerable<ChartOfAccountModel>> GetAllChatOfAccountsAsync()
        {
            var retVal = new List<ChartOfAccountModel>();
            try
            {
                string query = "SELECT * FROM ChartOfAccount WHERE IsActive = 1";
                var values = await Repository.QueryAsync(query);
                if(values != null || values?.Count > 0)
                {
                    foreach (var value in values)
                    {
                        var accountChart = new ChartOfAccountModel();
                        accountChart.AccountCategory = value?.GetValueFromDictonary("AccountCategory")?.ToString();
                        accountChart.AccountSubCategory = value?.GetValueFromDictonary("AccountSubCategory")?.ToString();
                        accountChart.AccountHeading =(AccountHeading)value?.GetValueFromDictonary("AccountHeading")?.ToString().ToEnum<AccountHeading>();
                        accountChart.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                        retVal.Add(accountChart);
                    }
                }
            }
            catch (Exception ex)
            {

                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }

        public async Task<IEnumerable<ChartOfAccountModel>> GetChartOfAccountByCategoryAsync(string category)
        {
            if (string.IsNullOrEmpty(category)) return null;
            var retVal = new List<ChartOfAccountModel>();
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_Category"] = category;
                string query = @"SELECT * FROM ChartOfAccount WHERE AccountSubCategory = @v_Category";
                var values = await Repository.QueryAsync(query,parameters:parameters);
                if(values != null || values?.Count > 0)
                {
                    foreach (var value in values)
                    {
                        var accountChart = new ChartOfAccountModel();
                        accountChart.AccountCategory = value?.GetValueFromDictonary("AccountCategory")?.ToString();
                        accountChart.AccountSubCategory = value?.GetValueFromDictonary("AccountSubCategory")?.ToString();
                        accountChart.AccountHeading = (AccountHeading)value?.GetValueFromDictonary("AccountHeading")?.ToString().ToEnum<AccountHeading>();
                        accountChart.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                        retVal.Add(accountChart);
                    }
                }

            }
            catch (Exception ex)
            {

                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }

        public async Task<ChartOfAccountModel> GetChartOfAccountByHeadingAsync(string heading)
        {
            if (string.IsNullOrEmpty(heading)) return null;
            var retVal = new ChartOfAccountModel();
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_Heading"] = heading;
                string query = @"SELECT * FROM ChartOfAccount WHERE AccountHeading = @v_Category";
                var values = await Repository.QueryAsync(query, parameters: parameters);
                if(values != null || values?.Count > 0)
                {
                    var value = values?.FirstOrDefault();
                    retVal.AccountCategory = value?.GetValueFromDictonary("AccountCategory")?.ToString();
                    retVal.AccountSubCategory = value?.GetValueFromDictonary("AccountSubCategory")?.ToString();
                    retVal.AccountHeading = (AccountHeading)value?.GetValueFromDictonary("AccountHeading")?.ToString().ToEnum<AccountHeading>();
                    retVal.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
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
