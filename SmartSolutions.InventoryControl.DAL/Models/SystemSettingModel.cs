namespace SmartSolutions.InventoryControl.DAL.Models
{
    public class SystemSettingModel :BaseModel
    {
        public string SettingKey { get; set; }
        public int SettingValue { get; set; }
        public bool DefaultValue { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
    }
}
