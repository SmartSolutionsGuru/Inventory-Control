using SmartSolutions.InventoryControl.DAL.Models.Inventory;
using SmartSolutions.InventoryControl.DAL.Models.Product;
using SmartSolutions.InventoryControl.DAL.Models.Stock;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.DictionaryUtils;
using SmartSolutions.Util.LogUtils;
using SmartSolutions.Util.NumericUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Inventory
{
    [Export(typeof(IInventoryManager)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InventoryManager : BaseManager, IInventoryManager
    {
        #region Private Members
        private readonly IRepository Repository;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public InventoryManager()
        {
            Repository = GetRepository<InventoryModel>();
        }

        public async Task<bool> AddBulkInventoryAsync(List<InventoryModel> models)
        {
            bool retVal = false;
            try
            {
                if (models != null || models?.Count > 0)
                {
                    foreach (var model in models)
                    {
                        await AddInventoryAsync(model);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }
        #endregion

        #region Public Methods
        public async Task<bool> AddInventoryAsync(InventoryModel model)
        {
            bool retVal = false;
            try
            {
                var lastStock = await GetLastStockInHandAsync(model?.Product, model?.ProductColor, model?.ProductSize);
                if (model == null) return false;
                string query = string.Empty;
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_InvoiceId"] = model?.InvoiceId;
                parameters["@v_InvoiceGuid"] = model?.InvoiceGuid;
                parameters["@v_ProductId"] = model?.Product.Id;
                parameters["@v_ProductColorId"] = model?.ProductColor.Id;
                parameters["@v_ProductSizeId"] = model?.ProductSize.Id;
                parameters["@v_Price"] = model?.Price;
                parameters["@v_Quantity"] = model?.Quantity;
                parameters["@v_StockInHand"] = model.StockInHand = (lastStock.StockInHand + model.Quantity);
                parameters["@v_IsStockIn"] = model.IsStockIn = true;
                parameters["@v_IsStockOut"] = model.IsStockOut = false;
                parameters["@v_Total"] = model?.Total;
                parameters["@v_IsActive"] = model.IsActive = true;
                parameters["@v_IsDeleted"] = model.IsDeleted = false;
                parameters["@v_CreatedAt"] = model.CreatedAt == null ? DateTime.Now : model.CreatedAt;
                parameters["@v_CreatedBy"] = model.CreatedBy == null ? DBNull.Value : (object)model.CreatedBy;
                parameters["@v_UpdatedAt"] = model.UpdatedAt == null ? DBNull.Value : (object)model.UpdatedAt;
                parameters["@v_UpdatedBy"] = model.UpdatedBy == null ? DBNull.Value : (object)model.UpdatedBy;
                query = @"INSERT INTO Inventory (InvoiceId,InvoiceGuid,ProductId,ProductColorId,ProductSizeId,Price,Quantity,IsStockIn,IsStockOut,StockInHand,Total,IsActive,IsDeleted,CreatedAt,CreatedBy,UpdatedAt,UpdatedBy)
                                        VALUES(@v_InvoiceId,@v_InvoiceGuid,@v_ProductId,@v_ProductColorId,@v_ProductSizeId,@v_Price,@v_Quantity,@v_IsStockIn,@v_IsStockOut,@v_StockInHand,@v_Total,@v_IsActive,@v_IsDeleted,@v_CreatedAt,@v_CreatedBy,@v_UpdatedAt,@v_UpdatedBy)";
                var result = await Repository.NonQueryAsync(query, parameters: parameters);
                retVal = result > 0 ? true : false;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }

        public async Task<bool> AddInventoryReturnAsync(InventoryModel model)
        {
            bool retVal = false;
            try
            {
                string query = string.Empty;
                await Repository.NonQueryAsync(query);
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }

        public async Task<double> GetLastBalanceAsync(int? Id)
        {
            double balance = 0d;
            try
            {
                string query = string.Empty;
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_Id"] = Id;
                query = @"SELECT * FROM PurchaseTransaction WHERE Id = @v_Id AND IsActive = 1 AND IsDeleted = 0";
                var values = await Repository.QueryAsync(query, parameters: parameters);
                if (values != null)
                {
                    foreach (var value in values)
                    {
                        value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                        value?.GetValueFromDictonary("Total")?.ToString()?.ToInt();
                        value?.GetValueFromDictonary("Balance")?.ToString()?.ToInt();
                    }
                }

            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return balance;
        }

        public async Task<InventoryModel> GetLastStockInHandAsync(ProductModel product, ProductColorModel color, ProductSizeModel size)
        {
            var retVal = new InventoryModel();
            try
            {
                if (product == null || color == null || size == null) return null;
                string query = string.Empty;
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_ProductId"] = product?.Id;
                parameters["@v_ProductColorId"] = color?.Id;
                parameters["@v_ProductSizeId"] = size?.Id;
                query = @"SELECT StockInHand,Price FROM Inventory WHERE ProductId = @v_ProductId AND ProductColorId = @v_ProductColorId AND ProductSizeId = @v_ProductSizeId  Order By 1 ASC LIMIT 1;";
                var values = await Repository.QueryAsync(query: query, parameters: parameters);
                if (values != null || values?.Count > 0)
                {
                    retVal.StockInHand = values?.FirstOrDefault()?.GetValueFromDictonary("StockInHand")?.ToString()?.ToInt() ?? 0;
                    retVal.Price = values?.FirstOrDefault()?.GetValueFromDictonary("Price")?.ToString()?.ToInt() ?? 0;
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }

        public async Task<int> GetLastTransationIdAsync()
        {
            int? Id = 0;
            try
            {
                string query = @"SELECT MAX(ROWID) FROM Invoice";
                var values = await Repository.QueryAsync(query);
                if (values != null)
                {
                    var value = values.FirstOrDefault();
                    Id = value?.GetValueFromDictonary("MAX(ROWID)")?.ToString()?.ToInt();
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return Id.Value;
        }

        public async Task<double> GetProductPurchasePrice(int? productId, int? productColorId, int? productSizeId)
        {
            double lastPrice = 0;
            try
            {
                string query = string.Empty;
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_ProductId"] = productId;
                parameters["@v_ProductColorId"] = productColorId;
                parameters["@v_ProductSizeId"] = productSizeId;
                query = @"SELECT Price FROM Inventory WHERE ProductId = @v_ProductId AND ProductColorId = @vProductColorId AND ProductSizeId = @v_ProductSizeId AND IsStockIn = true AND IsActive = true ORDER BY 1 DESC";
                var values = await Repository.QueryAsync(query, parameters: parameters);
                if (values != null)
                {
                    var value = values.FirstOrDefault();
                    lastPrice = Convert.ToDouble(value.GetValueFromDictonary("Price")?.ToString()?.ToInt());
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return lastPrice;
        }

        public async Task<bool> RemoveBulkInventoryAsync(List<InventoryModel> models)
        {
            bool retVal = false;
            try
            {
                if (models != null)
                {
                    foreach (var product in models)
                    {
                        await RemoveInventoryAsync(product);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }

        public async Task<bool> RemoveInventoryAsync(InventoryModel model)
        {
            bool retVal = false;
            try
            {
                var lastStock = await GetLastStockInHandAsync(model?.Product, model?.ProductColor, model?.ProductSize);
                if (model == null) return false;
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_InvoiceId"] = model?.InvoiceId;
                parameters["@v_InvoiceGuid"] = model?.InvoiceGuid;
                parameters["@v_ProductId"] = model?.Product.Id;
                parameters["@v_ProductColorId"] = model?.ProductColor.Id;
                parameters["@v_ProductSizeId"] = model?.ProductSize.Id;
                parameters["@v_Price"] = model?.Price;
                parameters["@v_Quantity"] = model?.Quantity;
                parameters["@v_StockInHand"] = model.StockInHand = (lastStock.StockInHand - model.Quantity);
                parameters["@v_IsStockIn"] = model.IsStockIn = false;
                parameters["@v_IsStockOut"] = model.IsStockOut = true;
                parameters["@v_Total"] = model?.Total;
                parameters["@v_IsActive"] = model.IsActive = true;
                parameters["@v_IsDeleted"] = model.IsDeleted = false;
                parameters["@v_CreatedAt"] = model.CreatedAt == null ? DateTime.Now : model.CreatedAt;
                parameters["@v_CreatedBy"] = model.CreatedBy == null ? DBNull.Value : (object)model.CreatedBy;
                parameters["@v_UpdatedAt"] = model.UpdatedAt == null ? DBNull.Value : (object)model.UpdatedAt;
                parameters["@v_UpdatedBy"] = model.UpdatedBy == null ? DBNull.Value : (object)model.UpdatedBy;
                string query = string.Empty;
                query = @"INSERT INTO Inventory (InvoiceId,InvoiceGuid,ProductId,ProductColorId,ProductSizeId,Price,Quantity,IsStockIn,IsStockOut,StockInHand,Total,IsActive,IsDeleted,CreatedAt,CreatedBy,UpdatedAt,UpdatedBy)
                                        VALUES(@v_InvoiceId,@v_InvoiceGuid,@v_ProductId,@v_ProductColorId,@v_ProductSizeId,@v_Price,@v_Quantity,@v_IsStockIn,@v_IsStockOut,@v_StockInHand,@v_Total,@v_IsActive,@v_IsDeleted,@v_CreatedAt,@v_CreatedBy,@v_UpdatedAt,@v_UpdatedBy)";
                var result = await Repository.NonQueryAsync(query, parameters: parameters);
                retVal = result > 0 ? true : false;
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
