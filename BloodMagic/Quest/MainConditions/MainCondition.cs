using BloodMagic.Quest.Rewards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace BloodMagic.Quest.Conditions
{
    public class MainCondition
    {
        public float progress = 0;
        public float progessGoal = 0;

        public List<Condition> conditions = new List<Condition>();

        public string conditionText = "";

        public int seed;
        public int level;
        protected System.Random random;

        public List<Reward> rewards = new List<Reward>();

        protected int totalCost;

        public virtual MainCondition SetupMainCondition(int p_id, int p_level)
        {
            seed = p_id;
            level = p_level;

            random = new System.Random(seed);

            int conditionPool = random.Next(0, (level*2)+1);
            int maxConditions = random.Next(0, level);

            List<Type> existing = new List<Type>();

            while (conditionPool > 0 && existing.Count < maxConditions)
            {
                List<System.Type> conditionList = GetSubConditions(conditionPool, existing);

                if (conditionList.Count <= 0)
                    break;

                System.Type conditionType = conditionList[random.Next(0, conditionList.Count)];
                Condition newCon = (Condition)conditionType.GetMethod("SetupCondition", new Type[] { typeof(int), typeof(int) }).
                    Invoke(Activator.CreateInstance(conditionType), new object[] { seed, level });

                existing.Add(conditionType);

                conditionPool -= newCon.conditionCost;
                totalCost += newCon.conditionCost;

                conditions.Add(newCon);
            }



            SetupRewards();

            return null;
        }

        public virtual void SetupRewards()
        {

        }

        public void GiveAllRewards()
        {
            foreach (Reward reward in rewards)
            {
                reward.GiveReward();
            }
        }

        private List<System.Type> GetSubConditions(int maxCost, List<System.Type> existing)
        {
            Type parentType = typeof(Condition);
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();

            IEnumerable<Type> subclasses = types.Where(t => t.BaseType == parentType &&
                (int)t.GetProperty("conditionCost").GetValue(Activator.CreateInstance(t)) <= maxCost && 
                !existing.Contains(t) &&
                (bool)t.GetMethod("CanBeUsedWithType", new Type[] { typeof(Type) }).Invoke(Activator.CreateInstance(t), new object[] { this.GetType() }));
            return subclasses.ToList();
        }

        public delegate void MainCondtionAction(MainCondition main);
        public event MainCondtionAction OnMainConditionCompleted;
        public static event MainCondtionAction OnAnyMainConditionProgressedEvent;

        public void IsAllConditionsMet(Type mainType, object[] parseProperties = null, float p_progress = 1)
        {
            foreach (Condition condition in conditions)
            {
                if (!condition.IsConditionMet(mainType, parseProperties))
                {
                    return;
                }
            }

            UpdateProgress(p_progress);
        }

        public void UpdateProgress(float p_progress)
        {
            progress += p_progress;
            if (progress >= progessGoal)
            {
                progress = progessGoal;
                CompleteMainCondition();
            }

            if (OnAnyMainConditionProgressedEvent != null)
                OnAnyMainConditionProgressedEvent(this);
        }

        public virtual void CompleteMainCondition()
        {
            if (OnMainConditionCompleted != null)
                OnMainConditionCompleted(this);
        }
    }
}
