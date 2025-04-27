using Sanayii.Core.Entities;
using Sanayii.Repository;
using Sanayii.Repository.Data;
namespace Sanayii.Core.Repository
{
    public class ServiceRequestPaymentRepository:GenericRepository<ServiceRequestPayment>
    {
        public ServiceRequestPaymentRepository(SanayiiContext db) : base(db)
        {
        }
        public ServiceRequestPayment GetByIDS(string customerId, int paymentId, int serviceId)
        {
            return db.ServiceRequestPayments.FirstOrDefault(x => x.CustomerId == customerId && x.PaymentId == paymentId && x.ServiceId == serviceId);
        }
    }
}
