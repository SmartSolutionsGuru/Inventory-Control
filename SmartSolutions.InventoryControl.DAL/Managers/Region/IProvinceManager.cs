using SmartSolutions.InventoryControl.DAL.Models.Region;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Region
{
    public interface IProvinceManager
    {
        Task<IEnumerable<ProvinceModel>> GetProvinceByCountryIdAsync(int? countryId);
    }
}
