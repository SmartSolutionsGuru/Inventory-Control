using AutoCompleteTextBox.Editors;
using SmartSolutions.Util.LogUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace SmartSolutions.InventoryControl.Core.Helpers.SuggestionProvider
{
    [Export(typeof(ISuggestionProvider)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class PartnerSuggestionProvider : ISuggestionProvider
    {
        #region Private Members
      
        #endregion
        #region Properties
        public List<DAL.Models.BussinessPartnerModel> BussinessPartners { get; set; }
        #endregion

        #region Constructor
        [ImportingConstructor]
        public PartnerSuggestionProvider(List<DAL.Models.BussinessPartnerModel> partners)
        {
            BussinessPartners = partners;
        }
        #endregion
        public IEnumerable GetSuggestions(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) return null;
                BussinessPartners = BussinessPartners?.Where(p => p.FullName.ToLower().Contains(filter.ToLower())).ToList();
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return BussinessPartners;
        }
    }
}
