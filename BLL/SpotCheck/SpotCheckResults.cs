namespace BLL.SpotCheck
{
    // התוצאות האפשריות של בדיקת חניה.
    // שומרים אותן כאן במקום אחד, כדי לא לכתוב את אותה מילה בכמה מקומות (ולטעות באות)
    public static class SpotCheckResults
    {
        public const string Paying = "Paying";           // רכב של נהג שהזמין את החניה. תקין
        public const string OwnerCar = "OwnerCar";       // הרכב של בעל החניה עצמו
        public const string OtherDriver = "OtherDriver"; // רכב של נהג אחר שחנה בלי להזמין. קנס
        public const string Unknown = "Unknown";         // רכב שלא רשום באתר
    }
}