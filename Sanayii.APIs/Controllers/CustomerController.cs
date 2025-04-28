using Microsoft.AspNetCore.Mvc;
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
        [HttpPost]
        public IActionResult Edit(Customer customer)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingCustomer = _unitOfWork._CustomerRepo.GetById(customer.Id);
            if (existingCustomer == null)
                return NotFound();

            existingCustomer.FName = customer.FName;
            existingCustomer.LName = customer.LName;
            existingCustomer.Age = customer.Age;
            existingCustomer.Email = customer.Email;
            existingCustomer.City = customer.City;
            existingCustomer.Street = customer.Street;
            existingCustomer.Government = customer.Government;
            existingCustomer.UserPhones.Clear();
            existingCustomer.UserPhones = customer.UserPhones;

            _unitOfWork.save();

            return Ok(existingCustomer);
        }
    }
}
