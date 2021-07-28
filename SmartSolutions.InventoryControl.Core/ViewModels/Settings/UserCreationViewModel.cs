using Caliburn.Micro;
using SmartSolutions.InventoryControl.DAL;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.Core.ViewModels.Settings
{
    [Export(typeof(UserCreationViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class UserCreationViewModel : BaseViewModel
    {
        #region Private Members
        private readonly DAL.Managers.Authentication.IAuthenticationManager _authenticationManager;
        #endregion

        #region Constructor
        public UserCreationViewModel() { }
        [ImportingConstructor]
        public UserCreationViewModel(DAL.Managers.Authentication.IAuthenticationManager authenticationManager)
        {
            _authenticationManager = authenticationManager;
        }
        #endregion

        #region Methods
        protected override void OnActivate()
        {
            base.OnActivate();
            if (Execute.InDesignMode)
            {
                User = new DAL.Models.Authentication.IdentityUserModel
                {
                    FirstName = "Shabab",
                    LastName = "Butt",
                    DisplayName = "Sbutt",
                    PasswordHash = "Password",
                    SecretKey = "SecretKey"
                };
            }
            User = new DAL.Models.Authentication.IdentityUserModel();
        }
        public async Task Save()
        {
            
            IsLoading = true;
            LoadingMessage = "Saving User";
            if (User != null)
            {
                if(!string.Equals(Password,ReTypePassword))
                {
                    //TODO: Display User Friendly Error And Return
                    return;
                }
                User.DisplayName = UserName;
                User.PasswordHash = Password;
                User.CreatedBy = AppSettings.LoggedInUser.User.DisplayName;
                bool x = await _authenticationManager.CompareDisplayNameAsync(UserName);
                if (x)
                {
                    IsDisplayNameAvailable = true;
                    UserName = string.Empty;
                    NotifyOfPropertyChange(nameof(User.DisplayName));
                    return;
                }

                User.Roles = (await _authenticationManager.GetAllRolesAsync()).ToList();
                User.CreatedBy = AppSettings.LoggedInUser.CreatedBy;
                await _authenticationManager.CreatUserAsync(User);
                User = new DAL.Models.Authentication.IdentityUserModel();
                await Task.Delay(2000);
                IsLoading = false;
                ClearInfo();
                TryClose();
            }
        }

        public void Close()
        {
            ClearInfo();
            TryClose();
        }
        #endregion

        #region Private Methods
        private void ClearInfo()
        {
            User = null;
            Password = string.Empty;
            UserName = string.Empty;
            IsAdmin = false;
            IsPurchase = false;
            IsReport = false;
            CanEdit = false;
            CanDelete = false;
        }
        #endregion

        #region Properties
        private DAL.Models.Authentication.IdentityUserModel _User;
        public DAL.Models.Authentication.IdentityUserModel User
        {
            get { return _User ?? new DAL.Models.Authentication.IdentityUserModel(); }
            set { _User = value; NotifyOfPropertyChange(nameof(User)); }
        }
        private bool _IsAdmin;
        public bool IsAdmin
        {
            get => _IsAdmin;
            set { _IsAdmin = value; NotifyOfPropertyChange(nameof(IsAdmin)); }
        }

        private bool _IsPurchase;
        public bool IsPurchase
        {
            get { return _IsPurchase; }
            set { _IsPurchase = value; NotifyOfPropertyChange(nameof(IsPurchase)); }
        }

        private bool _IsReport;
        public bool IsReport
        {
            get { return _IsReport; }
            set { _IsReport = value; NotifyOfPropertyChange(nameof(IsReport)); }
        }

        private bool _CanEdit;
        public bool CanEdit
        {
            get { return _CanEdit; }
            set { _CanEdit = value; NotifyOfPropertyChange(nameof(CanEdit)); }
        }

        private bool _CanDelete;
        public bool CanDelete
        {
            get { return _CanDelete; }
            set { _CanDelete = value; NotifyOfPropertyChange(nameof(CanDelete)); }
        }

        private bool _IsDisplayNameAvailable;

        public bool IsDisplayNameAvailable
        {
            get { return _IsDisplayNameAvailable; }
            set { _IsDisplayNameAvailable = value; NotifyOfPropertyChange(nameof(IsDisplayNameAvailable)); }
        }
        private string _UserName;

        public string UserName
        {
            get { return _UserName; }
            set { _UserName = value; NotifyOfPropertyChange(nameof(UserName)); }
        }

        private string _Password;

        public string Password
        {
            get { return _Password; }
            set { _Password = value; NotifyOfPropertyChange(nameof(Password)); }
        }
        private string _ReTypePassword;

        public string ReTypePassword
        {
            get { return _ReTypePassword; }
            set { _ReTypePassword = value; NotifyOfPropertyChange(nameof(ReTypePassword)); }
        }

        #endregion
    }
}

