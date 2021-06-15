using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSolutions.InventoryControl.DAL.Models.Inventory
{
    public class InvoiceModel : BaseModel
    {

        #region Constructor
        public InvoiceModel()
        {
            InvoiceGuid = Guid.NewGuid();
            PaymentType = new List<string> {"Unknown" ,"Cash", "Bank", "JazzCash", "Easy Paisa", "U Paisa", "Partial", "Other" };
            Products = new List<InventoryModel>();
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
        /// Verify the Transaction Type isPurchase Transaction
        /// </summary>
        public bool IsPurchaseInvoice { get; set; }
        /// <summary>
        /// Verify that ths is Sale Transaction
        /// </summary>
        public bool IsSaleInvoice { get; set; }
        /// <summary>
        /// Verify this is Purchase Return Transaction
        /// </summary>
        public bool IsPurchaseReturnInvoice { get; set; }
        /// <summary>
        /// Verify This is Sales Return Transaction
        /// </summary>
        public bool IsSaleReturnInvoice { get; set; }
        /// <summary>
        /// Verify in This Transaction Amount is Recived
        /// </summary>
        public bool IsAmountRecived { get; set; }
        /// <summary>
        /// Verify in This Transaction Amount is Paid
        /// </summary>
        public bool IsAmountPaid { get; set; }
        /// <summary>
        /// Transaction Type in String Like Purchase Or Sale Or Purchase Return etc..
        /// </summary>
        public string TransactionType { get; set; }
        /// <summary>
        /// Selected Bussiness Partner With Which Opreation is Performed
        /// </summary>
        public BussinessPartnerModel SelectedPartner { get; set; }
        /// <summary>
        /// Payment Types Like Jazz Cash Bank Cash etc...
        /// </summary>
        public List<string> PaymentType { get; set; }
        /// <summary>
        /// Selected Payment Type
        /// </summary>
        public string SelectedPaymentType { get; set; }
        /// <summary>
        /// Discount In Percent if Any
        /// </summary>
        public int PercentDiscount { get; set; }
        /// <summary>
        /// Discount Amiunt Exactly
        /// </summary>
        public double Discount { get; set; }
        /// <summary>
        /// List Of Products on Which Opreation is Performed
        /// </summary>
        public List<InventoryModel> Products { get; set; }
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
        public double Payment { get; set; }
        /// <summary>
        /// Grand Total Of transaction
        /// </summary>
        public double InvoiceTotal { get; set; }

        #endregion
    }
}
