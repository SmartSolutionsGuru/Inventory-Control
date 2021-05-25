using SmartSolutions.InventoryControl.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Bussiness_Partner
{
    public interface IBussinessPartnerManager
    {
        Task<bool> AddBussinesPartner(BussinessPartnerModel partner);
        Task<IEnumerable<BussinessPartnerModel>> GetAllBussinessPartners();
        Task<BussinessPartnerModel> GetBussinessPartner(int? Id);
        Task<bool> UpdateBussinessPartner(int? Id);
        Task<bool> RemoveBussinessPartner(int? Id);
    }
}
