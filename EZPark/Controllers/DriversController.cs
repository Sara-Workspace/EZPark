//זה שכבה - דלת של המסעדה כמו מלצר

using Microsoft.AspNetCore.Mvc;
using BLL.Services;
using DTO;
using BLL;

namespace Parking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriversController : ControllerBase
    {
        private readonly DriverService _driverService;

        // הזרקת ה-Service בבנאי (Dependency Injection)
        public DriversController(DriverService driverService)
        {
            _driverService = driverService;
        }

        // GET: api/drivers
        [HttpGet]
        public ActionResult<List<DriverDTO>> GetAllDrivers()
        {
            var drivers = _driverService.GetAllDrivers();
            return Ok(drivers);
        }

        // GET: api/drivers/5
        [HttpGet("{id}")]
        public ActionResult<DriverDTO> GetDriverById(int id)
        {
            var driver = _driverService.GetDriverById(id);
            if (driver == null)
            {
                return NotFound($"Driver with ID {id} not found.");
            }
            return Ok(driver);
        }

        // POST: api/drivers
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

        // PUT: api/drivers/5
        [HttpPut("{id}")]
        public ActionResult<DriverDTO> UpdateDriver(int id, [FromBody] DriverUpdateDTO dto)
        {
            var updatedDriver = _driverService.UpdateDriver(id, dto);
            if (updatedDriver == null)
            {
                return NotFound($"Driver with ID {id} not found.");
            }
            return Ok(updatedDriver);
        }

        // DELETE: api/drivers/5
        [HttpDelete("{id}")]
        public IActionResult DeleteDriver(int id)
        {
            var success = _driverService.DeleteDriver(id);
            if (!success)
            {
                return NotFound($"Driver with ID {id} not found.");
            }
            return NoContent();
        }

        // POST: api/drivers/5/topup (לדוגמה לפעולה חישובית/עסקית)
        [HttpPost("{id}/topup")]
        public ActionResult<DriverDTO> TopUpWallet(int id, [FromBody] decimal amount)
        {
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