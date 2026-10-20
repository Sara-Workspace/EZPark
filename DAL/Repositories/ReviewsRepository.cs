using DAL;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class ReviewsRepository : IReviewsRepository
    {
        private readonly ParkingDbContext _context;

        public ReviewsRepository(ParkingDbContext context)
        {
            _context = context;
        }

        public List<Reviews> GetAllReviews()
        {
            return _context.Reviews
                            .Include(r => r.Reservation)
                            .ToList();
        }

        public Reviews? GetReviewById(int id)
        {
            return _context.Reviews
                            .Include(r => r.Reservation)
                            .FirstOrDefault(r => r.ReviewId == id);
        }

        public void AddReview(Reviews review)
        {
            _context.Reviews.Add(review);
            _context.SaveChanges();
        }

        public void UpdateReview(Reviews review)
        {
            _context.Reviews.Update(review);
            _context.SaveChanges();
        }

        public void DeleteReview(Reviews review)
        {
            _context.Reviews.Remove(review);
            _context.SaveChanges();
        }

        public List<Reviews> GetReviewsBySpotId(int spotId)
        {
            return _context.Reviews
                .Include(r => r.Reservation)
                .Where(r => r.Reservation.SpotID == spotId)
                .ToList();
        }
    }
}