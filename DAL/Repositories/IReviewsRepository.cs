using DAL;

namespace DAL.Repositories
{
    public interface IReviewsRepository
    {
        List<Reviews> GetAllReviews();
        Reviews? GetReviewById(int id);
        void AddReview(Reviews review);
        void UpdateReview(Reviews review);
        void DeleteReview(Reviews review);
        List<Reviews> GetReviewsBySpotId(int spotId);
    }
}