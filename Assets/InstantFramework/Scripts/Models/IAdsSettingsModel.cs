/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;

namespace TurboLabz.InstantFramework
{
    public interface IAdsSettingsModel
    {
        int slotHour { get; set; }
        int freeNoAdsPeriod { get; set; }
        int globalCap { get; set; }
        int rewardedVideoCap { get; set; }
        int interstitialCap { get; set; }
    }
}

