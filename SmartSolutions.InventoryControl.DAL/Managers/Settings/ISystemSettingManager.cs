using SmartSolutions.InventoryControl.DAL.Models;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Settings
{
    public interface ISystemSettingManager
    {
        Task<SystemSettingModel> GetsystemSettingByKeyAsync(string key);
        Task<SystemSettingModel> GetSystemSettingById(int? Id);  
        Task<bool> SaveSettingAsync(SystemSettingModel setting);
    }
}
