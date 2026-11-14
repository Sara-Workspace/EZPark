using AutoMapper;
using BLL.Exceptions;
using BLL.Interfaces;
using DAL;
using DAL.Repositories;
using DTO;
using Microsoft.AspNetCore.Identity;

namespace BLL
{
    public class OwnerService : IOwnerService
    {
        private readonly IOwnersRepository _ownerRepo;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<Owners> _passwordHasher;

        public OwnerService(IOwnersRepository ownerRepo, IMapper mapper, IPasswordHasher<Owners> passwordHasher)
        {
            _ownerRepo = ownerRepo;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }

        // התחברות: כאן null זה לא שגיאה, אלא תשובה "הפרטים לא נכונים".
        // ה-AuthController מנסה קודם נהג ואחר כך בעל חניה, ורק אם שניהם null מחזיר 401
        public async Task<OwnerDTO?> ValidateOwnerCredentialsAsync(string email, string password)
        {
            // 1. שליפת בעל החניה לפי מייל
            var owner = await _ownerRepo.GetByEmailAsync(email);
            if (owner == null || owner.User == null)
                return null;

            // 2. אימות הסיסמה המוצפנת
            var verificationResult = _passwordHasher.VerifyHashedPassword(owner, owner.User.PasswordHash, password);
            if (verificationResult == PasswordVerificationResult.Failed)
                return null;

            // 3. החזרת DTO
            return _mapper.Map<OwnerDTO>(owner);
        }

        public List<OwnerDTO> GetAllOwners()
        {
            var owners = _ownerRepo.GetAllOwners();
            return _mapper.Map<List<OwnerDTO>>(owners);
        }

        public OwnerDTO GetOwnerById(int id)
        {
            // אם בעל החניה לא קיים - זורקים שגיאה, והשומר (GlobalExceptionFilter) מחזיר 404
            var owner = _ownerRepo.GetOwnerById(id)
                ?? throw new NotFoundException($"בעל חניה מספר {id} לא נמצא.");

            return _mapper.Map<OwnerDTO>(owner);
        }

        // לוגיקה עסקית: יצירת בעל חניה חדש ואתחול שדות ברירת מחדל כמו תאריך יצירה
        public OwnerDTO CreateOwner(OwnerCreateDTO dto)
        {
            // ArgumentException - השומר מחזיר 400
            if (string.IsNullOrWhiteSpace(dto.PasswordHash))
            {
                throw new ArgumentException("חובה להזין סיסמה בעת יצירת בעל חניה חדש");
            }

            var owner = new Owners
            {
                BankDetails = dto.BankDetails,
                User = new Users
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    IsAdmin = false,
                    CreatedAt = DateTime.UtcNow
                }
            };

            owner.User.PasswordHash = _passwordHasher.HashPassword(owner, dto.PasswordHash);

            _ownerRepo.AddOwner(owner);

            return _mapper.Map<OwnerDTO>(owner);
        }

        public OwnerDTO UpdateOwner(int id, OwnerUpdateDTO dto)
        {
            var owner = _ownerRepo.GetOwnerById(id)
                ?? throw new NotFoundException($"בעל חניה מספר {id} לא נמצא.");

            owner.BankDetails = dto.BankDetails;
            if (owner.User != null)
            {
                owner.User.FirstName = dto.FirstName;
                owner.User.LastName = dto.LastName;
                owner.User.Phone = dto.Phone;
            }

            _ownerRepo.UpdateOwner(owner);

            return _mapper.Map<OwnerDTO>(owner);
        }

        public void DeleteOwner(int id)
        {
            var owner = _ownerRepo.GetOwnerById(id)
                ?? throw new NotFoundException($"בעל חניה מספר {id} לא נמצא.");

            _ownerRepo.DeleteOwner(owner);
        }

        // חיפוש לפי מייל: כאן null זה לא שגיאה, אלא תשובה "אין בעל חניה עם המייל הזה".
        // נשתמש בזה בהמשך כדי לבדוק אם מייל כבר תפוס בהרשמה
        public OwnerDTO? GetOwnerByEmail(string email)
        {
            var owner = _ownerRepo.GetOwnerByEmail(email);
            if (owner == null) return null;
            return _mapper.Map<OwnerDTO>(owner);
        }

        // שליפת בעל החניה יחד עם רשימת החניות שלו כולל מיפוי ל-DTO המתאים
        public OwnerWithSpotsDTO GetOwnerWithSpots(int id)
        {
            var owner = _ownerRepo.GetOwnerWithSpots(id)
                ?? throw new NotFoundException($"בעל חניה מספר {id} לא נמצא.");

            return _mapper.Map<OwnerWithSpotsDTO>(owner);
        }
    }
}