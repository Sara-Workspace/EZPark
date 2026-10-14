using DTO;                                              // כדי להכיר את מחלקות ה-DTO

namespace BLL.Repositories                              // הקובץ נמצא בתיקייה Repositories שב-BLL
{
    public interface IOwnersRepository                  // ממשק: רשימת הפעולות על בעלי חניות
    {
        List<OwnerDTO> GetAllOwners();                  // מחזיר את כל הבעלים
        OwnerDTO? GetOwnerById(int id);                 // מחזיר בעלים לפי קוד (null אם לא נמצא)
        OwnerDTO CreateOwner(OwnerCreateDTO dto);       // מוסיף בעלים חדש ומחזיר אותו
        OwnerDTO? UpdateOwner(int id, OwnerUpdateDTO dto); // מעדכן בעלים (null אם לא נמצא)
        bool DeleteOwner(int id);                       // מוחק בעלים (true אם הצליח)

        OwnerDTO? GetOwnerByEmail(string email);        // נוסף: מחפש בעלים לפי מייל (להתחברות)
        OwnerWithSpotsDTO? GetOwnerWithSpots(int id);   // נוסף: בעלים + רשימת החניות שלו
    }
}