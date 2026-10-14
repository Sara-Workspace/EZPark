using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO;                                      // ParkingSpotDTO

namespace BLL.Repositories
{
    public interface IParkingSpotRepository
    {
        List<ParkingSpotDTO> GetAllParkingSpots();                      // כל החניות
        ParkingSpotDTO? GetParkingSpotById(int id);                     // חניה לפי קוד, או null
        ParkingSpotDTO CreateParkingSpot(ParkingSpotDTO dto);           // הוספת חניה (בעלים מציע חניה להשכרה)
        ParkingSpotDTO? UpdateParkingSpot(int id, ParkingSpotDTO dto);  // עדכון חניה
        bool DeleteParkingSpot(int id);                                 // מחיקת חניה

        List<ParkingSpotDTO> GetAvailableParkingSpots();                // ספציפית: רק חניות פנויות להשכרה
        List<ParkingSpotDTO> GetParkingSpotsByOwnerId(int ownerId);     // ספציפית: החניות של בעלים מסוים
        List<ParkingSpotDTO> GetParkingSpotsByCity(string city);        // ספציפית: חיפוש חניות לפי עיר
    }
}
