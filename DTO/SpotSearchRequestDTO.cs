using System.ComponentModel.DataAnnotations;

namespace DTO
{
    // הפרמטרים של חיפוש חניה (מגיעים בכתובת ה-URL)
    public class SpotSearchRequestDTO
    {
        [Range(-90, 90)]
        public double Latitude { get; set; }            // קו רוחב של היעד

        [Range(-180, 180)]
        public double Longitude { get; set; }           // קו אורך של היעד

        [Range(0.1, 50)]
        public double RadiusKm { get; set; } = 2;       // רדיוס חיפוש בק"מ

        public string SortBy { get; set; } = "smart";   // smart / distance / price

        public List<int> ExcludeIds { get; set; } = new(); // חניות שלא להציג (למשל "החניה תפוסה")

        public bool EvOnly { get; set; }                // רק חניות עם עמדת טעינה

        [Range(1, 100)]
        public int Limit { get; set; } = 30;            // כמה תוצאות לכל היותר
    }
}