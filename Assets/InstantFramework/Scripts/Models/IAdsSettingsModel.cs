/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;

namespace TurboLabz.InstantFramework
{
    public interface IAdsSettingsModel
    {
        int maxImpressionsPerSlot { get; set; }
        int slotHour { get; set; }
        int adsRewardIncrement { get; set; }

        void Reset();
    }
}

