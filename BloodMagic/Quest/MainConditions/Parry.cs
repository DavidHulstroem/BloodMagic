using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;
using UnityEngine.Events;

namespace BloodMagic.Quest.Conditions
{
    public class Parry : MainCondition
    {
        public override MainCondition SetupMainCondition(int p_id, int p_level)
        {
            base.SetupMainCondition(p_id, p_level);

            progessGoal = random.Next(level, (level+4)*level);

            conditionText = $"Parry {progessGoal} attacks";

            EventManager.onCreatureParry += EventManager_onCreatureParry; ;

#if UNITY_ENGINE_AVAILABLE
            Debug.Log($"{GetType().FullName} :: Setup main condition called");
#endif
            return this;
        }

        private void EventManager_onCreatureParry(Creature creature, CollisionInstance collisionInstance)
        {
            if (creature != null && !creature.isPlayer && !collisionInstance.IsDoneByPlayer())
            {
                UpdateProgress(1);
            }
        }

        public override void CompleteMainCondition()
        {
            base.CompleteMainCondition();

            EventManager.onCreatureParry -= EventManager_onCreatureParry;
        }

        public override void SetupRewards()
        {
            base.SetupRewards();

            rewards.Add( random.NextDouble() > 0.5 ? (Rewards.Reward)new Rewards.DarkXP() : new Rewards.LightXP());
            rewards[0].CreateReward(seed, totalCost + 1, random);
        }

    }
}
