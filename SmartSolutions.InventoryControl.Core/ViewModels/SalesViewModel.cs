using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using SmartSolutions.InventoryControl.DAL.Managers.Product.ProductColor;
using SmartSolutions.InventoryControl.DAL.Managers.Product.ProductSize;
using SmartSolutions.InventoryControl.DAL.Managers.Product;
using System.Text;

namespace SmartSolutions.InventoryControl.Core.ViewModels
{  
    [Export(typeof(SalesViewModel)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class SalesViewModel : BaseViewModel
    {
        #region Private Members
        private readonly IProductColorManager _productColorManager;
        private readonly IProductSizeManager _productSizeManager;
        private readonly IProductManager _productManager;
        #endregion
        #region Constructor
        [ImportingConstructor]
        public SalesViewModel(IProductColorManager productColorManager
                              ,IProductSizeManager productSizeManager,IProductManager productManager)
        {
            _productColorManager = productColorManager;
            _productSizeManager = productSizeManager;
            _productManager = productManager;
        }
        #endregion

        #region Methods
        protected override void OnActivate()
        {
            base.OnActivate();
            SaleTransaction = new DAL.Models.Sales.SaleTransactionModel();
      
        }
        #endregion

        #region Properties
        private DAL.Models.Sales.SaleTransactionModel _SaleTransaction;
          /// <summary>
          /// Sales Transaction Model
          /// </summary>
        public DAL.Models.Sales.SaleTransactionModel SaleTransaction
        {
            get { return _SaleTransaction; }
            set { _SaleTransaction = value; NotifyOfPropertyChange(nameof(SaleTransaction)); }
        }

        private string _SelectedSaleType;

        public string SelectedSaleType
        {
            get { return _SelectedSaleType; }
            set { _SelectedSaleType = value; NotifyOfPropertyChange(nameof(SelectedSaleType)); }
        }


        #endregion
    }
}
