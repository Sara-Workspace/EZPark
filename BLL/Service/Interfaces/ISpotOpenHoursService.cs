using DAL;

namespace BLL.Interfaces
{
    // "האם החניה פתוחה עכשיו לפי השעות שבעל החניה קבע?"
    public interface ISpotOpenHoursService
    {
        bool IsOpenNow(int spotId);                                // חניה אחת (לפני שמירה / "חניתי")
        List<ParkingSpots> KeepOnlyOpenNow(List<ParkingSpots> spots); // מסנן רשימה (לחיפוש, למפה ולבוט)
    }
}
