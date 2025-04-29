﻿using Microsoft.AspNetCore.Mvc;
using Sanayii.Repository.Repository;

namespace Sanayii.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountsController : Controller
    {
        private readonly UnitOFWork _unitOfWork;

        public DiscountsController(UnitOFWork unitOFWork)
        {
            _unitOfWork = unitOFWork;
        }
        [HttpGet("{id}")]
        public IActionResult GetCustomerDiscounts(string customerid)
        {
            var res = _unitOfWork._DiscountRepo.GetCustomerDiscounts(customerid);
            if (res == null)
            {
                return NotFound();
            }
            return Ok(res);
        }
    }
}
