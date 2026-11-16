namespace DTO
{
    // חניה מהחיפוש: כל הפרטים הרגילים + מה שהאלגוריתם חישב
    public class ParkingSpotSearchResultDTO : ParkingSpotDTO
    {
        public double DistanceKm { get; set; }               // מרחק מהיעד
        public decimal EffectivePricePerHour { get; set; }   // מחיר בפועל כרגע (כולל שעות עומס)
        public double Score { get; set; }                    // הציון (נמוך = טוב יותר)
    }
}