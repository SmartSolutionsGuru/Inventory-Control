using SmartSolutions.InventoryControl.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Proprietor
{
    public interface IProprietorInformationManager
    {
        Task<ProprietorInformationModel> GetProprietorInfoAsync();
        Task<bool> AddProprietorInfoAsync(ProprietorInformationModel ProprietorInfo);
    }
}
