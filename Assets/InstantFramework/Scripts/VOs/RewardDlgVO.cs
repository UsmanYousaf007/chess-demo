
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
namespace TurboLabz.InstantFramework
{
    public class RewardDlgVO
    {
        public string type;
        public string msgId;

        public List<string> rewardShortCodes;
        public List<int> rewardQty;
        public List<Sprite> rewardImages;

        public string league;
        public string tournamentName;
        public string chestName;
        public Sprite chestImage;

        public RewardDlgVO(string typeId)
        {
            type = typeId;
            msgId = null;
            rewardShortCodes = new List<string>();
            rewardQty = new List<int>();
            rewardImages = new List<Sprite>();
        }

        public void AddRewardItem(string shortCode, int qty, Sprite image)
        {
            rewardShortCodes.Add(shortCode);
            rewardQty.Add(qty);
            rewardImages.Add(image);
        }

        public string GetRewardItemShortCode(int index)
        {
            return rewardShortCodes[index];
        }

        public int GetRewardItemQty(int index)
        {
            return rewardQty[index];
        }

        public Sprite GetRewardImage(int index)
        {
            return rewardImages[index];
        }

        public int GetRewardItemsCount()
        {
            return rewardShortCodes.Count;
        }
    }
}
