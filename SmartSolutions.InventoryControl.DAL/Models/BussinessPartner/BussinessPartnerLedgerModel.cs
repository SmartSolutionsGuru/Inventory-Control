using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSolutions.InventoryControl.DAL.Models.BussinessPartner
{
    public class BussinessPartnerLedgerModel : BaseModel
    {
        #region Constructor
        public BussinessPartnerLedgerModel()
        {
            Partner = new BussinessPartnerModel();
        }
        #endregion

        #region Properties
        public BussinessPartnerModel Partner { get; set; }
        public string InvoiceId { get; set; }
        public Guid InvoiceGuid { get; set; }
        public bool IsBalancePayable { get; set; }
        public double AmountReciveable { get; set; }
        public double AmountPayable { get; set; }
        public double BalanceAmount { get; set; }
        public string Description { get; set; }
        #endregion
    }
}
