using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Services;
using DAL.Repositories;
using DTO;

namespace BLL.Service
{
    public class SpotHoldService
    {
        private static readonly TimeSpan HoldDuration = TimeSpan.FromMinutes(5);
        private readonly SpotHoldRegistry _registry;
        private readonly IParkingSpotRepository _spotRepository;
        private readonly IDriverRepository _driverRepository;
        private readonly ISpotOpenHoursService _openHours; // שעות הזמינות שבעל החניה קבע

        public SpotHoldService(
            SpotHoldRegistry registry,
            IParkingSpotRepository spotRepository,
            IDriverRepository driverRepository,
            ISpotOpenHoursService openHours)
        {
            _registry = registry;
            _spotRepository = spotRepository;
            _driverRepository = driverRepository;
            _openHours = openHours;
        }

        public SpotHoldResultDTO? HoldSpot(int spotId, string userEmail)
        {
            var driver = _driverRepository.GetDriverByEmail(userEmail);
            if (driver == null || string.IsNullOrWhiteSpace(driver.LicensePlate))
            {
                throw new BusinessRuleException("יש להזין מספר רישוי רכב לפני תפיסת חניה.");
            }

            // אין כסף בארנק (או שהוא במינוס) - לא שומרים חניה
            if (driver.User != null && driver.User.Balance <= 0)
            {
                throw new BusinessRuleException("אין מספיק כסף בארנק. טענו את הארנק כדי לחנות.");
            }

            var spot = _spotRepository.GetParkingSpotById(spotId);
            if (spot == null || !spot.IsActive)
            {
                return null;
            }

            // שומרים חניה רק בשעות שבעל החניה קבע שהיא זמינה
            // (גם אם מישהו מנסה לעקוף את החיפוש, למשל דרך Swagger)
            if (!_openHours.IsOpenNow(spotId))
            {
                throw new BusinessRuleException("החניה לא זמינה בשעה הזאת לפי השעות שבעל החניה קבע.");
            }

            if (!_registry.TryHold(spotId, driver.DriverID, HoldDuration, out var expiresAtUtc))
            {
                return null;
            }

            int remainingSeconds = Math.Max(0, (int)(expiresAtUtc - DateTime.UtcNow).TotalSeconds);

            return new SpotHoldResultDTO
            {
                SpotId = spotId,
                HeldUntilUtc = expiresAtUtc,
                ExpiresInSeconds = remainingSeconds
            };
        }

        public void ReleaseSpot(int spotId, string userEmail)
        {
            var driver = _driverRepository.GetDriverByEmail(userEmail);
            if (driver != null)
            {
                _registry.Release(spotId, driver.DriverID);
            }
        }
        // מזהה הנהג של המשתמש (או null אם עדיין אין לו פרופיל נהג)
        public int? GetDriverId(string userEmail)
        {
            return _driverRepository.GetDriverByEmail(userEmail)?.DriverID;
        }
    }
}
