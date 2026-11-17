namespace DTO.SpotCheck
{
    // מה השרת מחזיר אחרי בדיקת חניה
    public class SpotCheckResultDTO
    {
        public int SpotId { get; set; }                       // איזו חניה נבדקה
        public string LicensePlate { get; set; } = string.Empty; // המספר שה-AI קרא (רק ספרות)

        // התוצאה, בשביל הקוד ב-React:
        // Paying      = רכב של נהג שהזמין את החניה (תקין)
        // OwnerCar    = הרכב של בעל החניה
        // OtherDriver = רכב של נהג אחר שחנה בלי להזמין (קנס)
        // Unknown     = רכב שלא רשום באתר
        public string Result { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;  // הסבר בעברית, להצגה למשתמש
        public int FinePoints { get; set; }                   // כמה נקודות קנס ניתנו (0 = בלי קנס)
        public bool SpotAvailable { get; set; }               // האם החניה עדיין מוצגת לנהגים בחיפוש
    }
}