
using System.Collections.Generic;
/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
namespace TurboLabz.InstantFramework
{
    public class RewardDlgVO
    {
        public string type;

        public List<string> rewardShortCodes;
        public List<int> rewardQty;
        //public string rewardShortCode1;
        //public int rewardQty1;
        //public string rewardShortCode2;
        //public int rewardQty2;
        //public string rewardShortCode3;
        //public int rewardQty3;

        public string league;
        public string tournamentName;
        public string chestName;

        public RewardDlgVO(string typeId)
        {
            type = typeId;
            rewardShortCodes = new List<string>();
            rewardQty = new List<int>();
        }

        public void AddRewardItem(string shortCode, int qty)
        {
            rewardShortCodes.Add(shortCode);
            rewardQty.Add(qty);
        }

        public string GetRewardItemShortCode(int index)
        {
            return rewardShortCodes[index];
        }

        public int GetRewardItemQty(int index)
        {
            return rewardQty[index];
        }

        public int GetRewardItemsCount()
        {
            return rewardShortCodes.Count;
        }
    }
}
