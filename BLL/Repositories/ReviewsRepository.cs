using AutoMapper;
using AutoMapper.QueryableExtensions;
using DAL;
using DTO;

namespace BLL.Repositories
{
    public class ReviewsRepository : IReviewsRepository
    {
        private readonly ParkingDbContext _context;
        private readonly IMapper _mapper;

        public ReviewsRepository(ParkingDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<ReviewDTO> GetAllReviews()
        {
            return _context.Reviews
                .ProjectTo<ReviewDTO>(_mapper.ConfigurationProvider)
                .ToList();
        }

        public ReviewDTO? GetReviewById(int id)
        {
            return _context.Reviews
                .Where(r => r.ReviewId == id)
                .ProjectTo<ReviewDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefault();
        }

        public ReviewDTO CreateReview(ReviewCreateDTO dto)
        {
            var review = _mapper.Map<Reviews>(dto);
            review.CreatedAt = DateTime.Now;

            _context.Reviews.Add(review);
            _context.SaveChanges();

            return _mapper.Map<ReviewDTO>(review);
        }

        public ReviewDTO? UpdateReview(int id, ReviewDTO dto)
        {
            var review = _context.Reviews.Find(id);
            if (review == null) return null;

            _mapper.Map(dto, review);
            _context.SaveChanges();

            return _mapper.Map<ReviewDTO>(review);
        }

        public bool DeleteReview(int id)
        {
            var review = _context.Reviews.Find(id);
            if (review == null) return false;

            _context.Reviews.Remove(review);
            _context.SaveChanges();
            return true;
        }

        public List<ReviewDTO> GetReviewsBySpotId(int spotId)
        {
            return _context.Reviews
                .Where(r => r.Reservation.SpotID == spotId)     // ביקורת -> הזמנה -> חניה
                .ProjectTo<ReviewDTO>(_mapper.ConfigurationProvider)
                .ToList();
        }

        public double GetAverageRatingBySpotId(int spotId)
        {
            var reviews = _context.Reviews
                .Where(r => r.Reservation.SpotID == spotId);    // כל הביקורות על החניה

            if (!reviews.Any()) return 0;                       // אין ביקורות: הממוצע 0
            return reviews.Average(r => r.Rating);              // Average: מחשב ממוצע של הדירוגים
        }
    }
}