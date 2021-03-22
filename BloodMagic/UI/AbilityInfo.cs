using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


namespace BloodMagic.UI
{
    public class AbilityInfo : MonoBehaviour
    {
        public SkillData selectedSkillData;

        public Text title;
        public Text description;
        public Text cost;
        public Button button;


        void Awake()
        {
            BookUIHandler.AbilityInfo = this;
        }

        public void OnUnlockClicked()
        {

            if (selectedSkillData != null && !selectedSkillData.unlocked)
            {

                //Not unlocked and is not null
                if (selectedSkillData.GetComponentInParent<SkillTreeInfo>().skillTreeName == "Light")
                {
                    if (BookUIHandler.saveData.lightPoints >= selectedSkillData.cost)
                    {
                        if (UnlockSkill())
                        {
                            BookUIHandler.saveData.lightPoints -= selectedSkillData.cost;
                            button.interactable = false;
                            button.GetComponentInChildren<Text>().text = "Unlocked";

                            BookUIHandler.Instance.UpdateSkilltree(BookUIHandler.Instance.lightSkillTree);

                            BookUIHandler.Instance.SaveJson();
                        }
                    }

                } else if (selectedSkillData.GetComponentInParent<SkillTreeInfo>().skillTreeName == "Dark")
                {
                    if (BookUIHandler.saveData.darkPoints >= selectedSkillData.cost)
                    {
                        if (UnlockSkill())
                        {
                            BookUIHandler.saveData.darkPoints -= selectedSkillData.cost;
                            button.interactable = false;
                            button.GetComponentInChildren<Text>().text = "Unlocked";

                            BookUIHandler.Instance.UpdateSkilltree(BookUIHandler.Instance.darkSkillTree);

                            BookUIHandler.Instance.SaveJson();
                        }
                    }

                }

            }
        }

        bool UnlockSkill()
        {
            if (!BookUIHandler.saveData.unlockedSkills.Contains(selectedSkillData.skillName)) 
            {
                BookUIHandler.saveData.unlockedSkills.Add(selectedSkillData.skillName);

                return true;
            }   
            else
            {
                Debug.LogWarning($"{GetType().FullName} :: Trying to unlock skill that is already unlocked");
                return false;
            }
                
        }

        public void SetSelectedSkill(SkillData p_skilldata, bool lightSide)
        {
            BookUIHandler.AbilityInfo.gameObject.SetActive(true);
            selectedSkillData = p_skilldata;

            //Set button
            if (p_skilldata.unlocked)
            {
                button.interactable = false;
                button.GetComponentInChildren<Text>().text = "Unlocked";
            } else
            {
                if (lightSide)
                {
                    if (BookUIHandler.saveData.lightPoints >= selectedSkillData.cost)
                    {
                        button.GetComponentInChildren<Text>().text = "Unlock";
                        button.interactable = true;
                    } else
                    {
                        button.GetComponentInChildren<Text>().text = "Not enough points";
                        button.interactable = false;
                    }


                }
                else if (!lightSide)
                {
                    if (BookUIHandler.saveData.darkPoints >= selectedSkillData.cost)
                    {
                        button.GetComponentInChildren<Text>().text = "Unlock";
                        button.interactable = true;
                    }
                    else
                    {
                        button.GetComponentInChildren<Text>().text = "Not enough points";
                        button.interactable = false;
                    }

                }
            }



            title.text = p_skilldata.skillName;
            description.text = p_skilldata.description;
            if (lightSide)
            {
                cost.text = $"{p_skilldata.cost} LP";
            } else
            {
                cost.text = $"{p_skilldata.cost} DP";
            }
            
            
        }
    }
}
