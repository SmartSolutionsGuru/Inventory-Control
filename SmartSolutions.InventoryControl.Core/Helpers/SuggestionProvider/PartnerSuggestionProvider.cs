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
    [Export(typeof(ISuggestionProvider)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PartnerSuggestionProvider : ISuggestionProvider
    {
        #region Private Members
        private static List<DAL.Models.BussinessPartner.BussinessPartnerModel> myPartners;
        #endregion

        #region Properties
        public List<DAL.Models.BussinessPartner.BussinessPartnerModel> BussinessPartners { get; set; }
        #endregion

        #region Constructor
        [ImportingConstructor]
        public PartnerSuggestionProvider(List<DAL.Models.BussinessPartner.BussinessPartnerModel> partners)
        {
            BussinessPartners = partners;
            myPartners = partners;
        }
        #endregion
        public IEnumerable GetSuggestions(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) return null;
                if (BussinessPartners.Count < myPartners.Count)
                {
                    BussinessPartners.Clear();
                    BussinessPartners = myPartners;
                }
                BussinessPartners = BussinessPartners?.Where(p => p.FullName.ToLower().StartsWith(filter.ToLower())).ToList();
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return BussinessPartners;
        }
    }
}
