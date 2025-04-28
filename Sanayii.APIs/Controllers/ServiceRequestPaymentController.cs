using Microsoft.AspNetCore.Mvc;
using Sanayii.Repository.Repository;

namespace Sanayii.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceRequestPaymentController : Controller
    {
        private readonly UnitOFWork _unitOfWork;
        public ServiceRequestPaymentController(UnitOFWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet("CustomerRequests")]
        public IActionResult GetCustomerRequests(string cutomerid)
        {

            var res = _unitOfWork.ServiceRequestPaymentRepos.GetByCustomerId(cutomerid);
            if (res == null)
            {
                return NotFound();
            }
            return Ok(res);
        }
    }
}
