using SmartSolutions.InventoryControl.DAL.Models.Warehouse;
using SmartSolutions.InventoryControl.Plugins.Repositories;
using SmartSolutions.Util.DictionaryUtils;
using SmartSolutions.Util.LogUtils;
using SmartSolutions.Util.NumericUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Warehouse
{
    [Export(typeof(IWarehouseManager)),PartCreationPolicy(creationPolicy:CreationPolicy.Shared)]
    public class WarehouseManager : BaseManager, IWarehouseManager
    {
        #region Private Members
        private readonly IRepository Repository;
        #endregion

        #region Constructor
        public WarehouseManager()
        {
            Repository = GetRepository<WarehouseIssueModel>();
        }
        #endregion

        #region Methods
        public async Task<IEnumerable<WarehouseModel>> GetAllWarehousesAsync()
        {
            var warehouses = new List<WarehouseModel>();
            try
            {
                string query = @"SELECT * FROM Warehouse WHERE IsActive = 1";
                var values = await Repository.QueryAsync(query:query);
                if(values != null || values?.Count > 0)
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
        #endregion
    }
}
