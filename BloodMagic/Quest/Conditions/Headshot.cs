using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;

namespace BloodMagic.Quest.Conditions
{
    public class Headshot : Condition
    {
        public override int conditionCost => 2;

        public override Condition SetupCondition(int p_seed, int p_level)
        {
            base.SetupCondition(p_seed, p_level);

            conditionText = $"with a headshot";

            return this;
        }


        public override bool CanBeUsedWithType(Type mainType)
        {
            if (mainType == typeof(Kill))
                return true;

            return false;
        }

        public override bool IsConditionMet(Type mainType, object[] parseProperties = null)
        {
            Debug.Log($"{GetType().FullName} :: Headshot check condition called");
            if ((parseProperties[2] as CollisionInstance).damageStruct.hitRagdollPart.type == RagdollPart.Type.Head ||
                (parseProperties[2] as CollisionInstance).damageStruct.hitRagdollPart.type == RagdollPart.Type.Neck)
                return true;

            Debug.Log($"{GetType().FullName} :: Returned false");
            return false;
        }
    }
}
