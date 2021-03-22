using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodMagic.Quest.Conditions
{
    public class Condition
    {
        public string conditionText = "";

        public virtual int conditionCost => 1;

        protected Random random;

        public virtual Condition SetupCondition(int p_seed, int p_level)
        {
            random = new Random(p_seed);
            return null;
        }
        
        public virtual bool CanBeUsedWithType(Type mainType)
        {
            return true;
        }

        public virtual bool IsConditionMet(Type mainType, object[] parseProperties = null)
        {
            return false;
        }
    }
}
