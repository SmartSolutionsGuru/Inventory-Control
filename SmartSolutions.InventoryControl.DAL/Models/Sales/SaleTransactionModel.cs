using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSolutions.InventoryControl.DAL.Models.Sales
{
    public class SaleTransactionModel : BaseModel
    {
        #region Constructor
        public SaleTransactionModel()
        {
            SalesType = new List<string> { "Sales", "Sales Return" };
            PaymentType = new List<string> { "Cash", "Bank", "JazzCash", "Easy Paisa", "U Paisa", "Partial", "Other" };
            Products = new List<SaleModel>();
        }
        #endregion

        #region Properties
        public List<string> SalesType { get; set; }
        public List<string> PaymentType { get; set; }
        public List<SaleModel> Products { get; set; }
        #endregion
    }
}
