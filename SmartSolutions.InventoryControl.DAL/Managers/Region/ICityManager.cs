using SmartSolutions.InventoryControl.DAL.Models.Region;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Region
{
    public interface ICityManager
    {
        /// <summary>
        /// Get the All Cities From Country Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>List Of CityModels</returns>
        Task<IEnumerable<CityModel>> GetCitiesByCountryIdAsync(int? Id);
        /// <summary>
        /// Get the All Cities Of Country From Country Name
        /// </summary>
        /// <param name="countryName"></param>
        /// <returns>List Of CityModels</returns>
        Task<IEnumerable<CityModel>> GetCitiesByCountryNameAsync(string countryName);
        /// <summary>
        /// Get the City Detail From City Id
        /// </summary>
        /// <param name="cityId"> </param>
        /// <returns> List Of City Models</returns>
        Task<CityModel> GetCityFromIdAsync(int? cityId);
        /// <summary>
        /// Get All Matching Cities According To Search Text
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        Task<IEnumerable<CityModel>> GetCitiesAsync(string searchText);

        /// <summary>
        /// Add City or Town if not Added
        /// </summary>
        /// <param name="city"></param>
        /// <returns>Return True if add vice versa</returns>
        Task<bool> AddCityAsync(CityModel city);
        
    }
}
