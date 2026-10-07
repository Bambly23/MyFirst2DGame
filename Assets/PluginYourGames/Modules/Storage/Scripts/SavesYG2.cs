
using System;

namespace YG
{
    public partial class SavesYG
    {
        public int idSave;

        public int Score = 0;
        public int[] CostInt = new int[] { 500, 5000 };
        public int ClickScore = 1;
        public int[] CostBonus = new int[] { 0 };
        public int Achievement1Max = 0;
        public bool isAchievement1 = true;
        public bool isAchievement2 = true;
        public bool isAchievement2Get = false;
        public bool isAchievement3 = true;
        public bool isAchievement3Get = false;
        public bool isAchievement4Completed = false;
        public bool isAchievement4RewardTaken = false;
        public bool isAchievement5Completed = false;
        public bool isAchievement5RewardTaken = false;
        public bool isAchievement6Completed = false;
        public bool isAchievement6RewardTaken = false;

        public int petLevel = 0;
        public int petXP = 0;
        public int petBonus = 0;
    }
}
