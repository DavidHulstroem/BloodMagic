using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;

namespace BloodMagic.Quest.Conditions
{
    public class FromFarAway : Condition
    {
        public override int conditionCost => 2;

        private float distance;

        public override Condition SetupCondition(int p_seed, int p_level)
        {
            base.SetupCondition(p_seed, p_level);

            distance = random.Next(2, 3 * ((p_level+2) / 2));
            conditionText = $"from {distance} meters away";

            return this;
        }


        public override bool CanBeUsedWithType(Type mainType)
        {
            if (mainType != typeof(Kill))
            {
                return false;
            }

            return true;
        }

        public override bool IsConditionMet(Type mainType, object[] parseProperties = null)
        {
            if (mainType == typeof(Kill))
            {
                if (Vector3.Distance((parseProperties[0] as Creature).transform.position, Player.currentCreature.transform.position) > distance)
                    return true;
            }

            return false;
        }
    }
}
