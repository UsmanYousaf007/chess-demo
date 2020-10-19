/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using System.Collections.Generic;
using TurboLabz.TLUtils;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class AppsFlyerService : IAppsFlyerService
    {
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }

        public void Init()
        {
            ProcessLaunchEvents();
        }

        public void TrackRichEvent(string eventName, Dictionary<string, object> eventValues = null)
        {
            if (eventValues == null)
            {
                eventValues = new Dictionary<string, object>();
                eventValues.Add("none", "");
            }

            hAnalyticsService.LogAppsFlyerEvent(eventName, eventValues);
        }

        public void TrackMonetizationEvent(string name, int cents)
        {
            hAnalyticsService.LogAppsFlyerMonetizationEvent(name, cents);
        }

        private void ProcessLaunchEvents()
        {
            TrackRichEvent(AnalyticsEventId.launch.ToString());

            var currentTime = TimeUtil.ToDateTime(backendService.serverClock.currentTimestamp);

            var daysBetweenLastLogin = (currentTime - preferencesModel.appsFlyerLastLaunchTime).Days;

            if (daysBetweenLastLogin == 1)
            {
                preferencesModel.appsFlyerLastLaunchTime = currentTime;
                preferencesModel.continousPlayCount++;
                if (preferencesModel.continousPlayCount >= 2 && preferencesModel.continousPlayCount <= 6)
                {
                    TrackRichEvent(string.Format("{0}_{1}_days", AnalyticsEventId.continuous_play, preferencesModel.continousPlayCount + 1));
                }
            }
            else if (daysBetweenLastLogin > 1)
            {
                preferencesModel.appsFlyerLastLaunchTime = currentTime;
                preferencesModel.continousPlayCount = 10;
            }
        }

        public void TrackLimitedEvent(AnalyticsEventId eventName, int  currentValue)
        {
            if (IsWithInLimits(currentValue))
            {
                TrackRichEvent(string.Format("{0}_{1}", eventName, currentValue));
            }
        }

        private bool IsWithInLimits(int currentValue)
        {
            if (currentValue <= 20 &&
                currentValue % 5 == 0 ||
                currentValue < 5)
            {
                return true;
            }
            return false;
        }
    }
}