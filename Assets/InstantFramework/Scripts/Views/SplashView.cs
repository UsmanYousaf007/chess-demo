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

        public GameObject wifiWarning;

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
            wifiWarning.gameObject.SetActive(true);
        }
    }
}
