using AutoMapper;
using AutoMapper.QueryableExtensions;
using DAL;
using DTO;

namespace BLL.Repositories
{
    public class DynamicPricingRulesRepository : IDynamicPricingRulesRepository
    {
        private readonly ParkingDbContext _context;
        private readonly IMapper _mapper;

        public DynamicPricingRulesRepository(ParkingDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<DynamicPricingRulesDTO> GetAllDynamicPricingRules()
        {
            return _context.DynamicPricingRules
                .ProjectTo<DynamicPricingRulesDTO>(_mapper.ConfigurationProvider)
                .ToList();
        }

        public DynamicPricingRulesDTO? GetDynamicPricingRuleById(int id)
        {
            return _context.DynamicPricingRules
                .Where(r => r.Id == id)
                .ProjectTo<DynamicPricingRulesDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefault();
        }

        public DynamicPricingRulesDTO CreateDynamicPricingRule(DynamicPricingRulesCreateDTO dto)
        {
            var rule = _mapper.Map<DynamicPricingRules>(dto);

            _context.DynamicPricingRules.Add(rule);
            _context.SaveChanges();

            return _mapper.Map<DynamicPricingRulesDTO>(rule);
        }

        public DynamicPricingRulesDTO? UpdateDynamicPricingRule(int id, DynamicPricingRulesDTO dto)
        {
            var rule = _context.DynamicPricingRules.Find(id);
            if (rule == null) return null;

            _mapper.Map(dto, rule);
            _context.SaveChanges();

            return _mapper.Map<DynamicPricingRulesDTO>(rule);
        }

        public bool DeleteDynamicPricingRule(int id)
        {
            var rule = _context.DynamicPricingRules.Find(id);
            if (rule == null) return false;

            _context.DynamicPricingRules.Remove(rule);
            _context.SaveChanges();
            return true;
        }

        public List<DynamicPricingRulesDTO> GetRulesBySpotId(int spotId)
        {
            return _context.DynamicPricingRules
                .Where(r => r.SpotID == spotId)
                .ProjectTo<DynamicPricingRulesDTO>(_mapper.ConfigurationProvider)
                .ToList();
        }
    }
}