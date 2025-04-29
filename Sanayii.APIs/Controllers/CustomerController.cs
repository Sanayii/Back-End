using Microsoft.AspNetCore.Mvc;
using Sanayii.Core.DTOs.CustomerDTOs;
using Sanayii.Core.Entities;
using Sanayii.Repository.Repository;

namespace Sanayii.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : Controller
    {
        private readonly UnitOFWork _unitOfWork;
        public CustomerController(UnitOFWork unitOFWork) {
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
    }
}
