using BLL.Exceptions;
using BLL.PlateRecognition;
using BLL.Services;
using DAL;
using DAL.Repositories;
using DTO.SpotCheck;
using Microsoft.AspNetCore.Http;

namespace BLL.SpotCheck
{
    // בדיקת חניה: איזה רכב עומד בחניה, והאם הוא אמור להיות שם?
    // 3 דרכים להגיע לכאן: נהג ("החניה תפוסה"), בעל חניה (מעלה תמונה), מנהל (בדיקה מהמצלמה).
    // בכל השלוש, ההחלטה עצמה זהה (CheckAsync למטה)
    public class SpotCheckService : ISpotCheckService
    {
        public const int FinePoints = 200; // גובה הקנס בנקודות. רוצים לשנות? משנים רק כאן

        // לא נותנים לאותו רכב קנס פעמיים על אותה חניה תוך 24 שעות
        private static readonly TimeSpan NoDoubleFineWindow = TimeSpan.FromHours(24);

        private readonly IPlateRecognitionService _plateRecognition; // קורא הלוחיות (AI)
        private readonly ICameraService _camera;                     // המצלמה המדומה
        private readonly IParkingSpotRepository _spotRepo;
        private readonly IReservationsRepository _reservationsRepo;
        private readonly IDriverRepository _driverRepo;
        private readonly IOwnersRepository _ownerRepo;
        private readonly ISpotChecksRepository _checksRepo;
        private readonly SpotHoldRegistry _holdRegistry;              // השמירות ל-5 דקות (של שרה)

        public SpotCheckService(
            IPlateRecognitionService plateRecognition,
            ICameraService camera,
            IParkingSpotRepository spotRepo,
            IReservationsRepository reservationsRepo,
            IDriverRepository driverRepo,
            IOwnersRepository ownerRepo,
            ISpotChecksRepository checksRepo,
            SpotHoldRegistry holdRegistry)
        {
            _plateRecognition = plateRecognition;
            _camera = camera;
            _spotRepo = spotRepo;
            _reservationsRepo = reservationsRepo;
            _driverRepo = driverRepo;
            _ownerRepo = ownerRepo;
            _checksRepo = checksRepo;
            _holdRegistry = holdRegistry;
        }

        // ===== מנהל: העלאת תמונת מצלמה =====
        public void SaveCameraSnapshot(int spotId, IFormFile image)
        {
            GetSpotOrThrow(spotId); // מוודאים שהחניה קיימת
            _camera.SaveSnapshot(spotId, image);
        }

        // ===== נהג: "החניה תפוסה" =====
        public async Task<SpotCheckResultDTO> ReportOccupiedAsync(int spotId, string driverEmail, CancellationToken ct)
        {
            var driver = _driverRepo.GetDriverByEmail(driverEmail)
                ?? throw new NotFoundException("פרופיל נהג לא נמצא.");

            var spot = GetSpotOrThrow(spotId);

            if (!spot.IsActive)
                throw new BusinessRuleException("החניה כבר מסומנת כלא זמינה. אפשר לבחור חניה אחרת מהרשימה.");

            // הנהג ממשיך לחניה אחרת, אז משחררים את השמירה שלו על החניה הזאת
            _holdRegistry.Release(spotId, driver.DriverID);

            var snapshot = _camera.GetSnapshot(spotId)
                ?? throw new BusinessRuleException("אין מצלמה בחניה הזאת, ולכן אי אפשר לבדוק אותה. אפשר לבחור חניה אחרת מהרשימה.");

            return await CheckAsync(spot, snapshot.Data, snapshot.MediaType, "Driver", driverEmail, ct);
        }

        // ===== בעל חניה: מעלה תמונה =====
        public async Task<SpotCheckResultDTO> OwnerCheckAsync(int spotId, string ownerEmail, IFormFile image, CancellationToken ct)
        {
            var owner = _ownerRepo.GetOwnerByEmail(ownerEmail)
                ?? throw new NotFoundException("פרופיל בעל חניה לא נמצא.");

            var spot = GetSpotOrThrow(spotId);

            // בעל חניה בודק רק חניות שלו. לחניה של מישהו אחר עונים "לא נמצאה" (לא מגלים שהיא קיימת)
            if (spot.OwnerId != owner.OwnerID)
                throw new NotFoundException($"חניה מספר {spotId} לא נמצאה.");

            // מעתיקים את הקובץ לזיכרון, כדי לקבל אותו כמערך של בתים
            using var memory = new MemoryStream();
            await image.CopyToAsync(memory, ct);

            return await CheckAsync(spot, memory.ToArray(), CameraService.GetMediaType(image.FileName), "Owner", ownerEmail, ct);
        }

