using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSolutions.InventoryControl.DAL.Models.Authentication
{
   public class IdentityUserModel :BaseModel
    {
        #region Default Constructor

        public IdentityUserModel() { }

        public IdentityUserModel(string userName)
        {
            DisplayName = userName;
        }

        public IdentityUserModel(string userName, string email, List<UserRoleModel> userRole)
        {
            DisplayName = userName;
            Email = email;
            UserRoles = userRole;
        }
        public IdentityUserModel(string userName, string email, List<RoleModel> role = null, List<UserRoleModel> userRole = null)
        {
            DisplayName = userName;
            Email = email;
            Roles = role;
            UserRoles = userRole;
        }
        public IdentityUserModel(IdentityUserModel model, string email, List<RoleModel> role = null, List<UserRoleModel> userRole = null)
        {
            User = model;
            DisplayName = model.DisplayName;
            Email = email;
            Roles = role;
            UserRoles = userRole;
        }
        #endregion

        #region Public Properties
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; private set; }
        public string SecretKey { get; set; }
        public IdentityUserModel User { get; set; }

        /// <summary>
        /// string that defines authentication Type
        /// </summary>
        public string AuthenticationType => "Custom Authentication ";

        public bool IsAuthenticated => !string.IsNullOrEmpty(DisplayName);

        /// <summary>
        /// Gets or sets the user name for this user.
        /// </summary>
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the normalized user name for this user.
        /// </summary>
        public virtual string NormalizedUserName { get; set; }

        /// <summary>
        /// List of roles that are Associated with User
        /// </summary>
        public List<RoleModel> Roles { get; set; }

        public List<UserRoleModel> UserRoles { get; set; }
        /// <summary>
        /// Gets or sets a flag indicating if a user has confirmed their email address.
        /// </summary>
        /// <value>True if the email address has been confirmed, otherwise false.</value>
        public bool EmailConfirmed { get; set; }

        /// <summary>
        /// Gets or sets a salted and hashed representation of the password for this user.
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// A DateTime value that is when ever  the user SuccessFully Logged In
        /// </summary>
        public DateTime? ConcurrencyStamp { get; set; }

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
        /// Returns the user name for this user.
        /// </summary>
        public override string ToString()
        {
            return DisplayName;
        }
        #endregion
    }
}

