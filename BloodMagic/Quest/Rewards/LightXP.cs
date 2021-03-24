﻿using BloodMagic.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodMagic.Quest.Rewards
{
    class LightXP : Reward
    {
        private int lp;
        public override Reward CreateReward(int id, float totalCost, Random random)
        {
            base.CreateReward(id, totalCost, random);

            float rng = UnityEngine.Mathf.Clamp((float)random.NextDouble(), 0.25f, 1);

            int total = (int)Math.Round(totalCost + rng * 3 * BookUIHandler.saveData.xpMultiplier, 0);

            lp = total;

            rewardText = $"{lp} LP";

            return this;
        }

        public override void GiveReward()
        {
            base.GiveReward();
            BookUIHandler.saveData.lightPoints += lp;
        }
    }
}
