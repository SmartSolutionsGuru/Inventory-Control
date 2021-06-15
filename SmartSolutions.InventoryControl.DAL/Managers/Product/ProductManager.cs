﻿using SmartSolutions.InventoryControl.DAL.Models.Product;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System;
using SmartSolutions.Util.LogUtils;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.DictionaryUtils;
using SmartSolutions.Util.NumericUtils;
using SmartSolutions.Util.BooleanUtils;
using SmartSolutions.Util.DateAndTimeUtils;
using System.Linq;

namespace SmartSolutions.InventoryControl.DAL.Managers.Product
{
    [Export(typeof(IProductManager)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ProductManager : BaseManager, IProductManager
    {
        #region Private Members
        private readonly IRepository Repository;
        #endregion

        #region Constructor
        public ProductManager()
        {
            Repository = GetRepository<ProductModel>();
        }
        #endregion

        #region Public Methods
        public async Task<bool> AddProductAsync(ProductModel model)
        {
            bool retVal = false;
            try
            {
                string query = string.Empty;
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                if (model != null)
                {
                    parameters["@v_Name"] = model?.Name;
                    parameters["@v_ProductTypeId"] = model.ProductType?.Id;
                    parameters["@v_ProductSubTypeId"] = model?.ProductSubType?.Id;
                    parameters["@v_ProductColorId"] = model?.ProductColor?.Id;
                    parameters["@v_ProductSizeId"] = model?.ProductSize?.Id;
                    parameters["@v_Image"] = model.Image == null ? DBNull.Value : (object)model.Image;
                    parameters["@v_IsActive"] = model.IsActive = true;
                    parameters["@v_IsDeleted"] = model.IsDeleted = false;
                    parameters["@v_CreatedAt"] = model.CreatedAt == null ? DateTime.Now : model.CreatedAt;
                    parameters["@v_CreatedBy"] = model.CreatedBy == null ? DBNull.Value : (object)model.CreatedBy;
                    parameters["@v_UpdatedAt"] = model.UpdatedAt == null ? DBNull.Value : (object)model.UpdatedAt;
                    parameters["@v_UpdatedBy"] = model.UpdatedBy == null ? DBNull.Value : (object)model.UpdatedBy;
                }
                query = @"INSERT INTO Product (Name,ProductTypeId,ProductSubTypeId,ProductColorId,ProductSizeId,Image,IsActive,IsDeleted,CreatedAt,CreatedBy,UpdatedAt,UpdatedBy)
                                  VALUES(@v_Name,@v_ProductTypeId,@v_ProductSubTypeId,@v_ProductColorId,@v_ProductSizeId,@v_Image,@v_IsActive,@v_IsDeleted,@v_CreatedAt,@v_CreatedBy,@v_UpdatedAt,@v_UpdatedBy)";
                var result = await Repository.NonQueryAsync(query: query, parameters: parameters);
                retVal = result > 0 ? true : false;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }
        public async Task<IEnumerable<ProductModel>> GetAllProductsAsync()
        {
            List<ProductModel> Products = new List<ProductModel>();
            try
            {
                string query = @"SELECT * FROM Product WHERE IsActive = 1 AND IsDeleted = 0"; ;
                var values = await Repository.QueryAsync(query);
                if (values != null)
                {
                    foreach (var value in values)
                    {
                        var product = new ProductModel();
                        product.ProductType = new ProductTypeModel();
                        product.ProductSubType = new ProductSubTypeModel();
                        product.ProductColor = new ProductColorModel();
                        product.ProductSize = new ProductSizeModel();
                        product.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                        product.Name = value?.GetValueFromDictonary("Name")?.ToString();
                        product.ProductType.Id = value?.GetValueFromDictonary("ProductTypeId")?.ToString()?.ToInt();
                        product.ProductSubType.Id = value?.GetValueFromDictonary("ProductSubTypeId")?.ToString()?.ToInt();
                        product.ProductColor.Id = value?.GetValueFromDictonary("ProductColorId")?.ToString()?.ToInt();
                        product.ProductSize.Id = value?.GetValueFromDictonary("ProductSizeId")?.ToString()?.ToInt();
                        product.Image = value?.GetValueFromDictonary("Image") as byte[];
                        product.IsActive = value?.GetValueFromDictonary("IsActive")?.ToString()?.ToNullableBoolean();
                        product.IsDeleted = value?.GetValueFromDictonary("IsDeleted")?.ToString()?.ToNullableBoolean();
                        product.CreatedAt = value?.GetValueFromDictonary("CreatedAt")?.ToString()?.ToNullableDateTime();
                        product.CreatedBy = value?.GetValueFromDictonary("CreatedBy")?.ToString();
                        product.UpdatedAt = value?.GetValueFromDictonary("UpdatedAt")?.ToString()?.ToNullableDateTime();
                        product.UpdatedBy = value?.GetValueFromDictonary("UpdatedBy")?.ToString();

                        Products.Add(product);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return Products;
        }

        public async Task<ProductModel> GetLastAddedProduct()
        {
            var product = new ProductModel();
            try
            {
                string query = @"SELECT * FROM  Product Order by 1 DESC LIMIT 1";
                var values = await Repository.QueryAsync(query);
                var value = values.FirstOrDefault();
                product.ProductType = new ProductTypeModel();
                product.ProductSubType = new ProductSubTypeModel();
                product.ProductColor = new ProductColorModel();
                product.ProductSize = new ProductSizeModel();
                product.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                product.Name = value?.GetValueFromDictonary("Name")?.ToString();
                product.ProductType.Id = value?.GetValueFromDictonary("ProductTypeId")?.ToString()?.ToInt();
                product.ProductSubType.Id = value?.GetValueFromDictonary("ProductSubTypeId")?.ToString()?.ToInt();
                product.ProductColor.Id = value?.GetValueFromDictonary("ProductColorId")?.ToString()?.ToInt();
                product.ProductSize.Id = value?.GetValueFromDictonary("ProductSizeId")?.ToString()?.ToInt();
                product.Image = value?.GetValueFromDictonary("Image") as byte[];
                product.IsActive = value?.GetValueFromDictonary("IsActive")?.ToString()?.ToNullableBoolean();
                product.IsDeleted = value?.GetValueFromDictonary("IsDeleted")?.ToString()?.ToNullableBoolean();
                product.CreatedAt = value?.GetValueFromDictonary("CreatedAt")?.ToString()?.ToNullableDateTime();
                product.CreatedBy = value?.GetValueFromDictonary("CreatedBy")?.ToString();
                product.UpdatedAt = value?.GetValueFromDictonary("UpdatedAt")?.ToString()?.ToNullableDateTime();
                product.UpdatedBy = value?.GetValueFromDictonary("UpdatedBy")?.ToString();

            }
            catch (Exception ex)
            {
                 LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return product;
        }

        public async Task<ProductModel> GetProductByIdAsync(int? Id)
        {
            var product = new ProductModel();
            try
            {
                string query = string.Empty;
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_Id"] = Id;
                query = @"SELECT * FROM Product WHERE Id = @v_Id AND IsActive = 1 AND IsDeleted = 0";
                var values = await Repository.QueryAsync(query, parameters: parameters);
                if (values != null)
                {
                    if (values.Count() > 1)
                    {
                        var value = values.FirstOrDefault();
                        product.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                        product.Name = value?.GetValueFromDictonary("Name")?.ToString();
                        product.ProductType.Id = value?.GetValueFromDictonary("ProductTypeId")?.ToString()?.ToInt();
                        product.ProductSubType.Id = value?.GetValueFromDictonary("ProductSubTypeId")?.ToString()?.ToInt();
                        product.ProductColor.Id = value?.GetValueFromDictonary("ProductColorId")?.ToString()?.ToInt();
                        product.ProductSize.Id = value?.GetValueFromDictonary("ProductSizeId")?.ToString()?.ToInt();
                        product.Image = value?.GetValueFromDictonary("Image") as byte[];
                        product.IsActive = value?.GetValueFromDictonary("IsActive")?.ToString()?.ToNullableBoolean();
                        product.IsDeleted = value?.GetValueFromDictonary("IsDeleted")?.ToString()?.ToNullableBoolean();
                        product.CreatedAt = value?.GetValueFromDictonary("CreatedAt")?.ToString()?.ToNullableDateTime();
                        product.CreatedBy = value?.GetValueFromDictonary("CreatedBy")?.ToString();
                        product.UpdatedAt = value?.GetValueFromDictonary("UpdatedAt")?.ToString()?.ToNullableDateTime();
                        product.UpdatedBy = value?.GetValueFromDictonary("UpdatedBy")?.ToString();
                    }
                }

            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return product;
        }
        public async Task<bool> RemoveProductAsync(int? Id)
        {
            bool retVal = false;
            try
            {
                string query = string.Empty;
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_Id"] = Id;
                query = @"UPDATE Product SET IsActive = 0 AND IsDeleted = 1 WHERE Id = @v_Id";
                var result = await Repository.NonQueryAsync(query, parameters: parameters);
                retVal = result > 0 ? true : false;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }
        public async Task<bool> UpdateProductAsync(ProductModel product)
        {
            bool retVal = false;
            try
            {
                if (product != null)
                {
                    string query = string.Empty;
                    Dictionary<string, object> parameters = new Dictionary<string, object>();
                    parameters["@v_Name"] = product?.Name;
                    parameters["@v_ProductTypeId"] = product.ProductType?.Id;
                    parameters["@v_ProductSubTypeId"] = product?.ProductSubType?.Id;
                    parameters["@v_ProductColorId"] = product?.ProductColor?.Id;
                    parameters["@v_ProductSizeId"] = product?.ProductSize?.Id;
                    parameters["@v_Image"] = product.Image == null ? DBNull.Value : (object)product.Image;
                    parameters["@v_IsActive"] = product.IsActive = true;
                    parameters["@v_IsDeleted"] = product.IsDeleted = false;
                    parameters["@v_CreatedAt"] = product.CreatedAt;
                    parameters["@v_CreatedBy"] = product.CreatedBy == null ? DBNull.Value : (object)product.CreatedBy;
                    parameters["@v_UpdatedAt"] = product.UpdatedAt == null ? DateTime.Now : product.UpdatedAt;
                    parameters["@v_UpdatedBy"] = product.UpdatedBy == null ? DBNull.Value : (object)product.UpdatedBy;
                    query = @"";
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
        #endregion
    }
}