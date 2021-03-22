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
    public class Kill : MainCondition
    {
        public override MainCondition SetupMainCondition(int p_id, int p_level)
        {
            base.SetupMainCondition(p_id, p_level);

            progessGoal = random.Next(level, (level+4)*level);

            conditionText = $"Kill {progessGoal} enemies";

            EventManager.onCreatureKill += OnCreatureKill;
#if UNITY_ENGINE_AVAILABLE
            Debug.Log($"{GetType().FullName} :: Setup main condition called");
#endif
            return this;
        }

        private void OnCreatureKill(Creature creature, Player player, CollisionInstance collisionInstance, EventTime eventTime)
        {
            if (eventTime == EventTime.OnStart)
            {
                Debug.Log($"{GetType().FullName} :: On Creature Killed");
                IsAllConditionsMet(GetType(), new object[] { creature, player, collisionInstance, eventTime });
            }
        }

        public override void CompleteMainCondition()
        {
            base.CompleteMainCondition();

            EventManager.onCreatureKill -= OnCreatureKill;
        }

        public override void SetupRewards()
        {
            base.SetupRewards();

            rewards.Add(new Rewards.DarkXP());
            rewards[0].CreateReward(seed, totalCost + 1, random);
        }

    }
}
