using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Bussiness_Partner
{
    public interface IPartnerLedgerManager
    {
        Task<BussinessPartnerLedgerModel> GetPartnerLedgerLastBalance(int partnerId);
        Task<bool> AddPartnerBalance(BussinessPartnerLedgerModel partnerLedger);
        Task<bool> UpdatePartnerCurrentBalance(BussinessPartnerLedgerModel partnerLedger);
    }
}
