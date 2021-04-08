/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using UnityEngine;
using UnityEngine.UI;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using System;
using TMPro;
using TurboLabz.InstantGame;
using System.Collections;
using TurboLabz.TLUtils;
using DG.Tweening;

namespace TurboLabz.InstantFramework
{
    [CLSCompliant(false)]
    public class SelectTimeModeView : View
    {
        public TMP_Text startGame3mText;
        public Button startGame3mButton;

        public TMP_Text startGame5mText;
        public Button startGame5mButton;

        public TMP_Text startGame10mText;
        public Button startGame10mButton;

        public TMP_Text startGame30mText;
        public Button startGame30mButton;

        public Button powerPlayOnBtn;
        public Image powerPlayTick;
        //public Image powerPlayPlus;
        public GameObject powerPlayPlus;
        public TMP_Text onText;
        public Image gemIcon;
        public TMP_Text gemCost;
        public string shortCode;
        public Button closeButton;
        public TMP_Text freeHintsText;

        public Button powerPlayWithRVBtn;
        public Image powerPlayWithRVTick;
        public GameObject powerPlayPlusWithRV;
        public Image gemIconWithRv;
        public TMP_Text gemCostWithRv;

        public Button rewardedVideoBtn;
        public GameObject powerPlayAdTimer;
        public TMP_Text powerPlayAdTimerRemainingTime;
        public GameObject getRV;
        public GameObject tooltip;

        //Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }
        [Inject] public IStoreSettingsModel storeSettingsModel { get; set; }
        [Inject] public IAdsSettingsModel adsSettingsModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        //Signals
        public Signal<string, bool> playMultiplayerButtonClickedSignal = new Signal<string, bool>();
        public Signal powerModeButtonClickedSignal = new Signal();
        public Signal notEnoughCoinsSignal = new Signal();
        public Signal notEnoughGemsSignal = new Signal();
        public Signal closeButtonSignal = new Signal();
        public Signal<AdPlacements> showRewardedAdSignal = new Signal<AdPlacements>();
        public Signal<long> setCoolDownTimer = new Signal<long>();
        public Signal<bool> schedulerSubscription = new Signal<bool>();
        //Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IBlurBackgroundService blurBackgroundService { get; set; }
        [Inject] public IAdsService adsService { get; set; }
        [Inject] public IBackendService backendService { get; set; }

        private StoreItem storeItem;
        private bool isPowerModeOn;
        private long betValue;

        private bool canSeeRewardedVideo;
        private long coolDownTimeUTC;
        private float coolDownInterval;
        public void Init()
        {
            UIDlgManager.Setup(gameObject);

            startGame3mButton.onClick.AddListener(delegate { OnStartGameBtnClicked(FindMatchAction.ActionCode.Random3.ToString()); });
            startGame5mButton.onClick.AddListener(delegate { OnStartGameBtnClicked(FindMatchAction.ActionCode.Random.ToString()); });
            startGame10mButton.onClick.AddListener(delegate { OnStartGameBtnClicked(FindMatchAction.ActionCode.Random10.ToString()); });
            startGame30mButton.onClick.AddListener(delegate { OnStartGameBtnClicked(FindMatchAction.ActionCode.Random30.ToString()); });
            closeButton.onClick.AddListener(OnCloseButtonClicked);
            startGame3mText.text = localizationService.Get(LocalizationKey.MIN3_GAME_TEXT);
            startGame5mText.text = localizationService.Get(LocalizationKey.MIN5_GAME_TEXT);
            startGame10mText.text = localizationService.Get(LocalizationKey.MIN10_GAME_TEXT);
            startGame30mText.text = localizationService.Get(LocalizationKey.MIN30_GAME_TEXT);

            powerPlayOnBtn.onClick.AddListener(OnPowerPlayButtonClicked);
            powerPlayWithRVBtn.onClick.AddListener(OnPowerPlayButtonClicked);

            rewardedVideoBtn.onClick.AddListener(OnPlayRewardedVideoClicked);
        }

        public void Show()
        {
            UIDlgManager.Show(gameObject);
            SyncRvTimer();
        }

        public void Hide()
        {
            UIDlgManager.Hide(gameObject);
        }

        public void UpdateView(SelectTimeDlgVO vo)
        {
            this.betValue = vo.bet;
            storeItem = vo.storeItem;
            //if (storeItem == null && storeSettingsModel.items.ContainsKey(shortCode))
            //{
            //    storeItem = storeSettingsModel.items[shortCode];
            //}

            if (storeItem == null)
            {
                return;
            }
            canSeeRewardedVideo = false;// vo.canSeeRewardedVideo;
            coolDownInterval = vo.rewardedVideoCoolDownInterval;
            freeHintsText.text = $"Get {settingsModel.powerModeFreeHints} Free Hints";
            SetupState(isPowerModeOn, canSeeRewardedVideo);
            SetupPlayButtons(true);
        }

