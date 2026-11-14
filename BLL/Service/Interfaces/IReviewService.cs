using DTO;

namespace BLL.Interfaces
{
    public interface IReviewService
    {
        List<ReviewDTO> GetAllReviews();
        ReviewDTO? GetReviewById(int id);
        ReviewDTO? CreateReview(ReviewCreateDTO dto, int driverId);
        ReviewDTO? UpdateReview(int id, ReviewDTO dto);
        bool DeleteReview(int id, int driverId);
        List<ReviewDTO> GetReviewsBySpotId(int spotId);
        double GetAverageRatingBySpotId(int spotId);
    }
}
