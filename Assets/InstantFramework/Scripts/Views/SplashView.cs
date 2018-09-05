/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-11-20 04:07:23 UTC+05:00
/// 
/// @description
/// [add_description_here]

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


        public void Init()
        {
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
    }
}
