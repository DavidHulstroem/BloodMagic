using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using BloodMagic.Quest;

namespace BloodMagic.UI
{
    public class QuestUIComponets : MonoBehaviour
    {
        public Text questTitleText;

        public Text questInfoText;

        public Text progressText;

        public Text xpRewardText;

        public Button claimBTN;

        public Quest.Quest quest = null;
    }
}
