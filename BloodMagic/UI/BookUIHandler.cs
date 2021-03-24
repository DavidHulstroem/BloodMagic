using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.UI;
using UnityEngine.Events;
using BloodMagic.Quest;
using BloodMagic.Quest.Conditions;
using IngameDebugConsole;

namespace BloodMagic.UI
{
    public class BookUIHandler : MenuModule
    {

        public QuestUIComponets questUI1;
        public QuestUIComponets questUI2;
        public QuestUIComponets questUI3;

        public GameObject darkChoicePage;
        public GameObject lightChoicePage;
        public GameObject page1Picked;
        public GameObject page2Picked;

        public Text LP;
        public Text DP;

        public GameObject lightSkillTree;
        public GameObject darkSkillTree;

        public static SaveData saveData;

        public static BookUIHandler Instance;

        private string saveFileName = "Mods/BloodMagic/Saves/BloodSaveData.json";

        public override void Init(MenuData menuData, Menu menu)
        {
            base.Init(menuData, menu);

            //Setup commands for reloading json =)

            IngameDebugConsole.DebugLogConsole.AddCommand("reloadblood", "Reloads the blood save json", delegate
            {
                InitSaveJSON();
                SetupAllQuests();
                UpdateAllStats();
            });


            Instance = this;

            //Get quest ui components
            questUI1 = menu.GetCustomReference("Quest1").GetComponent<QuestUIComponets>();
            questUI2 = menu.GetCustomReference("Quest2").GetComponent<QuestUIComponets>();
            questUI3 = menu.GetCustomReference("Quest3").GetComponent<QuestUIComponets>();


            darkChoicePage = menu.GetCustomReference("DarkChoice").gameObject;
            lightChoicePage = menu.GetCustomReference("LightChoice").gameObject;
            page1Picked = menu.GetCustomReference("Page1Picked").gameObject;
            page2Picked = menu.GetCustomReference("Page2Picked").gameObject;

            lightSkillTree = menu.GetCustomReference("LightSkillTree").gameObject;
            darkSkillTree = menu.GetCustomReference("DarkSkillTree").gameObject;

            LP = menu.GetCustomReference("LP").GetComponent<Text>();
            DP = menu.GetCustomReference("DP").GetComponent<Text>();

            InitSaveJSON();

            QuestHandler.globalRandom = new System.Random(UnityEngine.Random.Range(1,10000000));

            menu.GetCustomReference("SkillTreeBTN").GetComponent<Button>().onClick.AddListener(delegate
            {
                LoadSkillTree();
                AbilityInfo.gameObject.SetActive(false);
            });

            menu.GetCustomReference("OtherSkill").GetComponent<Button>().onClick.AddListener(delegate
            {
                LoadSkillTree();
                AbilityInfo.gameObject.SetActive(false);
            });

            menu.GetCustomReference("QuestBTN").GetComponent<Button>().onClick.AddListener(delegate
            {
                LoadQuestPage();
                AbilityInfo.gameObject.SetActive(false);
            });

            SetupAllQuests();

           
            menu.GetCustomReference("DebugNewQuest").GetComponent<Button>().onClick.AddListener(delegate
            {
                saveData.quest1 = null;
                saveData.quest2 = null;
                saveData.quest3 = null;

                questUI1.quest = null;
                questUI2.quest = null;
                questUI3.quest = null;

                SetupAllQuests();
            });
            menu.GetCustomReference("DebugNewQuest").gameObject.SetActive(false);

            UpdateAllStats();

            MainCondition.OnAnyMainConditionProgressedEvent += OnAnyMainConditionCompleted;

            EventManager.onCreatureKill += AddDrainComponentOnKill;
        }

        private void AddDrainComponentOnKill(Creature creature, Player player, CollisionInstance collisionInstance, EventTime eventTime)
        {
            if (!creature.isPlayer)
            {
                Spell.CreatureDrainComponent drain = creature.GetComponent<Spell.CreatureDrainComponent>() ? creature.gameObject.GetComponent<Spell.CreatureDrainComponent>() : creature.gameObject.AddComponent<Spell.CreatureDrainComponent>();
                drain.health = creature.maxHealth;
            }
        }

