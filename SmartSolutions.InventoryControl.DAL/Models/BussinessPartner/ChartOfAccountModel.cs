using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SmartSolutions.InventoryControl.DAL.Models.BussinessPartner
{
    /// <summary>
    /// This Class Represents the Account which are used for 
    /// Formation Of Partner Account Code 
    /// </summary>
    public class ChartOfAccountModel : BaseModel
    {
        /// <summary>
        /// Account Head Like Assets,Fixed Assets,Liabalities etc...
        /// </summary>
        public string AccountCategory { get; set; }
        /// <summary>
        /// Liqued Assets Or Fixed Assets etc...
        /// </summary>
        public string AccountSubCategory { get; set; }
        /// <summary>
        /// Sub Heads Like Bank, Petty Cash etc..
        /// </summary>
        public AccountHeading AccountHeading { get; set; }
        /// <summary>
        /// Account Number Assiged Like  
        /// Assets from 1000 To 1200 
        ///  Liabalites 1500 to 1999 etc...
        /// </summary>
        public string AccountNumber { get; set; }
        /// <summary>
        /// Description Of  Account
        /// </summary>
        public string Description { get; set; }
    }
    /// <summary>
    /// Enum for Heading in Data base
    /// </summary>
    public enum AccountHeading
    {
        [Description("Cash In Hand")]
        CashInHand = 1,
        [Description("Bank")]
        Bank = 2,
        [Description("Petty Cash")]
        PettyCash = 3,
        [Description("Accounts Receivable")]
        AccountsReceivable = 4,
        [Description("Any Other Assets")]
        AnyOtherAssets = 5,
        [Description("Fixed Assets")]
        FixedAssets = 7,
        [Description("Office/Shop Equipments")]
        OfficeShopEquipments = 8,
        [Description("Motor Vehicle")]
        MotorVehicle = 9,
        [Description("Liabalities")]
        Liabalities = 10,
        [Description("Current Liablities")]
        CurrentLiablities = 11,
        [Description("Credit Card")]
        CreditCard = 12,
        [Description("Accounts Payable")]
        AccountsPayable = 13,
        [Description("Long term Liabalities")]
        LongtermLiabalities = 14,
        [Description("Bank Loan")]
        BankLoan = 15,
        [Description("Vehical Loan")]
        VehicalLoan = 16,
        [Description("Any Other Liabality")]
        AnyOtherLiabality = 16,
        [Description("Equity")]
        Equity = 17,
        [Description("Capital")]
        Capital = 18,
        [Description("Drawing")]
        Drawing = 19,
        [Description("Revenue Accounts")]
        RevenueAccounts = 20,
        [Description("Sale")]
        Sale = 21,
        [Description("Sale Return")]
        SaleReturn = 22,
        [Description("Labour")]
        Labour= 23,
        [Description("Materials")]
        Materials = 24,
        [Description("Bank Intrest")]
        BankIntrest = 25,
        [Description("Commisions")]
        SaleCommisions = 26,
        [Description("Sale Discount")]
        SaleDiscount = 27,
        [Description("Purchase")]
        Purchase = 28,
        [Description("Row Material Purchase")]
        RowMaterialPurchase = 29,
        [Description("Commisions")]
        PurchaseCommisions = 30,
        [Description("Freight")]
        Freight = 31,
        [Description("Purchase Return")]
        PurchaseReturn = 32,
        [Description("Expenses")]
        Expenses = 33,
        [Description("Cost of Goods Sold")]
        CostofGoodsSold = 34,
        [Description("Advertising")]
        Advertising = 35,
        [Description("Bank Fees")]
        BankFees = 36,
        [Description("Genral Expenses")]
        GenralExpenses = 37,
        [Description("Internet")]
        Internet = 38,
        [Description("Stationery")]
        Stationery = 39,
        [Description("Subscription")]
        Subscription = 40,
        [Description("Telephone")]
        Telephone  = 41,
    }
}
