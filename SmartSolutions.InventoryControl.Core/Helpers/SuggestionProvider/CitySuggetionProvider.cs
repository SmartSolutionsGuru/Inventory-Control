using AutoCompleteTextBox.Editors;
using SmartSolutions.InventoryControl.DAL.Models.Region;
using SmartSolutions.Util.LogUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartSolutions.InventoryControl.Core.Helpers.SuggestionProvider
{
    public class CitySuggetionProvider : ISuggestionProvider
    {
        public List<CityModel> Cities { get; set; }
        #region Constructor
        public CitySuggetionProvider(List<CityModel> cities)
        {
            Cities = cities;
        }
        #endregion
        public IEnumerable GetSuggestions(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) return null;
                Cities = Cities?.Where(c => c.Name.ToLower().StartsWith(filter.ToLower())).ToList();
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return Cities;
        }
    }
}
