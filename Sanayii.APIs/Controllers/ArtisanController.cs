using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sanayii.Repository.Data;
using Sanayii.Repository.Repository;
using Sanayii.Core.DTOs.ArtisanDTOS;
using Sanayii.Core.Entities;
using Sanayii.Enums;

namespace Sanayii.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtisanController : ControllerBase
    {
        private readonly UnitOFWork _unitOfWork;
        public ArtisanController(UnitOFWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Get(string id)
        {
            try
            {
                var artisan = _unitOfWork._ArtisanRepo.GetArtisanById(id);
                if (artisan == null)
                {
                    return NotFound();
                }

                // Option 1: Return full name
                return Ok($"{artisan.FName} {artisan.LName}");

                // Option 2: Return a DTO
                // return Ok(new ArtisanDto { FullName = $"{artisan.FName} {artisan.LName}" });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

    }
}
