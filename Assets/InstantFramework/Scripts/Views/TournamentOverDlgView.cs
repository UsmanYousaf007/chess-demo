/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using UnityEngine.UI;

using strange.extensions.mediation.impl;
using strange.extensions.signal.impl ;

using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using System;
using System.Collections;
using DG.Tweening;

namespace TurboLabz.InstantFramework
{
    public class TournamentOverDlgView : View
    {
        [Inject] public ILocalizationService localizationService { get; set; }

        public Text headingText;
        public Text subHeadingText;
        public Button button;

        public Signal buttonClickedSignal = new Signal();

        public void Init()
        {
            headingText.text = "Tournament Over";
            subHeadingText.text = "This tournament is over";
            button.onClick.AddListener(() => buttonClickedSignal.Dispatch());
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}