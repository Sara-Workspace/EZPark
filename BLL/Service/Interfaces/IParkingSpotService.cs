using DTO;

namespace BLL.Interfaces
{
    public interface IParkingSpotService
    {
        List<ParkingSpotDTO> GetAllParkingSpots();
        ParkingSpotDTO? GetParkingSpotById(int id);
        ParkingSpotDTO CreateParkingSpot(ParkingSpotCreateDTO dto, int ownerId);
        ParkingSpotDTO? UpdateParkingSpot(int id, ParkingSpotDTO dto, int ownerId);
        bool DeleteParkingSpot(int id, int ownerId);
        List<ParkingSpotDTO> GetParkingSpotsByOwnerId(int ownerId);
    }
}
