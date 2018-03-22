/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-11-30 18:30:31 UTC+05:00
/// 
/// @description
/// [add_description_here]

using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public class LeagueSettingsModel : ILeagueSettingsModel
    {
        public IOrderedDictionary<string, LeagueInfo> settings { get; set; }
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
