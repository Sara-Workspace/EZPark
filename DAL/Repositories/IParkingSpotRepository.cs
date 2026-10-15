using DAL;

namespace DAL.Repositories
{
    public interface IParkingSpotRepository
    {
        List<ParkingSpots> GetAllParkingSpots();
        ParkingSpots? GetParkingSpotById(int id);
        void AddParkingSpot(ParkingSpots spot);
        void UpdateParkingSpot(ParkingSpots spot);
        void DeleteParkingSpot(ParkingSpots spot);
        List<ParkingSpots> GetParkingSpotsByOwnerId(int ownerId);
        List<ParkingSpots> GetParkingSpotsByCity(string city);
    }
}