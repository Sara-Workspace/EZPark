using AutoMapper;
using BLL.Exceptions;
using BLL.Geocoding;
using BLL.Interfaces;
using DAL;
using DAL.Repositories;
using DTO;

namespace BLL.Services
{
    public class ParkingSpotService : IParkingSpotService
    {
        private readonly IParkingSpotRepository _spotRepo;
        private readonly IOwnersRepository _ownerRepo;
        private readonly IDriverRepository _driverRepo;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;   // שירות התמונות
        private readonly IGeocodingService _geocoding;  // חדש: שירות ההמרה מכתובת למיקום (ה-API החיצוני)

        public ParkingSpotService(
            IParkingSpotRepository spotRepo,
            IOwnersRepository ownerRepo,
            IDriverRepository driverRepo,
            IMapper mapper,
            IImageService imageService,
            IGeocodingService geocoding)
        {
            _spotRepo = spotRepo;
            _ownerRepo = ownerRepo;
            _driverRepo = driverRepo;
            _mapper = mapper;
            _imageService = imageService;
            _geocoding = geocoding;
        }
        public async Task<ParkingSpotDTO> CreateParkingSpotAsync(ParkingSpotCreateDTO dto, string userEmail, CancellationToken ct = default)
        {
            // 0. איתור או יצירת פרופיל Owner עבור המשתמש המחובר
            var owner = _ownerRepo.GetOwnerByEmail(userEmail);
            if (owner == null)
            {
                var driver = _driverRepo.GetDriverByEmail(userEmail)
                    ?? throw new NotFoundException("המשתמש לא נמצא במערכת.");

                owner = new Owners
                {
                    UserId = driver.UserId,
                    BankDetails = string.Empty
                };

                _ownerRepo.AddOwner(owner);
            }

            // ===== 1. מוצאים את המיקום =====
            double latitude;
            double longitude;

            if (dto.Latitude.HasValue && dto.Longitude.HasValue)
            {
                latitude = dto.Latitude.Value;
                longitude = dto.Longitude.Value;
            }
            else if (dto.Latitude.HasValue || dto.Longitude.HasValue)
            {
                throw new BusinessRuleException("יש למלא גם קו רוחב וגם קו אורך, או להשאיר את שניהם ריקים.");
            }
            else
            {
                var location = await _geocoding.GetLocationAsync(dto.Address, dto.City, ct)
                    ?? throw new BusinessRuleException("לא הצלחנו למצוא את הכתובת. בדקו שהיא נכונה, או סמנו את המיקום על המפה.");

                latitude = location.Latitude;
                longitude = location.Longitude;
            }

            // ===== 2. בונים את החניה =====
            var spot = _mapper.Map<ParkingSpots>(dto);
            spot.OwnerId = owner.OwnerID;
            spot.IsActive = true;
            spot.Latitude = latitude;
            spot.Longitude = longitude;

            // ===== 3. שומרים תמונה (אם יש) =====
            if (dto.ImageFile != null)
                spot.Image = _imageService.Save(dto.ImageFile);

            // ===== 4. שומרים במסד =====
            try
            {
                _spotRepo.AddParkingSpot(spot);
            }
            catch
            {
                if (spot.Image != null)
                    _imageService.Delete(spot.Image);

                throw;
            }

            return _mapper.Map<ParkingSpotDTO>(spot);
        }

        public List<ParkingSpotDTO> GetAllParkingSpots()
        {
            var spots = _spotRepo.GetAllParkingSpots();
            return _mapper.Map<List<ParkingSpotDTO>>(spots);
        }

        public ParkingSpotDTO GetParkingSpotById(int id)
        {
            // אם החניה לא קיימת - זורקים שגיאה, והשומר (GlobalExceptionFilter) מחזיר 404
            var spot = _spotRepo.GetParkingSpotById(id)
                ?? throw new NotFoundException($"חניה מספר {id} לא נמצאה.");

            return _mapper.Map<ParkingSpotDTO>(spot);
        }

