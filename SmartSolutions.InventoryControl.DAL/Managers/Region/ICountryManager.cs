using SmartSolutions.InventoryControl.DAL.Models.Region;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Region
{
    public interface ICountryManager
    {
        Task<IEnumerable<CountryModel>> GetCountriesAsync();
        Task<CountryModel> GetCountryByIdAsync(int? Id);
        Task<CountryModel> GetCountryByNameAsync(string countryName);
    }
}
