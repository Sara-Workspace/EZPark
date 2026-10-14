using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO;                                      // כדי להשתמש ב-OwnerDTO

namespace BLL.Repositories
{
    public interface IOwnersRepository
    {
        List<OwnerDTO> GetAllOwners();                      // כל הבעלים
        OwnerDTO? GetOwnerById(int id);                     // בעלים לפי קוד, או null
        OwnerDTO CreateOwner(OwnerDTO dto);                 // הוספת בעלים חדש
        OwnerDTO? UpdateOwner(int id, OwnerDTO dto);        // עדכון בעלים, או null אם לא קיים
        bool DeleteOwner(int id);                           // מחיקת בעלים

        OwnerDTO? GetOwnerByEmail(string email);            // ספציפית: בעלים לפי מייל
    }
}
