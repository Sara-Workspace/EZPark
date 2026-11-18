using DAL;
using DTO.Wallet;

namespace BLL.Interfaces
{
    // 👛 הארנקים: יתרה, טעינה ב-PayPal, תשלום על חניה (90% / 10%), קנסות ומשיכה
    public interface IWalletService
    {
        WalletDTO GetMyWallet(string email);
        Task<TopUpOrderDTO> StartTopUpAsync(string email, decimal amount, CancellationToken ct = default);
        Task<WalletDTO> CompleteTopUpAsync(string email, string orderId, CancellationToken ct = default);
        WalletDTO Withdraw(string email, decimal amount);
        PlatformSummaryDTO GetPlatformSummary();

        // נקרא מ-ReservationsService בסיום חניה. מחזיר כמה ירד מהנהג
        void PayForParking(Users driverUser, Users? ownerUser, decimal cost, int reservationId, string spotName);

        // נקרא מ-SpotCheckService כשנהג חנה בלי להזמין
        void ChargeFine(Users driverUser, decimal amount, string spotName);
    }
}
