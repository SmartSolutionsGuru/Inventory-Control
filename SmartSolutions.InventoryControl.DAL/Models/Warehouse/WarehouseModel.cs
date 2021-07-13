using SmartSolutions.InventoryControl.DAL.Models.Region;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSolutions.InventoryControl.DAL.Models.Warehouse
{
    public class WarehouseModel :BaseModel
    {
        #region Constructor
        public WarehouseModel()
        {

        }
        #endregion

        #region Properties
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public CountryModel Country { get; set; }
        public CityModel City { get; set; }
        public string Address { get; set; }

        #endregion
    }
}
