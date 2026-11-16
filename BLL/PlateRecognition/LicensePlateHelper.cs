namespace BLL.PlateRecognition
{
    // כלי עזר קטן למספרי רישוי.
    // נהגים מקלידים מספרים בכל מיני צורות: "12-345-67", "12 345 67", "1234567".
    // כדי שנוכל להשוות ביניהם, משאירים רק את הספרות
    public static class LicensePlateHelper
    {
        // "12-345-67" ← "1234567"
        public static string Normalize(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            // עוברים על כל תו ושומרים רק ספרות 0 עד 9
            return new string(text.Where(c => c >= '0' && c <= '9').ToArray());
        }

        // מספר רישוי בישראל: 7 או 8 ספרות (ברכבים ישנים מאוד גם 5 או 6)
        public static bool IsValid(string digits)
        {
            return digits.Length >= 5 && digits.Length <= 8;
        }
    }
}