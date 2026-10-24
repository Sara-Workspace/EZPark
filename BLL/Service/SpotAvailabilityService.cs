using AutoMapper;
using BLL.Interfaces;
using DAL;
using DAL.Repositories;
using DTO;

namespace BLL.Services
{
    public class SpotAvailabilityService:ISpotAvailabilityService
    {
        private readonly ISpotAvailabilityRepository _availabilityRepo;
        private readonly IMapper _mapper;

        public SpotAvailabilityService(ISpotAvailabilityRepository availabilityRepo, IMapper mapper)
        {
            _availabilityRepo = availabilityRepo;
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

        // לוגיקה עסקית: סימון זמינות חדשה כפנויה (IsBooked = false) כברירת מחדל
        public SpotAvailabilityDTO CreateSpotAvailability(SpotAvailabilityCreateDTO dto)
        {
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

        public bool DeleteSpotAvailability(int id)
        {
            var availability = _availabilityRepo.GetSpotAvailabilityById(id);
            if (availability == null) return false;

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