using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;

namespace BloodMagic.Quest.Conditions
{
    /*
    public class AtOneHP : Condition
    {
        public override int conditionCost => 4;

        public override Condition SetupCondition(int p_seed, int p_level)
        {
            base.SetupCondition(p_seed, p_level);

            conditionText = "while at one hp";

            return this;
        }

        public override bool CanBeUsedWithType(Type mainType)
        {
            if (mainType == typeof(Drain))
            {
                return false;
            }

            return true;
        }

        public override bool IsConditionMet(Type mainType, object[] parseProperties = null)
        {
            if (Player.currentCreature.currentHealth > 1)
                return false;

            return true;
        }
    }*/
}
