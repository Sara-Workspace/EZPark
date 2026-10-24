using DTO;

namespace BLL.Interfaces
{
    public interface IParkingSpotService
    {
        List<ParkingSpotDTO> GetAllParkingSpots();
        ParkingSpotDTO? GetParkingSpotById(int id);
        ParkingSpotDTO CreateParkingSpot(ParkingSpotCreateDTO dto);
        ParkingSpotDTO? UpdateParkingSpot(int id, ParkingSpotDTO dto);
        bool DeleteParkingSpot(int id);
        List<ParkingSpotDTO> GetParkingSpotsByOwnerId(int ownerId);
    }
}