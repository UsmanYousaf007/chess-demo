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
        public Button rvFullAnalysisBtn;
        public Button watchVideoBtn;

        public TMP_Text fullAnalysisGemsCount;
        public TMP_Text blunders;
        public TMP_Text mistakes;
        public TMP_Text perfect;
        public TMP_Text titleText;
        public TMP_Text rvGemsCost;
        public TMP_Text rvTimer;

        public GameObject freeTag;
        public GameObject freeTitle;
        public GameObject sparkle;
        public GameObject gemIcon;
        public GameObject rvPanel;
        public GameObject watchVideoPanel;
        public GameObject timerPanel;
        public GameObject videoNotAvailableTooltip;
        public GameObject timerTooltip;

        public IServerClock serverClock;

        //Signals
        public Signal fullAnalysisButtonClickedSignal = new Signal();
        public Signal notEnoughGemsSignal = new Signal();
        public Signal closeDlgSignal = new Signal();
        public Signal watchVideoSignal = new Signal();
        public Signal<bool> schedulerSubscription = new Signal<bool>();

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
        private Sequence animationSequence;
        private long coolDownTime;

        public void Init()
        {
            closeBtn.onClick.AddListener(OnCloseBtnClicked);
            buyFullAnalysisBtn.onClick.AddListener(OnBuyFullAnalysisBtnClicked);
            watchVideoBtn.onClick.AddListener(OnWatchVideoClicked);

            titleText.text = localizationService.Get(LocalizationKey.GM_BUY_GAME_ANALYSIS_TITLE_TEXT);
            UIDlgManager.Setup(gameObject);
        }

        public void Show()
        {
            UIDlgManager.Show(gameObject);
        }

        public void Hide()
        {
            UIDlgManager.Hide(gameObject);
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

        public void UpdateView(BuyGameAnalysisVO vo)
        {
            storeItem = vo.storeItem;
            availableForFree = vo.availableForFree;
            coolDownTime = vo.coolDownTime;
            blunders.text = vo.matchAnalysis.blunders.ToString();
            mistakes.text = vo.matchAnalysis.mistakes.ToString();
            perfect.text = vo.matchAnalysis.perfectMoves.ToString();
            freeTag.SetActive(availableForFree);
            freeTitle.SetActive(availableForFree);
            sparkle.SetActive(!availableForFree);
            gemIcon.SetActive(!availableForFree);
            fullAnalysisGemsCount.enabled = !availableForFree;
            buyFullAnalysisBtn.interactable = true;
            closeBtn.interactable = true;
            SetupPrice();
            AnimateFreeTag(availableForFree);
            SetupAnalysisRV(vo.showRV);
        }

        public void SetupPrice()
        {
            hasEnoughGems = availableForFree || playerModel.gems >= storeItem.currency3Cost;
            fullAnalysisGemsCount.text = storeItem.currency3Cost.ToString();
        }

        private void AnimateFreeTag(bool animate)
        {
            if (animationSequence != null && animationSequence.IsPlaying())
            {
                animationSequence.Kill();
                animationSequence = null;
            }

            if (!animate)
            {
                return;
            }

            if (animationSequence == null)
            {
                animationSequence = DOTween.Sequence();
                animationSequence.AppendCallback(() => freeTag.transform.localEulerAngles = Vector3.zero);
                animationSequence.AppendCallback(() => freeTag.transform.DOPunchRotation(Vector3.forward * 8, 1.3f));
                animationSequence.AppendInterval(3.0f);
                animationSequence.SetLoops(-1);
            }

            animationSequence.PlayForward();
        }

        private void SetupAnalysisRV(bool showRV)
        {
            rvPanel.SetActive(showRV);
            buyFullAnalysisBtn.gameObject.SetActive(!showRV);
            freeTitle.SetActive(showRV);
            sparkle.SetActive(!showRV);
            gemIcon.SetActive(!showRV);
            fullAnalysisGemsCount.enabled = !showRV;
            rvFullAnalysisBtn.interactable = true;

            if (showRV)
            {
                if (IsCoolDownComplete())
                {
                    OnTimerCompleted();
                }
                else
                {
                    StartTimer();
                }
            }
        }

        private bool IsCoolDownComplete()
        {
            return coolDownTime < serverClock.currentTimestamp;
        }

        private void OnTimerCompleted()
        {
            timerPanel.SetActive(false);
            watchVideoPanel.SetActive(true);
            timerTooltip.SetActive(false);
        }

        private void StartTimer()
        {
            UpdateTimerText();
            timerPanel.SetActive(true);
            watchVideoPanel.SetActive(false);
            schedulerSubscription.Dispatch(true);
        }

        private void UpdateTimerText()
        {
            long timeLeft = coolDownTime - serverClock.currentTimestamp;
            if (timeLeft > 0)
            {
                timeLeft -= 1000;
                rvTimer.text = TimeUtil.FormatTournamentClock(TimeSpan.FromMilliseconds(timeLeft));
            }
            else
            {
                rvTimer.text = "0s";
            }
        }

        public void SchedulerCallBack()
        {
            if (!IsCoolDownComplete())
            {
                UpdateTimerText();
            }
            else
            {
                schedulerSubscription.Dispatch(false);
                OnTimerCompleted();
            }
        }

        private void OnWatchVideoClicked()
        {
            if (IsCoolDownComplete())
            {
                watchVideoSignal.Dispatch();
            }
            else
            {
                timerTooltip.SetActive(true);
                Invoke("DisableTimerTooltip", 5);
            }
        }

        private void DisableTimerTooltip()
        {
            timerTooltip.SetActive(false);
        }

        public void EnableRVNotAvailableTooltip()
        {
            videoNotAvailableTooltip.SetActive(true);
            Invoke("DisableRVNotAvaillableTooltip", 5);

        }

        private void DisableRVNotAvaillableTooltip()
        {
            videoNotAvailableTooltip.SetActive(false);
        }
    }
}
