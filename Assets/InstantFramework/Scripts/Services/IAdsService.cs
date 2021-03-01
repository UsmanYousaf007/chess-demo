/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.promise.api;

namespace TurboLabz.InstantFramework
{
    public interface IAdsService
    {
        void Init();
        bool IsRewardedVideoAvailable(AdPlacements placementId);
        IPromise<AdsResult> ShowRewardedVideo(AdPlacements placementId);
        bool IsInterstitialAvailable(AdPlacements placementId);
        IPromise<AdsResult> ShowInterstitial(AdPlacements placementId);
        void ShowBanner();
        void HideBanner();
        void CollectSensitiveData(bool consentStatus);
        void ShowTestSuite();
        bool IsInterstitialReady(AdPlacements placementId);
        bool IsInterstitialNotCapped();
        bool GetAdsConsent();
        bool IsPersonalisedAdDlgShown();
    }
}
