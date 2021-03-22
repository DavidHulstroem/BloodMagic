using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BloodMagic;
using BloodMagic.Quest;
using BloodMagic.Quest.Conditions;
using BloodMagic.UI;

namespace Debugger
{
    class Program
    {
        static void Main(string[] args)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            QuestHandler.globalRandom = new Random();

            BookUIHandler.saveData = new SaveData()
            {

            };

            while (true)
            {
                Quest quest = new Quest(0, 3).SetupRandomQuest();

                string final = quest.mainCondition.conditionText;

                foreach (Condition condition in quest.mainCondition.conditions)
                {
                    final += $" {condition.conditionText}";
                }

                Console.WriteLine(final);

                Console.ReadKey();
            }
        }
    }
}
