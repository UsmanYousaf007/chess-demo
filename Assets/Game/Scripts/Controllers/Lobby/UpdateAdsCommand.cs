/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @description
/// [add_description_here]

using strange.extensions.command.impl;
using TurboLabz.TLUtils;
using TurboLabz.InstantFramework;
using System;
using UnityEngine;

namespace TurboLabz.InstantChess
{
    public class UpdateAdCommand : Command
    {
        // Dispatch Signals
        [Inject] public UpdateLobbyAdsSignal updateLobbyAdsSignal { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IMetaDataModel metaDataModel { get; set; }

        // Services
        [Inject] public IAdsService adsService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }

        // Our ad slots will reset every day at 7 PM which is the prime time for gaming
        // https://www.prnewswire.com/news-releases/prime-time-is-peak-time-for-mobile-gaming-and-social-media-199165791.html
        const int SLOT_HOURS = 19;

        public override void Execute()
        {
            // The code below is based on this algo: https://docs.google.com/drawings/d/1fsbwyNYrXJKs5sANyQ47v6wqqE7Ov8GjyN1TMhYvDl8/edit

            AdsState state;
            long currentSlotId = (long)GetCurrentSlotId();

            if (playerModel.adSlotId == currentSlotId)
            {
                if (playerModel.adSlotImpressions < metaDataModel.adSettings.maxImpressionsPerSlot)
                {
                    state = adsService.isRewardedAdAvailable ? AdsState.AVAILABLE : AdsState.NOT_AVAILABLE;
                }
                else
                {
                    state = AdsState.WAIT;
                }
            }
            else
            {
                playerModel.adSlotId = currentSlotId;
                playerModel.adSlotImpressions = 0;
                state = adsService.isRewardedAdAvailable ? AdsState.AVAILABLE : AdsState.NOT_AVAILABLE;
            }

            Assertions.Assert(state != AdsState.NONE, "Ad state should never be none, something broke in the algorithm");

            AdsVO vo = new AdsVO();
            vo.state = state;

            if (state == AdsState.AVAILABLE)
            {
                vo.bucks = playerModel.adLifetimeImpressions * metaDataModel.adSettings.adsRewardIncrement;
                analyticsService.AdOffer(true, UnityAdsPlacementId.REWARDED_VIDEO);

            }
            else if (state == AdsState.WAIT)
            {
                vo.waitMs = GetWaitMs();
            }

            updateLobbyAdsSignal.Dispatch(vo);
        }

        private double GetCurrentSlotId()
        {
            DateTime slotTime = DateTime.Today.AddHours(SLOT_HOURS);
            int condition = DateTime.Compare(DateTime.Now, slotTime);

            if (condition >= 0)
            {
                slotTime = DateTime.Today.AddDays(1).AddHours(SLOT_HOURS);
            }

            return slotTime.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        private double GetWaitMs()
        {
            DateTime nextSlotTime = DateTime.Today.AddDays(1).AddHours(SLOT_HOURS);
            double nextSlotMs = nextSlotTime.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
            double currentMs = DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;

            return nextSlotMs - currentMs;
        }
    }
}
