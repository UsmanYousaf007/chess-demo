/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-12-08 15:49:58 UTC+05:00
/// 
/// @description
/// [add_description_here]

using strange.extensions.promise.api;

namespace TurboLabz.InstantFramework
{
    public interface IAdsService
    {
        // Rewarded ads
        bool isRewardedAdAvailable { get; }
        IPromise<AdsResult> ShowRewardedAd();
    }
}
