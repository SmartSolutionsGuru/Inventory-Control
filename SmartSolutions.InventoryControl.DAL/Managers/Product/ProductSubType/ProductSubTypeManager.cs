using SmartSolutions.InventoryControl.DAL.Models.Product;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.DateAndTimeUtils;
using SmartSolutions.Util.DictionaryUtils;
using SmartSolutions.Util.LogUtils;
using SmartSolutions.Util.NumericUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Product.ProductSubType
{
    [Export(typeof(IProductSubTypeManager)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ProductSubTypeManager : BaseManager, IProductSubTypeManager
    {
        #region Private Members
        private readonly IRepository Repository;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public ProductSubTypeManager()
        {
            Repository = GetRepository<ProductSubTypeModel>();
        }
        #endregion

        #region Product Sub Type
        public async Task<IEnumerable<ProductSubTypeModel>> GetAllProductSubTypeAsync(int? Id = null)
        {
            List<ProductSubTypeModel> ProductSubTypeList = new List<ProductSubTypeModel>();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["@v_Id"] = Id;
            try
            {
                string query = string.Empty;
                if (Id == null)
                    query = @"SELECT * FROM ProductSubType WHERE IsActive = 1 AND IsDeleted = 0";
                else
                    query = @"SELECT * FROM ProductSubType WHERE ProductTypeId = @v_Id AND  IsActive = 1 AND IsDeleted = 0";
                var values = await Repository.QueryAsync(query, parameters: parameters);
                if (values != null || values?.Count > 0)
                {
                    foreach (var value in values)
                    {
                        var model = new ProductSubTypeModel();
                        model.Id = value.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                        model.ProductTypeId = value?.GetValueFromDictonary("ProductTypeId")?.ToString()?.ToInt();
                        model.Name = value?.GetValueFromDictonary("Name")?.ToString();
                        model.CreatedAt = Convert.ToDateTime(value.GetValueFromDictonary("CreatedAt"));
                        model.CreatedBy = value?.GetValueFromDictonary("CreatedBy")?.ToString();
                        model.UpdatedAt = value?.GetValueFromDictonary("UpdatedAt")?.ToString().ToNullableDateTime();
                        model.UpdatedBy = value?.GetValueFromDictonary("UpdatedBy")?.ToString();
                        ProductSubTypeList.Add(model);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return ProductSubTypeList;
        }

        public async Task<ProductSubTypeModel> GetProductSubTypeById(int? Id)
        {
            var model = new ProductSubTypeModel();
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_Id"] = Id;
                string query = @"SELECT * FROM ProductSubType WHERE Id = @v_Id AND  IsActive = 1 AND IsDeleted = 0";
                var values = await Repository.QueryAsync(query, parameters: parameters);
                if (values != null || values?.Count > 0)
                {
                    foreach (var value in values)
                    {
                        model.Id = value.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                        model.ProductTypeId = value?.GetValueFromDictonary("ProductTypeId")?.ToString()?.ToInt();
                        model.Name = value?.GetValueFromDictonary("Name")?.ToString();
                        model.CreatedAt = Convert.ToDateTime(value.GetValueFromDictonary("CreatedAt"));
                        model.CreatedBy = value?.GetValueFromDictonary("CreatedBy")?.ToString();
                        model.UpdatedAt = value?.GetValueFromDictonary("UpdatedAt")?.ToString().ToNullableDateTime();
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

        public async Task<bool> AddProductSubTypeAsync(int? productId, ProductSubTypeModel model)
        {
            bool retVal = false;
            try
            {
                if (productId != null || productId > 0)
                {
                    string query = string.Empty;
                    Dictionary<string, object> parameters = new Dictionary<string, object>();
                    parameters["@v_ProductTypeId"] = productId;
                    parameters["@v_Name"] = model?.Name;
                    parameters["@v_IsActive"] = model.IsActive = true;
                    parameters["@v_IsDeleted"] = model.IsDeleted = false;
                    parameters["@v_CreatedAt"] = model.CreatedAt == null ? DateTime.Now : model.CreatedAt;
                    parameters["@v_CreatedBy"] = model.CreatedBy == null ? DBNull.Value : (object)model.CreatedBy;
                    parameters["@v_UpdatedAt"] = model.UpdatedAt == null ? DBNull.Value : (object)model.UpdatedAt;
                    parameters["@v_UpdatedBy"] = model.UpdatedBy == null ? DBNull.Value : (object)model.UpdatedBy;
                    query = @"INSERT INTO ProductSubType 
                             (ProductTypeId,Name,IsActive,IsDeleted,CreatedAt,CreatedBy,UpdatedAt,UpdatedBy)
                            VALUES(@v_ProductTypeId,@v_Name,@v_IsActive ,@v_IsDeleted ,@v_CreatedAt,@v_CreatedBy,@v_UpdatedAt,@v_UpdatedBy)";
                    await Repository.QueryAsync(query, parameters: parameters);
                    retVal = true;
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }

        public async Task<bool> RemoveProductSubTypeAsync(int? Id)
        {
            bool retVal = false;
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_Id"] = Id;
                string query = string.Empty;
                query = @"UPDATE ProductSubType SET IsActive = 0,IsDeleted = 1 WHERE Id = @v_Id";
                await Repository.QueryAsync(query,parameters:parameters);
                retVal = true;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }

        public IEnumerable<ProductSubTypeModel> GetAllProductSubType(int? Id)
        {
            List<ProductSubTypeModel> ProductSubTypeList = new List<ProductSubTypeModel>();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["@v_Id"] = Id;
            try
            {
                string query = string.Empty;
                if (Id == null)
                    query = @"SELECT * FROM ProductSubType WHERE IsActive = 1 AND IsDeleted = 0";
                else
                    query = @"SELECT * FROM ProductSubType WHERE ProductTypeId = @v_Id AND  IsActive = 1 AND IsDeleted = 0";
                var values =  Repository.Query(query, parameters: parameters);
                if (values != null || values?.Count > 0)
                {
                    foreach (var value in values)
                    {
                        var model = new ProductSubTypeModel();
                        model.Id = value.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                        model.ProductTypeId = value?.GetValueFromDictonary("ProductTypeId")?.ToString()?.ToInt();
                        model.Name = value?.GetValueFromDictonary("Name")?.ToString();
                        model.CreatedAt = Convert.ToDateTime(value.GetValueFromDictonary("CreatedAt"));
                        model.CreatedBy = value?.GetValueFromDictonary("CreatedBy")?.ToString();
                        model.UpdatedAt = value?.GetValueFromDictonary("UpdatedAt")?.ToString().ToNullableDateTime();
                        model.UpdatedBy = value?.GetValueFromDictonary("UpdatedBy")?.ToString();
                        ProductSubTypeList.Add(model);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return ProductSubTypeList;
        }
        #endregion
    }
}
