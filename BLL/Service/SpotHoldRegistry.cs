using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class SpotHoldRegistry
    {
        private readonly object _lock = new();
        private readonly Dictionary<int, (int UserId, DateTime ExpiresAtUtc)> _holds = new();

        private void CleanupExpired(DateTime nowUtc)
        {
            var expiredSpots = _holds
                .Where(kvp => kvp.Value.ExpiresAtUtc <= nowUtc)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var spotId in expiredSpots)
            {
                _holds.Remove(spotId);
            }
        }

        public bool TryHold(int spotId, int userId, TimeSpan duration, out DateTime expiresAtUtc)
        {
            lock (_lock)
            {
                var now = DateTime.UtcNow;
                CleanupExpired(now);

                // אם החניה כבר שמורה למישהו אחר
                if (_holds.TryGetValue(spotId, out var existing) && existing.UserId != userId)
                {
                    expiresAtUtc = default;
                    return false;
                }

                // אם היא כבר שמורה לאותו משתמש, לא מאריכים לו את הזמן - מחזירים את מועד הפקיעה הקיים
                if (_holds.TryGetValue(spotId, out existing) && existing.UserId == userId)
                {
                    expiresAtUtc = existing.ExpiresAtUtc;
                    return true;
                }

                // שחרור חניה קודמת אם המשתמש החזיק חניה אחרת במקביל
                var previousSpot = _holds.FirstOrDefault(kvp => kvp.Value.UserId == userId).Key;
                if (previousSpot != 0)
                {
                    _holds.Remove(previousSpot);
                }

                expiresAtUtc = now.Add(duration);
                _holds[spotId] = (userId, expiresAtUtc);
                return true;
            }
        }

        public void Release(int spotId, int userId)
        {
            lock (_lock)
            {
                if (_holds.TryGetValue(spotId, out var existing) && existing.UserId == userId)
                {
                    _holds.Remove(spotId);
                }
            }
        }

        public bool IsHeldByAnother(int spotId, int? requesterUserId)
        {
            lock (_lock)
            {
                CleanupExpired(DateTime.UtcNow);
                if (_holds.TryGetValue(spotId, out var existing))
                {
                    return requesterUserId == null || existing.UserId != requesterUserId.Value;
                }
                return false;
            }
        }
    }
}