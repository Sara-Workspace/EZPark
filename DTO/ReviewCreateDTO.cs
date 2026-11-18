using System.ComponentModel.DataAnnotations;

namespace DTO
{
    public class ReviewCreateDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "יש לבחור הזמנה")]
        public int ReservationID { get; set; }

        [Range(1, 5, ErrorMessage = "הדירוג חייב להיות בין 1 ל-5")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "יש לכתוב תגובה")]
        [StringLength(500, MinimumLength = 2, ErrorMessage = "התגובה חייבת להכיל בין 2 ל-500 תווים")]
        public string Comment { get; set; } = string.Empty;
    }
}