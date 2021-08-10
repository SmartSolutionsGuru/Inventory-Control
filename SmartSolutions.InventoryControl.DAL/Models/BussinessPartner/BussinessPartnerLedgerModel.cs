using SmartSolutions.InventoryControl.DAL.Models.Payments;

namespace SmartSolutions.InventoryControl.DAL.Models.BussinessPartner
{
    public class BussinessPartnerLedgerModel : BaseModel
    {
        #region Constructor
        public BussinessPartnerLedgerModel()
        {
            Partner = new BussinessPartnerModel();
            Payment = new PaymentModel();
            CurrentBalanceType = PaymentType.None;
        }
        #endregion

        #region Properties
        public BussinessPartnerModel Partner { get; set; }
        public PaymentModel Payment { get; set; }
        public decimal CurrentBalance { get; set; }
        public PaymentType CurrentBalanceType { get; set; }
        public string Description { get; set; }
       
        #endregion
    }
  
}
