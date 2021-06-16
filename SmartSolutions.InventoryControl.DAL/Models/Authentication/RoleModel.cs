using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSolutions.InventoryControl.DAL.Models.Authentication
{
    public class RoleModel :BaseModel
    {
        public string ShotName { get; set; }
        public DateTime? ConcurrencyStamp { get; set; }
        public List<IdentityUserModel> Users { get; set; }
    }
}
