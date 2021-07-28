using SmartSolutions.InventoryControl.DAL.Models.Payments;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Payments
{
    public interface IPaymentTypeManager
    {
        Task<IEnumerable<PaymentTypeModel>> GetAllPaymentTypesAsync();
    }
}
