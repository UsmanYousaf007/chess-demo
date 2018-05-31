/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public interface ILeagueSettingsModel
    {
        void Reset();
        IOrderedDictionary<string, LeagueInfo> settings { get; set; }
    }
}
