using AutoMapper;
using BLL.Exceptions;
using BLL.Interfaces;
using DAL;
using DAL.Repositories;
using DTO;

namespace BLL.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewsRepository _reviewRepo;
        private readonly IReservationsRepository _reservationRepo;
        private readonly IParkingSpotRepository _spotRepo;
        private readonly IMapper _mapper;

        public ReviewService(
            IReviewsRepository reviewRepo,
            IReservationsRepository reservationRepo,
            IParkingSpotRepository spotRepo,
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

        public ReviewDTO GetReviewById(int id)
        {
            // אם הביקורת לא קיימת - זורקים שגיאה, והשומר (GlobalExceptionFilter) מחזיר 404
            var review = _reviewRepo.GetReviewById(id)
                ?? throw new NotFoundException($"ביקורת מספר {id} לא נמצאה.");

            return _mapper.Map<ReviewDTO>(review);
        }

        // הנהג מגיע מהטוקן, ובודקים שההזמנה באמת שלו
        public ReviewDTO CreateReview(ReviewCreateDTO dto, int driverId)
        {
            // 1. ההזמנה קיימת, והיא של הנהג הזה?
            //    לא קיימת או לא שלו - אותה תשובה (404), כדי לא לגלות הזמנות של אחרים
            var reservation = _reservationRepo.GetReservationById(dto.ReservationID);
            if (reservation == null || reservation.DriverID != driverId)
                throw new NotFoundException($"הזמנה מספר {dto.ReservationID} לא נמצאה.");

            // 2. מי בעל החניה שמקבל את הביקורת?
            var spot = _spotRepo.GetParkingSpotById(reservation.SpotID)
                ?? throw new NotFoundException($"חניה מספר {reservation.SpotID} לא נמצאה.");

            // 3. יוצרים את הביקורת
            var review = _mapper.Map<Reviews>(dto);
            review.DriverId = driverId;        // מהטוקן, לא מהלקוח
            review.OwnerId = spot.OwnerId;     // מהמסד, לא מהלקוח
            review.CreatedAt = DateTime.Now;

            _reviewRepo.AddReview(review);
            return _mapper.Map<ReviewDTO>(review);
        }

        public ReviewDTO UpdateReview(int id, ReviewDTO dto)
        {
            var review = _reviewRepo.GetReviewById(id)
                ?? throw new NotFoundException($"ביקורת מספר {id} לא נמצאה.");

            _mapper.Map(dto, review);
            _reviewRepo.UpdateReview(review);

            return _mapper.Map<ReviewDTO>(review);
        }

        // מוחקים רק ביקורת שהנהג כתב בעצמו
        public void DeleteReview(int id, int driverId)
        {
            // לא קיימת או לא שלו - אותה תשובה (404)
            var review = _reviewRepo.GetOwnedReview(id, driverId)
                ?? throw new NotFoundException($"ביקורת מספר {id} לא נמצאה.");

            _reviewRepo.DeleteReview(review);
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