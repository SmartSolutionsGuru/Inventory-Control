using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Bussiness_Partner
{
    public interface IPartnerSetupAccountManager
    {
        Task<bool> SavePartnerSetAccountAsync(BussinessPartnerSetupAccountModel partnersetupAccount);
        Task<List<Dictionary<string,string>>> GenratePartnerAccountCodeAsync(string partnerType, int partnerId);
    }
}
