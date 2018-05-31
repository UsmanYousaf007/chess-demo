/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

namespace TurboLabz.InstantFramework
{
    public class LevelSettingsModel : ILevelSettingsModel
    {
        public int maxLevel { get; set; }
        public LevelInfo currentLevelInfo { get; set; }

        public void Reset()
        {
            maxLevel = 0;
            currentLevelInfo = new LevelInfo();
        }
    }

    public struct LevelInfo
    {
        public int level;
        public int startXp;
        public int endXp;
        public int levelPromotionRewardBucks; // TODO: Change 'bucks' to currency2
    }
}
