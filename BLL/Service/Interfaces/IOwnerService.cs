using DTO;

namespace BLL.Interfaces
{
    public interface IOwnerService
    {
        Task<OwnerDTO?> ValidateOwnerCredentialsAsync(string email, string password);   // התחברות: null = פרטים שגויים
        List<OwnerDTO> GetAllOwners();
        OwnerDTO GetOwnerById(int id);                                                   // זורק NotFoundException אם לא נמצא
        OwnerDTO CreateOwner(OwnerCreateDTO dto);
        OwnerDTO UpdateOwner(int id, OwnerUpdateDTO dto);                                // זורק NotFoundException אם לא נמצא
        void DeleteOwner(int id);                                                        // זורק NotFoundException אם לא נמצא
        OwnerDTO? GetOwnerByEmail(string email);                                         // null = אין בעל חניה עם המייל הזה
        OwnerWithSpotsDTO GetOwnerWithSpots(int id);                                     // זורק NotFoundException אם לא נמצא
    }
}