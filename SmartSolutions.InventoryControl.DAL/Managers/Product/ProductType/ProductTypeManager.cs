using SmartSolutions.InventoryControl.DAL.Models;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.LogUtils;
using SmartSolutions.Util.NumericUtils;
using SmartSolutions.Util.DateAndTimeUtils;
using SmartSolutions.Util.BooleanUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using SmartSolutions.Util.DictionaryUtils;
using SmartSolutions.InventoryControl.DAL.Models.Product;

namespace SmartSolutions.InventoryControl.DAL.Managers.Product.ProductType
{
    [Export(typeof(IProductTypeManager)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ProductTypeManager : BaseManager, IProductTypeManager
    {
        #region Private Members
        private readonly IRepository Repository;
        #endregion

        #region Default Constructor
        [ImportingConstructor]
        public ProductTypeManager()
        {
            Repository = GetRepository<ProductTypeModel>();
        }
        #endregion

        #region Public Methods
 
        public async Task<bool> AddProductTypeAsync(ProductTypeModel model)
        {
            bool retVal = false;
            string query = string.Empty;
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            try
            {
                parameters["@v_Name"] = model.Name == null ? DBNull.Value : (object)model.Name;
                parameters["@v_IsActive"] = model.IsActive = true;
                parameters["@v_CreatedAt"] = model.CreatedAt == null ? DateTime.Now : model.CreatedAt;
                parameters["@v_CreatedBy"] = model.CreatedBy == null ? DBNull.Value : (object)model.CreatedBy;
                parameters["@v_UpdatedAt"] = model.UpdatedAt == null ? DBNull.Value : (object)model.UpdatedAt;
                parameters["@v_UpdatedBy"] = model.UpdatedBy == null ? DBNull.Value : (object)model.UpdatedBy;
                query = @"INSERT INTO ProductType
                                     (Name
                                     ,IsActive
                                     ,CreatedAt
                                     ,CreatedBy
                                     ,UpdatedAt
                                     ,UpdatedBy)
                             VALUES
                                   (@v_Name
                                   ,@v_IsActive
                                   ,@v_CreatedAt
                                   ,@v_CreatedBy
                                   ,@v_UpdatedAt
                                   ,@v_UpdatedBy)";


                await Repository.QueryAsync(query: query, parameters: parameters);
                //await Repository.QueryAsync(filename: "AddProductType", parameters: parameters);
                retVal = true;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString());
            }
            return retVal;
        }
        public async Task<ProductTypeModel> GetProductTypeById(int? Id)
        {
            ProductTypeModel model = new ProductTypeModel();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["@v_Id"] = Id;
            try
            {
                var query = $"SELECT * FROM ProductType WHERE Id = @v_Id";
                var values = await Repository.QueryAsync(query: query, parameters: parameters);
                if (values != null)
                {
                    foreach (var value in values)
                    {
                        model.Name = value?.GetValueFromDictonary("Name")?.ToString();
                        model.IsActive = value?.GetValueFromDictonary("IsActive")?.ToString()?.ToNullableBoolean();
                    }
                }
            }
            catch (Exception ex)
            {

                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return model;
        }
        public async Task<IEnumerable<ProductTypeModel>> GetAllProductsTypesAsync(string searchText = null)
        {
            List<ProductTypeModel> productTypes = new List<ProductTypeModel>();
            string query = string.Empty;
            try
            {
                query = @"Select * FROM ProductType Where IsActive=1";
                var values = new List<Dictionary<string, object>>();
                values = await Repository.QueryAsync(query: query);

                if (values != null || values?.Count > 0)
                {
                    foreach (var value in values)
                    {
                        var model = new ProductTypeModel();
                        model.Id = value.GetValueFromDictonary("Id").ToString().ToInt();
                        model.Name = value.GetValueFromDictonary("Name").ToString();
                        model.CreatedAt = Convert.ToDateTime(value.GetValueFromDictonary("CreatedAt"));
                        model.CreatedBy = value?.GetValueFromDictonary("CreatedBy")?.ToString();
                        model.UpdatedAt = value?.GetValueFromDictonary("UpdatedAt")?.ToString().ToNullableDateTime();
                        model.UpdatedBy = value?.GetValueFromDictonary("UpdatedBy")?.ToString();
                        productTypes.Add(model);
                    }
                }
                else
                    return productTypes;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString());
            }
            return productTypes;
        }
        public async Task<bool> RemoveProductType(int? Id)
        {
            bool retVal = false;
            try
            {
                string query = string.Empty;
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["2v_Id"] = Id;
                query = @"UPDATE ProductType SET IsActive = 0 WHERE Id= @v_Id";
                await Repository.QueryAsync(query,parameters:parameters);
                retVal = true;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }

        #endregion
    }
}
