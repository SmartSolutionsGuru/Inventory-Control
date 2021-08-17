using SmartSolutions.InventoryControl.DAL.Managers.Product;
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
        private readonly IProductManager _productManager;
        #endregion

        #region Costructor
        [ImportingConstructor]
        public ReportsViewModel(IProductManager productManager)
        {
            _productManager = productManager;
        }
        #endregion

        #region Methods
        protected override void OnActivate()
        {
            base.OnActivate();
            ReportsCategory = new List<string> 
            {
                "Bussiness Partners",
                "Purchase",
                "Products",
                "Sales",
                "Payments",
                "Bank Accounts"
            };
        }
        #endregion

        #region Properties
        private List<string> _ReportsCategory;
        /// <summary>
        /// Reports List For Displaying Report
        /// </summary>
        public List<string> ReportsCategory
        {
            get { return _ReportsCategory; }
            set { _ReportsCategory = value; NotifyOfPropertyChange(nameof(ReportsCategory)); }
        }

        private string _SelectedReportCategory;
         /// <summary>
         /// Selected Report
         /// </summary>
        public string SelectedReportCategory
        {
            get { return _SelectedReportCategory; }
            set { _SelectedReportCategory = value; NotifyOfPropertyChange(nameof(SelectedReportCategory)); }
        }

        private List<string> _ReportsSubCategory;
        /// <summary>
        /// Selected Reports By Sub Category Like Vender Partners, Sale Partners etc...
        /// </summary>
        public List<string> ReportsSubCategory
        {
            get { return _ReportsSubCategory; }
            set { _ReportsSubCategory = value; NotifyOfPropertyChange(nameof(ReportsSubCategory)); }
        }
        private string _SelectedReportSubCategory;
        /// <summary>
        /// Selected Report By Sub Category Like Vender Partners, Sale Partners etc...
        /// </summary>
        public string SelectedReportSubCategory
        {
            get { return _SelectedReportSubCategory; }
            set { _SelectedReportSubCategory = value; NotifyOfPropertyChange(nameof(SelectedReportSubCategory)); }
        }

        #endregion
    }
}
