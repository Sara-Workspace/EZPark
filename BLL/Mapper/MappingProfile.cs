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
            CreateMap<Driver, DriverDTO>();             // נהג -> DTO (בלי סיסמה)
            CreateMap<DriverCreateDTO, Driver>();       // נהג חדש -> טבלה
            CreateMap<DriverUpdateDTO, Driver>();       // עדכון נהג -> טבלה

            // ===== בעלי חניות =====
            CreateMap<Owners, OwnerDTO>();              // בעלים -> DTO (בלי סיסמה)
            CreateMap<OwnerCreateDTO, Owners>();        // בעלים חדש -> טבלה
            CreateMap<OwnerUpdateDTO, Owners>();        // עדכון בעלים -> טבלה
            CreateMap<Owners, OwnerWithSpotsDTO>();     // בעלים + החניות שלו

            // ===== חניות =====
            CreateMap<ParkingSpots, ParkingSpotDTO>();          // חניה -> DTO
            CreateMap<ParkingSpotCreateDTO, ParkingSpots>();    // חניה חדשה -> טבלה
            CreateMap<ParkingSpotDTO, ParkingSpots>()           // עדכון חניה -> טבלה
                .ForMember(dest => dest.SpotId,                 // השדה SpotId
                           opt => opt.Ignore());                // לא לשנות אותו
            CreateMap<ParkingSpots, ParkingSpotDetailedDTO>();  // חניה + פרטי הבעלים
            CreateMap<ParkingSpots, ParkingSpotWithReviewsDTO>()    // חניה + ביקורות
                .ForMember(dest => dest.Reviews,                // רשימת הביקורות
                           opt => opt.Ignore());                // לא למלא כאן (ממולא ב-Repository)

            // ===== הזמנות =====
            CreateMap<Reservations, ReservationDTO>();          // הזמנה -> DTO
            CreateMap<ReservationCreateDTO, Reservations>();    // הזמנה חדשה -> טבלה
            CreateMap<ReservationDTO, Reservations>()           // עדכון הזמנה -> טבלה
                .ForMember(dest => dest.ReservationID,          // השדה ReservationID
                           opt => opt.Ignore())                 // לא לשנות אותו
                .ForMember(dest => dest.CreatedAt,              // תאריך היצירה
                           opt => opt.Ignore());                // לא לשנות אותו

            // ===== תשלומים =====
            CreateMap<Payment, PaymentDTO>();                   // תשלום -> DTO
            CreateMap<PaymentCreateDTO, Payment>();             // תשלום חדש -> טבלה
            CreateMap<PaymentDTO, Payment>()                    // עדכון תשלום -> טבלה
                .ForMember(dest => dest.PaymentID,              // השדה PaymentID
                           opt => opt.Ignore());                // לא לשנות אותו

            // ===== ביקורות =====
            CreateMap<Reviews, ReviewDTO>();                    // ביקורת -> DTO
            CreateMap<ReviewCreateDTO, Reviews>();              // ביקורת חדשה -> טבלה
            CreateMap<ReviewDTO, Reviews>()                     // עדכון ביקורת -> טבלה
                .ForMember(dest => dest.ReviewId,               // השדה ReviewId
                           opt => opt.Ignore())                 // לא לשנות אותו
                .ForMember(dest => dest.CreatedAt,              // תאריך הכתיבה
                           opt => opt.Ignore());                // לא לשנות אותו

            // ===== זמינות חניה =====
            CreateMap<SpotAvailability, SpotAvailabilityDTO>();         // זמינות -> DTO
            CreateMap<SpotAvailabilityCreateDTO, SpotAvailability>();   // זמינות חדשה -> טבלה
            CreateMap<SpotAvailabilityDTO, SpotAvailability>()          // עדכון זמינות -> טבלה
                .ForMember(dest => dest.AvailabilityId,                 // השדה AvailabilityId
                           opt => opt.Ignore());                        // לא לשנות אותו

            // ===== תמחור דינמי =====
            CreateMap<DynamicPricingRules, DynamicPricingRulesDTO>();       // כלל תמחור -> DTO
            CreateMap<DynamicPricingRulesCreateDTO, DynamicPricingRules>(); // כלל חדש -> טבלה
            CreateMap<DynamicPricingRulesDTO, DynamicPricingRules>()        // עדכון כלל -> טבלה
                .ForMember(dest => dest.Id,                                 // השדה Id
                           opt => opt.Ignore());                            // לא לשנות אותו
        }
    }
}