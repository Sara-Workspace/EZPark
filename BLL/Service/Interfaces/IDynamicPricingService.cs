using DTO;

namespace BLL.Interfaces
{
    public interface IDynamicPricingService
    {
        List<DynamicPricingRulesDTO> GetAllRules();
        DynamicPricingRulesDTO CreateRule(DynamicPricingRulesCreateDTO dto);
        decimal CalculateDynamicPrice(int spotId, DateTime targetTime, decimal basePrice);
    }
}