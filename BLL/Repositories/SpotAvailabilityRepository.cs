using AutoMapper;
using AutoMapper.QueryableExtensions;
using DAL;
using DTO;

namespace BLL.Repositories
{
    public class SpotAvailabilityRepository : ISpotAvailabilityRepository
    {
        private readonly ParkingDbContext _context;
        private readonly IMapper _mapper;

        public SpotAvailabilityRepository(ParkingDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<SpotAvailabilityDTO> GetAllSpotAvailabilities()
        {
            return _context.SpotAvailability
                .ProjectTo<SpotAvailabilityDTO>(_mapper.ConfigurationProvider)
                .ToList();
        }

        public SpotAvailabilityDTO? GetSpotAvailabilityById(int id)
        {
            return _context.SpotAvailability
                .Where(a => a.AvailabilityId == id)
                .ProjectTo<SpotAvailabilityDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefault();
        }

        public SpotAvailabilityDTO CreateSpotAvailability(SpotAvailabilityCreateDTO dto)
        {
            var availability = _mapper.Map<SpotAvailability>(dto);
            availability.IsBooked = false;              // זמינות חדשה עוד לא הוזמנה

            _context.SpotAvailability.Add(availability);
            _context.SaveChanges();

            return _mapper.Map<SpotAvailabilityDTO>(availability);
        }

        public SpotAvailabilityDTO? UpdateSpotAvailability(int id, SpotAvailabilityDTO dto)
        {
            var availability = _context.SpotAvailability.Find(id);
            if (availability == null) return null;

            _mapper.Map(dto, availability);
            _context.SaveChanges();

            return _mapper.Map<SpotAvailabilityDTO>(availability);
        }

        public bool DeleteSpotAvailability(int id)
        {
            var availability = _context.SpotAvailability.Find(id);
            if (availability == null) return false;

            _context.SpotAvailability.Remove(availability);
            _context.SaveChanges();
            return true;
        }

        public List<SpotAvailabilityDTO> GetAvailabilityBySpotId(int spotId)
        {
            return _context.SpotAvailability
                .Where(a => a.SpotId == spotId)
                .OrderBy(a => a.StartTime)              // OrderBy: ממיין לפי שעת ההתחלה, מהמוקדם למאוחר
                .ProjectTo<SpotAvailabilityDTO>(_mapper.ConfigurationProvider)
                .ToList();
        }
    }
}