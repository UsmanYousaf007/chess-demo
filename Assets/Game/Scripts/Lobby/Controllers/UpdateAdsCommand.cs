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

namespace TurboLabz.InstantGame
{
    public class UpdateAdCommand : Command
    {
        // Dispatch Signals
        [Inject] public UpdateLobbyAdsSignal updateLobbyAdsSignal { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IMetaDataModel metaDataModel { get; set; }
        [Inject] public IPreferencesModel prefsModel { get; set; }

        // Services
        [Inject] public IAdsService adsService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }

        // Our ad slots will reset every day at 7 PM which is the prime time for gaming
        // https://www.prnewswire.com/news-releases/prime-time-is-peak-time-for-mobile-gaming-and-social-media-199165791.html

        public override void Execute()
        {
            // The code below is based on this algo: https://docs.google.com/drawings/d/1fsbwyNYrXJKs5sANyQ47v6wqqE7Ov8GjyN1TMhYvDl8/edit

            AdsState state;
            long runningSlotMs = (long)GetRunningSlotMs();

            if (prefsModel.adSlotId == runningSlotMs)
            {
                if (prefsModel.adSlotImpressions < metaDataModel.adsSettings.maxImpressionsPerSlot)
                {
                    state = adsService.IsAdAvailable(AdPlacementIds.AD_PLACEMENT_REWARDED_VIDEO) ? AdsState.AVAILABLE : AdsState.NOT_AVAILABLE;
                }
                else
                {
                    state = AdsState.WAIT;
                }
            }
            else
            {
                LogUtil.Log("Slot id has shifted..", "cyan");

                prefsModel.adSlotId = runningSlotMs;
                prefsModel.adSlotImpressions = 0;
                state = adsService.IsAdAvailable(AdPlacementIds.AD_PLACEMENT_REWARDED_VIDEO) ? AdsState.AVAILABLE : AdsState.NOT_AVAILABLE;
            }

            Assertions.Assert(state != AdsState.NONE, "Ad state should never be none, something broke in the algorithm");

            AdsVO vo = new AdsVO();
            vo.state = state;

            if (state == AdsState.AVAILABLE)
            {
                vo.bucks = (playerModel.adLifetimeImpressions + 1) * metaDataModel.adsSettings.adsRewardIncrement;
                vo.count = metaDataModel.adsSettings.maxImpressionsPerSlot - prefsModel.adSlotImpressions;
            }
            else if (state == AdsState.WAIT)
            {
                vo.waitMs = GetWaitMs(runningSlotMs);
            }

            updateLobbyAdsSignal.Dispatch(vo);
        }

        private double GetRunningSlotMs()
        {
            // Are we ahead of today's slot reset time?
            DateTime todaySlotTime = GetTodaySlotTime();
            int condition = DateTime.Compare(GetCurrentTime(), todaySlotTime);

            if (condition >= 0)
            {
                return GetEpochMs(GetTomorrowSlotTime());
            }
            else
            {
                return GetEpochMs(todaySlotTime);
            }
        }

        private double GetWaitMs(long runningSlotMs)
        {
            double currentMs = GetEpochMs(GetCurrentTime());
            return runningSlotMs - currentMs;
        }

        private DateTime GetTodaySlotTime()
        {
            // We can directly add hours to Today but we use a timestamp so we
            // can configure different values for testing.
            //TimeSpan ts = new TimeSpan(19, 50, 0);

            TimeSpan ts = new TimeSpan(metaDataModel.adsSettings.slotHour, 0, 0);
            return DateTime.Today.Add(ts);
        }

        private DateTime GetTomorrowSlotTime()
        {
            // We can directly add hours to Today but we use a timestamp so we
            // can configure different values for testing.
            //TimeSpan ts = new TimeSpan(19, 50, 0);

            TimeSpan ts = new TimeSpan(metaDataModel.adsSettings.slotHour, 0, 0);
            return DateTime.Today.AddDays(1).Add(ts);
        }

        private double GetEpochMs(DateTime dt)
        {
            return dt.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        private DateTime GetCurrentTime()
        {
            return DateTime.Now;
        }
    }
}
