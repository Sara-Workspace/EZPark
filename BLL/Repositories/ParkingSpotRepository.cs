using AutoMapper;
using AutoMapper.QueryableExtensions;
using DAL;
using DTO;

namespace BLL.Repositories
{
    public class ParkingSpotRepository : IParkingSpotRepository
    {
        private readonly ParkingDbContext _context;
        private readonly IMapper _mapper;

        public ParkingSpotRepository(ParkingDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<ParkingSpotDTO> GetAllParkingSpots()
        {
            return _context.ParkingSpots
                .ProjectTo<ParkingSpotDTO>(_mapper.ConfigurationProvider)
                .ToList();
        }

        public ParkingSpotDTO? GetParkingSpotById(int id)
        {
            return _context.ParkingSpots
                .Where(p => p.SpotId == id)
                .ProjectTo<ParkingSpotDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefault();
        }

        public ParkingSpotDTO CreateParkingSpot(ParkingSpotCreateDTO dto)
        {
            var spot = _mapper.Map<ParkingSpots>(dto);
            spot.IsActive = true;                               // חניה חדשה פעילה כברירת מחדל

            _context.ParkingSpots.Add(spot);
            _context.SaveChanges();

            return _mapper.Map<ParkingSpotDTO>(spot);
        }

        public ParkingSpotDTO? UpdateParkingSpot(int id, ParkingSpotDTO dto)
        {
            var spot = _context.ParkingSpots.Find(id);
            if (spot == null) return null;

            _mapper.Map(dto, spot);
            _context.SaveChanges();

            return _mapper.Map<ParkingSpotDTO>(spot);
        }

        public bool DeleteParkingSpot(int id)
        {
            var spot = _context.ParkingSpots.Find(id);
            if (spot == null) return false;

            _context.ParkingSpots.Remove(spot);
            _context.SaveChanges();
            return true;
        }

        public List<ParkingSpotDTO> GetAvailableParkingSpots()     // חניות פנויות
        {
            var now = DateTime.Now;                                 // השעה הנוכחית

            return _context.ParkingSpots
                .Where(p => p.IsActive &&                           // רק חניות פעילות
                            _context.SpotAvailability.Any(a =>      // שיש להן לפחות זמינות אחת:
                                a.SpotId == p.SpotId &&             //   של החניה הזו
                                !a.IsBooked &&                      //   שעוד לא הוזמנה
                                a.EndTime > now))                   //   ושעוד לא עברה
                .ProjectTo<ParkingSpotDTO>(_mapper.ConfigurationProvider)
                .ToList();
        }

        public List<ParkingSpotDTO> GetParkingSpotsByOwnerId(int ownerId)  // החניות של בעלים
        {
            return _context.ParkingSpots
                .Where(p => p.OwnerId == ownerId)                   // רק של הבעלים הזה
                .ProjectTo<ParkingSpotDTO>(_mapper.ConfigurationProvider)
                .ToList();
        }

        public List<ParkingSpotDTO> GetParkingSpotsByCity(string city)     // חיפוש לפי עיר
        {
            return _context.ParkingSpots
                .Where(p => p.City == city && p.IsActive)           // בעיר הזו, ורק פעילות
                .ProjectTo<ParkingSpotDTO>(_mapper.ConfigurationProvider)
                .ToList();
        }

        public ParkingSpotDetailedDTO? GetParkingSpotDetailed(int id)      // חניה + פרטי הבעלים
        {
            return _context.ParkingSpots
                .Where(p => p.SpotId == id)
                .ProjectTo<ParkingSpotDetailedDTO>(_mapper.ConfigurationProvider) // ProjectTo מביא גם את הבעלים, בלי Include
                .FirstOrDefault();
        }

        public ParkingSpotWithReviewsDTO? GetParkingSpotWithReviews(int id) // חניה + ביקורות
        {
            var spot = _context.ParkingSpots
                .Where(p => p.SpotId == id)
                .ProjectTo<ParkingSpotWithReviewsDTO>(_mapper.ConfigurationProvider) // פרטי החניה
                .FirstOrDefault();

            if (spot == null) return null;

            spot.Reviews = _context.Reviews                         // ממלא את רשימת הביקורות:
                .Where(r => r.Reservation.SpotID == id)             // ביקורת -> הזמנה -> חניה
                .ProjectTo<ReviewDTO>(_mapper.ConfigurationProvider)
                .ToList();

            return spot;
        }
    }
}