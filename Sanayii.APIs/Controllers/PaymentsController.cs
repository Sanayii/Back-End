using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System;
using Sanayii.Core.Entities;
using Sanayii.Core.Repositories;
using Sanayii.Repository.Data;
using static System.Collections.Specialized.BitVector32;
using Sanayii.Repository;
using Sanayii.Enums;
using Sanayii.Core.DTOs.PaymentDTOs;
using Microsoft.EntityFrameworkCore;


namespace Sanayii.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork ;
        private readonly string _publishableKey;
        public PaymentsController(IConfiguration config,UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _publishableKey = config["Stripe:PublishableKey"];
        }

        [HttpPost("create-session")]
        public async Task<ActionResult> CreateCheckoutSession([FromBody] CreateSessionRequest req)
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
            {
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(req.Amount * 100),
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions { Name = req.ProductName }
                },
                Quantity = 1
            }
            },
                Mode = "payment",
                SuccessUrl = req.SuccessUrl,
                CancelUrl = req.CancelUrl
            };
            var service = new SessionService();
            Session session = service.Create(options);
            var payment = new Payment
            {
                Status = PaymentStatus.Pending,
                Amount = (int)(req.Amount * 100),
                Method = PaymentMethod.CreditCard
            };
            _unitOfWork.Repository<Payment>().Add(payment);
            await _unitOfWork.Complete();

            var srp = new ServiceRequestPayment
            {
                CustomerId = req.CustomerId,
                ServiceId = req.ServiceId,
                PaymentId = payment.Id,
                CreatedAt = DateTime.UtcNow,
                ExecutionTime = 0,
                Status = ServiceStatus.Pending
            };
            _unitOfWork.ServiceRequestPaymentRepository.AddAsync(srp);
            await _unitOfWork.Complete();


            return Ok(new { sessionId = session.Id, publishableKey = _publishableKey, paymentId = payment.Id });
        }
    }

}
