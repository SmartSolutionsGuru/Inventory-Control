namespace SmartSolutions.InventoryControl.DAL.Models.Authentication
{
    public class UserRoleModel:BaseModel
    {
        #region Properties
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string Description { get; set; }
        #endregion
    }
}
