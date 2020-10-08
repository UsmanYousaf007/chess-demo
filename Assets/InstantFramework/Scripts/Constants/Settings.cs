/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-31 16:30:51 UTC+05:00
/// 
/// @description
/// [add_description_here]

namespace TurboLabz.InstantFramework
{
    public static class Settings
    {
        // Unity settings
        public const bool MULTI_TOUCH_ENABLED = false;
        public const int TARGET_FRAME_RATE = 60;
        public const string SUPPORT_EMAIL = "support@turbolabz.com";

        public class Ads
        {
            public const long TIME_BETWEEN_INGAME_ADS = 3 * 60;
            //public const long TIME_DISABLE_TOURNAMENT_PREGAME_ADS = 10 * 60;
            public const long TIME_DISABLE_TOURNAMENT_PREGAME_ADS = 15;
            public const long TIME_DISABLE_30MIN_INGAME_ADS = 10 * 60;
        }

    }
}
