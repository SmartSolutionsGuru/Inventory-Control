using SmartSolutions.InventoryControl.DAL.Models.Bank;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace SmartSolutions.InventoryControl.Core.ViewModels.Bank
{
    [Export(typeof(BankViewModel)),PartCreationPolicy(CreationPolicy.Shared)]
    public class BankViewModel : BaseViewModel
    {
        #region Private Members

        #endregion

        #region Constructor
        [ImportingConstructor]
        public BankViewModel()
        {

        }
        #endregion

        #region Methods

        #endregion

        #region Properties
        private List<BankModel> _Banks;

        public List<BankModel> Banks
        {
            get { return _Banks; }
            set { _Banks = value;  NotifyOfPropertyChange(nameof(Banks)); }
        }

        #endregion
    }
}
