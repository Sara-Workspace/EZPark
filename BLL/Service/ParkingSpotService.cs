using AutoMapper;
using BLL.Interfaces;
using DAL;
using DAL.Repositories;
using DTO;

namespace BLL.Services
{
    public class ParkingSpotService:IParkingSpotService
    {
        private readonly IParkingSpotRepository _spotRepo;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService; // חדש: שירות התמונות

        public ParkingSpotService(IParkingSpotRepository spotRepo, IMapper mapper, IImageService imageService)
        {
            _spotRepo = spotRepo;
            _mapper = mapper;
            _imageService = imageService; // חדש
        }

        public List<ParkingSpotDTO> GetAllParkingSpots()
        {
            var spots = _spotRepo.GetAllParkingSpots();
            return _mapper.Map<List<ParkingSpotDTO>>(spots);
        }

        public ParkingSpotDTO? GetParkingSpotById(int id)
        {
            var spot = _spotRepo.GetParkingSpotById(id);
            if (spot == null) return null;
            return _mapper.Map<ParkingSpotDTO>(spot);
        }

        // לוגיקה עסקית: הגדרת חניה כפעילה כברירת מחדל בעת יצירה
        public ParkingSpotDTO CreateParkingSpot(ParkingSpotCreateDTO dto,int ownerId)
        {
            var spot = _mapper.Map<ParkingSpots>(dto);
            spot.OwnerId = ownerId;
            spot.IsActive = true;

            // חדש: אם הגיעה תמונה - שירות התמונות שומר אותה ומחזיר את השם
            if (dto.ImageFile != null)
                spot.Image = _imageService.Save(dto.ImageFile);

            _spotRepo.AddParkingSpot(spot);
            return _mapper.Map<ParkingSpotDTO>(spot);
        }

        public ParkingSpotDTO? UpdateParkingSpot(int id, ParkingSpotDTO dto,int ownerId)
        {
            var spot = _spotRepo.GetOwnedSpot(id,ownerId);
            if (spot == null) return null;

            _mapper.Map(dto, spot);
            _spotRepo.UpdateParkingSpot(spot);

            return _mapper.Map<ParkingSpotDTO>(spot);
        }

        public bool DeleteParkingSpot(int id, int ownerId)
        {
            var spot = _spotRepo.GetOwnedSpot(id, ownerId);
            if (spot == null) return false;

            _spotRepo.DeleteParkingSpot(spot);

            // חדש: מוחקים גם את קובץ התמונה, כדי שלא יישאר בתיקייה סתם
            if (spot.Image != null)
                _imageService.Delete(spot.Image);

            return true;
        }

        public List<ParkingSpotDTO> GetParkingSpotsByOwnerId(int ownerId)
        {
            var spots = _spotRepo.GetParkingSpotsByOwnerId(ownerId);
            return _mapper.Map<List<ParkingSpotDTO>>(spots);
        }
    }
}