        private void OnAnyMainConditionCompleted(MainCondition main)
        {
            UpdateQuestProgress(questUI1);
            UpdateQuestProgress(questUI2);
            UpdateQuestProgress(questUI3);
            //UpdateAllQuestDisplays(); ///Todo, make it not update all the displays only progress

            SaveJson();
        }

        public void UpdateQuestProgress(QuestUIComponets p_questUIC)
        {
            if (p_questUIC.quest.mainCondition.progress >= p_questUIC.quest.mainCondition.progessGoal)
            {
                p_questUIC.ShowQuestData(p_questUIC.quest);
                return;
            }

            p_questUIC.progressText.text = $"{Math.Round(p_questUIC.quest.mainCondition.progress, 1)} / {p_questUIC.quest.mainCondition.progessGoal}";
        }

        private void SetupAllQuests()
        {
            saveData.quest1 = LoadQuestItem(questUI1, saveData.quest1, 1).GetQuestInfo();
            saveData.quest2 = LoadQuestItem(questUI2, saveData.quest2, 2).GetQuestInfo();
            saveData.quest3 = LoadQuestItem(questUI3, saveData.quest3, 3).GetQuestInfo();

            SaveJson();
        }

        private void LoadQuestPage()
        {
            UpdateAllQuestDisplays();
        }

        private Quest.Quest LoadQuestItem(QuestUIComponets questUIComponent, SaveData.QuestInfo savedQuestInfo = null, int lvl = 1)
        {
            if (questUIComponent.quest == null)
            {
                if (savedQuestInfo != null)
                {
                    Quest.Quest quest = new Quest.Quest(savedQuestInfo.id, savedQuestInfo.level).SetupRandomQuest();
                    quest.mainCondition.UpdateProgress(savedQuestInfo.progress);

                    questUIComponent.quest = quest;
                    questUIComponent.ShowQuestData(questUIComponent.quest);
                }
                else
                {
                    questUIComponent.quest = new Quest.Quest(0, lvl).SetupRandomQuest();
                    questUIComponent.ShowQuestData(questUIComponent.quest);
                }
            }
            else
            {

                Debug.Log($"{GetType().FullName} :: Quest in component is not null");
                questUIComponent.ShowQuestData(questUIComponent.quest);
            }

            //Setup claim btn
            questUIComponent.claimBTN.onClick.AddListener(() => OnClaimClicked(questUIComponent, questUIComponent.quest));

            return questUIComponent.quest;
        }

        private void OnClaimClicked(QuestUIComponets questUIComponent, Quest.Quest quest)
        {
            if (quest.completed)
            {
                questUIComponent.claimBTN.onClick.RemoveAllListeners();

                questUIComponent.quest = null;
                if (saveData.quest1.id == quest.id)
                {
                    saveData.quest1 = null;
                    saveData.quest1 = LoadQuestItem(questUIComponent, null, 1).GetQuestInfo();
                }

                if (saveData.quest2.id == quest.id)
                {
                    saveData.quest2 = null;
                    saveData.quest2 = LoadQuestItem(questUIComponent, null, 2).GetQuestInfo();
                }

                if (saveData.quest3.id == quest.id)
                {
                    saveData.quest3 = null;
                    saveData.quest3 = LoadQuestItem(questUIComponent, null, 3).GetQuestInfo();
                }


                quest.mainCondition.GiveAllRewards();

                UpdateAllQuestDisplays();

                SaveJson();
            } else
            {
                Debug.LogError($"{GetType().FullName} :: Quest is not completed and is trying to be redeemed");
            }

        }


        void UpdateAllQuestDisplays()
        {
            questUI1.ShowQuestData(questUI1.quest);
            questUI2.ShowQuestData(questUI2.quest);
            questUI3.ShowQuestData(questUI3.quest);

            UpdateAllStats();
        }

        void UpdateAllStats()
        {
            LP.text = $"{saveData.lightPoints} LP";
            DP.text = $"{saveData.darkPoints} DP";
        }


        public static AbilityInfo AbilityInfo;
        private bool darkSide;

