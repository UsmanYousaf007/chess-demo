/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using UnityEngine;
using UnityEngine.UI;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TurboLabz.TLUtils;
using System;
using TMPro;
using System.Collections.Generic;
using System.Text;
using TurboLabz.InstantGame;
using TurboLabz.CPU;
using System.Collections;
using DG.Tweening;

namespace TurboLabz.InstantFramework
{
    [CLSCompliant(false)]
    public class BuyGameAnalysisView : View
    {

        public Button closeBtn;

        public Button buyFullAnalysisBtn;

        public TMP_Text fullAnalysisGemsCount;
        public TMP_Text bunders;
        public TMP_Text mistakes;
        public TMP_Text perfect;
        public TMP_Text titleText;

        //Models

        //Signals

        //Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IBlurBackgroundService blurBackgroundService { get; set; }


        public void Init()
        {
            closeBtn.onClick.AddListener(OnCloseBtnClicked);
            buyFullAnalysisBtn.onClick.AddListener(OnBuyFullAnalysisBtnClicked);
            titleText.text = localizationService.Get(LocalizationKey.WIN_TEXT);

        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnCloseBtnClicked()
        {

        }

        private void OnBuyFullAnalysisBtnClicked()
        {

        }
    }
}
