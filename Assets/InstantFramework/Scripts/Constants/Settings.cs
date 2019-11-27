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

        //Helpshift settings
        public const string HELPSHIFT_API_KEY = "b1ac5777b999d34713f2f1c878d18014";
        public const string HELPSHIFT_DOMAIN_NAME = "freetrialturbo-labz.helpshift.com";
#if UNITY_ANDROID
        public const string HELPSHIFT_APP_ID = "freetrialturbo-labz_platform_20191127091425948-cd85b37895d2f59";
#elif UNITY_IOS
        public const string HELPSHIFT_APP_ID = "freetrialturbo-labz_platform_20191127091425930-f4431922493a1f5";
#endif
    }
}
