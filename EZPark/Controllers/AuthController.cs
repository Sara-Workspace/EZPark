using BLL.Interfaces;
using BLL.Services;
using DTO;
using Microsoft.AspNetCore.Mvc;

namespace Parking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IOwnerService _ownerService;      // ממשק במקום המחלקה עצמה
        private readonly IDriverService _driverService;    // ממשק במקום המחלקה עצמה
        private readonly ITokenService _tokenService;

        public AuthController(
            IOwnerService ownerService,
            IDriverService driverService,
            ITokenService tokenService)
        {
            _ownerService = ownerService;
            _driverService = driverService;
            _tokenService = tokenService;
        }

        /// <summary>
        /// התחברות מאוחדת לכלל המשתמשים (נהג, בעל חניה או מנהל)
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            // 1. ניסיון אימות מול שירות הנהגים
            var driver = await _driverService.ValidateDriverCredentialsAsync(dto.Email, dto.Password);
            if (driver != null)
            {
                string driverRole = driver.IsAdmin ? "Admin" : "Driver";
                var authData = _tokenService.CreateToken(driver.DriverID, driver.Email, driverRole);

                return Ok(new
                {
                    Token = authData.Token,
                    ExpiresAt = authData.ExpiresAt,
                    UserId = authData.UserId,
                    User = driver,
                    Role = driverRole,
                    IsAdmin = driver.IsAdmin
                });
            }

            // 2. ניסיון אימות מול שירות בעלי החניות
            var owner = await _ownerService.ValidateOwnerCredentialsAsync(dto.Email, dto.Password);
            if (owner != null)
            {
                string ownerRole = owner.IsAdmin ? "Admin" : "Owner";
                var authData = _tokenService.CreateToken(owner.OwnerID, owner.Email, ownerRole);

                return Ok(new
                {
                    Token = authData.Token,
                    ExpiresAt = authData.ExpiresAt,
                    UserId = authData.UserId,
                    User = owner,
                    Role = ownerRole,
                    IsAdmin = owner.IsAdmin
                });
            }

            // 3. במידה והסיסמה או האימייל לא נמצאו בשום שירות
            return Unauthorized(new { message = "דוא\"ל או סיסמה שגויים" });
        }
    }
}
