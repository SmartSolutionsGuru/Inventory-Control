using Caliburn.Micro;
using SmartSolutions.InventoryControl.DAL.Managers.Bussiness_Partner;
using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.Core.ViewModels.Reports.BussinessPartner
{
    [Export(typeof(DisplayAllPartnersViewModel)), PartCreationPolicy(CreationPolicy.Shared)]
    public class DisplayAllPartnersViewModel : BaseViewModel, IHandle<object>
    {
        #region Private Members
        private readonly IBussinessPartnerManager _bussinessPartnerManager;
        private readonly IEventAggregator _eventAggregator;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public DisplayAllPartnersViewModel(IBussinessPartnerManager bussinessPartnerManager
                                          , IEventAggregator eventAggregator)
        {
            _bussinessPartnerManager = bussinessPartnerManager;
            _eventAggregator = eventAggregator;
        }
        #endregion

        #region Public Methods
        protected async override void OnActivate()
        {
            base.OnActivate();
            _eventAggregator.Subscribe(this);
            Handle(this.Parent);
            await GetPartnersByCategory();
            if (BussinessPartners != null && BussinessPartners.Count == 0)
            {
                IoC.Get<IDialogManager>().ShowMessageBox("There is No Business Partner Available At this Time", options: Dialogs.MessageBoxOptions.Ok);
            }
        }
        public async void Handle(object message)
        {
            if (message == null) return;
            if (message is ReportsViewModel)
            {
                var result = (ReportsViewModel)message;
                SelectedSubCategory = result.SelectedReportSubCategory;

            }
            else if (message is string)
            {
                SelectedSubCategory = (string)message;
                await GetPartnersByCategory();
            }

        }

        private async Task GetPartnersByCategory()
        {
            var partners = (await _bussinessPartnerManager.GetAllBussinessPartnersWithBalanceAsync()).ToList();
            if (SelectedSubCategory.Equals("Business Partner By Vendor"))
            {
                List<int?> partnerTypes = new List<int?> { 1, 3 };
                BussinessPartners = partners.Where(x => x.Partner?.PartnerType?.Id == 1 || x.Partner?.PartnerType?.Id == 3).ToList();
            }
            else if (SelectedSubCategory.Equals("Business Partner By Seller"))
            {
                List<int?> partnerTypes = new List<int?> { 2, 3 };
                BussinessPartners = partners.Where(x => x.Partner?.PartnerType?.Id == 2 || x.Partner?.PartnerType?.Id == 3).ToList();
            }
            else if (SelectedSubCategory.Equals("Business Partner By City"))
            {
                int cityId = 274;
                BussinessPartners = partners?.Where(x => x.Partner?.City?.Id == cityId).ToList();
            }
            else if (SelectedSubCategory.Equals("Business Partner By DR Balance"))
            {
                BussinessPartners = partners?.Where(x => x.CurrentBalanceType == DAL.Models.PaymentType.DR).ToList();
            }
            else if (SelectedSubCategory.Equals("Business Partner By CR Balance"))
            {
                BussinessPartners = partners?.Where(x => x.CurrentBalanceType == DAL.Models.PaymentType.CR).ToList();
            }
            else if (SelectedSubCategory.Equals("All Business Partners"))
            {
                BussinessPartners = (await _bussinessPartnerManager.GetAllBussinessPartnersWithBalanceAsync()).ToList();
            }
        }
        #endregion

        #region Properties
        public string SelectedSubCategory { get; set; }
        private List<BussinessPartnerLedgerModel> _BussinessPartners;

        public List<BussinessPartnerLedgerModel> BussinessPartners
        {
            get { return _BussinessPartners; }
            set { _BussinessPartners = value; NotifyOfPropertyChange(nameof(BussinessPartners)); }
        }
        private BussinessPartnerLedgerModel _SelectedPartner;

        public BussinessPartnerLedgerModel SelectedPartner
        {
            get { return _SelectedPartner; }
            set { _SelectedPartner = value; NotifyOfPropertyChange(nameof(SelectedPartner)); }
        }
        private string message;
        /// <summary>
        /// Message from Main
        /// </summary>
        public string Message
        {
            get { return message; }
            set { message = value; NotifyOfPropertyChange(nameof(Message)); }
        }

        #endregion
    }
}
