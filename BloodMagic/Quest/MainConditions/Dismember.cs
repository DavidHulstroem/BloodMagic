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
    public class Dismember : MainCondition
    {
        public override MainCondition SetupMainCondition(int p_id, int p_level)
        {
            base.SetupMainCondition(p_id, p_level);

            progessGoal = random.Next(level*2, (level+4)*level);

            conditionText = $"Dismember {progessGoal} limbs";

            EventManager.onCreatureSpawn += EventManager_onCreatureSpawn;

#if UNITY_ENGINE_AVAILABLE
            Debug.Log($"{GetType().FullName} :: Setup main condition called");
#endif
            return this;
        }

        private void EventManager_onCreatureSpawn(Creature creature)
        {
            if (!creature.isPlayer)
            {
                creature.ragdoll.OnSliceEvent += Ragdoll_OnSliceEvent;
            }
        }

        private void Ragdoll_OnSliceEvent(RagdollPart ragdollPart, EventTime eventTime)
        {
            if (eventTime == EventTime.OnStart)
            {
                ragdollPart.ragdoll.OnSliceEvent -= Ragdoll_OnSliceEvent;
                UpdateProgress(1);
            }
            
        }


        public override void CompleteMainCondition()
        {
            base.CompleteMainCondition();

            EventManager.onCreatureSpawn -= EventManager_onCreatureSpawn;
        }

        public override void SetupRewards()
        {
            base.SetupRewards();

            rewards.Add(new Rewards.DarkXP());
            rewards[0].CreateReward(seed, totalCost + 1, random);
        }

    }
}
