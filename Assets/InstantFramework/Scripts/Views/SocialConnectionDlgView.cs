/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-03 16:10:44 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;
using UnityEngine.UI;

using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using System;
using System.Collections;
using DG.Tweening;

namespace TurboLabz.InstantGame
{
    [CLSCompliant(false)]
    public class SocialConnectionDlgView : View
    {
        public Button closeButton;

        //Signals
        public Signal closeButtonClickedSignal = new Signal();

        // Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }

        public void Init()
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnCloseButtonClicked()
        {
            closeButtonClickedSignal.Dispatch();
        }
    }
}