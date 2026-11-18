using BLL.Interfaces;
using DAL;
using DAL.Repositories;

namespace BLL.Services
{
    // בודק אם חניה פתוחה עכשיו לפי שעות הזמינות שבעל החניה קבע (טבלת SpotAvailability).
    // הכלל: החניה פתוחה רק אם יש לה שורת זמינות להיום, והשעה עכשיו בתוך החלון.
    // יום / שעה שבעל החניה לא כתב = החניה לא מופיעה באתר.
    public class SpotOpenHoursService : ISpotOpenHoursService
    {
        private readonly ISpotAvailabilityRepository _availabilityRepo;

        // אזור הזמן של ישראל: השעות שבעל החניה כותב הן לפי השעון בישראל,
        // ולא לפי השעון של המחשב שעליו רץ השרת (שיכול להיות בכל מקום בעולם)
        private static readonly TimeZoneInfo IsraelTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time");

        public SpotOpenHoursService(ISpotAvailabilityRepository availabilityRepo)
        {
            _availabilityRepo = availabilityRepo;
        }

        // השעה עכשיו בישראל
        private static DateTime GetIsraelNow()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, IsraelTimeZone);
        }

        // האם אחד מחלונות הזמינות כולל את "עכשיו": אותו יום בשבוע, ושעה בין ההתחלה לסיום
        private static bool IsInsideOneOfWindows(IEnumerable<SpotAvailability> windows, DateTime now)
        {
            return windows.Any(w =>
                w.DayOfWeek == now.DayOfWeek &&
                w.StartTime <= now.TimeOfDay &&
                now.TimeOfDay < w.EndTime);
        }

        public bool IsOpenNow(int spotId)
        {
            var windows = _availabilityRepo.GetAvailabilityBySpotId(spotId);
            return IsInsideOneOfWindows(windows, GetIsraelNow());
        }

        public List<ParkingSpots> KeepOnlyOpenNow(List<ParkingSpots> spots)
        {
            if (spots.Count == 0) return spots;

            // שאילתה אחת לכל החניות, ואז מקבצים לפי חניה
            var spotIds = spots.Select(s => s.SpotId).ToList();
            var windowsBySpot = _availabilityRepo.GetAvailabilityForSpots(spotIds)
                .GroupBy(w => w.SpotId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var now = GetIsraelNow();

            // חניה בלי שום שורת זמינות לא נכנסת לדיקשנרי, ולכן לא מוצגת
            return spots
                .Where(s => windowsBySpot.ContainsKey(s.SpotId) &&
                            IsInsideOneOfWindows(windowsBySpot[s.SpotId], now))
                .ToList();
        }
    }
}
