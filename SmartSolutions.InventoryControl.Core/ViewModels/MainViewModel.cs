using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace SmartSolutions.InventoryControl.Core.ViewModels
{
    [Export(typeof(MainViewModel)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class MainViewModel : Conductor<Screen>, IHandle<Screen>
    {
        #region Private Members
        private readonly IEventAggregator _eventAggregator;

        #endregion

        #region Constructor
        [ImportingConstructor]
        public MainViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }
        #endregion
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
        }

        public void Product()
        {
           Handle(IoC.Get<Product.ProductViewModel>());
        }
        public void BussinessPartner()
        {
            Handle(IoC.Get<BussinessPartner.BussinessPartnerViewModel>());
        }
        public void Purchase()
        {

        }
        public void Sales()
        {

        }

       
            public void Handle(Screen screen)
            {

                if (screen is Product.ProductViewModel || screen is BussinessPartner.BussinessPartnerViewModel)
                {
                    ActivateItem(screen);
                }
            }
    }
}
