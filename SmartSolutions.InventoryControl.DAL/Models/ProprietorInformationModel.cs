using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using SmartSolutions.InventoryControl.DAL.Models.Region;

namespace SmartSolutions.InventoryControl.DAL.Models
{
    public class ProprietorInformationModel : BaseModel
    {
        #region Constructor
        public ProprietorInformationModel()
        {
            City = new CityModel();
            BussinessCategory = new BussinessPartnerCategoryModel();
            BussinessType = new BussinessPartnerTypeModel();
        }
        #endregion

        #region Properties
        public string ProprietorName { get; set; }
        public string BussinessName { get; set; }
        public CityModel City { get; set; }
        public BussinessPartnerTypeModel BussinessType { get; set; }
        public BussinessPartnerCategoryModel BussinessCategory { get; set; }
        public string LandLineNumber { get; set; }
        public string LandLineNumber1 { get; set; }
        public string MobileNumber { get; set; }
        public string MobileNumber1 { get; set; }
        public string  WhatsAppNumber { get; set; }   
        public string EmergenceyNumber { get; set; }
        public string BussinessAddress { get; set; }
        public string HomeAddress { get; set; }
        public string Description { get; set; }
        #endregion
    }
}
