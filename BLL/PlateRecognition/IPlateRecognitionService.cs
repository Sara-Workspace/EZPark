namespace BLL.PlateRecognition
{
    // "החוזה" של קורא הלוחיות: מקבל תמונה ומחזיר את מספר הרישוי (רק ספרות)
    public interface IPlateRecognitionService
    {
        // image     = התמונה עצמה (בתים)
        // mediaType = סוג התמונה, למשל "image/jpeg"
        // אם לא נמצאה לוחית בתמונה: זורק BusinessRuleException (שגיאה 400 עם הודעה בעברית)
        Task<string> ReadPlateAsync(byte[] image, string mediaType, CancellationToken ct);
    }
}