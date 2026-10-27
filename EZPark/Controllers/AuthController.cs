using BLL;
using DTO;
using BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace Parking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly OwnerService _ownerService;
        private readonly DriverService _driverService;
        private readonly ITokenService _tokenService;

        public AuthController(
            OwnerService ownerService,
            DriverService driverService,
            ITokenService tokenService)
        {
            _ownerService = ownerService;
            _driverService = driverService;
            _tokenService = tokenService;
        }

        // 1. התחברות בעל חניה
        [HttpPost("owner/login")]
        public async Task<IActionResult> OwnerLogin([FromBody] LoginDTO dto)
        {
            var owner = await _ownerService.ValidateOwnerCredentialsAsync(dto.Email, dto.Password);
            if (owner == null)
                return Unauthorized(new { message = "דוא\"ל או סיסמה שגויים" });

            var token = _tokenService.CreateToken(owner.OwnerID, owner.Email, "Owner");

            return Ok(new
            {
                Token = token,
                User = owner,
                Role = "Owner"
            });
        }

        // 2. התחברות נהג
        [HttpPost("driver/login")]
        public async Task<IActionResult> DriverLogin([FromBody] LoginDTO dto)
        {
            var driver = await _driverService.ValidateDriverCredentialsAsync(dto.Email, dto.Password);
            if (driver == null)
                return Unauthorized(new { message = "דוא\"ל או סיסמה שגויים" });

            var token = _tokenService.CreateToken(driver.DriverID, driver.Email, "Driver");

            return Ok(new
            {
                Token = token,
                User = driver,
                Role = "Driver"
            });
        }
    }
}