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
using TurboLabz.Chess;

namespace TurboLabz.InstantFramework
{
    [CLSCompliant(false)]
    public class BuyGameAnalysisView : View
    {

        public Button closeBtn;
        public Button buyFullAnalysisBtn;

        public TMP_Text fullAnalysisGemsCount;
        public TMP_Text blunders;
        public TMP_Text mistakes;
        public TMP_Text perfect;
        public TMP_Text titleText;

        public GameObject freeTag;
        public GameObject freeTitle;
        public GameObject sparkle;
        public GameObject gemIcon;

        //Signals
        public Signal fullAnalysisButtonClickedSignal = new Signal();
        public Signal notEnoughGemsSignal = new Signal();
        public Signal closeDlgSignal = new Signal();

        //Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IBlurBackgroundService blurBackgroundService { get; set; }

        //Models
        [Inject] public IPlayerModel playerModel { get; set; }

        private bool hasEnoughGems;
        private bool availableForFree;
        private StoreItem storeItem;

        public void Init()
        {
            closeBtn.onClick.AddListener(OnCloseBtnClicked);
            buyFullAnalysisBtn.onClick.AddListener(OnBuyFullAnalysisBtnClicked);
            titleText.text = localizationService.Get(LocalizationKey.GM_BUY_GAME_ANALYSIS_TITLE_TEXT);
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
            audioService.PlayStandardClick();
            closeDlgSignal.Dispatch();
        }

        private void OnBuyFullAnalysisBtnClicked()
        {
            audioService.PlayStandardClick();

            if (hasEnoughGems)
            {
                buyFullAnalysisBtn.interactable = false;
                closeBtn.interactable = false;
                fullAnalysisButtonClickedSignal.Dispatch();
            }
            else
            {
                notEnoughGemsSignal.Dispatch();
            }
        }

        public void UpdateView(MatchAnalysis matchAnalysis, StoreItem storeItem, bool availableForFree)
        {
            this.storeItem = storeItem;
            this.availableForFree = availableForFree;
            blunders.text = matchAnalysis.blunders.ToString();
            mistakes.text = matchAnalysis.mistakes.ToString();
            perfect.text = matchAnalysis.perfectMoves.ToString();
            freeTag.SetActive(availableForFree);
            freeTitle.SetActive(availableForFree);
            sparkle.SetActive(!availableForFree);
            gemIcon.SetActive(!availableForFree);
            fullAnalysisGemsCount.enabled = !availableForFree;
            buyFullAnalysisBtn.interactable = true;
            closeBtn.interactable = true;
            SetupPrice();
        }

        public void SetupPrice()
        {
            hasEnoughGems = availableForFree || playerModel.gems >= storeItem.currency3Cost;
            fullAnalysisGemsCount.text = storeItem.currency3Cost.ToString();
        }
    }
}
