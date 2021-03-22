using BloodMagic.Quest.Conditions;
using BloodMagic.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodMagic.Quest
{
    public static class QuestHandler
    {
        public static Random globalRandom;

        public static void ShowQuestData(this QuestUIComponets questData, Quest quest)
        {
            string final = quest.mainCondition.conditionText;

            foreach (Condition condition in quest.mainCondition.conditions)
            {
                final += $" {condition.conditionText}";
            }

            questData.questInfoText.text = final;

            questData.progressText.text = $"{Math.Round(quest.mainCondition.progress, 1)} / {quest.mainCondition.progessGoal}";


            string allRewards = "";

            for (int i = 0; i < quest.mainCondition.rewards.Count; i++)
            {
                if (i != 0)
                {
                    allRewards += $"& {quest.mainCondition.rewards[i].rewardText}";
                } else
                {
                    allRewards += $"{quest.mainCondition.rewards[i].rewardText} ";
                }
            }

            if (quest.completed)
            {
                questData.claimBTN.gameObject.SetActive(true);
            } else
            {
                questData.claimBTN.gameObject.SetActive(false);
            }

            questData.xpRewardText.text = allRewards;
        }

        public static SaveData.QuestInfo GetQuestInfo(this Quest quest)
        {
            return new SaveData.QuestInfo()
            {
                id = quest.id,
                level = quest.mainCondition.level,
                progress = quest.mainCondition.progress
            };
        }


        /*
        public void Initialize()
        {
            if (globalRandom == null)
                globalRandom = new Random();


            if (BookUIHandler.saveData.quest1ID != 0)
            {
                quest1 = CreateQuest(BookUIHandler.saveData.quest1ID, 1).SetupRandomQuest();
            } else 
            {
                quest1 = CreateQuest(0, 1).SetupRandomQuest();
                BookUIHandler.saveData.quest1ID = quest1.id;
            }




            if (BookUIHandler.saveData.quest2ID != 0)
            {
                quest2 = CreateQuest(BookUIHandler.saveData.quest2ID, 2).SetupRandomQuest();
            } else
            {
                quest2 = CreateQuest(0, 2).SetupRandomQuest();
                BookUIHandler.saveData.quest2ID = quest2.id;
            }




            if (BookUIHandler.saveData.quest3ID != 0)
            {
                quest3 = CreateQuest(BookUIHandler.saveData.quest3ID, 3).SetupRandomQuest();
            } else
            {
                quest3 = CreateQuest(0, 3).SetupRandomQuest();
                BookUIHandler.saveData.quest3ID = quest3.id;
            }
        }
        
        public Quest CreateQuest(int id = 0, int level = 1)
        {
            return new Quest(id, level);
        }*/

    }
}
