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

        public void Init()
        {
            /* Mandatory - set your AppsFlyer’s Developer key. */
            //HUUUUGE KEY 
            AppsFlyer.setAppsFlyerKey("xPcmC6rPafKmCueR3W68Mk");
            //TURBO LABZ KEY
            //AppsFlyer.setAppsFlyerKey("2Rcqu5eJmG7svYc2RJauwh");
            /* For detailed logging */
            // AppsFlyer.setIsDebug (true); 
#if UNITY_IOS
            /* Mandatory - set your apple app ID
             NOTE: You should enter the number only and not the "ID" prefix */
            AppsFlyer.setAppID("1386718098");
            AppsFlyer.trackAppLaunch();
#elif UNITY_ANDROID
            /* Mandatory - set your Android package name */
            AppsFlyer.setAppID ("com.turbolabz.instantchess.android.googleplay");
            /* For getting the conversion data in Android, you need to add the "AppsFlyerTrackerCallbacks" listener.*/
            AppsFlyer.init ("xPcmC6rPafKmCueR3W68Mk", "AppsFlyerTrackerCallbacks");
#endif

            Debug.Log("################################## AppsFlyer: Initialized: "+ AppsFlyer.getAppsFlyerId());

            ProcessLaunchEvents();
        }

        public void TrackRichEvent(string eventName, Dictionary<string, string> eventValues = null)
        {
            //Dictionary<string, string> purchaseEvent = new Dictionary<string, string>();
            //purchaseEvent.Add("af_currency", "USD");
            //purchaseEvent.Add("af_revenue", "0.99");
            //purchaseEvent.Add("af_quantity", "1");
            //AppsFlyer.trackRichEvent("af_purchase", purchaseEvent);
            if (eventValues == null)
            {
                eventValues = new Dictionary<string, string>();
                eventValues.Add("none", "");
            }
            AppsFlyer.trackRichEvent(eventName, eventValues);
            Debug.Log("Appsflyer Event Name: " + eventName);
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
                preferencesModel.continousPlayCount = 0;
            }
        }
    }
}