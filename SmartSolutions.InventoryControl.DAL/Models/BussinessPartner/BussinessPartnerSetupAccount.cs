using SmartSolutions.Util.LogUtils;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSolutions.InventoryControl.DAL.Models.BussinessPartner
{
    public class BussinessPartnerSetupAccount : BaseModel
    {
        #region Constructor
        public BussinessPartnerSetupAccount()
        {
            Partner = new BussinessPartnerModel();
            PartnerAccountType = new BussinessPartnerTypeModel();
        }
        #endregion

        #region Properties
        public BussinessPartnerModel Partner { get; set; }
        public string PartnerAccountCode { get; set; }
        public BussinessPartnerTypeModel PartnerAccountType { get; set; }
        #endregion

        #region Private Helepers
        /// <summary>
        /// Private Helper Method that Will 
        /// Genrate Account Code According To Specification
        /// </summary>
        /// <returns></returns>
        private string CreatePartnerAccountCode()
        {
            string accountCode = string.Empty;
            try
            {

            }
            catch (Exception ex)
            {
               LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return accountCode;
        }
        #endregion
    }
    public class ChartOfAccounts
    {

    }
}
