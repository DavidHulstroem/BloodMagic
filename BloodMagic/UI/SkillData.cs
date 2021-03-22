using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace BloodMagic.UI
{
    public class SkillData : MonoBehaviour
    {
        public int cost = 1;
        public string skillName;
        public Image ringIMG;

        public List<SkillData> required;

        public string description;

        public bool unlocked;

        public void SkillBTNClicked()
        {
            BookUIHandler.AbilityInfo.gameObject.SetActive(true);
            if (GetComponentInParent<SkillTreeInfo>())
            {
                BookUIHandler.AbilityInfo.SetSelectedSkill(this, GetComponentInParent<SkillTreeInfo>().skillTreeName == "Light");
            }
            
            Debug.Log("SkillBTN pressed");
        }
    }
}
