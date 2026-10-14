using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using AutoMapper;



namespace DTO
{
    public class MappingProfile:Profile
    {
        // מיפויים עבור נהג (Drivers)
        CreateMap<Driver, DriverDTO>().ReverseMap();
        CreateMap<DriverCreateDTO, Driver>();

            // מיפויים עבור בעל חניה (Owners)
            CreateMap<Owners, OwnerDTO>().ReverseMap();
        CreateMap<OwnerCreateDTO, Owner>();

            // מיפויים עבור מקומות חניה (ParkingSpots)
            CreateMap<ParkingSpots, ParkingSpotDTO>().ReverseMap();
        CreateMap<ParkingSpotCreateDTO, ParkingSpots>();

            // מיפויים עבור הזמנות (Reservations)
            CreateMap<Reservations, ReservationDTO>().ReverseMap();
        CreateMap<ReservationCreateDTO, Reservations>();

            // מיפויים עבור תשלומים (Payment)
            CreateMap<Payment, PaymentDTO>().ReverseMap();
        CreateMap<PaymentCreateDTO, Payment>();
    }
}
