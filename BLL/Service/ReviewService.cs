using AutoMapper;
using BLL.Interfaces;
using DAL;
using DAL.Repositories;
using DTO;

namespace BLL.Services
{
    public class ReviewService:IReviewService
    {
        private readonly IReviewsRepository _reviewRepo;
        private readonly IReservationsRepository _reservationRepo;   // ← תיקון 4
        private readonly IParkingSpotRepository _spotRepo;           // ← תיקון 4
        private readonly IMapper _mapper;

        public ReviewService(
            IReviewsRepository reviewRepo,
            IReservationsRepository reservationRepo,                 // ← תיקון 4
            IParkingSpotRepository spotRepo,                         // ← תיקון 4
            IMapper mapper)
        {
            _reviewRepo = reviewRepo;
            _reservationRepo = reservationRepo;
            _spotRepo = spotRepo;
            _mapper = mapper;
        }

        public List<ReviewDTO> GetAllReviews()
        {
            var reviews = _reviewRepo.GetAllReviews();
            return _mapper.Map<List<ReviewDTO>>(reviews);
        }

        public ReviewDTO? GetReviewById(int id)
        {
            var review = _reviewRepo.GetReviewById(id);
            if (review == null) return null;
            return _mapper.Map<ReviewDTO>(review);
        }

        // ← תיקון 4: הנהג מגיע מהטוקן, ובודקים שההזמנה באמת שלו
        public ReviewDTO? CreateReview(ReviewCreateDTO dto, int driverId)
        {
            // 1. ההזמנה קיימת, והיא של הנהג הזה?
            var reservation = _reservationRepo.GetReservationById(dto.ReservationID);
            if (reservation == null || reservation.DriverID != driverId)
                return null;   // לא קיימת, או לא שלו

            // 2. מי בעל החניה שמקבל את הביקורת?
            var spot = _spotRepo.GetParkingSpotById(reservation.SpotID);
            if (spot == null)
                return null;

            // 3. יוצרים את הביקורת
            var review = _mapper.Map<Reviews>(dto);
            review.DriverId = driverId;        // מהטוקן, לא מהלקוח
            review.OwnerId = spot.OwnerId;     // מהמסד, לא מהלקוח
            review.CreatedAt = DateTime.Now;

            _reviewRepo.AddReview(review);
            return _mapper.Map<ReviewDTO>(review);
        }

        public ReviewDTO? UpdateReview(int id, ReviewDTO dto)
        {
            var review = _reviewRepo.GetReviewById(id);
            if (review == null) return null;

            _mapper.Map(dto, review);
            _reviewRepo.UpdateReview(review);

            return _mapper.Map<ReviewDTO>(review);
        }

        // ← תיקון 4: מוחקים רק ביקורת שהנהג כתב בעצמו
        public bool DeleteReview(int id, int driverId)
        {
            var review = _reviewRepo.GetOwnedReview(id, driverId);
            if (review == null) return false;   // לא קיימת, או לא שלו

            _reviewRepo.DeleteReview(review);
            return true;
        }

        public List<ReviewDTO> GetReviewsBySpotId(int spotId)
        {
            var reviews = _reviewRepo.GetReviewsBySpotId(spotId);
            return _mapper.Map<List<ReviewDTO>>(reviews);
        }

        // לוגיקה חישובית: חישוב ממוצע דירוגים לחניה
        public double GetAverageRatingBySpotId(int spotId)
        {
            var reviews = _reviewRepo.GetReviewsBySpotId(spotId);
            if (!reviews.Any()) return 0;
            return reviews.Average(r => r.Rating);
        }
    }
}