/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class RateAppService : IRateAppService
    {
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IMetaDataModel metaDataModel { get; set; }

        public void RateApp(bool goRate)
        {
            preferencesModel.hasRated = true;

            // Bail if not going to rate
            if (!goRate)
            {
                return;
            }

            #if UNITY_IOS
            //UnityEngine.iOS.Device.RequestStoreReview();
            Application.OpenURL(metaDataModel.appInfo.iosURL + "?action=write-review");
            #endif

            #if UNITY_ANDROID
            Application.OpenURL(metaDataModel.appInfo.androidURL);
            #endif

            #if UNITY_EDITOR
            Application.OpenURL("https://itunes.apple.com/us/app/chess/id1386718098?mt=8");
            Application.OpenURL("https://play.google.com/store/apps/details?id=com.turbolabz.instantchess.android.googleplay");
            #endif
        }
    }
}