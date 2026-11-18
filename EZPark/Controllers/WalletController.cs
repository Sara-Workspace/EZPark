using System.Security.Claims;
using BLL.Interfaces;
using DTO.Wallet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Parking.Controllers
{
    // 👛 הארנק: יתרה, טעינה ב-PayPal ומשיכה. כל משתמש מחובר רואה רק את הארנק שלו (לפי האימייל בטוקן)
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        private string GetEmail()
        {
            return User.FindFirst(ClaimTypes.Email)?.Value
                ?? throw new UnauthorizedAccessException("לא נמצא אימייל בטוקן.");
        }

        // היתרה + 20 התנועות האחרונות
        [HttpGet("me")]
        public ActionResult<WalletDTO> GetMyWallet()
        {
            return Ok(_walletService.GetMyWallet(GetEmail()));
        }

        // טעינה, שלב 1: פותחים הזמנה ב-PayPal, ומחזירים לדפדפן את הכתובת של דף התשלום
        [HttpPost("topup/create")]
        public async Task<ActionResult<TopUpOrderDTO>> CreateTopUp([FromBody] TopUpCreateDTO dto, CancellationToken ct)
        {
            return Ok(await _walletService.StartTopUpAsync(GetEmail(), dto.Amount, ct));
        }

        // טעינה, שלב 2: המשתמש שילם וחזר מ-PayPal. השרת בודק מול PayPal ומוסיף ליתרה
        [HttpPost("topup/capture")]
        public async Task<ActionResult<WalletDTO>> CaptureTopUp([FromBody] TopUpCaptureDTO dto, CancellationToken ct)
        {
            return Ok(await _walletService.CompleteTopUpAsync(GetEmail(), dto.OrderId, ct));
        }

        // משיכה (בעל חניה)
        [Authorize(Roles = "Owner")]
        [HttpPost("withdraw")]
        public ActionResult<WalletDTO> Withdraw([FromBody] WithdrawDTO dto)
        {
            return Ok(_walletService.Withdraw(GetEmail(), dto.Amount));
        }

        // מנהל: כמה האתר הרוויח מעמלות וקנסות
        [Authorize(Roles = "Admin")]
        [HttpGet("platform")]
        public ActionResult<PlatformSummaryDTO> GetPlatformSummary()
        {
            return Ok(_walletService.GetPlatformSummary());
        }
    }
}
