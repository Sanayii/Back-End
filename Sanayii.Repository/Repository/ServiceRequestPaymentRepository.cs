using Sanayii.Core.Entities;
using Sanayii.Repository;
using Sanayii.Repository.Data;
namespace Sanayii.Core.Repository
{
    public class ServiceRequestPaymentRepo:GenericRepository<ServiceRequestPayment>
    {
        public ServiceRequestPaymentRepo(SanayiiContext db) : base(db)
        {
        }
        public ServiceRequestPayment GetByIDS(string customerId, int paymentId, int serviceId)
        {
            return db.ServiceRequestPayments.FirstOrDefault(x => x.CustomerId == customerId && x.PaymentId == paymentId && x.ServiceId == serviceId);
        }
        public List<ServiceRequestPayment> GetByCustomerId(string customerId)
        {
            return db.ServiceRequestPayments.Where(x => x.CustomerId == customerId).ToList();
        }
    }
}
