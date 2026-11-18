namespace DAL.Repositories
{
    // הגישה לטבלת SpotChecks במסד
    public class SpotChecksRepository : ISpotChecksRepository
    {
        private readonly ParkingDbContext _context;

        public SpotChecksRepository(ParkingDbContext context)
        {
            _context = context;
        }

        public void AddSpotCheck(SpotChecks check)
        {
            _context.SpotChecks.Add(check);
            _context.SaveChanges();
        }

        // Any = "יש לפחות שורה אחת כזאת?" (מחזיר true/false בלי לשלוף את השורות)
        public bool WasFinedRecently(int spotId, string licensePlate, DateTime sinceUtc)
        {
            return _context.SpotChecks.Any(c =>
                c.SpotId == spotId &&
                c.LicensePlate == licensePlate &&
                c.FinePoints > 0 &&
                c.CheckedAt >= sinceUtc);
        }
    }
}