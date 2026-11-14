using AutoMapper;
using BLL.Exceptions;
using BLL.Interfaces;
using DAL;
using DAL.Repositories;
using DTO;

namespace BLL.Services
{
    public class ParkingSpotService : IParkingSpotService
    {
        private readonly IParkingSpotRepository _spotRepo;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService; // שירות התמונות

        public ParkingSpotService(IParkingSpotRepository spotRepo, IMapper mapper, IImageService imageService)
        {
            _spotRepo = spotRepo;
            _mapper = mapper;
            _imageService = imageService;
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

        // לוגיקה עסקית: הגדרת חניה כפעילה כברירת מחדל בעת יצירה
        public ParkingSpotDTO CreateParkingSpot(ParkingSpotCreateDTO dto, int ownerId)
        {
            var spot = _mapper.Map<ParkingSpots>(dto);
            spot.OwnerId = ownerId;
            spot.IsActive = true;

            // אם הגיעה תמונה - שירות התמונות שומר אותה ומחזיר את השם
            // (אם הקובץ לא תקין, הוא זורק ArgumentException והשומר מחזיר 400)
            if (dto.ImageFile != null)
                spot.Image = _imageService.Save(dto.ImageFile);

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

        public ParkingSpotDTO UpdateParkingSpot(int id, ParkingSpotDTO dto, int ownerId)
        {
            // לא קיימת או לא שלך - אותה תשובה (404), כדי לא לגלות חניות של אחרים
            var spot = _spotRepo.GetOwnedSpot(id, ownerId)
                ?? throw new NotFoundException($"חניה מספר {id} לא נמצאה.");

            _mapper.Map(dto, spot);
            _spotRepo.UpdateParkingSpot(spot);

            return _mapper.Map<ParkingSpotDTO>(spot);
        }

        public void DeleteParkingSpot(int id, int ownerId)
        {
            var spot = _spotRepo.GetOwnedSpot(id, ownerId)
                ?? throw new NotFoundException($"חניה מספר {id} לא נמצאה.");

            _spotRepo.DeleteParkingSpot(spot);

            // מוחקים גם את קובץ התמונה, כדי שלא יישאר בתיקייה סתם
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