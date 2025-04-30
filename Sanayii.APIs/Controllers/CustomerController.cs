using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sanayii.Core.DTOs.ArtisanDTOS;
using Sanayii.Core.DTOs.CustomerDTOs;
using Sanayii.Core.Entities;
using Sanayii.Enums;
using Sanayii.Repository.Data;
using Sanayii.Repository.Repository;

namespace Sanayii.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : Controller
    {
        private readonly UnitOFWork _unitOfWork;
        private readonly SanayiiContext _dbContext;
        public CustomerController(UnitOFWork unitOFWork, SanayiiContext dbContext) {
            _dbContext = dbContext;
            _unitOfWork = unitOFWork;
        }
        [HttpGet("{customerid}")]
        public IActionResult Get(string customerid)
        {
            var customer = _unitOfWork._CustomerRepo.GetById(customerid);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }
        [HttpPut("{id}")]
        public IActionResult UpdateCustomer(string id, EditCustomerDTO dTO)
        {
            try
            {
                if (id != dTO.Id)
                {
                    return BadRequest("Customer ID mismatch.");
                }
                var existingCustomer = _unitOfWork._CustomerRepo.GetCustomerById(id);
                if (existingCustomer == null)
                {
                    return NotFound("Customer not found.");
                }
                existingCustomer.FName = dTO.FName;
                existingCustomer.LName = dTO.LName;
                existingCustomer.Age = dTO.Age;
                existingCustomer.City = dTO.City;
                existingCustomer.Street = dTO.Street;
                existingCustomer.Government = dTO.Government;
                existingCustomer.Email = dTO.Email;
                existingCustomer.PhoneNumber = dTO.PhoneNumber;

                _unitOfWork._CustomerRepo.Edit(existingCustomer);
                _unitOfWork._CustomerRepo.SaveChanges();
                return Ok("Customer updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");

            }
        }
        [HttpGet("getServiceRequest/{id}")]
        public async Task<IActionResult> GetCustomerServiceRequests(string id)
        {
            var allServicesRequests = await _dbContext.ServiceRequestPayments
                .Where(s => s.CustomerId == id)
                .Include(s => s.Service)
                .Include(s => s.Artisan)
                .Include(s => s.Payment) // ✅ include Payment
                .ToListAsync();

            if (allServicesRequests == null || !allServicesRequests.Any())
            {
                return NotFound("No service requests found for this customer.");
            }

            var allServicesDtos = allServicesRequests.Select(s => new CustomerServiceRequestDto
            {
                CreatedAt = s.CreatedAt,
                ExecutionTime = s.ExecutionTime,
                Status = (int)s.Status,
                PaymentMethod = s.Payment != null ? (int)s.Payment.Method : 0,       // default 0 = Unknown
                PaymentAmount = s.Payment?.Amount ?? 0,
                ServiceName = s.Service?.ServiceName ?? "Unknown",
                ArtisanName = s.Artisan != null ? $"{s.Artisan.FName} {s.Artisan.LName}" : "Not assigned"
            }).ToList();

            return Ok(allServicesDtos);
        }



    }
}
