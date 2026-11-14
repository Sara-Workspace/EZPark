using System.ComponentModel.DataAnnotations;

namespace DTO
{
    public class ParkingSpotDTO
    {
        public int SpotId { get; set; }
        public int OwnerId { get; set; }

        [Required(ErrorMessage = "כתובת היא שדה חובה")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "הכתובת חייבת להכיל בין 2 ל-200 תווים")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "עיר היא שדה חובה")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "שם העיר חייב להכיל בין 2 ל-100 תווים")]
        public string City { get; set; } = string.Empty;

        [Range(-90, 90, ErrorMessage = "קו רוחב (Latitude) חייב להיות בין 90- ל-90")]
        public double Latitude { get; set; }

        [Range(-180, 180, ErrorMessage = "קו אורך (Longitude) חייב להיות בין 180- ל-180")]
        public double Longitude { get; set; }

        public bool EvCharger { get; set; }

        [Range(0.01, 10000.0, ErrorMessage = "המחיר לשעה חייב להיות בין 0.01 ל-10,000")]
        public decimal BasePricePerHour { get; set; }

        public bool IsActive { get; set; }
        public string? Image { get; set; } // שם הקובץ
    }
}