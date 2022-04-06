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
        #region [Private Fields]
        private static List<CityModel> myCities;
        #endregion

        #region [Public Properties]
        public List<CityModel> Cities { get; set; }

        #endregion

        #region Constructor
        public CitySuggetionProvider(List<CityModel> cities)
        {
            Cities = cities;
            myCities = cities;
        }
        #endregion

        #region [Methods]
        public IEnumerable GetSuggestions(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) return null;
                if (Cities.Count < myCities.Count)
                {
                    Cities.Clear();
                    Cities.AddRange(myCities);
                }
                Cities = Cities?.Where(c => c.Name.ToLower().StartsWith(filter.ToLower())).ToList();
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return Cities;
        }
        #endregion
    }
}
