using SmartSolutions.InventoryControl.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Bussiness_Partner
{
    public interface IBussinessPartnerManager
    {
        Task<bool> AddBussinesPartnerAsync(BussinessPartnerModel partner);
        Task<IEnumerable<BussinessPartnerModel>> GetAllBussinessPartnersAsync();
        Task<BussinessPartnerModel> GetBussinessPartnerAsync(int? Id);
        Task<bool> UpdateBussinessPartnerAsync(BussinessPartnerModel model);
        Task<bool> RemoveBussinessPartnerAsync(int? Id);
    }
}
