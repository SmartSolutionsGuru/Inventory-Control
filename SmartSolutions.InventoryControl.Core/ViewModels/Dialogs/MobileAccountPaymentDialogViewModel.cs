using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace SmartSolutions.InventoryControl.Core.ViewModels.Dialogs
{
    [Export(typeof(MobileAccountPaymentDialogViewModel)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class MobileAccountPaymentDialogViewModel:BaseViewModel
    {
        #region [constructor]
        [ImportingConstructor]
        public MobileAccountPaymentDialogViewModel()
        {

        }
        #endregion

        #region [Public Methods]
        public void Close()
        {
            TryClose();
        }
        public void Cancel()
        {
            TryClose(true);
        }
        #endregion
        #region [Properties]
        private List<string> _MobileOperaters;
        /// <summary>
        /// Name of Operaters
        /// </summary>
        public List<string> MobileOperaters
        {
            get { return _MobileOperaters; }
            set { _MobileOperaters = value; NotifyOfPropertyChange(nameof(MobileOperaters)); }
        }
        private string _SelectedMobileOperater;
        /// <summary>
        /// Selected Mobile Operater
        /// </summary>
        public string SelectedMobileOperater
        {
            get { return _SelectedMobileOperater; }
            set { _SelectedMobileOperater = value; NotifyOfPropertyChange(nameof(SelectedMobileOperater)); }
        }
        private string _Mobilenumbers;
        /// <summary>
        /// Mobile No List
        /// </summary>
        public string Mobilenumbers
        {
            get { return _Mobilenumbers; }
            set { _Mobilenumbers = value; NotifyOfPropertyChange(nameof(Mobilenumbers)); }
        }
        private string _SelectedMobileNumber;
        /// <summary>
        /// Selected Mobile number
        /// </summary>
        public string SelectedMobileNumber
        {
            get { return _SelectedMobileNumber; }
            set { _SelectedMobileNumber = value; NotifyOfPropertyChange(nameof(SelectedMobileNumber)); }
        }
        private decimal _Amount;
        /// <summary>
        /// amount of Payment
        /// </summary>
        public decimal Amount
        {
            get { return _Amount; }
            set { _Amount = value; NotifyOfPropertyChange(nameof(Amount)); }
        }

        #endregion
    }
}
