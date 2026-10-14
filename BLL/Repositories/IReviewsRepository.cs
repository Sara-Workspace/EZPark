using DTO;                                                  // כדי להכיר את מחלקות ה-DTO

namespace BLL.Repositories                                  // הקובץ נמצא בתיקייה Repositories שב-BLL
{
    public interface IReviewsRepository                     // ממשק: רשימת הפעולות על ביקורות
    {
        List<ReviewDTO> GetAllReviews();                    // מחזיר את כל הביקורות
        ReviewDTO? GetReviewById(int id);                   // מחזיר ביקורת לפי קוד (null אם לא נמצאה)
        ReviewDTO CreateReview(ReviewCreateDTO dto);        // מוסיף ביקורת חדשה ומחזיר אותה
        ReviewDTO? UpdateReview(int id, ReviewDTO dto);     // מעדכן ביקורת (null אם לא נמצאה)
        bool DeleteReview(int id);                          // מוחק ביקורת (true אם הצליח)

        List<ReviewDTO> GetReviewsBySpotId(int spotId);     // נוסף: הביקורות על חניה מסוימת
        double GetAverageRatingBySpotId(int spotId);        // נוסף: ממוצע הדירוגים של חניה
    }
}