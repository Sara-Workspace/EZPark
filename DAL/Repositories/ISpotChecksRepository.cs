namespace DAL.Repositories
{
    // "החוזה" של טבלת בדיקות החניה
    public interface ISpotChecksRepository
    {
        void AddSpotCheck(SpotChecks check); // שומר בדיקה חדשה

        // האם הרכב הזה כבר קיבל קנס על החניה הזאת מאז sinceUtc?
        // (כדי לא לתת קנס פעמיים על אותה חניה, אם בודקים כמה פעמים באותו יום)
        bool WasFinedRecently(int spotId, string licensePlate, DateTime sinceUtc);
    }
}