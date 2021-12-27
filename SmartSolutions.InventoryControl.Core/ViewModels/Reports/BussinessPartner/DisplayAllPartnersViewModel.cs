using Caliburn.Micro;
using SmartSolutions.InventoryControl.DAL.Managers.Bussiness_Partner;
using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace SmartSolutions.InventoryControl.Core.ViewModels.Reports.BussinessPartner
{
    [Export(typeof(DisplayAllPartnersViewModel)),PartCreationPolicy(CreationPolicy.Shared)]
    public class DisplayAllPartnersViewModel : BaseViewModel
    {
        #region Private Members
        private readonly IBussinessPartnerManager _bussinessPartnerManager;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public DisplayAllPartnersViewModel(IBussinessPartnerManager bussinessPartnerManager)
        {
            _bussinessPartnerManager = bussinessPartnerManager;
        }
        #endregion

        #region Public Methods
        protected async override void OnActivate()
        {
            base.OnActivate();
            BussinessPartners = (await _bussinessPartnerManager.GetAllBussinessPartnersAsync()).OrderBy(x=>x.Name).ToList();
            if(BussinessPartners.Count == 0)
            {
                IoC.Get<IDialogManager>().ShowMessageBox("There is No Bussiness Partner Available At this Time",options: Dialogs.MessageBoxOptions.Ok);
            }
        }
        #endregion

        #region Properties
        private List<BussinessPartnerModel> _BussinessPartners;

        public List<BussinessPartnerModel> BussinessPartners
        {
            get { return _BussinessPartners; }
            set { _BussinessPartners = value;  NotifyOfPropertyChange(nameof(BussinessPartners)); }
        }
        private BussinessPartnerModel _SelectedPartner;

        public BussinessPartnerModel SelectedPartner
        {
            get { return _SelectedPartner; }
            set { _SelectedPartner = value; NotifyOfPropertyChange(nameof(SelectedPartner)); }
        }

        #endregion
    }
}
