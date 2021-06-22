using SmartSolutions.InventoryControl.DAL.Models.Authentication;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.BooleanUtils;
using SmartSolutions.Util.DictionaryUtils;
using SmartSolutions.Util.EncryptionUtils;
using SmartSolutions.Util.LogUtils;
using SmartSolutions.Util.NumericUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Authentication
{
    [Export(typeof(IAuthenticationManager)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class AuthenticationManager:BaseManager, IAuthenticationManager
    {
        #region Private Members
        private readonly IRepository Repository;
        #endregion

        #region Default Constructor
        [ImportingConstructor]
        public AuthenticationManager()
        {
            Repository = GetRepository<IdentityUserModel>();
        }
        #endregion

        #region Private Members
        /// <summary>
        /// Private static Member for testing Purpose
        /// TODO: here we replace the db values for Authentication
        /// </summary>
        private static readonly List<InternalUserData> _users = new List<InternalUserData>()
        {
            new InternalUserData("Mark", "mark@company.com",
            "MB5PYIsbI2YzCUe34Q5ZU2VferIoI4Ttd+ydolWV0OE=", new List<RoleModel> { new RoleModel { Name = "Administrators" } }),
            new InternalUserData("John", "john@company.com",
            "hMaLizwzOQ5LeOnMuj+C6W75Zl5CXXYbwDSHWW9ZOXc=", new List <RoleModel> { new RoleModel { Name="User"} })
        };

        private IEnumerable<IdentityUserModel> allUsers = new List<IdentityUserModel>();
        /// <summary>
        /// Calculating the Hash Password received from User
        /// </summary>
        /// <param name="clearTextPassword"> simple Password received from User</param>
        /// <param name="salt"> add the Salt</param>
        /// <returns></returns>
        private string CalculateHash(string clearTextPassword, string salt)
        {
            // Convert the salted password to a byte array
            byte[] saltedHashBytes = Encoding.UTF8.GetBytes(clearTextPassword + salt);
            // Use the hash algorithm to calculate the hash
            HashAlgorithm algorithm = new SHA256Managed();
            byte[] hash = algorithm.ComputeHash(saltedHashBytes);
            // Return the hash as a base64 encoded string to be compared to the stored password
            return Convert.ToBase64String(hash);
        }
        #endregion

        #region Authentication
        public async Task<IdentityUserModel> AuthenticateUserAsync(string username, string clearTextPassword, int? roleId = null)
        {
            #region New Implementation
            IdentityUserModel identityModel = new IdentityUserModel();
            allUsers = await GetAllUserAsync();
            if (allUsers.Count() > 0)
            {
                var hasedPassword = Encryption.Encrypt(Encryption.EncryptionType.MD5, clearTextPassword);
                var selectedUser = allUsers.Where(x => x.DisplayName == username && x.PasswordHash == hasedPassword).FirstOrDefault();
                if (selectedUser != null)
                {
                    var roles = await GetUserAllRoleAsync(selectedUser.Id.Value);
                    selectedUser.UserRoles = roles.ToList();
                    if (roleId != null && selectedUser?.UserRoles?.Count > 0)
                    {
                        var resultRole = selectedUser?.UserRoles?.Where(x => x.RoleId == roleId && x.IsActive == true).FirstOrDefault();
                        if (resultRole.IsActive == true)
                            return new IdentityUserModel(selectedUser, string.Empty, role: null, roles.ToList());
                        else
                            return null;
                    }
                    return new IdentityUserModel(selectedUser, string.Empty, role: null, roles.ToList());

                }
                else
                {
                    return null;
                }
            }
            else
            {
                identityModel = null;
            }
            #endregion

            return identityModel;
        }
        #endregion

        #region Private Class for Authenticating User
        private class InternalUserData
        {
            public InternalUserData() { }

            public InternalUserData(string username, string email, string hashedPassword, List<RoleModel> roles)
            {
                Username = username;
                Email = email;
                HashedPassword = hashedPassword;
                Roles = roles;
            }
            public int Id
            { get; private set; }
            public string Username { get; private set; }

            public string Email
            { get; private set; }
            public string SecretKey { get; private set; }

            public string HashedPassword { get; private set; }
            public List<RoleModel> Roles { get; private set; }
        }
        #endregion

        #region GET USERS
        public async Task<IEnumerable<IdentityUserModel>> GetAllUserAsync()
        {
            string query = string.Empty;
            List<IdentityUserModel> _users = new List<IdentityUserModel>();
            try
            {
                query = @"SELECT * From IdentityUser";
                var values = await Repository.QueryAsync(query: query);

                if (values != null)
                {
                    foreach (var value in values)
                    {
                        IdentityUserModel user = new IdentityUserModel();
                        user.Id = Convert.ToInt32(value.GetValueFromDictonary("Id").ToString());
                        user.DisplayName = value.GetValueFromDictonary("DisplayName")?.ToString();
                        user.FirstName = value.GetValueFromDictonary("FirstName")?.ToString();
                        user.LastName = value.GetValueFromDictonary("LastName")?.ToString();
                        user.PasswordHash = value.GetValueFromDictonary("Password")?.ToString();
                        user.SecretKey = value.GetValueFromDictonary("SecretKey")?.ToString();
                        user.IsActive = Convert.ToBoolean(value.GetValueFromDictonary("IsActive")?.ToString().ToNullableBoolean());
                        _users.Add(user);
                    }
                }

                return _users;
                //await Task.Run(() =>
                //{
                //    _users = GetAllUser().ToList();
                //});
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString());
            }
            return _users;
        }
        public IEnumerable<IdentityUserModel> GetAllUser()
        {
            string query = string.Empty;
            List<IdentityUserModel> _users = new List<IdentityUserModel>();
            try
            {
                query = @"SELECT * From IdentityUser";
                var values = Repository.QueryAsync(query: query);

                if (values != null)
                {
                    foreach (var value in values.Result)
                    {
                        IdentityUserModel user = new IdentityUserModel();
                        user.Id = Convert.ToInt32(value.GetValueFromDictonary("Id").ToString());
                        user.DisplayName = value.GetValueFromDictonary("DisplayName")?.ToString();
                        user.FirstName = value.GetValueFromDictonary("FirstName")?.ToString();
                        user.LastName = value.GetValueFromDictonary("LastName")?.ToString();
                        user.PasswordHash = value.GetValueFromDictonary("Password")?.ToString();
                        user.SecretKey = value.GetValueFromDictonary("SecretKey")?.ToString();
                        user.IsActive = Convert.ToBoolean(value.GetValueFromDictonary("IsActive")?.ToString().ToNullableBoolean());
                        _users.Add(user);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return _users;
        }
        public IEnumerable<UserRoleModel> GetUserAllRole(int Id)
        {
            string query = string.Empty;
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["@v_UserId"] = Id;
            List<UserRoleModel> roles = new List<UserRoleModel>();
            try
            {
                query = @"SELECT * FROM UserLogin WHERE UserId = @v_UserId";
                var values = Repository.QueryAsync(query: query, parameters: parameters);
                if (values != null)
                {
                    foreach (var value in values.Result)
                    {
                        UserRoleModel roleModel = new UserRoleModel();
                        roleModel.UserId = Id;
                        roleModel.RoleId = Convert.ToInt32(value.GetValueFromDictonary("RoleId")?.ToString());
                        roleModel.IsActive = Convert.ToBoolean(value.GetValueFromDictonary("IsActive")?.ToString().ToNullableBoolean());
                        roleModel.IsDeleted = Convert.ToBoolean(value.GetValueFromDictonary("IsDeleted")?.ToString().ToNullableBoolean());
                        roles.Add(roleModel);
                    }
                }
            }
            catch (Exception ex)
            {

                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return roles;
        }
        public async Task<IEnumerable<UserRoleModel>> GetUserAllRoleAsync(int Id)
        {
            List<UserRoleModel> roles = new List<UserRoleModel>();
            try
            {
                string query = string.Empty;
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                //parameter["@v_role_Id"] = 0;
                parameters["@v_User_Identity_Id"] = Id;
                query = @"SELECT * FROM UserLogin WHERE User_Identity_Id = @v_User_Identity_Id";
                var values = await Repository.QueryAsync(query: query, parameters: parameters);
                if (values != null)
                {
                    foreach (var value in values)
                    {
                        UserRoleModel roleModel = new UserRoleModel();
                        roleModel.UserId = Id;
                        roleModel.RoleId = Convert.ToInt32(value.GetValueFromDictonary("Role_Id")?.ToString());
                        roleModel.IsActive = Convert.ToBoolean(value.GetValueFromDictonary("IsActive")?.ToString().ToNullableBoolean());
                        roleModel.IsDeleted = Convert.ToBoolean(value.GetValueFromDictonary("IsDeleted")?.ToString().ToNullableBoolean());
                        roles.Add(roleModel);
                    }
                }
            }
            catch (Exception ex)
            {

                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return roles;
        }
        public IEnumerable<RoleModel> GetAllRoles()
        {
            List<RoleModel> roles = new List<RoleModel>();
            string query = string.Empty;
            try
            {
                query = @"SELECT * FROM Role WHERE IsActive = 1 AND IsDeleted = 0";
                var values = Repository.QueryAsync(query: query);
                if (values != null)
                {
                    foreach (var value in values.Result)
                    {
                        var role = new RoleModel();
                        role.Id = Convert.ToInt32(value.GetValueFromDictonary("Id")?.ToString());
                        role.Name = value.GetValueFromDictonary("Name")?.ToString();
                        role.ShotName = value.GetValueFromDictonary("ShortName")?.ToString();
                        role.IsActive = Convert.ToBoolean(value.GetValueFromDictonary("IsActive")?.ToString().ToNullableBoolean());
                        role.IsDeleted = Convert.ToBoolean(value.GetValueFromDictonary("IsDeleted")?.ToString().ToNullableBoolean());
                        roles.Add(role);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return roles;
        }
        public async Task<IEnumerable<RoleModel>> GetAllRolesAsync()
        {
            List<RoleModel> roles = new List<RoleModel>();
            string query = string.Empty;
            try
            {
                query = @"SELECT * FROM Role WHERE IsActive = 1 AND IsDeleted = 0";
                var values = await Repository.QueryAsync(query: query);
                if (values != null)
                {
                    foreach (var value in values)
                    {
                        var role = new RoleModel();
                        role.Id = Convert.ToInt32(value.GetValueFromDictonary("Id")?.ToString());
                        role.Name = value.GetValueFromDictonary("Name")?.ToString();
                        role.ShotName = value.GetValueFromDictonary("ShortName")?.ToString();
                        role.IsActive = Convert.ToBoolean(value.GetValueFromDictonary("IsActive")?.ToString().ToNullableBoolean());
                        role.IsDeleted = Convert.ToBoolean(value.GetValueFromDictonary("IsDeleted")?.ToString().ToNullableBoolean());
                        roles.Add(role);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return roles;
        }
        public async Task<int> GetLastUserId()
        {
            int id = 0;
            try
            {
                string query = string.Empty;
                await Task.Run(() =>
                {
                    query = @"SELECT IDENT_CURRENT('IdentityUser')";
                });
                var result = await Repository.QueryScalarAsync(query: query);

                if (result is decimal)
                    id = Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return id;
        }
        #endregion

        #region Change Password
        public async Task<bool> ChangePasswordAsync(string newPassword, string reTypePassword, IdentityUserModel user)
        {
            bool retVal = false;
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_Id"] = user.Id;
                parameters["@v_Password"] = newPassword;
                parameters["@v_UpdatedAt"] = DateTime.Now;
                if (!string.IsNullOrEmpty(newPassword) && !string.IsNullOrEmpty(reTypePassword))
                {
                    string query = @"Update IdentityUser SET Password = @v_Password,UpdatedAt = @v_UpdatedAt WHERE Id = @v_Id";
                    await Repository.QueryAsync(query: query, parameters: parameters);
                    retVal = true;
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }
        #endregion

        #region SET Create User
        public bool CreateUser(IdentityUserModel model)
        {
            bool retVal = false;

            try
            {
                if (model != null)
                {
                    var encryptedPassword = Encryption.Encrypt(Encryption.EncryptionType.MD5, model.PasswordHash);
                    if (!string.IsNullOrEmpty(encryptedPassword))
                        model.PasswordHash = encryptedPassword;
                    Dictionary<string, object> parameters = new Dictionary<string, object>();
                    parameters["@v_FirstName"] = model.FirstName;
                    parameters["@v_LastName"] = model.LastName;
                    parameters["@v_DisplayName"] = model.DisplayName;
                    parameters["@v_SecretKey"] = model.SecretKey;
                    parameters["@v_Password"] = model.PasswordHash;
                    parameters["@v_CuncurrencyStamp"] = DateTime.Now;
                    parameters["@v_IsActive"] = true;
                    parameters["@v_IsDeleted"] = false;
                    parameters["@v_CreatedAt"] = model.CreatedAt;
                    parameters["@v_CreatedBy"] = model.CreatedBy == null ? DBNull.Value : (object)model.CreatedBy;
                    parameters["@v_UpdatedAt"] = model.UpdatedAt == null ? DBNull.Value : (object)model.UpdatedAt;
                    parameters["@v_UpDatedBy"] = model.UpdatedBy == null ? DBNull.Value : (object)model.UpdatedBy;

                    string query = @"INSERT INTO [dbo].[IdentityUser]
                                                           ([FirstName]
                                                           ,[LastName]
                                                           ,[DisplayName]
                                                           ,[SecretKey]
                                                           ,[Password]
                                                           ,[CuncurrencyStamp]
                                                           ,[IsActive]
                                                           ,[IsDeleted]
                                                           ,[CreatedAt]
                                                           ,[CreatedBy]
                                                           ,[UpdatedAt]
                                                           ,[UpdatedBy])
                                                     VALUES
                                                           (@v_FirstName
                                                           ,@v_LastName
                                                           ,@v_DisplayName
                                                           ,@v_SecretKey
                                                           ,@v_Password
                                                           ,@v_CuncurrencyStamp
                                                           ,@v_IsActive
                                                           ,@v_IsDeleted
                                                           ,@v_CreatedAt
                                                           ,@v_CreatedBy
                                                           ,@v_UpdatedAt
                                                           ,@v_UpDatedBy)";
                    Repository.QueryAsync(query: query, parameters: parameters);
                    retVal = true;
                }
            }
            catch (Exception ex)
            {

                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }
        public async Task<bool> CreatUserAsync(IdentityUserModel model)
        {
            bool retVal = false;
            try
            {

                retVal = CreateUser(model);
                if (retVal)
                {
                    var lastId = await GetLastUserId();
                    //Get all roles from Role table 
                    var roles = await GetAllRolesAsync();
                    //asign roles to Created User
                    await AssignUserRolesAsync(lastId, model, roles);
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }

        /// <summary>
        /// Assign Selected Roles to the Created User
        /// </summary>
        /// <param name="userId"> User ID</param>
        /// <param name="user"> User Model</param>
        /// <param name="roles"> Selected Roles </param>
        /// <returns></returns>
        public bool AssignUserRoles(int userId, IdentityUserModel user, IEnumerable<RoleModel> roles)
        {
            bool retVal = false;
            try
            {
                if (user != null)
                {
                    List<UserRoleModel> userRoleModels = new List<UserRoleModel>();
                    foreach (var role in roles)
                    {
                        UserRoleModel userRoleModel = new UserRoleModel();
                        userRoleModel.UserId = userId;
                        userRoleModel.RoleId = role.Id.Value;
                        userRoleModel.CreatedBy = user?.CreatedBy;
                        userRoleModel.IsActive = user.Roles.FirstOrDefault().IsActive;
                        userRoleModel.IsDeleted = user.Roles.FirstOrDefault().IsDeleted;

                        userRoleModels.Add(userRoleModel);
                    }

                    if (userRoleModels.Count > 0)
                    {

                        foreach (var userRole in userRoleModels)
                        {
                            Dictionary<string, object> parameters = new Dictionary<string, object>();
                            parameters["@v_UserId"] = userRole.UserId;
                            parameters["@v_RoleId"] = userRole.RoleId;
                            parameters["@v_IsActive"] = userRole.IsActive;
                            parameters["@v_IsDeleted"] = userRole.IsDeleted;
                            parameters["@v_CreatedAt"] = userRole.CreatedAt;
                            parameters["@v_CreatedBy"] = userRole.CreatedBy == null ? DBNull.Value : (object)userRole.CreatedBy;
                            parameters["@v_UpdatedAt"] = userRole.UpdatedAt == null ? DBNull.Value : (object)userRole.UpdatedAt;
                            parameters["@v_UpdatedBy"] = userRole.UpdatedBy == null ? DBNull.Value : (object)userRole.UpdatedBy;
                            parameters["@v_Description"] = userRole.Description == null ? DBNull.Value : (object)userRole.Description;
                            string query = @"INSERT INTO [dbo].[UserLogin]
                                                        ([User_Identity_Id]
                                                        ,[Role_Id]
                                                        ,[IsActive]
                                                        ,[IsDeleted]
                                                        ,[CreatedAt]
                                                        ,[CreatedBy]
                                                        ,[UpdatedAt]
                                                        ,[UpdatedBy]
                                                        ,[Description])
                                                  VALUES
                                                        (@v_UserId
                                                        ,@v_RoleId
                                                        ,@v_IsActive
                                                        ,@v_IsDeleted
                                                        ,@v_CreatedAt
                                                        ,@v_CreatedBy
                                                        ,@v_UpdatedAt
                                                        ,@v_UpdatedBy
                                                        ,@v_Description)";
                            Repository.QueryAsync(query: query, parameters: parameters);
                            retVal = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);

            }
            return retVal;
        }
        public async Task<bool> AssignUserRolesAsync(int userId, IdentityUserModel user, IEnumerable<RoleModel> roles)
        {
            bool retVal = false;
            try
            {
                await Task.Run(() =>
                {
                    AssignUserRoles(userId, user, roles);
                    retVal = true;
                });
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }
        #endregion

        #region Helper
        public async Task<bool> CompareDisplayNameAsync(string displayname)
        {
            bool retVal = false;
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_DisplayName"] = displayname;
                string query = @"Select DisplayName from IdentityUser WHERE DisplayName = @v_DisplayName";
                var values = await Repository.QueryAsync(query: query, parameters: parameters);
                if (values != null)
                {
                    retVal = true;
                }

            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }
        #endregion

        #region Find
        public async Task<IdentityUserModel> FindUserByKeyAsync(string key, string displayName)
        {
            IdentityUserModel model = new IdentityUserModel();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["@v_DisplayName"] = displayName;
            parameters["@v_key"] = key;
            try
            {
                if (string.IsNullOrEmpty(key))
                    return null;
                string query = @"Select * from IdentityUser WHERE SecretKey = @v_key AND DisplayName = @v_DisplayName And IsActive =1 And IsDeleted = 0";
                var values = await Repository.QueryAsync(query: query, parameters: parameters);
                if (values != null)
                {
                    foreach (var value in values)
                    {
                        IdentityUserModel userModel = new IdentityUserModel();
                        userModel.Id = Convert.ToInt32(value.GetValueFromDictonary("Id")?.ToString()?.ToNullableInt());
                        userModel.FirstName = value.GetValueFromDictonary("FirstName")?.ToString();
                        userModel.LastName = value.GetValueFromDictonary("LastName")?.ToString();
                        model = userModel;
                    }
                }


            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return model;
        }
        #endregion

    }
}

