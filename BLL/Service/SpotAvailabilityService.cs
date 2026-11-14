using AutoMapper;
using BLL.Exceptions;
using BLL.Interfaces;
using DAL;
using DAL.Repositories;
using DTO;

namespace BLL.Services
{
    public class SpotAvailabilityService : ISpotAvailabilityService
    {
        private readonly ISpotAvailabilityRepository _availabilityRepo;
        private readonly IParkingSpotRepository _spotRepo;
        private readonly IMapper _mapper;

        public SpotAvailabilityService(
            ISpotAvailabilityRepository availabilityRepo,
            IParkingSpotRepository spotRepo,
            IMapper mapper)
        {
            _availabilityRepo = availabilityRepo;
            _spotRepo = spotRepo;
            _mapper = mapper;
        }

        public List<SpotAvailabilityDTO> GetAllSpotAvailabilities()
        {
            var availabilities = _availabilityRepo.GetAllSpotAvailabilities();
            return _mapper.Map<List<SpotAvailabilityDTO>>(availabilities);
        }

        public SpotAvailabilityDTO GetSpotAvailabilityById(int id)
        {
            // אם הזמינות לא קיימת - זורקים שגיאה, והשומר (GlobalExceptionFilter) מחזיר 404
            var availability = _availabilityRepo.GetSpotAvailabilityById(id)
                ?? throw new NotFoundException($"זמינות מספר {id} לא נמצאה.");

            return _mapper.Map<SpotAvailabilityDTO>(availability);
        }

        // מוסיפים זמינות רק לחניה של בעל החניה המחובר
        public SpotAvailabilityDTO CreateSpotAvailability(SpotAvailabilityCreateDTO dto, int ownerId)
        {
            // החניה לא קיימת או לא שלו - אותה תשובה (404)
            var spot = _spotRepo.GetOwnedSpot(dto.SpotID, ownerId)
                ?? throw new NotFoundException($"חניה מספר {dto.SpotID} לא נמצאה.");

            var availability = _mapper.Map<SpotAvailability>(dto);
            availability.IsBooked = false;

            _availabilityRepo.AddSpotAvailability(availability);
            return _mapper.Map<SpotAvailabilityDTO>(availability);
        }

        public SpotAvailabilityDTO UpdateSpotAvailability(int id, SpotAvailabilityDTO dto)
        {
            var availability = _availabilityRepo.GetSpotAvailabilityById(id)
                ?? throw new NotFoundException($"זמינות מספר {id} לא נמצאה.");

            _mapper.Map(dto, availability);
            _availabilityRepo.UpdateSpotAvailability(availability);

            return _mapper.Map<SpotAvailabilityDTO>(availability);
        }

        // מוחקים זמינות רק מחניה של בעל החניה המחובר
        public void DeleteSpotAvailability(int id, int ownerId)
        {
            var availability = _availabilityRepo.GetSpotAvailabilityById(id)
                ?? throw new NotFoundException($"זמינות מספר {id} לא נמצאה.");

            // החניה לא שלו - עונים "לא נמצאה", כדי לא לגלות שהיא קיימת
            var spot = _spotRepo.GetOwnedSpot(availability.SpotId, ownerId)
                ?? throw new NotFoundException($"זמינות מספר {id} לא נמצאה.");

            _availabilityRepo.DeleteSpotAvailability(availability);
        }

        public List<SpotAvailabilityDTO> GetAvailabilityBySpotId(int spotId)
        {
            var availabilities = _availabilityRepo.GetAvailabilityBySpotId(spotId);
            return _mapper.Map<List<SpotAvailabilityDTO>>(availabilities);
        }
    }
}