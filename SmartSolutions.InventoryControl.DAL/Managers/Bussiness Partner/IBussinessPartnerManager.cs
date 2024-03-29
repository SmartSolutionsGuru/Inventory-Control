﻿using SmartSolutions.InventoryControl.DAL.Models.BussinessPartner;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Bussiness_Partner
{
    public interface IBussinessPartnerManager
    {
        Task<bool> AddBussinesPartnerAsync(BussinessPartnerModel partner);
        Task<IEnumerable<BussinessPartnerModel>> GetAllBussinessPartnersAsync(string search = null);
        Task<IEnumerable<BussinessPartnerLedgerModel>> GetAllBussinessPartnersWithBalanceAsync();
        /// <summary>
        /// Get All the Business Partner By their Type Id 
        /// Like Vendor,Seller etc...
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns>List Of Partners</returns>
        Task<IEnumerable<BussinessPartnerModel>> GetBussinessPartnersByTypeAsync(List<int?> typeId);
        Task<BussinessPartnerModel> GetBussinessPartnerByIdAsync(int? Id);
        Task<bool> UpdateBussinessPartnerAsync(BussinessPartnerModel model);
        Task<bool> RemoveBussinessPartnerAsync(int? Id);
        Task<BussinessPartnerLedgerModel> GetPartnerCurrentBalanceAsync(int partnerId);
        Task<BussinessPartnerModel> GetLastAddedPartner();
    }
}
