using DAL;
using DAL.Repositories;

namespace DAL.Repositories
{
    public interface IDynamicPricingRulesRepository
    {
        List<DynamicPricingRules> GetAllDynamicPricingRules();
        DynamicPricingRules? GetDynamicPricingRuleById(int id);
        void AddDynamicPricingRule(DynamicPricingRules rule);
        void UpdateDynamicPricingRule(DynamicPricingRules rule);
        void DeleteDynamicPricingRule(DynamicPricingRules rule);
        List<DynamicPricingRules> GetRulesBySpotId(int spotId);
    }
}