namespace DTO.PlateRecognition
{
    // מה השרת מחזיר: המספר שה-AI קרא מהתמונה (רק ספרות, למשל "1234567")
    public class PlateReadResultDTO
    {
        public string LicensePlate { get; set; } = string.Empty;
    }
}