using AutoMapper;
using DAL;
using DAL.Repositories;
using DTO;

namespace BLL.Services
{
    public class SpotAvailabilityService
    {
        private readonly ISpotAvailabilityRepository _availabilityRepo;
        private readonly IParkingSpotRepository _spotRepo;           // ← תיקון 4
        private readonly IMapper _mapper;

        public SpotAvailabilityService(
            ISpotAvailabilityRepository availabilityRepo,
            IParkingSpotRepository spotRepo,                         // ← תיקון 4
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

        public SpotAvailabilityDTO? GetSpotAvailabilityById(int id)
        {
            var availability = _availabilityRepo.GetSpotAvailabilityById(id);
            if (availability == null) return null;
            return _mapper.Map<SpotAvailabilityDTO>(availability);
        }

        // ← תיקון 4: מוסיפים זמינות רק לחניה של בעל החניה המחובר
        public SpotAvailabilityDTO? CreateSpotAvailability(SpotAvailabilityCreateDTO dto, int ownerId)
        {
            var spot = _spotRepo.GetOwnedSpot(dto.SpotID, ownerId);
            if (spot == null) return null;   // החניה לא קיימת, או לא שלו

            var availability = _mapper.Map<SpotAvailability>(dto);
            availability.IsBooked = false;

            _availabilityRepo.AddSpotAvailability(availability);
            return _mapper.Map<SpotAvailabilityDTO>(availability);
        }

        public SpotAvailabilityDTO? UpdateSpotAvailability(int id, SpotAvailabilityDTO dto)
        {
            var availability = _availabilityRepo.GetSpotAvailabilityById(id);
            if (availability == null) return null;

            _mapper.Map(dto, availability);
            _availabilityRepo.UpdateSpotAvailability(availability);

            return _mapper.Map<SpotAvailabilityDTO>(availability);
        }

        // ← תיקון 4: מוחקים זמינות רק מחניה של בעל החניה המחובר
        public bool DeleteSpotAvailability(int id, int ownerId)
        {
            var availability = _availabilityRepo.GetSpotAvailabilityById(id);
            if (availability == null) return false;

            var spot = _spotRepo.GetOwnedSpot(availability.SpotId, ownerId);
            if (spot == null) return false;   // החניה לא שלו

            _availabilityRepo.DeleteSpotAvailability(availability);
            return true;
        }

        public List<SpotAvailabilityDTO> GetAvailabilityBySpotId(int spotId)
        {
            var availabilities = _availabilityRepo.GetAvailabilityBySpotId(spotId);
            return _mapper.Map<List<SpotAvailabilityDTO>>(availabilities);
        }
    }
}