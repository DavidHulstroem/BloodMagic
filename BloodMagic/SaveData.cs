using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodMagic
{
    public class SaveData
    {

        public int lightPoints { get; set; }
        public int darkPoints { get; set; }


        public float xp { get; set; }
        public int level { get; set; }

        public float bulletSpeed { get; set; } = 15;

        public float gesturePrescision { get; set; } = 0.7f;
        public float drainDistance { get; set; } = 5;
        public float drainPower { get; set; } = 2f;


        public float wavePushStrenght { get; set; } = 20f;


        public float xpMultiplier { get; set; } = 1;

        public QuestInfo quest1 { get; set; }
        public QuestInfo quest2 { get; set; }
        public QuestInfo quest3 { get; set; }

        public PathEnum pathChosen { get; set; } = PathEnum.None;

        public class QuestInfo
        {
            public int id { get; set; }
            public int level { get; set; }
            public float progress { get; set; }
        }

        public List<string> unlockedSkills { get; set; } = new List<string>();
    }

    

    public enum PathEnum
    {
        None,
        Light,
        Dark
    }
}