        // יצירת חניה חדשה.
        // async = הפעולה מחכה ל-API חיצוני (OpenStreetMap), ולכן היא "לוקחת זמן"
        public async Task<ParkingSpotDTO> CreateParkingSpotAsync(ParkingSpotCreateDTO dto, int ownerId, CancellationToken ct = default)
        {
            // ===== 1. מוצאים את המיקום (לפני ששומרים תמונה, כדי לא לשמור קובץ לחניה שלא תישמר) =====
            double latitude;
            double longitude;

            if (dto.Latitude.HasValue && dto.Longitude.HasValue)
            {
                // בעל החניה שלח מיקום בעצמו (למשל סימן על המפה) - משתמשים בו, בלי לשאול את OpenStreetMap
                latitude = dto.Latitude.Value;
                longitude = dto.Longitude.Value;
            }
            else if (dto.Latitude.HasValue || dto.Longitude.HasValue)
            {
                // נשלח רק אחד מהשניים - זה לא מספיק כדי לדעת איפה החניה
                throw new BusinessRuleException("יש למלא גם קו רוחב וגם קו אורך, או להשאיר את שניהם ריקים.");
            }
            else
            {
                // לא נשלח מיקום - שואלים את ה-API החיצוני לפי הכתובת
                var location = await _geocoding.GetLocationAsync(dto.Address, dto.City, ct)
                    ?? throw new BusinessRuleException("לא הצלחנו למצוא את הכתובת. בדקו שהיא נכונה, או סמנו את המיקום על המפה.");

                latitude = location.Latitude;
                longitude = location.Longitude;
            }

            // ===== 2. בונים את החניה =====
            var spot = _mapper.Map<ParkingSpots>(dto);
            spot.OwnerId = ownerId;
            spot.IsActive = true;
            spot.Latitude = latitude;
            spot.Longitude = longitude;

            // ===== 3. שומרים תמונה (אם יש) =====
            // (אם הקובץ לא תקין, הוא זורק ArgumentException והשומר מחזיר 400)
            if (dto.ImageFile != null)
                spot.Image = _imageService.Save(dto.ImageFile);

            // ===== 4. שומרים במסד =====
            try
            {
                _spotRepo.AddParkingSpot(spot);
            }
            catch
            {
                // השמירה במסד נכשלה - מוחקים את התמונה ששמרנו, כדי שלא תישאר "יתומה" בתיקייה
                if (spot.Image != null)
                    _imageService.Delete(spot.Image);

                throw; // ממשיכים לזרוק את השגיאה, והשומר יחזיר 500
            }

            return _mapper.Map<ParkingSpotDTO>(spot);
        }


        public ParkingSpotDTO UpdateParkingSpot(int id, ParkingSpotDTO dto, int ownerId, bool isAdmin)
        {
            // מנהל יכול לעדכן כל חניה, בעל חניה יכול לעדכן רק את שלו
            var spot = isAdmin
                ? _spotRepo.GetParkingSpotById(id)
                : _spotRepo.GetOwnedSpot(id, ownerId);

            if (spot == null)
                throw new NotFoundException($"חניה מספר {id} לא נמצאה.");

            _mapper.Map(dto, spot);
            _spotRepo.UpdateParkingSpot(spot);

            return _mapper.Map<ParkingSpotDTO>(spot);
        }

        public void DeleteParkingSpot(int id, int ownerId, bool isAdmin)
        {
            // מנהל יכול למחוק כל חניה, בעל חניה יכול למחוק רק את שלו
            var spot = isAdmin
                ? _spotRepo.GetParkingSpotById(id)
                : _spotRepo.GetOwnedSpot(id, ownerId);

            if (spot == null)
                throw new NotFoundException($"חניה מספר {id} לא נמצאה.");

            _spotRepo.DeleteParkingSpot(spot);

            // מוחקים גם את קובץ התמונה
            if (spot.Image != null)
                _imageService.Delete(spot.Image);
        }


        public List<ParkingSpotDTO> GetParkingSpotsByOwnerId(int ownerId)
        {
            var spots = _spotRepo.GetParkingSpotsByOwnerId(ownerId);
            return _mapper.Map<List<ParkingSpotDTO>>(spots);
        }
    }
}