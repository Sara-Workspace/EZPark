namespace DTO
{
    public class ReservationCreateDTO
    {
        public int SpotID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool UseCharging { get; set; }
        // ← תיקון 5: אין כאן DriverId. הנהג נלקח מהטוקן
    }
}