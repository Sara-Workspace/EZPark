using DAL;

namespace DAL.Repositories
{
    public class DynamicPricingRulesRepository : IDynamicPricingRulesRepository
    {
        private readonly ParkingDbContext _context;

        public DynamicPricingRulesRepository(ParkingDbContext context)
        {
            _context = context;
        }

        public List<DynamicPricingRules> GetAllDynamicPricingRules()
        {
            return _context.DynamicPricingRules.ToList();
        }

        public DynamicPricingRules? GetDynamicPricingRuleById(int id)
        {
            return _context.DynamicPricingRules.Find(id);
        }

        public void AddDynamicPricingRule(DynamicPricingRules rule)
        {
            _context.DynamicPricingRules.Add(rule);
            _context.SaveChanges();
        }

        public void UpdateDynamicPricingRule(DynamicPricingRules rule)
        {
            _context.DynamicPricingRules.Update(rule);
            _context.SaveChanges();
        }

        public void DeleteDynamicPricingRule(DynamicPricingRules rule)
        {
            _context.DynamicPricingRules.Remove(rule);
            _context.SaveChanges();
        }

        public List<DynamicPricingRules> GetRulesBySpotId(int spotId)
        {
            return _context.DynamicPricingRules
                .Where(r => r.SpotID == spotId)
                .ToList();
        }
    }
}