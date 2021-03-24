using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

namespace BloodMagic.Quest.Conditions
{
    public class Drain : MainCondition
    {
        public override MainCondition SetupMainCondition(int p_id, int p_level)
        {
            base.SetupMainCondition(p_id, p_level);

            progessGoal = random.Next((50*level), (level*60)*level);

            conditionText = $"Drain {progessGoal} health from enemies";
#if UNITY_ENGINE_AVAILABLE
            Debug.Log($"{GetType().FullName} :: Setup main condition called");
#endif

            BloodMagic.Spell.Abilities.BloodDrain.OnDrain += OnDrainEvent;

            return this;
        }

        public override void CompleteMainCondition()
        {
            base.CompleteMainCondition();

            BloodMagic.Spell.Abilities.BloodDrain.OnDrain -= OnDrainEvent;
        }

        private void OnDrainEvent(float drainedHealth)
        {
            IsAllConditionsMet(GetType(), null, (float)Math.Round(drainedHealth, 5));
        }

        public override void SetupRewards()
        {
            base.SetupRewards();

#if UNITY_ENGINE_AVAILABLE
            Debug.Log($"{GetType().FullName} :: Setup rewards called, SEED {seed}");
#endif

            rewards.Add(new Rewards.LightXP());
            rewards[0].CreateReward(seed, totalCost + 1, random);

           

            if (random.NextDouble() < 0.3)
            {
                rewards.Add(new Rewards.DrainPower().CreateReward(seed, totalCost + 1, random));
            }
        }
    }
}
