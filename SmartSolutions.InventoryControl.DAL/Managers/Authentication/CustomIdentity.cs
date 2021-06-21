using SmartSolutions.InventoryControl.DAL.Models.Authentication;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace SmartBooks.Repo.Identity
{
    public class CustomIdentity : IIdentity
    {
        #region Defualt Constructor
        public CustomIdentity(string name, string email, List<UserRoleModel> userRoles)
        {
            Name = name;
            Email = email;
            UserRoles = userRoles;
        }
        public CustomIdentity(string name,string email,List<RoleModel> roles,List<UserRoleModel>userRoles =null)
        {
            Name = name;
            Email = email;
            UserRoles = userRoles;
            Roles = roles;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the email address for this user.
        /// </summary>
        public string Email { get; private set; }
        public List<UserRoleModel> UserRoles { get; private set; }
        public List<RoleModel> Roles { get; private set; }
        public string Name { get; private set; }
        
        public string AuthenticationType =>  "Custom Authentication ";
        public bool IsAuthenticated => !string.IsNullOrEmpty(Name);
       


        /// <summary>
        /// Gets or sets the primary key for this user.
        /// </summary>
        public virtual int Id { get; set; }

        /// <summary>
        /// Gets or sets the user name for this user.
        /// </summary>
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the normalized user name for this user.
        /// </summary>
        public virtual string NormalizedUserName { get; set; }


        /// <summary>
        /// Gets or sets a flag indicating if a user has confirmed their email address.
        /// </summary>
        /// <value>True if the email address has been confirmed, otherwise false.</value>
        public  bool EmailConfirmed { get; set; }

        /// <summary>
        /// Gets or sets a salted and hashed representation of the password for this user.
        /// </summary>
        public  string PasswordHash { get; set; }

       

        /// <summary>
        /// A DateTime value that is when ever  the user SuccessFully Logged In
        /// </summary>
        public  DateTime? ConcurrencyStamp { get; set; }

        /// <summary>
        /// Gets or sets a telephone number for the user.
        /// </summary>
        public virtual string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if a user has confirmed their telephone address.
        /// </summary>
        /// <value>True if the telephone number has been confirmed, otherwise false.</value>
        public virtual bool PhoneNumberConfirmed { get; set; }


        /// <summary>
        /// Gets or sets the date and time, in UTC, when any user lockout ends.
        /// </summary>
        /// <remarks>
        /// A value in the past means the user is not locked out.
        /// </remarks>
        public virtual DateTimeOffset? LockoutEnd { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if the user could be locked out.
        /// </summary>
        /// <value>True if the user could be locked out, otherwise false.</value>
        public virtual bool LockoutEnabled { get; set; }

        /// <summary>
        /// Gets or sets the number of failed login attempts for the current user.
        /// </summary>
        public virtual int AccessFailedCount { get; set; }
        #endregion

        #region Helper Methods

        /// <summary>
        /// Returns the username for this user.
        /// </summary>
        public override string ToString()
        {
            return DisplayName;
        }
        #endregion


    }
}
