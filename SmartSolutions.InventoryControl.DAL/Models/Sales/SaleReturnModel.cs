using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using System;

namespace SmartSolutions.InventoryControl.DAL.Models.Sales
{
    public class SaleReturnModel : BaseModel
    {
        #region Constructor
        public SaleReturnModel()
        {
            Partner = new BussinessPartnerModel();
            SaleReturnGuid = Guid.NewGuid();
        }
        #endregion

        #region Properties
        public string SaleReturnId { get; set; }
        public Guid SaleReturnGuid { get; set; }
        public BussinessPartnerModel Partner { get; set; }
        public string Description { get; set; }
        public decimal Total { get; set; }
        #endregion
    }
}
