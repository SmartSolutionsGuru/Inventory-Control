using SmartSolutions.InventoryControl.DAL.Models.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SmartBooks.Repo.Identity
{
    public class AuthenticationService : IAuthenticationService
    {
        #region Default Constructor
        public AuthenticationService() { }
        #endregion

        #region Private Memebers
        /// <summary>
        /// Private static Member for testing Purpose
        /// TODO: here we replace the db values for Authntication
        /// </summary>
        private static readonly List<InternalUserData> _users = new List<InternalUserData>()
        {
            new InternalUserData("Mark", "mark@company.com",
            "MB5PYIsbI2YzCUe34Q5ZU2VferIoI4Ttd+ydolWV0OE=", new List<RoleModel> { new RoleModel { Name = "Administrators" } }),
            new InternalUserData("John", "john@company.com",
            "hMaLizwzOQ5LeOnMuj+C6W75Zl5CXXYbwDSHWW9ZOXc=", new List <RoleModel> { new RoleModel { Name="User"} })
        };
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

        #region Public Methods
        public IdentityUserModel AuthenticateUser(string username, string clearTextPassword)
        {
            InternalUserData userData = _users.FirstOrDefault(u => u.Username.Equals(username)
                && u.HashedPassword.Equals(CalculateHash(clearTextPassword, u.Username)));
            if (userData == null)
                throw new UnauthorizedAccessException("Access denied. Please provide some valid credentials.");

            return new IdentityUserModel(userData.Username);
        }
        #endregion

        #region Private Class for Authenticating User
        private class InternalUserData
        {
            #region Default Constructor
            public InternalUserData(string username, string email, string hashedPassword, List<RoleModel> roles)
            {
                Username = username;
                Email = email;
                HashedPassword = hashedPassword;
                Roles = roles;
            }
            #endregion

            #region Public Properties
            public string Username { get; private set;}

            public string Email { get; private set;}

            public string HashedPassword { get; private set;}

            public List<RoleModel> Roles { get;private set;}
            #endregion
        }
        #endregion
    }
}
