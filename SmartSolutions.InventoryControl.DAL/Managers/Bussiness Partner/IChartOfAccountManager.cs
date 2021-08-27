using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Bussiness_Partner
{
    public interface IChartOfAccountManager
    {
        Task<IEnumerable<ChartOfAccountModel>> GetAllChatOfAccountsAsync();
        Task<ChartOfAccountModel> GetChartOfAccountByHeadingAsync(string heading);
        Task<IEnumerable<ChartOfAccountModel>> GetChartOfAccountByCategoryAsync(string category);
    }
}
