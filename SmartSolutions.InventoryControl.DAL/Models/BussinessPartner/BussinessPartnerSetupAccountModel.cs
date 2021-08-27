using SmartSolutions.InventoryControl.DAL.Managers.Bussiness_Partner;
using SmartSolutions.Util.LogUtils;
using System;

namespace SmartSolutions.InventoryControl.DAL.Models.BussinessPartner
{
    public class BussinessPartnerSetupAccountModel : BaseModel
    {
        #region Private Members
        private readonly IChartOfAccountManager _chartOfAccountManager;
        #endregion

        #region Constructor
        public BussinessPartnerSetupAccountModel()
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
    }
   
}
