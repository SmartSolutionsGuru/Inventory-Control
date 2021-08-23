using Caliburn.Micro;
using SmartBooks.Repo.Identity;
using SmartSolutions.InventoryControl.Core.ViewModels.Commands;
using SmartSolutions.InventoryControl.DAL;
using SmartSolutions.InventoryControl.DAL.Models.Authentication;
using SmartSolutions.Util.LogUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace SmartSolutions.InventoryControl.Core.ViewModels.Login
{
    [Export(typeof(LoginViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class LoginViewModel : BaseViewModel
    {
        #region Private Members
        private readonly IEventAggregator _eventAggregator;
        private readonly DAL.Managers.Authentication.IAuthenticationManager _authenticationManager;
        #endregion

        #region Public Properties
        private bool _IsSecretKeyEmpty;
         /// <summary>
         /// flag for Indicating Secret Key is not available
         /// </summary>
        public bool IsSecretKeyEmpty
        {
            get { return _IsSecretKeyEmpty; }
            set { _IsSecretKeyEmpty = value; NotifyOfPropertyChange(nameof(IsSecretKeyEmpty)); }
        }

        /// <summary>
        /// The UserName of the user
        /// </summary>
        private string _UserName;
        public string UserName
        {
            get { return _UserName; }
            set { _UserName = value; NotifyOfPropertyChange(nameof(UserName)); IsUserErrorMessage = false; }
        }
        /// <summary>
        /// A flag indicating if the login command is running
        /// </summary>
        private bool m_LoginIsRunning;

        public bool LoginIsRunning
        {
            get { return m_LoginIsRunning; }
            set { m_LoginIsRunning = value; NotifyOfPropertyChange(nameof(LoginIsRunning)); }
        }

        /// <summary>
        /// A flag indicating if the login is succeed or not
        /// </summary>
        public bool IsSuccess => Thread.CurrentPrincipal.Identity.IsAuthenticated;


        private bool m_IsPasswordErrorMessage;
        public bool IsPasswordErrorMessage
        {
            get { return m_IsPasswordErrorMessage; }
            set { m_IsPasswordErrorMessage = value; NotifyOfPropertyChange(nameof(IsPasswordErrorMessage)); }
        }

        private bool mIsUserErrorMessage;
        public bool IsUserErrorMessage
        {
            get { return mIsUserErrorMessage; }
            set { mIsUserErrorMessage = value; NotifyOfPropertyChange(nameof(IsUserErrorMessage)); }
        }

        /// <summary>
        /// The Password of the user
        /// </summary>
        private string _password;
        public string Password
        {
            get { return _password; }
            set { _password = value; NotifyOfPropertyChange(nameof(Password)); IsPasswordErrorMessage = false; }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; NotifyOfPropertyChange(nameof(ErrorMessage)); }
        }
        public string this[string columnName]
        {
            get
            {
                string result = null;
                switch (columnName)
                {
                    case nameof(Password):
                        result = ErrorMessage;
                        break;
                    case nameof(UserName):
                        result = ErrorMessage;
                        break;
                }
                return result;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool CanLogin => !IsSuccess;

        /// <summary>
        /// Option for Reentreing Password
        /// </summary>
        private bool _CanReTypePassword;
        public bool CanReTypePassword
        {
            get { return _CanReTypePassword; }
            set { _CanReTypePassword = value; NotifyOfPropertyChange(nameof(CanReTypePassword)); }
        }

        private string _ReTypePassword;
        public string ReTypePassword
        {
            get { return _ReTypePassword; }
            set { _ReTypePassword = value; NotifyOfPropertyChange(nameof(ReTypePassword)); }
        }

        public string AuthenticatedUser
        {
            get
            {
                if (IsSuccess)
                    return string.Format("signed in as {0},{1}", Thread.CurrentPrincipal.Identity.Name,
                        Thread.CurrentPrincipal.IsInRole("Administration") ? "you are Logging as Adminstraotr!"
                        : "You are not a Member Administrstor of Group ");
                return "Not Authenticated";
            }
        }



        private bool _IsLoginAsAdmin;
        /// <summary>
        /// check login as admin is checked or not
        /// </summary>
        public bool IsLoginAsAdmin
        {
            get { return _IsLoginAsAdmin; }
            set { _IsLoginAsAdmin = value; NotifyOfPropertyChange(nameof(IsLoginAsAdmin)); }
        }

        /// <summary>
        /// Get the value that forget login is pressed or not
        /// </summary>
        private bool _IsLoginForget = false;
        public bool IsLoginForget
        {
            get { return _IsLoginForget; }
            set { _IsLoginForget = value; NotifyOfPropertyChange(nameof(IsLoginForget)); }
        }

        private string _SecretKey;
        public string SecretKey
        {
            get { return _SecretKey; }
            set { _SecretKey = value; NotifyOfPropertyChange(nameof(SecretKey)); }
        }
        public IdentityUserModel User { get; set; }
        private DAL.Models.ProprietorInformationModel _Properitor;

        public DAL.Models.ProprietorInformationModel Properitor
        {
            get { return _Properitor; }
            set { _Properitor = value; }
        }



        #endregion

        #region Commands
        /// <summary>
        /// The command to login
        /// </summary>
        public ICommand LoginCommand { get; set; }

        /// <summary>
        /// The command for Executing the Forget Password Scenario 
        /// </summary>
        public ICommand ForgetPasswordCommand { get; set; }

        #endregion

        #region Constructor
        public LoginViewModel() { }

        /// <summary>
        /// Default constructor
        /// </summary>
        [ImportingConstructor]
        public LoginViewModel(IEventAggregator eventAggregator,
                              DAL.Managers.Authentication.IAuthenticationManager authenticationManager)
        {
            _eventAggregator = eventAggregator;
            _authenticationManager = authenticationManager;
            // Create commands
            LoginCommand = new RelayParameterizedCommand(async (parameter) => await LoginAsync(parameter));
            Properitor = AppSettings.Proprietor;
        }



        #endregion

        #region Protected Methods
        protected override void OnInitialize()
        {
            base.OnInitialize();
        }
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            if (Execute.InDesignMode)
            {
                IsLoginForget = false;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Attempts to log the user in
        /// </summary>
        /// <param name="parameter">The <see cref="SecureString"/> passed in from the view for the users password</param>
        /// <returns></returns>
        public async Task LoginAsync(object parameter)
        {
            IsLoading = true;
            LoadingMessage = "Verifing...";
            await RunCommandAsync(() => LoginIsRunning, async () =>
            {
                PasswordBox passwordBox = parameter as PasswordBox;
                string clearPassword = passwordBox.Password;

                try
                {
                    if (!string.IsNullOrEmpty(Password))
                    {

                        IdentityUserModel user = await _authenticationManager.AuthenticateUserAsync(UserName, clearPassword,isAdmin: IsLoginAsAdmin);

                        //Get the Current Principle Object

                        //CustomPrinciple customPrincipal = Thread.CurrentPrincipal as CustomPrinciple;
                        //if (customPrincipal == null)
                        //{
                        //    IsUserErrorMessage = true;
                        //    IsPasswordErrorMessage = true;
                        //    throw new ArgumentException("The application Default Thread principle bust be set on startup");
                        //}

                        if (user != null)
                        {
                           // customPrincipal.Identity = new CustomIdentity(user.DisplayName, user.Email, user.Roles);
                            //Update UI
                            NotifyOfPropertyChange(nameof(AuthenticatedUser));
                            NotifyOfPropertyChange(nameof(IsSuccess));
                            UserName = string.Empty;
                            passwordBox.Password = string.Empty;
                            User = user;
                            AppSettings.LoggedInUser = user;
                            await Task.Delay(500);
                            _eventAggregator?.PublishOnCurrentThread("LOGGED-IN");
                            _eventAggregator?.PublishOnUIThread(IoC.Get<MainViewModel>());
                        }
                        else
                        {
                            IsUserErrorMessage = true;
                            IsPasswordErrorMessage = true;
                            NotifyOfPropertyChange(nameof(AuthenticatedUser));
                            NotifyOfPropertyChange(nameof(IsSuccess));
                        }

                        //Update UI
                        //NotifyOfPropertyChange(nameof(AuthenticatedUser));
                        //NotifyOfPropertyChange(nameof(IsSuccess));
                        //UserName = string.Empty;
                        //passwordBox.Password = string.Empty;
                        //await Task.Delay(500);
                        //_eventAggregator?.PublishOnCurrentThread("LOGGED-IN");
                        //_eventAggregator?.PublishOnUIThread(IoC.Get<MainPageViewModel>());
                    }
                    else
                    {
                        //If user is Entering New Password
                        if (!string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(ReTypePassword) && string.Equals(Password, ReTypePassword))
                        {
                            await _authenticationManager.ChangePasswordAsync(Password, ReTypePassword, User);
                            _eventAggregator?.PublishOnCurrentThread("LOGGED-IN");
                            _eventAggregator?.PublishOnUIThread(IoC.Get<MainViewModel>());
                        }
                        else
                        {
                            await IoC.Get<IDialogManager>().ShowMessageBoxAsync("Please Enter the Same Password", options: Dialogs.MessageBoxOptions.Ok); ;
                        }
                    }
                    AppSettings.LoggedInUser = User;
                }
                catch (Exception ex)
                {
                    IsUserErrorMessage = true;
                    IsPasswordErrorMessage = true;
                    LogMessage.Write(ex.ToString());
                }
            });
            IsLoading = false;
        }
        public void ForgetPassword()
        {
            IsLoginForget = true;
            NotifyOfPropertyChange(nameof(IsLoginForget));
        }
        public async Task ChangePasswordAsync()
        {
            IsLoading = true;
            await RunCommandAsync(() => LoginIsRunning, async () =>
            {
                LoadingMessage = "Verfing Credentials...";
                if (!string.IsNullOrEmpty(SecretKey))
                {
                    var resultUser = await _authenticationManager.FindUserByKeyAsync(SecretKey, UserName);
                    if (resultUser != null)
                    {
                        CanReTypePassword = true;
                        IsLoginForget = false;
                        User = resultUser;
                    }
                }
                else
                {
                    //TODO: send User Friendly Message that SecretKey Or UserName is Not Filled
                    IsSecretKeyEmpty = true;
                }
            });
            IsLoading = false;
        }

        public void Cancel()
        {
            IsLoginForget = false;
        }
        #endregion
    }
}

