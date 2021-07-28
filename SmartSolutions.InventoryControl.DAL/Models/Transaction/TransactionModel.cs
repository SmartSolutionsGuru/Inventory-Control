using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using System;

namespace SmartSolutions.InventoryControl.DAL.Models.Transaction
{
    public class TransactionModel : BaseModel
    {
        #region constructor
        public TransactionModel()
        {
            TransactionGuid = Guid.NewGuid();
        }
        #endregion

        #region Properties
        public Guid TransactionGuid { get; set; }
        public BussinessPartnerModel BussinessPartner { get; set; }
        public BussinessPartnerLedgerModel PartnerAccount { get; set; }
        //public InvoiceModel  PartnerLastInvoice { get; set; }
        /// <summary>
        /// Is It Recivable or Payable
        /// </summary>
        public string PaymentMode { get; set; }
        public Payments.PaymentTypeModel PaymentType { get; set; }
        public byte[] PaymentImage { get; set; }

        #endregion
    }
}
