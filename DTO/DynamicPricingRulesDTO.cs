using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class DynamicPricingRulesDTO
    {
        public int RuleID { get; set; }
        public int SpotID { get; set; }
        public decimal PriceMultiplier { get; set; }
        public decimal EvExtraCharge { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime EffectiveTo { get; set; }
    }
}
