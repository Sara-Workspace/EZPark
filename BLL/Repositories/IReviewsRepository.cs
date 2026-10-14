using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO;                                      // ReviewDTO


namespace BLL.Repositories
{
    public interface IReviewsRepository
    {
        List<ReviewDTO> GetAllReviews();                        // כל הביקורות
        ReviewDTO? GetReviewById(int id);                       // ביקורת לפי קוד, או null
        ReviewDTO CreateReview(ReviewDTO dto);                  // הוספת ביקורת
        ReviewDTO? UpdateReview(int id, ReviewDTO dto);         // עדכון ביקורת
        bool DeleteReview(int id);                              // מחיקת ביקורת

        List<ReviewDTO> GetReviewsBySpotId(int spotId);         // ספציפית: ביקורות על חניה מסוימת
        double GetAverageRatingBySpotId(int spotId);            // ספציפית: ממוצע הדירוגים של חניה
    }
}
