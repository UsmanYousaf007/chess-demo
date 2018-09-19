/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.promise.api;

namespace TurboLabz.InstantFramework
{
    public interface IAdsService
    {
        bool IsAdAvailable(string placementId);
        IPromise<AdsResult> ShowAd(string placementId);
    }
}
