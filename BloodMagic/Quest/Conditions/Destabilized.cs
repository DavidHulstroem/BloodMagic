using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;

namespace BloodMagic.Quest.Conditions
{
    public class Destabilized : Condition
    {
        public override int conditionCost => 2;

        public override Condition SetupCondition(int p_seed, int p_level)
        {
            base.SetupCondition(p_seed, p_level);

            conditionText = $"while enemy is knocked out";

            return this;
        }


        public override bool CanBeUsedWithType(Type mainType)
        {
            if (mainType == typeof(Kill) || mainType == typeof(Dismember))
                return true;

            return false;
        }

        public override bool IsConditionMet(Type mainType, object[] parseProperties = null)
        {
            if ((parseProperties[0] as Creature).state == Creature.State.Destabilized)
                return true;

            return false;
        }
    }
}
