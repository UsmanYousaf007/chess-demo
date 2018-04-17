/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-15 17:46:40 UTC+05:00
/// 
/// @description
/// [add_description_here]

using strange.extensions.command.impl;
using TurboLabz.Chess;
using System;
using TurboLabz.TLUtils;
using TurboLabz.InstantFramework;

namespace TurboLabz.InstantChess
{
    public class ShowAdCommand : Command
    {
        // Dispatch signals
        [Inject] public LoadCPUGameSignal loadCPUGameSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

        // Services
        [Inject] public IAdsService adsService { get; set; }
        [Inject] public ICPUGameModel cpuGameModel { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }

        public override void Execute()
        {
            if (adsService.isAdAvailable)
            {
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_AD);
                adsService.ShowAd().Then(OnShowAd);

                analyticsService.AdStart(false, UnityAdsPlacementId.VIDEO);
            }
            else
            {
                loadCPUGameSignal.Dispatch();
            }
        }

        private void OnShowAd(AdsResult result)
        {
            loadCPUGameSignal.Dispatch();

            if (result == AdsResult.FINISHED)
            {
                analyticsService.AdComplete(false, UnityAdsPlacementId.VIDEO);
            }
            else if (result == AdsResult.SKIPPED)
            {
                analyticsService.AdSkip(false, UnityAdsPlacementId.VIDEO);
            }
        }
    }
}