        public void LoadSkillTree()
        {
            GameObject skilltree = null;

            if (darkSide)
            {
                lightSkillTree.SetActive(true);
                darkSkillTree.SetActive(false);

                darkSide = false;
                skilltree = lightSkillTree;
            }
            else if (!darkSide)
            {
                lightSkillTree.SetActive(false);
                darkSkillTree.SetActive(true);

                darkSide = true;
                skilltree = darkSkillTree;
            }

            UpdateSkilltree(skilltree);
            //List<SkillData> skillDatas = skilltree.GetComponentsInChildren<SkillData>().ToList();

        }

        public void UpdateSkilltree(GameObject skilltree)
        {
            foreach (SkillData skillData in skilltree.GetComponentsInChildren<SkillData>(true))
            {
                if (skillData.required.Count > 0)
                {

                    if (skillData.required.All(skill => saveData.unlockedSkills.Contains(skill.skillName)))
                    {
                        skillData.ShowSkill();
                    }
                    else
                    {
                        skillData.HideSkill();
                    }

                }
                else
                {
                    skillData.ShowSkill();
                }
            }
            UpdateAllStats();
        }

        private void InitSaveJSON()
        {
            if (File.Exists(Path.Combine(Application.streamingAssetsPath, saveFileName))) 
            {
                try
                {
                    saveData = JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(Path.Combine(Application.streamingAssetsPath, saveFileName)));
                } catch
                {
                    Debug.LogError($"{GetType().FullName} :: Error in savedat? Generating new one, and creating backup of old. Please contact Davi3684 for help");
                    File.Move(Path.Combine(Application.streamingAssetsPath, saveFileName), Path.Combine(Application.streamingAssetsPath, "CorruptedSaveBackup.json"));
                    saveData = new SaveData();
                    SaveJson();
                }
                
            } else
            {
                saveData = new SaveData();
                SaveJson();
            }
        }



        public override void OnShow(bool show)
        {
            base.OnShow(show);

            Button darkBTN = darkChoicePage.GetComponentInChildren<Button>();
            Button lightBTN = lightChoicePage.GetComponentInChildren<Button>();
            if (show)
            {
                if (saveData.pathChosen == PathEnum.None)
                {
                    darkChoicePage.SetActive(true);
                    lightChoicePage.SetActive(true);

                    page1Picked.SetActive(false);
                    page2Picked.SetActive(false);


                    
                    darkBTN.onClick.AddListener(delegate
                    {
                        ChoseDark();
                        darkBTN.onClick.RemoveAllListeners();
                    });

                    
                    lightBTN.onClick.AddListener(delegate
                    {
                        ChoseLight();
                        lightBTN.onClick.RemoveAllListeners();
                    });
                }
                else
                {
                    ShowDefaultPages();
                }
            } else
            {
                darkBTN.onClick.RemoveAllListeners();
                lightBTN.onClick.RemoveAllListeners();
            }

            AbilityInfo.gameObject.SetActive(false);
        }


        private void ChoseLight()
        {
            saveData.pathChosen = PathEnum.Light;
            saveData.unlockedSkills.Add("LightPath");
            saveData.unlockedSkills.Add("DarkPath");
            saveData.unlockedSkills.Add("LightSideChosen");
            saveData.unlockedSkills.Add("LightSide");

            ShowDefaultPages();
            SaveJson();
        }

        private void ChoseDark()
        {
            saveData.pathChosen = PathEnum.Dark;
            saveData.unlockedSkills.Add("DarkPath");
            saveData.unlockedSkills.Add("LightPath");
            saveData.unlockedSkills.Add("DarkSideChosen");
            saveData.unlockedSkills.Add("DarkSide");

            ShowDefaultPages();
            SaveJson();
        }

        private void ShowDefaultPages()
        {
            darkChoicePage.SetActive(false);
            lightChoicePage.SetActive(false);

            page1Picked.SetActive(true);
            page2Picked.SetActive(true);
        }

        public void SaveJson()
        {
            saveData.quest1 = questUI1.quest?.GetQuestInfo();
            saveData.quest2 = questUI2.quest?.GetQuestInfo();
            saveData.quest3 = questUI3.quest?.GetQuestInfo();

            if (saveData != null)
            {
                Directory.CreateDirectory(Path.Combine(Application.streamingAssetsPath, "Mods/BloodMagic/Saves"));
                File.WriteAllText(Path.Combine(Application.streamingAssetsPath, saveFileName), JsonConvert.SerializeObject(saveData, Formatting.Indented));
            }
        }
    }
}
