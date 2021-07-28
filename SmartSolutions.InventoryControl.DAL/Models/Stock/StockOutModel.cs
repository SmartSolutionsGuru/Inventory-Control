using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSolutions.InventoryControl.DAL.Models.Inventory
{
    /// <summary>
    /// Class that is Responsible for holding Inventory/Stock Out
    /// </summary>
    public class StockOutModel : BaseModel
    {
        public BussinessPartnerModel Partner { get; set; }
    }
}
