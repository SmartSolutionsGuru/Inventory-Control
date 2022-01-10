using SmartSolutions.InventoryControl.DAL.Models.Warehouse;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.DictionaryUtils;
using SmartSolutions.Util.LogUtils;
using SmartSolutions.Util.NumericUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Warehouse
{
    [Export(typeof(IWarehouseManager)), PartCreationPolicy(creationPolicy: CreationPolicy.Shared)]
    public class WarehouseManager : BaseManager, IWarehouseManager
    {
        #region Private Members
        private readonly IRepository Repository;
        #endregion

        #region Constructor
        public WarehouseManager()
        {
            Repository = GetRepository<WarehouseModel>();
        }
        #endregion

        #region Methods
        public async Task<IEnumerable<WarehouseModel>> GetAllWarehousesAsync()
        {
            var warehouses = new List<WarehouseModel>();
            try
            {
                string query = @"SELECT * FROM Warehouses WHERE IsActive = 1";
                var values = await Repository.QueryAsync(query: query);
                if (values != null || values?.Count > 0)
                {
                    foreach (var value in values)
                    {
                        var warehouse = new WarehouseModel();
                        warehouse.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                        warehouse.Name = value?.GetValueFromDictonary("Name")?.ToString();
                        warehouse.PhoneNumber = value?.GetValueFromDictonary("PhoneNumber")?.ToString();
                        warehouse.MobileNumber = value?.GetValueFromDictonary("MobileNumber")?.ToString();
                        warehouse.City = new Models.Region.CityModel
                        {
                            Id = value?.GetValueFromDictonary("CityId")?.ToString()?.ToInt(),
                        };
                        warehouses.Add(warehouse);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return warehouses;
        }
        public async Task<IEnumerable<WarehouseModel>> GetAllWarehouseByProductId(int Id)
        {
            var warehouses = new List<WarehouseModel>();
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_ProductId"] = Id;
                if (Id == 0) return null;
                else
                {
                    string query = @"SELECT WarehouseId FROM StockIn WHERE ProductId = @v_ProductId";
                    var values = await Repository.QueryAsync(query, parameters: parameters);
                    if (values != null && values?.Count > 0)
                    {
                        foreach (var value in values)
                        {
                            var warehouse = new WarehouseModel();
                            warehouse.Id = value?.GetValueFromDictonary("Id")?.ToString()?.ToInt();
                            warehouse.Name = value?.GetValueFromDictonary("Name")?.ToString();
                            warehouse.PhoneNumber = value?.GetValueFromDictonary("PhoneNumber")?.ToString();
                            warehouse.MobileNumber = value?.GetValueFromDictonary("MobileNumber")?.ToString();
                            warehouse.City = new Models.Region.CityModel
                            {
                                Id = value?.GetValueFromDictonary("CityId")?.ToString()?.ToInt(),
                            };
                            if (warehouse.Id != null)
                                warehouses.Add(warehouse);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return warehouses;
        }
        /// <summary>
        /// verifies that with this name Already Exist Or Not
        /// </summary>
        /// <param name="warehouseName"></param>
        /// <returns>true if Exist</returns>
        public async Task<bool> IsWarehouseNameAlreadyExist(string warehouseName)
        {
            if (string.IsNullOrEmpty(warehouseName)) return false;
            bool retVal = false;
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_Name"] = warehouseName;
                string query = @"SELECT * FROM Warehouses WHERE [Name] = @v_Name";
                var values = await Repository.QueryAsync(query: query, parameters: parameters);
                if (values != null || values?.Count > 0)
                    return true;
            }
            catch (Exception ex)
            {
                LogMessage.Write(ex.ToString(), LogMessage.Levels.Error);
            }
            return retVal;
        }

        public async Task<bool> SaveWarehouseAsync(WarehouseModel warehouse)
        {
            bool retVal = false;
            try
            {
                if (warehouse == null) return false;
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters["@v_Name"] = warehouse.Name;
                parameters["@v_CityId"] = warehouse.City?.Id;
                parameters["@v_PhoneNumber"] = warehouse.PhoneNumber;
                parameters["@v_MobileNumber"] = warehouse.MobileNumber;
                parameters["@v_Address"] = warehouse.Address == null ? DBNull.Value : (object)warehouse.Address;
                parameters["@v_Description"] = warehouse.Description == null ? DBNull.Value : (object)warehouse.Description;
                parameters["@v_IsActive"] = true;
                parameters["@v_CreatedAt"] = warehouse.CreatedAt == null ? DateTime.Now : warehouse.CreatedAt;
                parameters["@v_CreatedBy"] = warehouse.CreatedBy == null ? AppSettings.LoggedInUser.DisplayName : (object)warehouse.CreatedBy;
                parameters["@v_UpdatedAt"] = warehouse.UpdatedAt == null ? DBNull.Value : (object)warehouse.UpdatedAt;
                parameters["@v_UpdatedBy"] = warehouse.UpdatedBy == null ? DBNull.Value : (object)warehouse.UpdatedBy;
                string query = @"INSERT INTO Warehouses (Name,CityId,PhoneNumber,MobileNumber,Address,Description,IsActive,CreatedAt,CreatedBy,UpdatedAt,UpdatedBy)
                                                VALUES(@v_Name,@v_CityId,@v_PhoneNumber,@v_MobileNumber,@v_Address,@v_Description,@v_IsActive,@v_CreatedAt,@v_CreatedBy,@v_UpdatedAt,@v_UpdatedBy)";
                var result = await Repository.NonQueryAsync(query: query, parameters: parameters);
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
