using AutoMapper;   // ספריית ההמרות
using DAL;          // מחלקות הטבלאות
using DTO;          // מחלקות ה-DTO

namespace BLL       // שייך לשכבת BLL
{
    public class MappingProfile : Profile   // דף הוראות ל-AutoMapper
    {
        public MappingProfile()             // כאן מוגדרות כל ההמרות
        {
            // ===== נהגים =====
            CreateMap<Driver, DriverDTO>()  // נהג -> DTO (משיכת שדות האימות מ-User)
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.Phone))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.User.CreatedAt))
                .ForMember(dest => dest.IsAdmin, opt => opt.MapFrom(src => src.User.IsAdmin));

            CreateMap<DriverCreateDTO, Driver>();       // נהג חדש -> טבלה
            CreateMap<DriverUpdateDTO, Driver>();       // עדכון נהג -> טבלה

            // ===== בעלי חניות =====
            CreateMap<Owners, OwnerDTO>()   // בעלים -> DTO (משיכת שדות האימות מ-User)
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.Phone))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.User.CreatedAt))
                .ForMember(dest => dest.IsAdmin, opt => opt.MapFrom(src => src.User.IsAdmin));

            CreateMap<Owners, OwnerWithSpotsDTO>() // בעלים + החניות שלו (כולל פרטי User)
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.Phone))
                ;
            CreateMap<OwnerCreateDTO, Owners>();        // בעלים חדש -> טבלה
            CreateMap<OwnerUpdateDTO, Owners>();        // עדכון בעלים -> טבלה

            // ===== חניות =====
            CreateMap<ParkingSpots, ParkingSpotDTO>();          // חניה -> DTO
            CreateMap<ParkingSpotCreateDTO, ParkingSpots>()     // חניה חדשה -> טבלה
                .ForMember(dest => dest.Image,
                           opt => opt.Ignore());                // לא למלא כאן
            CreateMap<ParkingSpotDTO, ParkingSpots>()           // עדכון חניה -> טבלה
                .ForMember(dest => dest.SpotId,
                           opt => opt.Ignore())
                .ForMember(dest => dest.Image,
                           opt => opt.Ignore());
            CreateMap<ParkingSpots, ParkingSpotDetailedDTO>();  // חניה + פרטי הבעלים
            CreateMap<ParkingSpots, ParkingSpotWithReviewsDTO>()    // חניה + ביקורות
                .ForMember(dest => dest.Reviews,
                           opt => opt.Ignore());

            // ===== הזמנות =====
            CreateMap<Reservations, ReservationDTO>();          // הזמנה -> DTO
            CreateMap<ReservationCreateDTO, Reservations>();    // הזמנה חדשה -> טבלה
            CreateMap<ReservationDTO, Reservations>()           // עדכון הזמנה -> טבלה
                .ForMember(dest => dest.ReservationID,
                           opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt,
                           opt => opt.Ignore());

            // ===== תשלומים =====
            CreateMap<Payment, PaymentDTO>();                   // תשלום -> DTO
            CreateMap<PaymentCreateDTO, Payment>();              // תשלום חדש -> טבלה
            CreateMap<PaymentDTO, Payment>()                    // עדכון תשלום -> טבלה
                .ForMember(dest => dest.PaymentID,
                           opt => opt.Ignore());

            // ===== ביקורות =====
            CreateMap<Reviews, ReviewDTO>();                    // ביקורת -> DTO
            CreateMap<ReviewCreateDTO, Reviews>();              // ביקורת חדשה -> טבלה
            CreateMap<ReviewDTO, Reviews>()                     // עדכון ביקורת -> טבלה
                .ForMember(dest => dest.ReviewId,
                           opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt,
                           opt => opt.Ignore());

            // ===== זמינות חניה =====
            CreateMap<SpotAvailability, SpotAvailabilityDTO>();         // זמינות -> DTO
            CreateMap<SpotAvailabilityCreateDTO, SpotAvailability>();   // זמינות חדשה -> טבלה
            CreateMap<SpotAvailabilityDTO, SpotAvailability>()          // עדכון זמינות -> טבלה
                .ForMember(dest => dest.AvailabilityId,
                           opt => opt.Ignore());

            // ===== תמחור דינמי =====
            CreateMap<DynamicPricingRules, DynamicPricingRulesDTO>();       // כלל תמחור -> DTO
            CreateMap<DynamicPricingRulesCreateDTO, DynamicPricingRules>(); // כלל חדש -> טבלה
            CreateMap<DynamicPricingRulesDTO, DynamicPricingRules>()        // עדכון כלל -> טבלה
                .ForMember(dest => dest.Id,
                           opt => opt.Ignore());
        }
    }
}