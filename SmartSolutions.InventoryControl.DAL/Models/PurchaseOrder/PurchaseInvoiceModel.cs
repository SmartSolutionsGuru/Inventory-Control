using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using SmartSolutions.InventoryControl.DAL.Models.Payments;
using SmartSolutions.InventoryControl.DAL.Models.Stock;
using System;
using System.Collections.Generic;

namespace SmartSolutions.InventoryControl.DAL.Models.PurchaseOrder
{
    public class PurchaseInvoiceModel : BaseModel
    {
        #region Constructor
        public PurchaseInvoiceModel()
        {
            InvoiceGuid = Guid.NewGuid();
            PaymentTypes = new List<PaymentTypeModel>();
            Products = new List<StockModel>();
        }
        #endregion

        #region Properties
        private string _InvoiceId;
        /// <summary>
        /// Unique Guid For Transaction
        /// </summary>
        public string InvoiceId
        {
            get { return _InvoiceId; }
            set { _InvoiceId = value; NotifyOfPropertyChange(nameof(InvoiceId)); }
        }
        /// <summary>
        /// Create new Guid For Each Transaction For Unique Record Keeping
        /// </summary>
        public Guid InvoiceGuid { get; set; }
        /// <summary>
        /// Selected Bussiness Partner With Which Opreation is Performed
        /// </summary>
        public BussinessPartnerModel SelectedPartner { get; set; }

        private List<PaymentTypeModel> _PaymentTypes;
        /// <summary>
        /// Payment Types Like Jazz Cash Bank Cash etc...
        /// </summary>
        public List<PaymentTypeModel> PaymentTypes
        {
            get { return _PaymentTypes; }
            set { _PaymentTypes = value; NotifyOfPropertyChange(nameof(PaymentTypes)); }
        }
        /// <summary>
        /// Selected Payment Type
        /// </summary>
        public PaymentTypeModel SelectedPaymentType { get; set; }
        /// <summary>
        /// Discount In Percent if Any
        /// </summary>
        public int PercentDiscount { get; set; }
        /// <summary>
        /// Discount Amiunt Exactly
        /// </summary>
        public decimal Discount { get; set; }
        /// <summary>
        /// List Of Products on Which Opreation is Performed
        /// </summary>
        public List<StockModel> Products { get; set; }
        /// <summary>
        /// Description If any
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Image Of Payment
        /// </summary>
        public byte[] PaymentImage { get; set; }
        /// <summary>
        /// Amount Of Payment which is Paid Or Recived
        /// </summary>
        public PaymentModel Payment { get; set; }
        /// <summary>
        /// Grand Total Of transaction
        /// </summary>
        public decimal InvoiceTotal { get; set; }
        #endregion
    }
}
