namespace SmartSolutions.InventoryControl.DAL.Models.Payments
{
    public class PaymentTypeModel : BaseModel
    {
        #region Constructor
        public PaymentTypeModel()
        {

        }
        #endregion

        #region Properties
        public string PaymentType { get; set; }
        public string Description { get; set; }
        #endregion
    }
}
