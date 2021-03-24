using BloodMagic.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BloodMagic.Quest.Rewards
{
    class DrainPower : Reward
    {
        private float power;
        public override Reward CreateReward(int id, float totalCost, System.Random random)
        {
            base.CreateReward(id, totalCost, random);

            float rng = UnityEngine.Mathf.Lerp(0.5f, 1, (float)random.NextDouble());

            float total = rng * totalCost * 0.25f;

            power = (float)Math.Round((total), 1);

#if UNITY_ENGINE_AVAILABLE
            Debug.Log($"{GetType().FullName} :: Power = {power}");
#endif


           rewardText = $"{power} Drain power";

            return this;
        }

        public override void GiveReward()
        {
            base.GiveReward();
            BookUIHandler.saveData.drainPower += power;
        }
    }
}
