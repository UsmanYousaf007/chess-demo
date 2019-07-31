/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class AppsFlyerService : IAppsFlyerService
    {
        public void Init()
        {
            /* Mandatory - set your AppsFlyer’s Developer key. */
            AppsFlyer.setAppsFlyerKey("2Rcqu5eJmG7svYc2RJauwh");
            /* For detailed logging */
             AppsFlyer.setIsDebug (true); 
#if UNITY_IOS
            /* Mandatory - set your apple app ID
             NOTE: You should enter the number only and not the "ID" prefix */
            AppsFlyer.setAppID("1386718098");
            AppsFlyer.trackAppLaunch();
#elif UNITY_ANDROID
            /* Mandatory - set your Android package name */
            AppsFlyer.setAppID ("com.turbolabz.instantchess.android.googleplay");
            /* For getting the conversion data in Android, you need to add the "AppsFlyerTrackerCallbacks" listener.*/
            AppsFlyer.init ("2Rcqu5eJmG7svYc2RJauwh","AppsFlyerTrackerCallbacks");
#endif

            Debug.Log("################################## AppsFlyer: Initialized: "+ AppsFlyer.getAppsFlyerId());
        }

        public void TrackRichEvent(string eventName, Dictionary<string, string> eventValues)
        {
            //Dictionary<string, string> purchaseEvent = new Dictionary<string, string>();
            //purchaseEvent.Add("af_currency", "USD");
            //purchaseEvent.Add("af_revenue", "0.99");
            //purchaseEvent.Add("af_quantity", "1");
            //AppsFlyer.trackRichEvent("af_purchase", purchaseEvent);

            AppsFlyer.trackRichEvent(eventName, eventValues);
        }
    }
}