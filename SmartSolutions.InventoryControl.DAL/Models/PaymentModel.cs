using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSolutions.InventoryControl.DAL.Models
{
    public class PaymentModel : BaseModel
    {
        #region Constructor
        public PaymentModel()
        {
            PaymentType = new List<string> { "Cash", "Bank", "JazzCash", "Easy Paisa", "U Paisa", "Partial", "Other" };
        }
        #endregion

        #region Properties
        public BussinessPartnerModel SelectedPartner { get; set; }
        public string PaymentMode { get; set; }
        public List<string> PaymentType { get; set; }
        public int Amount { get; set; }
        public string Description { get; set; }
        public byte[] PaymentImage { get; set; }
        #endregion
    }
}
