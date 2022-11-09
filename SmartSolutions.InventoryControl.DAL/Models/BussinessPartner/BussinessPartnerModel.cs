using SmartSolutions.InventoryControl.DAL.Models.Region;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartSolutions.InventoryControl.DAL.Models.BussinessPartner
{
    public class BussinessPartnerModel : BaseModel
    {
        #region Constructor
        public BussinessPartnerModel()
        {
            PartnerType = new BussinessPartnerTypeModel();
            PartnerCategory = new BussinessPartnerCategoryModel();
            MobileNumbers = new List<string>();
            City = new CityModel();
        }
        #endregion

        #region Properties
        public BussinessPartnerTypeModel PartnerType { get; set; }
        public BussinessPartnerCategoryModel PartnerCategory { get; set; }
        /// <summary>
        /// Official Business Name
        /// </summary>
        public string BussinessName { get; set; }
        /// <summary>
        /// Full Name of Business Man
        /// </summary>
        public string FullName
        {
            get
            {
                if (!string.IsNullOrEmpty(BussinessName) && !BussinessName.Equals(Name,StringComparison.InvariantCultureIgnoreCase))
                {
                   return  $"{BussinessName} , {Name}".Trim();
                }
                else
                {
                    return $"{Name}".Trim();
                }
            }
            set
            {
                string []temp = value?.Split(new char[] { ',' }, 2);
                BussinessName = temp?.ElementAtOrDefault(0) ?? "";
                Name = temp?.ElementAtOrDefault(1) ?? "";
            }
        }
        /// <summary>
        /// Local Phone Number
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// City of Business
        /// </summary>
        public CityModel City { get; set; }
        /// <summary>
        /// Mobile Numbers
        /// </summary>
        public List<string> MobileNumbers { get; set; }
        /// <summary>
        /// Physical Address
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// WhatsApp Number of Partner
        /// </summary>
        public string WhatsAppNumber { get; set; }

        #endregion
    }
}
