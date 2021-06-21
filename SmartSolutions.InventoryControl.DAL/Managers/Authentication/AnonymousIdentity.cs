using SmartSolutions.InventoryControl.DAL.Models.Authentication;
using System.Collections.Generic;

namespace SmartBooks.Repo.Identity
{
    public class AnonymousIdentity : CustomIdentity
    {
        #region Default Constructor

        public AnonymousIdentity()
            : base(string.Empty, string.Empty,new List<UserRoleModel>())
        { }
    }
        #endregion
}