        // ===== מנהל: בדיקה מהמצלמה =====
        public async Task<SpotCheckResultDTO> AdminCheckAsync(int spotId, string adminEmail, CancellationToken ct)
        {
            var spot = GetSpotOrThrow(spotId);

            var snapshot = _camera.GetSnapshot(spotId)
                ?? throw new BusinessRuleException("עדיין לא הועלתה תמונת מצלמה לחניה הזאת.");

            return await CheckAsync(spot, snapshot.Data, snapshot.MediaType, "Admin", adminEmail, ct);
        }

        // ===== הלב: הבדיקה וההחלטה (זהה לכל 3 הדרכים) =====
        private async Task<SpotCheckResultDTO> CheckAsync(
            ParkingSpots spot, byte[] image, string mediaType, string source, string requestedByEmail, CancellationToken ct)
        {
            // 1. ה-AI קורא את המספר. אם אין לוחית בתמונה, נזרקת שגיאה 400 עם הודעה בעברית
            var plate = await _plateRecognition.ReadPlateAsync(image, mediaType, ct);

            var check = new SpotChecks
            {
                SpotId = spot.SpotId,
                LicensePlate = plate,
                CheckedAt = DateTime.UtcNow,
                Source = source,
                RequestedByEmail = requestedByEmail
            };

            string message;

            // 2. יש הזמנה פעילה בחניה, והרכב שלה הוא זה שבתמונה? אז הכול תקין
            var activeReservation = _reservationsRepo.GetReservationsBySpotId(spot.SpotId)
                .FirstOrDefault(r => r.Status == "Active");

            if (activeReservation != null &&
                LicensePlateHelper.Normalize(activeReservation.Driver.LicensePlate) == plate)
            {
                check.Result = SpotCheckResults.Paying;
                message = "הכול תקין: הרכב בחניה שייך לנהג שהזמין אותה, והוא משלם עליה נקודות.";
            }
            else
            {
                // 3. הרכב לא אמור להיות כאן. של מי הוא?
                var plateOwner = _driverRepo.GetDriverByLicensePlate(plate);

                if (plateOwner != null && plateOwner.UserId == spot.Owner.UserId)
                {
                    // 3א. הרכב של בעל החניה עצמו
                    check.Result = SpotCheckResults.OwnerCar;
                    message = "הרכב בחניה שייך לבעל החניה. החניה לא תוצג לנהגים עד שבעל החניה יחזיר אותה לזמינה.";
                }
                else if (plateOwner != null)
                {
                    // 3ב. רכב של נהג אחר, שחנה בלי להזמין
                    check.Result = SpotCheckResults.OtherDriver;

                    if (_checksRepo.WasFinedRecently(spot.SpotId, plate, DateTime.UtcNow - NoDoubleFineWindow))
                    {
                        message = "הרכב שייך לנהג אחר שחנה בלי להזמין. הוא כבר קיבל קנס על החניה הזאת ביממה האחרונה.";
                    }
                    else
                    {
                        // הקנס יורד מהנקודות של החודש הבא (ב-ReservationsService, בחידוש החודשי)
                        plateOwner.User.PendingFinePoints += FinePoints;
                        _driverRepo.UpdateDriver(plateOwner);

                        check.FinedUserId = plateOwner.UserId;
                        check.FinePoints = FinePoints;
                        message = $"הרכב שייך לנהג אחר שחנה בלי להזמין. הוא יקבל קנס של {FinePoints} נקודות מהחודש הבא.";
                    }
                }
                else
                {
                    // 3ג. רכב שלא רשום באתר
                    check.Result = SpotCheckResults.Unknown;
                    message = "הרכב לא רשום באתר. החניה לא תוצג לנהגים עד שבעל החניה יחזיר אותה לזמינה.";
                }

                // 4. בכל מקרה שהרכב לא משלם: החניה תפוסה בפועל, אז מסתירים אותה מהחיפוש
                if (spot.IsActive)
                {
                    spot.IsActive = false;
                    _spotRepo.UpdateParkingSpot(spot);
                }
            }

            // 5. שומרים את הבדיקה בטבלה
            _checksRepo.AddSpotCheck(check);

            return new SpotCheckResultDTO
            {
                SpotId = spot.SpotId,
                LicensePlate = plate,
                Result = check.Result,
                Message = message,
                FinePoints = check.FinePoints,
                SpotAvailable = spot.IsActive
            };
        }

        // מביא חניה לפי מזהה, או זורק 404
        private ParkingSpots GetSpotOrThrow(int spotId)
        {
            return _spotRepo.GetParkingSpotById(spotId)
                ?? throw new NotFoundException($"חניה מספר {spotId} לא נמצאה.");
        }
    }
}