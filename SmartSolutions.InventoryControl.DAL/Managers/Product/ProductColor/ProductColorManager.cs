using SmartSolutions.InventoryControl.DAL.Models;
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
using System.Text;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Product.ProductColor
{
    [Export(typeof(IProductColorManager)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ProductColorManager : BaseManager, IProductColorManager
    {
        #region Private Members
        private readonly IRepository Repository;
        #endregion

        #region Constructor
        public ProductColorManager()
        {
            Repository = GetRepository<ProductColorModel>();
        }
        #endregion
        public async Task<bool> AddProductColorAsync(ProductColorModel model)
        {
            bool retVal = false;
            try
            {
                if (model != null)
                {
                    string query = string.Empty;
                    Dictionary<string, object> parameters = new Dictionary<string, object>();
                    parameters["@v_Color"] = model.Color;
                    parameters["@v_Name"] = model?.Name ?? model.Color;
                    parameters["@v_IsActive"] = model.IsActive = true;
                    parameters["@v_CreatedAt"] = model.CreatedAt == null ? DateTime.Now : model.CreatedAt;
                    parameters["@v_CreatedBy"] = model.CreatedBy == null ? DBNull.Value : (object)model.CreatedBy;
                    parameters["@v_UpdatedAt"] = model.UpdatedAt == null ? DBNull.Value : (object)model.UpdatedAt;
                    parameters["@v_UpdatedBy"] = model.UpdatedBy == null ? DBNull.Value : (object)model.UpdatedBy;
                    query = @"INSERT INTO ProductColor 
                             (Color,Name,IsActive,CreatedAt,CreatedBy,UpdatedAt,UpdatedBy)
                            VALUES(@v_Color,@v_Name,@v_IsActive ,@v_CreatedAt,@v_CreatedBy,@v_UpdatedAt,@v_UpdatedBy)";
                    var result = await Repository.NonQueryAsync(query, parameters: parameters);
                    retVal = result > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }

        public async Task<IEnumerable<ProductColorModel>> GetProductAllColorsAsync()
        {
            List<ProductColorModel> ProductColors = new List<ProductColorModel>();
            try
            {
                string query = string.Empty;
                query = @"SELECT * FROM ProductColor WHERE IsActive = 1";
                var values = await Repository.QueryAsync(query: query);
                if (values != null)
                {
                    foreach (var value in values)
                    {
                        var model = new ProductColorModel();
                        model.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToNullableInt();
                        model.Name = value?.GetValueFromDictonary("Name")?.ToString()?.ToString();
                        model.Color = value?.GetValueFromDictonary("Color")?.ToString();
                        model.IsActive = value?.GetValueFromDictonary("IsActive")?.ToString()?.ToNullableBoolean();
                        model.CreatedAt = value?.GetValueFromDictonary("CreatedAt")?.ToString()?.ToNullableDateTime();
                        model.CreatedBy = value?.GetValueFromDictonary("CreatedBy")?.ToString();
                        model.UpdatedAt = value?.GetValueFromDictonary("UpdatedAt")?.ToString()?.ToNullableDateTime();
                        model.UpdatedBy = value?.GetValueFromDictonary("UpdatedBy")?.ToString();
                        ProductColors.Add(model);
                    }
                }

            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return ProductColors;
        }

        public async Task<ProductColorModel> GetProductColor(int Id)
        {
            var model = new ProductColorModel();
            try
            {
                string query = string.Empty;
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_Id"] = Id;
                query = @"SELECT * FROM ProductColor WHERE Id = @v_Id IsActive = 1";
                var values = await Repository.QueryAsync(query: query);
                if (values != null)
                {
                    foreach (var value in values)
                    {
                        model.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToNullableInt();
                        model.Name = value?.GetValueFromDictonary("Name")?.ToString()?.ToString();
                        model.Color = value?.GetValueFromDictonary("Color")?.ToString();
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

        public async Task<bool> RemoveProductColorAsync(int? Id)
        {
            bool retVal = false;
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_Id"] = Id;
                string query = string.Empty;
                query = @"UPDATE ProductColor SET IsActive = 0 WHERE Id = @v_Id";
                await Repository.QueryAsync(query: query, parameters: parameters);
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }

        public async Task<ProductColorModel> UpdateProductColorAsync(ProductColorModel model)
        {
            try
            {
                if (model != null)
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object>();
                    parameters["@v_Id"] = model.Id;
                    parameters["@v_Name"] = model.Name;
                    parameters["@v_Color"] = model.Color;
                    parameters["@v_IsActive"] = model.IsActive;
                    parameters["@v_CreatedAt"] = model.CreatedAt;
                    parameters["@v_CreatedBy"] = model.CreatedBy;
                    parameters["@v_UpdatedAt"] = DateTime.Now;
                    parameters["@v_UpdatedBy"] = model.UpdatedBy;
                    string query = string.Empty;
                    query = @"UPDATE ProductColor SET Name,Color,IsActive,CreatedAt,CreatedBy,UpdatedAt,UpdatedBy VALUES";
                    await Repository.QueryAsync(query: query, parameters: parameters);
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return model;
        }
    }
}
