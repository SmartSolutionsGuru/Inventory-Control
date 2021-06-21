using System.Linq;
using System.Security.Principal;

namespace SmartBooks.Repo.Identity
{
    public class CustomPrinciple : IPrincipal
    {
        #region Properties
        private CustomIdentity _identity;
        public CustomIdentity Identity
        {
            get { return _identity ?? new AnonymousIdentity(); }
            set { _identity = value; }
        }
        #endregion

        #region IPrincipal Members
        IIdentity IPrincipal.Identity
        {
            get { return this.Identity; }
        }

        public bool IsInRole(string role)
        {
            return _identity.Roles.Select(x => x.Name == role).FirstOrDefault();//.Contains(role);
        }
        #endregion
    }
}
