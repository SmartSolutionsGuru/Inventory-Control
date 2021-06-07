using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSolutions.InventoryControl.DAL.Models.Purchase
{
    public class PurchaseTransactionModel:BaseModel
    {
        #region Constructor
        public PurchaseTransactionModel()
        {
            PurchaseTypes = new List<string> { "Purchase", "Purchase Return" };
            PaymentType = new List<string> { "Cash", "Bank", "JazzCash", "Easy Paisa", "U Paisa", "Partial", "Other" };
            Products = new List<PurchaseModel>();
        }
        #endregion

        #region Properties
        private int _TransactionNo;
          /// <summary>
          /// Bill No Or transaction NO
          /// </summary>
        public int TransactionNo
        {
            get { return _TransactionNo; }
            set { _TransactionNo = value; NotifyOfPropertyChange(nameof(TransactionNo)); }
        }

        public List<string> PurchaseTypes { get; set; }
        public string SelectedPurchaseType { get; set; }
        public BussinessPartnerModel SelectedPartner { get; set; }
        public List<string> PaymentType { get; set; }
        public string SelectedPaymentType { get; set; }
        public int Discount { get; set; }
        public List<PurchaseModel> Products { get; set; }
        public string Description { get; set; }
        public byte[] PaymentImage { get; set; }
        public double GrandTotal { get; set; }
        public double BalancePayment { get; set; }

        #endregion
    }
}
