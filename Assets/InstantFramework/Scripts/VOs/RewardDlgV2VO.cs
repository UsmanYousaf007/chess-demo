
using System.Collections.Generic;
using strange.extensions.signal.impl;
using UnityEngine;
using UnityEngine.UI;

/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
namespace TurboLabz.InstantFramework
{
    public class RewardDlgV2VO
    {
        public class Reward
        {
            public string ShortCode;
            public int Quantity;

            public Reward(string shortCode, int quantity)
            {
                ShortCode = shortCode;
                Quantity = quantity;
            }
        }

        public List<Reward> Rewards;

        public RewardDlgV2VO()
        {
            Rewards = new List<Reward>();
        }
    }
}
