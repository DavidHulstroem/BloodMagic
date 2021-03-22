using BloodMagic.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;

namespace BloodMagic.Quest.Conditions
{
    public class UsingWeapon : Condition
    {
        public override int conditionCost => 1;


        public string chosenItemKey;

        public override Condition SetupCondition(int p_seed, int p_level)
        {
            base.SetupCondition(p_seed, p_level);

            Dictionary<string, string> ItemIDNameDict = new Dictionary<string, string>();

            if (SkillHandler.IsSkillUnlocked("Blood Sword"))
                ItemIDNameDict.Add("BloodSword", "Blood Sword");

            if (SkillHandler.IsSkillUnlocked("Blood Bow"))
                ItemIDNameDict.Add("BloodArrow", "Blood Bow");

            ItemIDNameDict.Add("BloodDagger", "Blood Dagger");
            ItemIDNameDict.Add("BloodBullet", "Blood Bullet");


            chosenItemKey = ItemIDNameDict.ToList()[random.Next(0, ItemIDNameDict.Count)].Key;
            

            conditionText = $"using a {ItemIDNameDict[chosenItemKey]}";

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
            Debug.Log($"{GetType().FullName} :: Using weapon called");
            CollisionInstance collisionInstance = parseProperties[2] as CollisionInstance;

            Item item = collisionInstance.sourceColliderGroup?.collisionHandler?.item;
            if (item != null)
            {
                if (item.itemId == chosenItemKey)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
