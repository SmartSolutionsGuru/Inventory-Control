using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Bussiness_Partner
{
    public interface IPartnerLedgerManager
    {
        Task<BussinessPartnerLedgerModel> GetPartnerLedgerLastBalanceAsync(int partnerId);
        Task<bool> AddPartnerBalanceAsync(BussinessPartnerLedgerModel partnerLedger);
        Task<bool> UpdatePartnerCurrentBalanceAsync(BussinessPartnerLedgerModel partnerLedger);
        Task<IEnumerable<BussinessPartnerLedgerModel>> GetPartnerBalanceSheetAsync(int? PartnerId);
    }
}
