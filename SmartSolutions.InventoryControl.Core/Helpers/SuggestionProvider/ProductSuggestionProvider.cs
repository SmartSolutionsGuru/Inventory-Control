using AutoCompleteTextBox.Editors;
using SmartSolutions.Util.LogUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SmartSolutions.InventoryControl.Core.Helpers.SuggestionProvider
{
    public class ProductSuggestionProvider : ISuggestionProvider
    {
        #region Properties
        public List<DAL.Models.Product.ProductModel> SuggestedProducts { get; set; }
        private static List<DAL.Models.Product.ProductModel> myProducts;
        #endregion

        #region [Constructor]
        public ProductSuggestionProvider(List<DAL.Models.Product.ProductModel> products)
        {
            SuggestedProducts = products;
            myProducts = products;
        }
        #endregion
        public IEnumerable GetSuggestions(string filter)
        {
            try
            {
                if(string.IsNullOrEmpty(filter)) return null;
                if(SuggestedProducts.Count < myProducts.Count)
                {
                    SuggestedProducts.Clear();
                    SuggestedProducts.AddRange(myProducts);
                }
                SuggestedProducts = SuggestedProducts.Where(p => p.Name.ToLower().StartsWith(filter)).ToList();
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return SuggestedProducts;
        }
    }
}
