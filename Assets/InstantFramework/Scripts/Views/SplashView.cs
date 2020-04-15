/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class SplashView : View
    {
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }

        public Text wifiWarning;
        public Text userMessage;
        public GameObject connectingDots;

        public void Init()
        {
            wifiWarning.gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);

            GameObject originalSplash = GameObject.FindGameObjectWithTag("OriginalSplash");
            GameObject.Destroy(originalSplash);
        }

        public void WifiHealthUpdate()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                wifiWarning.text = "No internet connection";
                userMessage.text = "Please check your internet connection";
            }
            else
            {
                wifiWarning.text = "Slow internet. Please wait..";
            }

            analyticsService.Event(AnalyticsEventId.internet_warning_on_splash);
            wifiWarning.gameObject.SetActive(true);
        }

        public void ShowContent(bool show)
        {
            connectingDots.SetActive(show);
            userMessage.gameObject.SetActive(show);
        }
    }
}
