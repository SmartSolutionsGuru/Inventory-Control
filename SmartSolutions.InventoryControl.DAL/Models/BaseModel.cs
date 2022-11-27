using System;
using System.ComponentModel;

namespace SmartSolutions.InventoryControl.DAL.Models
{
    public class BaseModel:INotifyPropertyChanged
    {
        #region Properties
        /// <summary>
        /// Unique Identification of Entity
        /// </summary>
        public int? Id { get; set; }
        /// <summary>
        /// Name of Model
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Date and Time of Creating specific Object or Row
        /// </summary>
        public DateTime? CreatedAt { get; set; }
        /// <summary>
        /// Entity Name of that who create that specific Object
        /// </summary>
        public string CreatedBy { get; set; }
        /// <summary>
        /// Time  of  update that specific Object
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
        /// <summary>        
        /// Entity Name of that who Update that specific Object
        /// </summary>       
        public string UpdatedBy { get; set; }
        /// <summary>
        /// Is this object row is Active or not
        /// </summary>
        private bool? _IsActive;

        public bool? IsActive
        {
            get { return _IsActive; }
            set { _IsActive = value; NotifyOfPropertyChange(nameof(IsActive)); }
        }

        /// <summary>
        /// Is this object row is deleted or not
        /// </summary>
        public bool? IsDeleted { get; set; }
        /// <summary>
        /// gets or set the Busy Status 
        /// </summary>
        public bool IsBusy { get; set; }
        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Methods
        public void NotifyOfPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
    public enum PaymentType
    {
        //None = 0,
        //DR = 1,
        //CR = 2
        None =0,
        //Cash Flow In
        Receivable = 1,
        //Cash Flow Out
        Payable = 2
    }
}
