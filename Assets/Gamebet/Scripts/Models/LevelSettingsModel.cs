/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-12-13 16:14:47 UTC+05:00
/// 
/// @description
/// The level settings here are only kept for the current level of the player.
/// Complete level settings are not needed on the client side. Only the one for
/// the current player level is required.

namespace TurboLabz.Gamebet
{
    public class LevelSettingsModel : ILevelSettingsModel
    {
        public int maxLevel { get; set; }
        public LevelInfo currentLevelInfo { get; set; }
    }

    public struct LevelInfo
    {
        public int level;
        public int startXp;
        public int endXp;
        public int levelPromotionRewardBucks; // TODO: Change 'bucks' to currency2
    }
}