        void OnStartGameBtnClicked(string actionCode)
        {
            Debug.Log("OnQuickMatchBtnClicked");
            audioService.PlayStandardClick();
            if (playerModel.coins >= betValue)
            {
                playMultiplayerButtonClickedSignal.Dispatch(actionCode, isPowerModeOn);
                isPowerModeOn = false;
                SetupPlayButtons(false);
            }
            else
            {
                notEnoughCoinsSignal.Dispatch();
            }
        }

        void OnPowerPlayButtonClicked()
        {
            audioService.PlayStandardClick();
            if (playerModel.gems >= storeItem.currency3Cost)
            {
                powerPlayOnBtn.interactable = false;
                powerPlayWithRVBtn.interactable = false;
                powerPlayWithRVTick.enabled = true;
                powerModeButtonClickedSignal.Dispatch();
            }
            else
            {
                notEnoughGemsSignal.Dispatch();
            }
        }

        void SetupState(bool powerModeEnabled, bool canSeeRV)
        {
            powerPlayOnBtn.gameObject.SetActive(!canSeeRV);
            powerPlayWithRVBtn.gameObject.SetActive(canSeeRV);
            rewardedVideoBtn.gameObject.SetActive(canSeeRV);
            if (!canSeeRV)
            {
                powerPlayTick.enabled = powerModeEnabled;
                powerPlayPlus.SetActive(!powerModeEnabled);
                onText.enabled = powerModeEnabled;
                gemIcon.enabled = !powerModeEnabled;
                gemCost.enabled = !powerModeEnabled;
                gemCost.text = storeItem.currency3Cost.ToString();
                powerPlayOnBtn.interactable = !powerModeEnabled;

            }
            else
            {
                bool isCoolDownComplete = IsCoolDownComplete();
                powerPlayWithRVTick.enabled = powerModeEnabled;
                powerPlayPlusWithRV.SetActive(!powerModeEnabled);
                gemIconWithRv.enabled = !powerModeEnabled;
                gemCostWithRv.enabled = !powerModeEnabled;
                powerPlayAdTimer.SetActive(!isCoolDownComplete);
                powerPlayWithRVBtn.interactable = !powerModeEnabled;
                getRV.SetActive(isCoolDownComplete);
                tooltip.SetActive(false);
            }

            isPowerModeOn = powerModeEnabled;

        }

        void SyncRvTimer()
        {
            //if (gameObject.activeInHierarchy && !isCoolDownComplete)
            //{
            //    StartCoroutine(RvCoolDownTimer());
            //}
        }

        public void OnEnablePowerMode()
        {
            audioService.Play(audioService.sounds.SFX_REWARD_UNLOCKED);
            SetupState(true, canSeeRewardedVideo);
        }

        private void OnCloseButtonClicked()
        {
            audioService.PlayStandardClick();
            closeButtonSignal.Dispatch();
        }

        private void SetupPlayButtons(bool enable)
        {
            startGame3mButton.interactable =
                startGame5mButton.interactable =
                startGame10mButton.interactable =
                startGame30mButton.interactable = enable;
        }


        private void OnPlayRewardedVideoClicked()
        {
            if (IsCoolDownComplete())
            {
                showRewardedAdSignal.Dispatch(AdPlacements.Rewarded_powerplay);
                powerPlayAdTimer.SetActive(true);
                getRV.SetActive(false);
                SetupTimer();
            }
            else
            {
                tooltip.SetActive(true);
            }
        }

        public bool IsCoolDownComplete()
        {
            return coolDownTimeUTC < DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            //return preferencesModel.rvCoolDownTimeUTC < backendService.serverClock.currentTimestamp;
        }


        private void SetupTimer()
        {
            if (IsCoolDownComplete())
            {
                coolDownTimeUTC = DateTimeOffset.UtcNow.AddMinutes(coolDownInterval).ToUnixTimeMilliseconds();
                setCoolDownTimer.Dispatch(coolDownTimeUTC);
                schedulerSubscription.Dispatch(true);
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

        private void UpdateTimerText()
        {
            long timeLeft = coolDownTimeUTC - DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (timeLeft>0)
            {
                timeLeft -=1000;
                powerPlayAdTimerRemainingTime.text = TimeUtil.FormatTournamentClock(TimeSpan.FromMilliseconds(timeLeft));
            }
            else
            {
                powerPlayAdTimerRemainingTime.text = "0s";
            }
        }

        private void OnTimerCompleted()
        {
            powerPlayAdTimer.SetActive(false);
            getRV.SetActive(true);
        }
    }
}
