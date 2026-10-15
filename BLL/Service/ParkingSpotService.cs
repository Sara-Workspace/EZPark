using AutoMapper;
using DAL;
using DAL.Repositories;
using DTO;

namespace BLL.Services
{
    public class ParkingSpotService
    {
        private readonly IParkingSpotRepository _spotRepo;
        private readonly IMapper _mapper;

        public ParkingSpotService(IParkingSpotRepository spotRepo, IMapper mapper)
        {
            _spotRepo = spotRepo;
            _mapper = mapper;
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
        public ParkingSpotDTO CreateParkingSpot(ParkingSpotCreateDTO dto)
        {
            var spot = _mapper.Map<ParkingSpots>(dto);
            spot.IsActive = true;

            _spotRepo.AddParkingSpot(spot);
            return _mapper.Map<ParkingSpotDTO>(spot);
        }

        public ParkingSpotDTO? UpdateParkingSpot(int id, ParkingSpotDTO dto)
        {
            var spot = _spotRepo.GetParkingSpotById(id);
            if (spot == null) return null;

            _mapper.Map(dto, spot);
            _spotRepo.UpdateParkingSpot(spot);

            return _mapper.Map<ParkingSpotDTO>(spot);
        }

        public bool DeleteParkingSpot(int id)
        {
            var spot = _spotRepo.GetParkingSpotById(id);
            if (spot == null) return false;

            _spotRepo.DeleteParkingSpot(spot);
            return true;
        }

        public List<ParkingSpotDTO> GetParkingSpotsByOwnerId(int ownerId)
        {
            var spots = _spotRepo.GetParkingSpotsByOwnerId(ownerId);
            return _mapper.Map<List<ParkingSpotDTO>>(spots);
        }
    }
}