using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using SmartSolutions.InventoryControl.DAL.Models.Inventory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Bussiness_Partner
{
    public interface IPartnerLedgerManager
    {
        Task<BussinessPartnerLedgerModel> GetPartnerLedgerLastBalance(int partnerId);
        Task<bool> AddPartnerBalance(BussinessPartnerLedgerModel partnerLedger);
    }
}
