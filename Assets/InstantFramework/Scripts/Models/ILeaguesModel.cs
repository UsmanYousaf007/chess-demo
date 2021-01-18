/// @license Propriety <http://license.url>
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
        League GetCurrentLeagueInfo();
        League GetLeagueInfo(int league);
    }

    public class League
    {
        public string name;
        public int qualifyTrophies;
        public Dictionary<string, int> dailyReward;
        public int winTrophies;
        public int lossTrophies;
    }
}