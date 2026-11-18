using DTO;

namespace BLL.Interfaces
{
    public interface IReviewService
    {
        List<ReviewDTO> GetAllReviews();
        ReviewDTO GetReviewById(int id);                              // זורק NotFoundException אם לא נמצאה
        ReviewDTO CreateReview(ReviewCreateDTO dto, int driverId);    // זורק NotFoundException אם ההזמנה לא קיימת / לא שלו
        ReviewDTO UpdateReview(int id, ReviewDTO dto);                // זורק NotFoundException אם לא נמצאה
        void DeleteReview(int id, int driverId);                      // זורק NotFoundException אם לא נמצאה / לא שלו
        List<ReviewDTO> GetReviewsBySpotId(int spotId);
        double GetAverageRatingBySpotId(int spotId);
    }
}