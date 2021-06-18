using SmartSolutions.InventoryControl.DAL.Models.Authentication;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Authentication
{
    public interface IAuthenticationManager
    {
        #region GET
        Task<IEnumerable<IdentityUserModel>> GetAllUserAsync();
        IEnumerable<IdentityUserModel> GetAllUser();
        /// <summary>
        /// Get All Roles of Specific User 
        /// Administrator Simple user etc..
        /// </summary>
        /// <param name="Id"> Id of Given User</param>
        /// <returns>Roles of Given User</returns>
        IEnumerable<UserRoleModel> GetUserAllRole(int Id);
        /// <summary>
        /// Get All Roles of Specific User 
        /// Administrator Simple user etc..
        /// </summary>
        /// <param name="Id"> Id of Given User</param>
        /// <returns>Roles of Given User</returns>
        Task<IEnumerable<UserRoleModel>> GetUserAllRoleAsync(int Id);
        /// <summary>
        /// Get All Roles from Db
        /// </summary>
        /// /// <returns></returns>
        IEnumerable<RoleModel> GetAllRoles();
        Task<IEnumerable<RoleModel>> GetAllRolesAsync();
        Task<int> GetLastUserId();
        #endregion

        #region SET
        Task<bool> CreatUserAsync(IdentityUserModel model);
        bool CreateUser(IdentityUserModel model);
        bool AssignUserRoles(int userId, IdentityUserModel user, IEnumerable<RoleModel> roles);
        Task<bool> AssignUserRolesAsync(int userId, IdentityUserModel user, IEnumerable<RoleModel> roles);
        #endregion

        #region Authenticate User
        Task<IdentityUserModel> AuthenticateUserAsync(string username, string password, int? roleId = null);
        #endregion

        #region Replace
        Task<bool> ChangePasswordAsync(string newPassword, string reTypePassword, IdentityUserModel user);
        #endregion

        #region Helper
        Task<bool> CompareDisplayNameAsync(string displayname);
        #endregion

        #region Find
        Task<IdentityUserModel> FindUserByKeyAsync(string key, string displayName);
        #endregion
    }
}

