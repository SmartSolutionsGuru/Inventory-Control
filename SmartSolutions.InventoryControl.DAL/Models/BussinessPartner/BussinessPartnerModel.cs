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
            City = new CityModel();
        }
        #endregion

        #region Properties
        public BussinessPartnerTypeModel PartnerType { get; set; }
        public BussinessPartnerCategoryModel PartnerCategory { get; set; }
        /// <summary>
        /// Offical Bussiness Name
        /// </summary>
        public string BussinessName { get; set; }
        /// <summary>
        /// Full Name of Bussiness Man
        /// </summary>
        public string FullName
        {
            get
            {
                if (!string.IsNullOrEmpty(BussinessName))
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
        /// City of Bussiness
        /// </summary>
        public CityModel City { get; set; }
        /// <summary>
        /// Mobile Numbers
        /// </summary>
        public List<string> MobileNumbers { get; set; }
        /// <summary>
        /// Phisical Address
        /// </summary>
        public string Address { get; set; }

        #endregion
    }
}
