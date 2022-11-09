using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using SmartSolutions.InventoryControl.DAL.Models.Region;

namespace SmartSolutions.InventoryControl.DAL.Models
{
    /// <summary>
    /// Model For Properiter Information
    /// </summary>
    public class ProprietorInformationModel : BaseModel
    {
        #region [Constructor]
        /// <summary>
        /// Default Constructor
        /// </summary>
        public ProprietorInformationModel()
        {
            City = new CityModel();
            BussinessCategory = new BussinessPartnerCategoryModel();
            BussinessType = new BussinessPartnerTypeModel();
        }
        #endregion

        #region [Properties]
        /// <summary>
        /// gets or sets properiter Name
        /// </summary>
        public string ProprietorName { get; set; }
        /// <summary>
        /// get or sets propriter bussiness Name
        /// </summary>
        public string BussinessName { get; set; }
        /// <summary>
        /// get or sets city Information
        /// </summary>
        public CityModel City { get; set; }
        /// <summary>
        /// get or sets Bussiness Partner Information
        /// </summary>
        public BussinessPartnerTypeModel BussinessType { get; set; }
        /// <summary>
        /// get or sets bussiness parter category
        /// </summary>
        public BussinessPartnerCategoryModel BussinessCategory { get; set; }
        /// <summary>
        /// get or sets Primary LandLine Number
        /// </summary>
        public string LandLineNumber { get; set; }
        /// <summary>
        /// get or sets Secondary Land Line Number
        /// </summary>
        public string LandLineNumber1 { get; set; }
        /// <summary>
        /// get or sets Secondary Land Line
        /// </summary>
        public string MobileNumber { get; set; }
        /// <summary>
        /// get or sets Primary Mobile Number
        /// </summary>
        public string MobileNumber1 { get; set; }
        /// <summary>
        /// get or sets Secondary Mobile Number
        /// </summary>
        public string  WhatsAppNumber { get; set; }
        /// <summary>
        /// get or sets Emergency Number
        /// </summary>
        public string EmergenceyNumber { get; set; }
        /// <summary>
        /// get or sets bussiness Address
        /// </summary>
        public string BussinessAddress { get; set; }
        /// <summary>
        /// get or sets home Address 
        /// </summary>
        public string HomeAddress { get; set; }
        /// <summary>
        /// get or sets Description
        /// </summary>
        public string Description { get; set; }
        #endregion
    }
}
