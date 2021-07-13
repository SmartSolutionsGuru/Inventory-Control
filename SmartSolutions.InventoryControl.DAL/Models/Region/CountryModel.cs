using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace SmartSolutions.InventoryControl.DAL.Models.Region
{
    public class CountryModel : BaseModel
    {
        #region Properties
        public string Iso { get; set; }
        public string NiceName { get; set; }
        public string Iso3 { get; set; }
        public int NumCode { get; set; }
        public int PhoneCode { get; set; }
        public string Description { get; set; }
        #endregion
    }
}
