using AutoMapper;
using DAL.Repositories;
using DTO;
using DAL;
using BLL.Interfaces;

namespace BLL
{
    public class DynamicPricingService:IDynamicPricingService
    {
        private readonly IDynamicPricingRulesRepository _rulesRepo;
        private readonly IMapper _mapper;

        public DynamicPricingService(IDynamicPricingRulesRepository rulesRepo, IMapper mapper)
        {
            _rulesRepo = rulesRepo;
            _mapper = mapper;
        }

        public List<DynamicPricingRulesDTO> GetAllRules()
        {
            var rules = _rulesRepo.GetAllDynamicPricingRules();
            return _mapper.Map<List<DynamicPricingRulesDTO>>(rules);
        }

        public DynamicPricingRulesDTO CreateRule(DynamicPricingRulesCreateDTO dto)
        {
            var rule = _mapper.Map<DynamicPricingRules>(dto);

            // לוגיקה עסקית / בדיקות תקינות לטווחי שעות עומס
            if (rule.StartHour >= rule.EndtHour)
            {
                throw new InvalidOperationException("שעת התחלת עומס חייבת להיות לפני שעת הסיום.");
            }

            _rulesRepo.AddDynamicPricingRule(rule);
            return _mapper.Map<DynamicPricingRulesDTO>(rule);
        }

        // חישוב מחיר דינמי מתקדם בהתאם לכללים ולזמן נתון (לוגיקה מורכבת ששייכת ל-Service ולא ל-DAL)
        public decimal CalculateDynamicPrice(int spotId, DateTime targetTime, decimal basePrice)
        {
            var rules = _rulesRepo.GetRulesBySpotId(spotId);

            // סינון הכלל הרלוונטי לשעה המבוקשת
            var matchingRule = rules.FirstOrDefault(r => targetTime.TimeOfDay >= r.StartHour.TimeOfDay &&
                                                         targetTime.TimeOfDay <= r.EndtHour.TimeOfDay);

            if (matchingRule == null)
            {
                return basePrice; // אין כלל תמחור דינמי שחלים בשעה זו
            }

            // חישוב משוקלל: מחיר בסיס כפול מקדם המחיר
            decimal finalPrice = basePrice * matchingRule.PriceMultiplier;
            return finalPrice;
        }
    }
}