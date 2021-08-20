using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Bussiness_Partner
{
    public interface IPartnerTypeManager
    {
        Task<IEnumerable<BussinessPartnerTypeModel>> GetPartnerTypesAsync();
        Task<BussinessPartnerTypeModel> GetPartnerTypeByIdAsync(int? Id);
    }
}
