namespace SmartSolutions.InventoryControl.DAL.Models.Region
{
    public class CityModel : BaseModel
    {
        #region Constructor
        public CityModel()
        {
            Country = new CountryModel();
            Province = new ProvinceModel();
        }
        #endregion
        #region Properties
        public CountryModel Country { get; set; }
        public ProvinceModel Province { get; set; }
        public int PhoneCode { get; set; }
        public string Description { get; set; }
        #endregion
    }
}
