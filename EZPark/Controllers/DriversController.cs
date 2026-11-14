using BLL.Exceptions;
using BLL.Interfaces;
using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parking.Extensions;

namespace Parking.Controllers
{
    // אין כאן try/catch ואין בדיקות null:
    // אם משהו לא נמצא או לא תקין, הסרוויס זורק שגיאה וה-GlobalExceptionFilter מחזיר את התשובה המתאימה
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DriversController : ControllerBase
    {
        private readonly IDriverService _driverService;

        // הזרקת ה-Service בבנאי (Dependency Injection)
        public DriversController(IDriverService driverService)
        {
            _driverService = driverService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult<List<DriverDTO>> GetAllDrivers()
        {
            return Ok(_driverService.GetAllDrivers());
        }

        [HttpGet("{id}")]
        public ActionResult<DriverDTO> GetDriverById(int id)
        {
            // רק הנהג עצמו, או מנהל.
            // מישהו אחר - עונים "לא נמצא" (404), כדי לא לגלות שהוא קיים
            bool isMe = User.IsInRole("Driver") && id == User.GetUserId();
            if (!isMe && !User.IsInRole("Admin"))
            {
                throw new NotFoundException($"נהג מספר {id} לא נמצא.");
            }

            return Ok(_driverService.GetDriverById(id));
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult<DriverDTO> CreateDriver([FromBody] DriverCreateDTO dto)
        {
            // בדיקת ModelState לא צריך: [ApiController] מחזיר 400 אוטומטית אם ה-DTO לא תקין
            var createdDriver = _driverService.CreateDriver(dto);
            return CreatedAtAction(nameof(GetDriverById), new { id = createdDriver.DriverID }, createdDriver);
        }

        [Authorize(Roles = "Driver")]
        [HttpPut("{id}")]
        public ActionResult<DriverDTO> UpdateDriver(int id, [FromBody] DriverUpdateDTO dto)
        {
            // מותר לעדכן רק את עצמך
            if (id != User.GetUserId())
            {
                return Forbid();
            }

            return Ok(_driverService.UpdateDriver(id, dto));
        }

        [Authorize(Roles = "Driver")]
        [HttpDelete("{id}")]
        public IActionResult DeleteDriver(int id)
        {
            // מותר למחוק רק את עצמך
            if (id != User.GetUserId())
            {
                return Forbid();
            }

            _driverService.DeleteDriver(id);
            return NoContent();
        }

        [Authorize(Roles = "Driver")]
        [HttpPost("{id}/topup")]
        public ActionResult<DriverDTO> TopUpWallet(int id, [FromBody] decimal amount)
        {
            // מותר לטעון רק את הארנק של עצמך
            if (id != User.GetUserId())
            {
                return Forbid();
            }

            // סכום לא חיובי - הסרוויס זורק BusinessRuleException והשומר מחזיר 400
            return Ok(_driverService.TopUpWallet(id, amount));
        }
    }
}