using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;

namespace SmartSolutions.InventoryControl.DAL.Models.Payments
{
    public class PaymentModel : BaseModel
    {
        #region Constructor
        public PaymentModel()
        {
            Partner = new BussinessPartnerModel();
            PaymentRefrencePartner = new BussinessPartnerModel();

        }
        #endregion

        #region Properties
        public BussinessPartnerModel Partner { get; set; }
        /// <summary>
        /// Payment Method Like DebitCard, Cash,JazzCash etc... 
        /// </summary>
        public PaymentTypeModel PaymentMethod { get; set; }
        public PaymentType PaymentType { get; set; }
        public BussinessPartnerModel PaymentRefrencePartner { get; set; }
        public byte[] PaymentImage { get; set; }
        public decimal PaymentAmount { get; set; }
        public bool IsPaymentReceived { get; set; }
        #endregion
    }
  
}
