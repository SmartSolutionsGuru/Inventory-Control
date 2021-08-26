namespace SmartSolutions.InventoryControl.DAL.Models.Bank
{
   public class BankBranchModel : BaseModel
    {
        #region Constructor
        public BankBranchModel()
        {
            Bank = new BankModel();
        }
        #endregion

        #region Properties
        public BankModel Bank { get; set; }
        public string Address { get; set; }
        public string BarnchDetails { get; set; }
        public string Description { get; set; }
        public string BussinessPhone { get; set; }
        public string BussinessPhone1 { get; set; }
        public string BussinessPhone2 { get; set; }
        public string MobilePhone1 { get; set; }
        public string MobilePhone { get; set; }
        public string Email { get; set; }
        #endregion
    }
}
