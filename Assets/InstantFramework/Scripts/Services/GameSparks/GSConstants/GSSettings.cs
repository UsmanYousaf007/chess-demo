/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-14 13:19:04 UTC+05:00
/// 
/// @description
/// [add_description_here]

namespace TurboLabz.InstantFramework
{
    public static class GSSettings
    {
        public const float PINGER_FREQUENCY = 15f;

        // These initial pinger settings ensure that we get a larger data set
        // for sampling the latency before the player gets into a game.
        public const float INITIAL_PINGER_FREQUENCY = 1f;
        public const int INITIAL_PING_COUNT = 6; // Note that in addition to this, the pinger does 1 extra instant ping as soon as it starts
        public const float SLOW_WIFI_WARNING_THRESHOLD = 1.0f;
    }
}
