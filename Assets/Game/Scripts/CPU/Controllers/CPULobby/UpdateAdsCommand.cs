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

namespace TurboLabz.InstantChess
{
    public class UpdateAdCommand : Command
    {
        // Dispatch Signals
        [Inject] public UpdateLobbyAdsSignal updateAdsSignal { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IMetaDataModel metaDataModel { get; set; }

        // Services
        [Inject] public IAdsService adsService { get; set; }

        // Our ad slots will reset every day at 7 PM which is the prime time for gaming
        // https://www.prnewswire.com/news-releases/prime-time-is-peak-time-for-mobile-gaming-and-social-media-199165791.html
        const int SLOT_HOURS = 19;

        public override void Execute()
        {
            // The code below is based on this algo: https://docs.google.com/drawings/d/1fsbwyNYrXJKs5sANyQ47v6wqqE7Ov8GjyN1TMhYvDl8/edit

            AdsState state = AdsState.NONE;
            long currentSlotId = GetCurrentSlotId();

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
            }
            else if (state == AdsState.WAIT)
            {
                vo.waitMs = GetWaitMs();
                LogUtil.Log("Wait state ms: " + vo.waitMs, "cyan");
            }

            updateAdsSignal.Dispatch(vo);
        }

        private long GetCurrentSlotId()
        {
            DateTime slotTime = DateTime.Today.AddHours(SLOT_HOURS);
            return slotTime.Subtract(new DateTime(1970, 1, 1)).Milliseconds;
        }

        private long GetWaitMs()
        {
            DateTime nextSlotTime = DateTime.Today.AddDays(1).AddHours(SLOT_HOURS);
            long nextSlotMs = nextSlotTime.Subtract(new DateTime(1970, 1, 1)).Milliseconds;

            DateTime currentTime = DateTime.Now;
            long currentMs = currentTime.Subtract(new DateTime(1970, 1, 1)).Milliseconds;

            return nextSlotMs - currentMs;
        }
    }
}
