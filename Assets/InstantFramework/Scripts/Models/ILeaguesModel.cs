﻿/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;
using TurboLabz.InstantGame;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public interface ILeaguesModel
    {
        Dictionary<string, League> leagues { get; set; }
    }

    public class League
    {
        public string name;
        public int qualifyTrophies;
        public Dictionary<string, int> dailyReward;
    }
}