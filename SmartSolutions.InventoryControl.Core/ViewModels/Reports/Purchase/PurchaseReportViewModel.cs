using Caliburn.Micro;
using SmartSolutions.InventoryControl.DAL;
using SmartSolutions.InventoryControl.DAL.Models.PurchaseOrder;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.Core.ViewModels.Reports.Purchase
{
    [Export(typeof(PurchaseReportViewModel)),PartCreationPolicy(CreationPolicy.Shared)]
    public class PurchaseReportViewModel : BaseViewModel, IHandle<object>
    {
        #region Private Members
        private readonly DAL.Managers.Purchase.IPurchaseOrderManager _purchaseOrderManager;
        private readonly DAL.Managers.Purchase.IPurchaseOrderDetailManager _purchaseOrderDetailManager;
        private readonly DAL.Managers.Invoice.IPurchaseInvoiceManager _invoiceManager;
        private readonly IEventAggregator _eventAggregator;
        #endregion

        #region Constructor
        public PurchaseReportViewModel() { }
       
        [ImportingConstructor]
        public PurchaseReportViewModel(DAL.Managers.Purchase.IPurchaseOrderManager purchaseOrderManager
                                    ,DAL.Managers.Purchase.IPurchaseOrderDetailManager purchaseOrderDetailManager
                                    ,DAL.Managers.Invoice.IPurchaseInvoiceManager purchaseInvoiceManager
                                    ,IEventAggregator eventAggregator)
        {
            _purchaseOrderManager = purchaseOrderManager;
            _purchaseOrderDetailManager = purchaseOrderDetailManager;
            _invoiceManager = purchaseInvoiceManager;
            _eventAggregator = eventAggregator;
        }
        #endregion

        #region Methods
        protected override void OnInitialize()
        {
            base.OnInitialize();
        }
        protected async override void OnActivate()
        {
            base.OnActivate();
            _eventAggregator.Subscribe(this);
            Handle(this.Parent);
            await GetPurchaseByCategory();
        }
        private async Task GetPurchaseByCategory()
        {
            var result = (await _invoiceManager.GetAllPurchaseInvoicesAsync()).ToList();
            if(SelectedSubCategory.Equals("All Purchase"))
            {
                if (result.Count > 0)
                {
                    Purchases = new List<DisplayPurchase>();
                    decimal total = 0;
                    foreach (var item in result)
                    {
                        DisplayPurchase model = new DisplayPurchase();
                        model.InvoiceId = item.InvoiceId;
                        model.InvoiceTotal = item.InvoiceTotal.ToString();
                        model.PartnerName = item.SelectedPartner?.Name;
                        model.CreatedDate = item.CreatedAt;
                        model.Discount = item.Discount;
                        total = (total + item.InvoiceTotal);
                        model.Total = total;
                        Purchases.Add(model);
                    }
                }
            }
            else if(SelectedSubCategory.Equals("Purchase Of Current Month"))
            {
                var date = DateTime.Now;    
                var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
                var filterdResult = result.Where(x => x.CreatedAt >= firstDayOfMonth).ToList();
                if (filterdResult.Count > 0)
                {
                    Purchases = new List<DisplayPurchase>();
                    decimal total = 0;
                    foreach (var item in filterdResult)
                    {
                        DisplayPurchase model = new DisplayPurchase();
                        model.InvoiceId = item.InvoiceId;
                        model.InvoiceTotal = item.InvoiceTotal.ToString();
                        model.PartnerName = item.SelectedPartner?.Name;
                        model.CreatedDate = item.CreatedAt;
                        model.Discount = item.Discount;
                        total = (total + item.InvoiceTotal);
                        model.Total = total;
                        Purchases.Add(model);
                    }
                }
            }
            else if(SelectedSubCategory.Equals("Purchase By Starting Date"))
            {

            }
            else if (SelectedSubCategory.Equals("Purchase By Ending Date"))
            {

            }
            else if (SelectedSubCategory.Equals("Purchase Of Specific Date"))
            {

            }
            else if (SelectedSubCategory.Equals("Purchase By Specific Vendor"))
            {

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
                await GetPurchaseByCategory();
            }

        }
        #endregion

        #region Properties
        public string SelectedSubCategory { get; set; }
        private List<DisplayPurchase> _Purchases;

        public List<DisplayPurchase> Purchases
        {
            get { return _Purchases; }
            set { _Purchases = value; NotifyOfPropertyChange(nameof(Purchases)); }
        }
        private PurchaseInvoiceModel _SelectedPurchase;

        public PurchaseInvoiceModel SelectedPurchase
        {
            get { return _SelectedPurchase; }
            set { _SelectedPurchase = value; NotifyOfPropertyChange(nameof(SelectedPurchase)); }
        }
        #endregion

        #region Private Class
        public class DisplayPurchase
        {
            public string InvoiceId { get; set; }
            public string InvoiceTotal { get; set; }
            public string PartnerName { get; set; }
            public DateTime? CreatedDate { get; set; }
            public decimal Discount { get; set; }
            public decimal Total { get; set; }
        }
        #endregion
    }
}
