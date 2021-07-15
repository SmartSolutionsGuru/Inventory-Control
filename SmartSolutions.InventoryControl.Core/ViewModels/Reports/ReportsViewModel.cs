using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace SmartSolutions.InventoryControl.Core.ViewModels.Reports
{
    [Export(typeof(ReportsViewModel)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class ReportsViewModel : BaseViewModel
    {

        #region Private Members

        #endregion

        #region Costructor
        [ImportingConstructor]
        public ReportsViewModel()
        {

        }
        #endregion

        #region Properties
        private List<string> _Reports;
        /// <summary>
        /// Reports List For Displaying Report
        /// </summary>
        public List<string> Reports
        {
            get { return _Reports; }
            set { _Reports = value; NotifyOfPropertyChange(nameof(Reports)); }
        }

        private string _SelectedReport;
         /// <summary>
         /// Selected Report
         /// </summary>
        public string SelectedReport
        {
            get { return _SelectedReport; }
            set { _SelectedReport = value; NotifyOfPropertyChange(nameof(SelectedReport)); }
        }


        #endregion
    }
}
