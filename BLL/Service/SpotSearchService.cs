using BLL.Interfaces;
using BLL.Search;
using DAL.Repositories;
using DTO;

namespace BLL.Services
{
    public class SpotSearchService
    {
        private readonly IParkingSpotRepository _spotRepo;
        private readonly DynamicPricingService _pricingService;
        private readonly SpotHoldRegistry _holdRegistry;
        private readonly ISpotOpenHoursService _openHours; // שעות הזמינות שבעל החניה קבע

        public SpotSearchService(IParkingSpotRepository spotRepo, DynamicPricingService pricingService, SpotHoldRegistry holdRegistry, ISpotOpenHoursService openHours)
        {
            _spotRepo = spotRepo;
            _pricingService = pricingService;
            _holdRegistry = holdRegistry;
            _openHours = openHours;
        }

        public List<ParkingSpotSearchResultDTO> Search(SpotSearchRequestDTO request, int? requesterUserId = null)
        {
            // שלב 1: סינון זול במסד הנתונים: רק חניות פעילות בתוך "ריבוע" סביב היעד
            var box = SpotSearchEngine.GetBoundingBox(request.Latitude, request.Longitude, request.RadiusKm);
            var spots = _spotRepo.GetActiveSpotsInBox(
                box.MinLat, box.MaxLat, box.MinLng, box.MaxLng, request.ExcludeIds, request.EvOnly);

            // סינון חניות ששמורות כרגע בזיכרון עבור משתמש אחר
            spots = spots
                .Where(s => !_holdRegistry.IsHeldByAnother(s.SpotId, requesterUserId))
                .ToList();

            // סינון לפי שעות הזמינות: מציגים רק חניות שפתוחות עכשיו לפי מה שבעל החניה קבע
            // (החיפוש, המפה והבוט כולם עוברים כאן, אז כולם מקבלים את אותו סינון)
            spots = _openHours.KeepOnlyOpenNow(spots);

            // שלב 2: המחיר בפועל ברגע זה (תמחור דינמי בשעות עומס)
            var now = DateTime.Now;
            var candidates = spots
                .Select(s => new SpotCandidate(
                    s.SpotId,
                    s.Latitude,
                    s.Longitude,
                    _pricingService.CalculateDynamicPrice(s.SpotId, now, s.BasePricePerHour)))
                .ToList();

            // שלב 3: האלגוריתם: מרחק מדויק, ציון ומיון
            var mode = Enum.TryParse<SpotSortMode>(request.SortBy, true, out var parsed)
                ? parsed
                : SpotSortMode.Smart;

            var ranked = SpotSearchEngine.Rank(
                candidates, request.Latitude, request.Longitude, request.RadiusKm, mode, request.Limit);

            // שלב 4: בניית התשובה
            var spotsById = spots.ToDictionary(s => s.SpotId);
            return ranked.Select(r =>
            {
                var s = spotsById[r.SpotId];
                return new ParkingSpotSearchResultDTO
                {
                    SpotId = s.SpotId,
                    OwnerId = s.OwnerId,
                    Address = s.Address,
                    City = s.City,
                    Latitude = s.Latitude,
                    Longitude = s.Longitude,
                    EvCharger = s.EvCharger,
                    BasePricePerHour = s.BasePricePerHour,
                    IsActive = s.IsActive,
                    Image = s.Image,
                    DistanceKm = Math.Round(r.DistanceKm, 2),
                    EffectivePricePerHour = r.PricePerHour,
                    Score = Math.Round(r.Score, 3)
                };
            }).ToList();
        }
    }
}