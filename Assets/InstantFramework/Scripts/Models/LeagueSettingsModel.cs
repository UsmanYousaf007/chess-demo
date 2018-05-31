/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class LeagueSettingsModel : ILeagueSettingsModel
    {
        public IOrderedDictionary<string, LeagueInfo> settings { get; set; }

        public void Reset()
        {
            settings = null;
        }
    }

    public struct LeagueInfo
    {
        public string id;
        public int startLevel;
        public int endLevel;
        public LeaguePrize prize1;
        public LeaguePrize prize2;
        public LeaguePrize prize3;
    }

    public struct LeaguePrize
    {
        public long currency1; // coins
        public long currency2; // bucks
    }
}
