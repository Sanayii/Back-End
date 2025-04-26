using Microsoft.EntityFrameworkCore;
using Sanayii.Core.Entities;
using Sanayii.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanayii.Repository
{
    public class ServiceRequestPaymentRepository
    {
        private readonly SanayiiContext _dbContext;

        public ServiceRequestPaymentRepository(SanayiiContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ServiceRequestPayment>> GetAllAsync()
        {
            return await _dbContext.ServiceRequestPayments.ToListAsync();
        }

        public async Task<ServiceRequestPayment> GetByIdsAsync(string customerId, int paymentId, int serviceId)
        {
            return await _dbContext.ServiceRequestPayments
                .FirstOrDefaultAsync(x => x.CustomerId == customerId && x.PaymentId == paymentId && x.ServiceId == serviceId);
        }

        public async Task AddAsync(ServiceRequestPayment entity)
        {
            await _dbContext.ServiceRequestPayments.AddAsync(entity);
        }

        public void Update(ServiceRequestPayment entity)
        {
            _dbContext.ServiceRequestPayments.Update(entity);
        }

        public void Delete(ServiceRequestPayment entity)
        {
            _dbContext.ServiceRequestPayments.Remove(entity);
        }
    }
}
