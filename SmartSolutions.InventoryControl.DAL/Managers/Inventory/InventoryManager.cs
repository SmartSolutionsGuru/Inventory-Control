using SmartSolutions.InventoryControl.DAL.Models.Inventory;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.DictionaryUtils;
using SmartSolutions.Util.LogUtils;
using SmartSolutions.Util.NumericUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
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
                if(models != null || models.Count > 0)
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
                if (model == null) return false;
                string query = string.Empty;
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_TransactionId"] = model.InvoiceId;
                parameters["@v_ProductId"] = model.Product.Id;
                parameters["@v_ProductColorId"] = model.ProductColor.Id;
                parameters["@v_ProductSizeId"] = model.ProductSize.Id;
                parameters["@v_Price"] = model.Price;
                parameters["@v_Quantity"] = model.Quantity;
                parameters["@v_TotalPrice"] = model.TotalPrice;
                parameters["@v_IsActive"] = model.IsActive;
                parameters["@v_IsDeleted"] = model.IsDeleted;
                parameters["@v_CreatedAt"] = model.CreatedAt;
                parameters["@v_CreatedBy"] = model.CreatedBy;
                parameters["@v_UpdatedAt"] = model.UpdatedAt;
                parameters["@v_UpdatedBy"] = model.UpdatedBy;
                query = @"INSERT INTO Inventory (TransactionId,ProductId,ProductColorId,ProductSizeId,Price,Quantity,TotalPrice,IsActive,IsDeleted,CreatedAt,CreatedBy,UpdatedAt,UpdatedBy)
                                        VALUES(@v_TransactionId,@v_ProductId,@v_ProductColorId,@v_ProductSizeId,@v_Price,@v_Quantity,@v_TotalPrice,@v_IsActive,@v_IsDeleted,@v_CreatedAt,@v_CreatedBy,@v_UpdatedAt,@v_UpdatedBy)";
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
                        value?.GetValueFromDictonary("Balnce")?.ToString()?.ToInt();
                    }
                }

            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return balance;
        }

        public async Task<int> GetLastTransationIdAsync()
        {
            int? Id = 0;
            try
            {
                string query = @"SELECT MAX(ROWID) FROM PurchaseTransaction";
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
        #endregion
    }
}
