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
        private readonly IMapper _mapper;

        public ReviewService(IReviewsRepository reviewRepo, IMapper mapper)
        {
            _reviewRepo = reviewRepo;
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

        // לוגיקה עסקית: קביעת תאריך כתיבת הביקורת לעכשיו
        public ReviewDTO CreateReview(ReviewCreateDTO dto)
        {
            var review = _mapper.Map<Reviews>(dto);
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

        public bool DeleteReview(int id)
        {
            var review = _reviewRepo.GetReviewById(id);
            if (review == null) return false;

            _reviewRepo.DeleteReview(review);
            return true;
        }

        public List<ReviewDTO> GetReviewsBySpotId(int spotId)
        {
            var reviews = _reviewRepo.GetReviewsBySpotId(spotId);
            return _mapper.Map<List<ReviewDTO>>(reviews);
        }

        // לוגיקה חישובית שעברה ל-Service: חישוב ממוצע דירוגים לחניה
        public double GetAverageRatingBySpotId(int spotId)
        {
            var reviews = _reviewRepo.GetReviewsBySpotId(spotId);
            if (!reviews.Any()) return 0;
            return reviews.Average(r => r.Rating);
        }
    }
}