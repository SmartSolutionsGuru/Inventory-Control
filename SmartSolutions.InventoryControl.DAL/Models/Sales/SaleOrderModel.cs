using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using SmartSolutions.InventoryControl.DAL.Models.Shipping;

namespace SmartSolutions.InventoryControl.DAL.Models.Sales
{
    public class SaleOrderModel: BaseModel
    {
        #region Constructor
        public SaleOrderModel()
        {
            SalePartner = new BussinessPartnerModel();
            Shipping = new ShippingModel();
        }
        #endregion

        #region Properties
        public BussinessPartnerModel SalePartner { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public ShippingModel Shipping { get; set; }
        public double SubTotal { get; set; }
        public double Discount { get; set; }
        public double GrandTotal { get; set; }
        #endregion
    }
}
