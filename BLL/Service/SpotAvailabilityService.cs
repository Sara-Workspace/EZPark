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
            // כלל עסקי: סיום אחרי התחלה - השומר מחזיר 400
            if (dto.EndTime <= dto.StartTime)
                throw new BusinessRuleException("שעת הסיום חייבת להיות אחרי שעת ההתחלה.");

            // החניה לא קיימת או לא שלו - אותה תשובה (404)
            var spot = _spotRepo.GetOwnedSpot(dto.SpotID, ownerId)
                ?? throw new NotFoundException($"חניה מספר {dto.SpotID} לא נמצאה.");

            // כלל עסקי: אותו יום לא יכול לכלול שני חלונות שעולים אחד על השני
            // (למשל 08:00-12:00 וגם 10:00-14:00). חפיפה = אחד מתחיל לפני שהשני נגמר
            var overlaps = _availabilityRepo.GetAvailabilityBySpotId(dto.SpotID)
                .Any(a => a.DayOfWeek == dto.DayOfWeek &&
                          dto.StartTime < a.EndTime &&
                          a.StartTime < dto.EndTime);
            if (overlaps)
                throw new BusinessRuleException("השעות האלה חופפות לשעות שכבר הוגדרו לאותו יום.");

            var availability = _mapper.Map<SpotAvailability>(dto);

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