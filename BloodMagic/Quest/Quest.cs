using BloodMagic.Quest.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BloodMagic.Quest
{
    public class Quest
    {
        public int id;

        public System.Random random;

        public bool completed;

        public MainCondition mainCondition;
        int level;

        public delegate void QuestAction(Quest quest);
        public event QuestAction OnQuestCompleted;

        public Quest(int p_id, int p_level)
        {
            level = p_level;
            if (p_id != 0)
            {
                id = p_id;
            } else
            {
                id = QuestHandler.globalRandom.Next(1, 100000);
            }

            random = new System.Random(id);
        }

        public Quest SetupRandomQuest()
        {
            List<System.Type> mainConditions = MainConditions();
            System.Type condType = mainConditions[random.Next(0, mainConditions.Count)];

            mainCondition = (MainCondition)condType.
                GetMethod("SetupMainCondition", new Type[] { typeof(int), typeof(int) }).
                Invoke(Activator.CreateInstance(condType), new object[] { id, level });

            mainCondition.OnMainConditionCompleted += FinishQuest;

            return this;
        }

        private void FinishQuest(MainCondition main)
        {
            Debug.Log($"{GetType().FullName} :: Quest finished");

            completed = true;
            if (OnQuestCompleted != null)
                OnQuestCompleted(this);

            main.OnMainConditionCompleted -= FinishQuest;
        }

        public List<System.Type> MainConditions()
        {
            Type parentType = typeof(MainCondition);
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();

            IEnumerable<Type> subclasses = types.Where(t => t.BaseType == parentType);
            return subclasses.ToList();
        }
    }
}
