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

using UnityEngine;

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
            public const long TIME_BETWEEN_INGAME_ADS = 3 * 60; // 3 minutes
            public const long TIME_DISABLE_TOURNAMENT_PREGAME_ADS = 10 * 60;    // When 10 minutes are left in tournament
            public const long TIME_DISABLE_30MIN_INGAME_ADS = 20 * 60;  // After 20 minutes
        }

        public class ABTest
        {
            public static string ADS_TEST_GROUP_DEFAULT = "ads_A";
            public static string ADS_TEST_GROUP = "ads_A";

            public static string COINS_TEST_GROUP_DEFAULT = "coins_A";
            public static string COINS_TEST_GROUP = "coins_A";
        }

        // Tween settings for new user flow
        public const float MIN_ALPHA = 0;
        public const float MAX_ALPHA = 1;
        public const float TWEEN_DURATION = 0.4f;
        public static Vector3 MIN_SCALE = new Vector3(0.3f, 0.3f, 1);
        public static Vector3 MAX_SCALE = Vector3.one;
    }
}
