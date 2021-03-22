using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;

namespace BloodMagic.UI
{
    public static class SkillHandler
    {
        public static SkillData ShowSkill(this SkillData skillData)
        {
            skillData.gameObject.SetActive(true);
            Debug.Log($"ShowSkill :: Does save data contain {skillData.skillName} = {BookUIHandler.saveData.unlockedSkills.Contains(skillData.skillName)}");
            if (BookUIHandler.saveData.unlockedSkills.Contains(skillData.skillName))
            {
                skillData.ringIMG.color = new Color(0, 1, 0);
                skillData.unlocked = true;
            } else
            {
                skillData.ringIMG.color = new Color(1, 0, 0);
                skillData.unlocked = false;
            }

            return skillData;
        }

        public static SkillData HideSkill(this SkillData skillData)
        {
            skillData.gameObject.SetActive(false);
            return skillData;
        }

        public static bool IsSkillUnlocked(string skillName)
        {
            if (BookUIHandler.saveData.unlockedSkills.Contains(skillName))
            {
                return true;
            }

            return false;
        }
    }
}
