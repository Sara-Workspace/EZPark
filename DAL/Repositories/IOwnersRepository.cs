using DAL;
using DAL.Repositories;


namespace DAL.Repositories
{
    public interface IOwnersRepository
    {
        List<Owners> GetAllOwners();
        Owners? GetOwnerById(int id);
        void AddOwner(Owners owner);
        void UpdateOwner(Owners owner);
        void DeleteOwner(Owners owner);
        Owners? GetOwnerByEmail(string email);
        Owners? GetOwnerWithSpots(int id);
    }
}