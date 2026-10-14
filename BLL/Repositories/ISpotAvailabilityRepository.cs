using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO;                                      // SpotAvailabilityDTO


namespace BLL.Repositories
{
    public interface ISpotAvailabilityRepository
    {
        List<SpotAvailabilityDTO> GetAllSpotAvailabilities();                           // כל רשומות הזמינות
        SpotAvailabilityDTO? GetSpotAvailabilityById(int id);                           // רשומה לפי קוד, או null
        SpotAvailabilityDTO CreateSpotAvailability(SpotAvailabilityDTO dto);            // הוספת זמינות
        SpotAvailabilityDTO? UpdateSpotAvailability(int id, SpotAvailabilityDTO dto);   // עדכון זמינות
        bool DeleteSpotAvailability(int id);                                            // מחיקת זמינות

        List<SpotAvailabilityDTO> GetAvailabilityBySpotId(int spotId);                  // ספציפית: הזמינות של חניה מסוימת
    }
}
