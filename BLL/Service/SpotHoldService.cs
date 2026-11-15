using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Exceptions;
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

        public SpotHoldService(
            SpotHoldRegistry registry,
            IParkingSpotRepository spotRepository,
            IDriverRepository driverRepository)
        {
            _registry = registry;
            _spotRepository = spotRepository;
            _driverRepository = driverRepository;
        }

        public SpotHoldResultDTO? HoldSpot(int spotId, string userEmail)
        {
            var driver = _driverRepository.GetDriverByEmail(userEmail);
            if (driver == null || string.IsNullOrWhiteSpace(driver.LicensePlate))
            {
                throw new BusinessRuleException("יש להזין מספר רישוי רכב לפני תפיסת חניה.");
            }

            if (driver.User != null && driver.User.Points <= 0)
            {
                throw new BusinessRuleException("נגמרו לך הנקודות למימוש לחודש זה.");
            }

            var spot = _spotRepository.GetParkingSpotById(spotId);
            if (spot == null || !spot.IsActive)
            {
                return null;
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
    }
}
