using AutoMapper;
using DAL.Repositories;
using DTO;
using DAL;
using Microsoft.AspNetCore.Identity;

namespace BLL
{
    public class OwnerService
    {
        private readonly IOwnersRepository _ownerRepo;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<Driver> _passwordHasher;

        public OwnerService(IOwnersRepository ownerRepo, IMapper mapper, IPasswordHasher<Driver> passwordHasher)
        {
            _ownerRepo = ownerRepo;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }


        public async Task<OwnerDTO?> ValidateOwnerCredentialsAsync(string email, string password)
        {
            // 1. שליפת בעל החניה לפי מייל
            var owner = await _ownerRepo.GetByEmailAsync(email);
            if (owner == null)
                return null;

            // 2. אימות הסיסמה המוצפנת
            var verificationResult = _passwordHasher.VerifyHashedPassword(owner, owner.PasswordHash, password);
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

        public OwnerDTO? GetOwnerById(int id)
        {
            var owner = _ownerRepo.GetOwnerById(id);
            if (owner == null) return null;
            return _mapper.Map<OwnerDTO>(owner);
        }

        // לוגיקה עסקית: יצירת בעל חניה חדש ואתחול שדות ברירת מחדל כמו תאריך יצירה
        public OwnerDTO CreateOwner(OwnerCreateDTO dto)
        {
            var owner = _mapper.Map<Owners>(dto);

            owner.CreatedAt = DateTime.Now;

            _ownerRepo.AddOwner(owner);

            return _mapper.Map<OwnerDTO>(owner);
        }

        public OwnerDTO? UpdateOwner(int id, OwnerUpdateDTO dto)
        {
            var owner = _ownerRepo.GetOwnerById(id);
            if (owner == null) return null;

            _mapper.Map(dto, owner);
            _ownerRepo.UpdateOwner(owner);

            return _mapper.Map<OwnerDTO>(owner);
        }

        public bool DeleteOwner(int id)
        {
            var owner = _ownerRepo.GetOwnerById(id);
            if (owner == null) return false;

            _ownerRepo.DeleteOwner(owner);
            return true;
        }

        public OwnerDTO? GetOwnerByEmail(string email)
        {
            var owner = _ownerRepo.GetOwnerByEmail(email);
            if (owner == null) return null;
            return _mapper.Map<OwnerDTO>(owner);
        }

        // שליפת בעל החניה יחד עם רשימת החניות שלו כולל מיפוי ל-DTO המתאים
        public OwnerWithSpotsDTO? GetOwnerWithSpots(int id)
        {
            var owner = _ownerRepo.GetOwnerWithSpots(id);
            if (owner == null) return null;
            return _mapper.Map<OwnerWithSpotsDTO>(owner);
        }
    
    }
}