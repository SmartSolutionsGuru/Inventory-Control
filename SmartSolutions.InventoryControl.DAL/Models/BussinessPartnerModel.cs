using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSolutions.InventoryControl.DAL.Models
{
    public class BussinessPartnerModel : BaseModel
    {
        /// <summary>
        /// Offical Bussiness Name
        /// </summary>
        public string BussinessName { get; set; }
        /// <summary>
        /// Full Name of Bussiness Man
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// Local Phone Number
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// City of Bussiness
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// Mobile Numbers
        /// </summary>
        public List<string> MobileNumbers { get; set; }
        /// <summary>
        /// Phisical Address
        /// </summary>
        public string Address { get; set; }

    }
}
