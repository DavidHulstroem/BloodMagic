using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodMagic.Quest.Rewards
{
    public class Reward
    {
        public string rewardText;

        public virtual Reward CreateReward(int id, float totalCost, Random random)
        {
            return null;
        }

        public virtual void GiveReward()
        {

        }
    }
}
