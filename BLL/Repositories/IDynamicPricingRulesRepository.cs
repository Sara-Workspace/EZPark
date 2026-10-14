using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO;                                      // DynamicPricingRuleDTO


namespace BLL.Repositories
{
    public interface IDynamicPricingRulesRepository
    {
        List<DynamicPricingRuleDTO> GetAllDynamicPricingRules();                            // כל חוקי התמחור
        DynamicPricingRuleDTO? GetDynamicPricingRuleById(int id);                           // חוק לפי קוד, או null
        DynamicPricingRuleDTO CreateDynamicPricingRule(DynamicPricingRuleDTO dto);          // הוספת חוק
        DynamicPricingRuleDTO? UpdateDynamicPricingRule(int id, DynamicPricingRuleDTO dto); // עדכון חוק
        bool DeleteDynamicPricingRule(int id);                                              // מחיקת חוק

        List<DynamicPricingRuleDTO> GetRulesBySpotId(int spotId);                           // ספציפית: החוקים של חניה מסוימת
    }
}
