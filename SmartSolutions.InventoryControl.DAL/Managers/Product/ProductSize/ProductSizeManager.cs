using SmartSolutions.InventoryControl.DAL.Models.Product;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.BooleanUtils;
using SmartSolutions.Util.DateAndTimeUtils;
using SmartSolutions.Util.DictionaryUtils;
using SmartSolutions.Util.LogUtils;
using SmartSolutions.Util.NumericUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Product.ProductSize
{
    [Export(typeof(IProductSizeManager)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ProductSizeManager : BaseManager, IProductSizeManager
    {
        #region Private Members
        private readonly IRepository Repository;
        #endregion

        #region Constructor
        public ProductSizeManager()
        {
            Repository = GetRepository<ProductSizeModel>();
        }
        #endregion
        public async Task<bool> AddProductSizeAsync(ProductSizeModel model)
        {
            bool retVal = false;
            try
            {
                string query = string.Empty;
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_Size"] = model.Size;
                parameters["@v_Name"] = model.Name == null ? DBNull.Value : (object)model.Name;
                parameters["@v_IsActive"] = model.IsActive = true;
                parameters["@v_CreatedAt"] = model.CreatedAt == null ? DateTime.Now : model.CreatedAt;
                parameters["@v_CreatedBy"] = model.CreatedBy == null ? DBNull.Value : (object)model.CreatedBy;
                parameters["@v_UpdatedAt"] = model.UpdatedAt == null ? DBNull.Value : (object)model.UpdatedAt;
                parameters["@v_UpdatedBy"] = model.UpdatedBy == null ? DBNull.Value : (object)model.UpdatedBy;
                query = @"INSERT INTO ProductSize(Name,Size,IsActive,CreatedAt,CreatedBy,UpDatedAt,UpdatedBy)
                                            VALUES(@v_Name,@v_Size,@v_IsActive,@v_CreatedAt,@v_CreatedBy,@v_UpdatedAt,@v_UpdatedBy)";
                await Repository.QueryAsync(query: query, parameters: parameters);
                retVal = true;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }

        public async Task<IEnumerable<ProductSizeModel>> GetProductAllSizeAsync()
        {
            List<ProductSizeModel> productSizes = new List<ProductSizeModel>(); 
            try
            {
                string query = @"SELECT * FROM ProductSize WHERE IsActive = 1";
                var values =await Repository.QueryAsync(query);
                if(values != null)
                {
                    foreach (var value in values)
                    {
                        var model = new ProductSizeModel();
                        model.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                        model.Name = value?.GetValueFromDictonary("Name")?.ToString();
                        model.Size = value?.GetValueFromDictonary("Size")?.ToString();
                        model.IsActive = value?.GetValueFromDictonary("IsActive")?.ToString()?.ToNullableBoolean();
                        model.CreatedAt = value?.GetValueFromDictonary("CreatedAt")?.ToString()?.ToNullableDateTime();
                        model.CreatedBy = value?.GetValueFromDictonary("CreatedBy")?.ToString();
                        model.UpdatedAt = value?.GetValueFromDictonary("UpdatedAt")?.ToString()?.ToNullableDateTime();
                        model.UpdatedBy = value?.GetValueFromDictonary("UpdatedBy")?.ToString();
                        productSizes.Add(model);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return productSizes;
        }

        public async Task<ProductSizeModel> GetProductSizeByIdAsync(int? Id)
        {
            var model = new ProductSizeModel();
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["v_Id"] = Id;
                string query = @"SELECT * FROM ProductSize WHERE Id = @v_Id AND IsActive = 1";
                var values = await Repository.QueryAsync(query,parameters:parameters);
                if(values != null)
                {
                    foreach (var value in values)
                    {
                        model.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                        model.Name = value?.GetValueFromDictonary("Name")?.ToString();
                        model.Size = value?.GetValueFromDictonary("Size")?.ToString();
                        model.IsActive = value?.GetValueFromDictonary("IsActive")?.ToString()?.ToNullableBoolean();
                        model.CreatedAt = value?.GetValueFromDictonary("CreatedAt")?.ToString()?.ToNullableDateTime();
                        model.CreatedBy = value?.GetValueFromDictonary("CreatedBy")?.ToString();
                        model.UpdatedAt = value?.GetValueFromDictonary("UpdatedAt")?.ToString()?.ToNullableDateTime();
                        model.UpdatedBy = value?.GetValueFromDictonary("UpdatedBy")?.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return model;
        }

        public async Task<bool> RemoveProductSizeAsync(int? Id)
        {
            bool retVal = false;
            try
            {
                string query = @"";
                await Repository.QueryAsync(query);
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }

        public async Task<bool> UpdateProductSizeAsync(ProductSizeModel model)
        {
            bool retVal = false;
            try
            {
                string query = @"";
                await Repository.QueryAsync(query);
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }
    }
}
