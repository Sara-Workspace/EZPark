using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BLL.Services;
using DTO;
using Parking.Extensions;
using BLL;

namespace Parking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DriversController : ControllerBase
    {
        private readonly DriverService _driverService;

        public DriversController(DriverService driverService)
        {
            _driverService = driverService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult<List<DriverDTO>> GetAllDrivers()
        {
            var drivers = _driverService.GetAllDrivers();
            return Ok(drivers);
        }

        [HttpGet("{id}")]
        public ActionResult<DriverDTO> GetDriverById(int id)
        {
            // ← תיקון 4: רק הנהג עצמו, או מנהל
            bool isMe = User.IsInRole("Driver") && id == User.GetUserId();
            if (!isMe && !User.IsInRole("Admin"))
            {
                return NotFound($"Driver with ID {id} not found.");
            }

            var driver = _driverService.GetDriverById(id);
            if (driver == null)
            {
                return NotFound($"Driver with ID {id} not found.");
            }
            return Ok(driver);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult<DriverDTO> CreateDriver([FromBody] DriverCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdDriver = _driverService.CreateDriver(dto);
            return CreatedAtAction(nameof(GetDriverById), new { id = createdDriver.DriverID }, createdDriver);
        }

        [Authorize(Roles = "Driver")]
        [HttpPut("{id}")]
        public ActionResult<DriverDTO> UpdateDriver(int id, [FromBody] DriverUpdateDTO dto)
        {
            if (id != User.GetUserId())
            {
                return Forbid();
            }

            var updatedDriver = _driverService.UpdateDriver(id, dto);
            if (updatedDriver == null)
            {
                return NotFound($"Driver with ID {id} not found.");
            }
            return Ok(updatedDriver);
        }

        [Authorize(Roles = "Driver")]
        [HttpDelete("{id}")]
        public IActionResult DeleteDriver(int id)
        {
            if (id != User.GetUserId())
            {
                return Forbid();
            }

            var success = _driverService.DeleteDriver(id);
            if (!success)
            {
                return NotFound($"Driver with ID {id} not found.");
            }
            return NoContent();
        }

        [Authorize(Roles = "Driver")]
        [HttpPost("{id}/topup")]
        public ActionResult<DriverDTO> TopUpWallet(int id, [FromBody] decimal amount)
        {
            if (id != User.GetUserId())
            {
                return Forbid();
            }

            try
            {
                var driver = _driverService.TopUpWallet(id, amount);
                if (driver == null) return NotFound();
                return Ok(driver);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}