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
        public Guid TransactionGuid { get; set;}
        /// <summary>
        ///  flag for Balnce Type
        ///  1) true means Bussiness Partner  balance is Credit 
        ///  2) false Bussiness Partner Balnce is Debit
        /// </summary>
        public bool IsBalancePayable { get; set; }
        public bool IsAmountReceived { get; set; }
        public bool IsAmountPaid { get; set; }
        public double AmountReciveable { get; set; }
        public double AmountPayable { get; set; }
        public double BalanceAmount { get; set; }
        public string Description { get; set; }
        #endregion
    }
}
