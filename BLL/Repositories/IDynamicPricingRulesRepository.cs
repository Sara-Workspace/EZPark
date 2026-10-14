using DTO;                                                                  // כדי להכיר את מחלקות ה-DTO

namespace BLL.Repositories                                                  // הקובץ נמצא בתיקייה Repositories שב-BLL
{
    public interface IDynamicPricingRulesRepository                         // ממשק: רשימת הפעולות על כללי תמחור
    {
        List<DynamicPricingRulesDTO> GetAllDynamicPricingRules();           // מחזיר את כל כללי התמחור
        DynamicPricingRulesDTO? GetDynamicPricingRuleById(int id);          // מחזיר כלל לפי קוד (null אם לא נמצא)
        DynamicPricingRulesDTO CreateDynamicPricingRule(DynamicPricingRulesCreateDTO dto); // מוסיף כלל חדש ומחזיר אותו
        DynamicPricingRulesDTO? UpdateDynamicPricingRule(int id, DynamicPricingRulesDTO dto); // מעדכן כלל (null אם לא נמצא)
        bool DeleteDynamicPricingRule(int id);                              // מוחק כלל (true אם הצליח)

        List<DynamicPricingRulesDTO> GetRulesBySpotId(int spotId);          // נוסף: הכללים של חניה מסוימת
    }
}