﻿using SmartSolutions.InventoryControl.DAL.Models.Warehouse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Warehouse
{
    public interface IWarehouseManager
    {
        Task<IEnumerable<WarehouseModel>> GetAllWarehousesAsync();
        /// <summary>
        /// Get All Warehouse Where the Product Exist or Available
        /// </summary>
        /// <param name="Id"> Id of The Product</param>
        /// <returns>List of Warehouse</returns>
        Task<IEnumerable<WarehouseModel>> GetAllWarehouseByProductId(int Id);
        Task<bool> SaveWarehouseAsync(WarehouseModel warehouse);
        /// <summary>
        /// verifies that with this name Already Exist Or Not
        /// </summary>
        /// <param name="warehouseName"></param>
        /// <returns>true if Exist</returns>
        Task<bool> IsWarehouseNameAlreadyExist(string warehouseName);
    }
}
