using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSolutions.InventoryControl.DAL.Models.Bank
{
    public class BankAccountModel : BaseModel
    {
        #region Constructor
        public BankAccountModel()
        {
            Branch = new BankBranchModel();
        }
        #endregion

        #region Properties
        public BankBranchModel Branch { get; set; }
        public string AccountType { get; set; }
        public string AccountStatus { get; set; }
        public DateTime? OpeningDate { get; set; }
        public string AccountNumber { get; set; }
        /// <summary>
        /// Opening Balance Of that Account
        /// </summary>
        public decimal? OpeningBalance { get; set; }
        public string  Description { get; set; }
        /// <summary>
        /// DR is Amount Is Recived in Transaction
        /// </summary>
        public decimal DR { get; set; }
        /// <summary>
        /// Amount is Paid in Transaction
        /// </summary>
        public decimal CR { get; set; }
        #endregion
    }
}
