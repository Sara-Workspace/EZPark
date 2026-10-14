using AutoMapper;
using AutoMapper.QueryableExtensions;
using DAL;
using DTO;
using Microsoft.EntityFrameworkCore;            // חדש: נותן את Include

namespace BLL.Repositories
{
    public class OwnersRepository : IOwnersRepository
    {
        private readonly ParkingDbContext _context;
        private readonly IMapper _mapper;

        public OwnersRepository(ParkingDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<OwnerDTO> GetAllOwners()
        {
            return _context.Owners
                .ProjectTo<OwnerDTO>(_mapper.ConfigurationProvider)
                .ToList();
        }

        public OwnerDTO? GetOwnerById(int id)
        {
            return _context.Owners
                .Where(o => o.OwnerID == id)
                .ProjectTo<OwnerDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefault();
        }

        public OwnerDTO CreateOwner(OwnerCreateDTO dto)
        {
            var owner = _mapper.Map<Owners>(dto);
            owner.CreatedAt = DateTime.Now;

            _context.Owners.Add(owner);
            _context.SaveChanges();

            return _mapper.Map<OwnerDTO>(owner);
        }

        public OwnerDTO? UpdateOwner(int id, OwnerUpdateDTO dto)
        {
            var owner = _context.Owners.Find(id);
            if (owner == null) return null;

            _mapper.Map(dto, owner);
            _context.SaveChanges();

            return _mapper.Map<OwnerDTO>(owner);
        }

        public bool DeleteOwner(int id)
        {
            var owner = _context.Owners.Find(id);
            if (owner == null) return false;

            _context.Owners.Remove(owner);
            _context.SaveChanges();
            return true;
        }

        public OwnerDTO? GetOwnerByEmail(string email)
        {
            return _context.Owners
                .Where(o => o.Email == email)
                .ProjectTo<OwnerDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefault();
        }

        public OwnerWithSpotsDTO? GetOwnerWithSpots(int id)    // בעלים + החניות שלו
        {
            var owner = _context.Owners
                .Include(o => o.ParkingSpots)                   // Include: טוען גם את החניות של הבעלים (קשר גומלין)
                .FirstOrDefault(o => o.OwnerID == id);          // מחפש את הבעלים לפי קוד

            if (owner == null) return null;                     // לא נמצא: מחזיר null
            return _mapper.Map<OwnerWithSpotsDTO>(owner);       // ממיר ל-DTO, כולל רשימת החניות
        }
    }
}