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
using Sanayii.Repository.Repository;


namespace Sanayii.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly UnitOFWork _unitOfWork ;
        private readonly string _publishableKey;
        public PaymentsController(IConfiguration config,UnitOFWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _publishableKey = config["Stripe:PublishableKey"];
        }
        [HttpGet("{id}")]
        public IActionResult get(int id)
        {
            var payment = _unitOfWork._PaymentRepo.GetById(id);
            if (payment == null)
            {
                return NotFound();
            }
            return Ok(payment);
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
                Method = PaymentMethods.CreditCard
            };
            _unitOfWork._PaymentRepo.Add(payment);
            _unitOfWork.save();
            var srp = new ServiceRequestPayment
            {
                CustomerId = req.CustomerId,
                ServiceId = req.ServiceId,
                PaymentId = payment.Id,
                CreatedAt = DateTime.UtcNow,
                ExecutionTime = 0,
                Status = ServiceStatus.Pending
            };
            _unitOfWork._ServiceRequestPaymentRepo.AddAsync(srp);
            _unitOfWork.save();

            return Ok(new { sessionId = session.Id, publishableKey = _publishableKey, paymentId = payment.Id });
        }
        [HttpGet("CustomerPayments/{cutomerid}")]
        public IActionResult GetCustomerPayments(string cutomerid)
        {
            var payments = _unitOfWork._PaymentRepo.GetPaymentsByCustomerId(cutomerid);
            if (payments == null)
            {
                return NotFound();
            }
            return Ok(payments);
        }
    }

}
