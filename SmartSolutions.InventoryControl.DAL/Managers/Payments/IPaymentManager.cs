using SmartSolutions.InventoryControl.DAL.Models.Payments;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartSolutions.InventoryControl.DAL.Managers.Payments
{
    public interface IPaymentManager
    {
        Task<bool> AddPaymentAsync(PaymentModel payment);
        Task<IEnumerable<PaymentModel>> GetPaymentsByPartnerIdAsync(int? partnerId);
        Task<PaymentModel> GetLastPaymentByPartnerIdAsync(int? partnerId);
        Task<PaymentModel> GetPaymentByIdAsync(int? paymentId);
    }
}
