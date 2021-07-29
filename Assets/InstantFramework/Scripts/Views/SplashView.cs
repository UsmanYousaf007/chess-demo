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
using System.Collections;

namespace TurboLabz.InstantFramework
{
    public class SplashView : View
    {
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }

        public Text wifiWarning;
        public Text userMessage;
        public CanvasGroup connectingDots;

        public void Init()
        {
            wifiWarning.gameObject.SetActive(false);
            connectingDots.alpha = Settings.MIN_ALPHA;
        }

        public void Show()
        {
            preferencesModel.sessionsBeforePregameAdCount++;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            StartCoroutine(HideWithDelay());
        }

        IEnumerator HideWithDelay()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            gameObject.SetActive(false);

            GameObject originalSplash = GameObject.FindGameObjectWithTag("OriginalSplash");
            GameObject.Destroy(originalSplash);
        }

        public void WifiHealthUpdate()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                wifiWarning.text = "No Internet";
                userMessage.text = "Please check your internet connection";
            }
            else
            {
                wifiWarning.text = "Connecting...";
            }

            analyticsService.Event(AnalyticsEventId.internet_warning_on_splash);
            wifiWarning.gameObject.SetActive(true);
        }

        public void ShowContent(bool show)
        {
            connectingDots.gameObject.SetActive(show);
            userMessage.gameObject.SetActive(show);
            connectingDots.DOFade(Settings.MAX_ALPHA, Settings.TWEEN_DURATION);
        }
    }
}
