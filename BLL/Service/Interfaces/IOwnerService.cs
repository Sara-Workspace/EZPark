using DTO;

namespace BLL.Interfaces
{
    public interface IOwnerService
    {
        Task<OwnerDTO?> ValidateOwnerCredentialsAsync(string email, string password);   // התחברות
        List<OwnerDTO> GetAllOwners();
        OwnerDTO? GetOwnerById(int id);
        OwnerDTO CreateOwner(OwnerCreateDTO dto);
        OwnerDTO? UpdateOwner(int id, OwnerUpdateDTO dto);
        bool DeleteOwner(int id);
        OwnerDTO? GetOwnerByEmail(string email);
        OwnerWithSpotsDTO? GetOwnerWithSpots(int id);
    }
}
