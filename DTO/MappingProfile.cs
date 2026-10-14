using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DAL;


namespace DTO
{
    public class MappingProfile : Profile
    {
        // שימי לב: הסוגריים המסולסלים האלו הם קריטיים!
        public MappingProfile()
        {
            // כל פקודות המיפוי חייבות להיות אך ורק בתוך הבלוק הזה

            CreateMap<Driver, DriverDTO>().ReverseMap();
            CreateMap<DriverCreateDTO, Driver>();

            CreateMap<Owners, OwnerDTO>().ReverseMap();
            CreateMap<OwnerCreateDTO, Owners>();

            CreateMap<ParkingSpots, ParkingSpotDTO>().ReverseMap();
            CreateMap<ParkingSpotCreateDTO, ParkingSpots>();

            CreateMap<Reservations, ReservationDTO>().ReverseMap();
            CreateMap<ReservationCreateDTO, Reservations>();

            CreateMap<Payment, PaymentDTO>().ReverseMap();
            CreateMap<PaymentCreateDTO, Payment>();

            // מיפויים עבור ביקורות
            CreateMap<Reviews, ReviewDTO>().ReverseMap();
            CreateMap<ReviewCreateDTO, Reviews>();

            // מיפויים עבור זמינות חניה
            // שימי לב: בדקי אם ב-DAL קוראים לטבלה SpotAvailability או שם דומה ותקני בהתאם
            CreateMap<SpotAvailability, SpotAvailabilityDTO>().ReverseMap();
            CreateMap<SpotAvailabilityCreateDTO, SpotAvailability>();

            // מיפויים עבור תמחור דינמי
            CreateMap<DynamicPricingRules, DynamicPricingRulesDTO>().ReverseMap();
            CreateMap<DynamicPricingRulesCreateDTO, DynamicPricingRules>();

            CreateMap<DriverUpdateDTO, Driver>();
            CreateMap<OwnerUpdateDTO, Owners>();

            CreateMap<ParkingSpots, ParkingSpotDetailedDTO>();

            CreateMap<Owners, OwnerWithSpotsDTO>();
            CreateMap<ParkingSpots, ParkingSpotWithReviewsDTO>();

        }
    }
}