using SmartSolutions.InventoryControl.DAL.Models.Authentication;

namespace SmartBooks.Repo.Identity
{
    /// <summary>
    /// Interface that use to authnticate User
    /// </summary>
    public interface IAuthenticationService
    {
        IdentityUserModel AuthenticateUser(string username, string password);
    }
}
