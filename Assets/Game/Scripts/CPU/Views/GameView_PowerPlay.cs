using System;
using strange.extensions.signal.impl;
using TMPro;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using UnityEngine;
using UnityEngine.UI;

namespace TurboLabz.CPU
{
    public partial class GameView
    {
        [Header("Powerplay")]
        public GameObject powerPlayDlg;
        public Button powerPlayOnBtn;
        public Image powerPlayTick;
        public Image powerPlayPlus;
        public TMP_Text powerPlayOnText;
        public Image powerPlayGemIcon;
        public TMP_Text powerPlayGemCost;
        public string powerPlayShortCode;
        public Button powerPlayCloseButton;
        public TMP_Text powerPlayFreeHintsText;
        public Image powerplayImage;
        public Sprite powerplayActiveSprite;
        public Sprite powerplayInActiveSprite;
        public Button showPowerplayDlgButton;

        public Button powerPlayWithRVBtn;
        public Image powerPlayWithRVTick;
        public GameObject powerPlayPlusWithRV;
        public Image gemIconWithRv;
        public TMP_Text gemCostWithRv;

        public Button rewardedVideoBtn;
        public GameObject powerPlayAdTimer;
        public TMP_Text powerPlayAdTimerRemainingTime;
        public GameObject getRV;
        public GameObject timerRunningTooltip;
        public GameObject videoNotAvailableTooltip;

        public IServerClock serverClock;

        private StoreItem storeItem;
        private bool canSeeRewardedVideo;
        private long coolDownTimeUTC;
        private bool isTimerRunning;
        
        private bool isPowerModeOn;

        //Models
        [Inject] public ISettingsModel settingsModel { get; set; }
        [Inject] public IStoreSettingsModel storeSettingsModel { get; set; }

        //Signals
        public Signal powerModeButtonClickedSignal = new Signal();
        public Signal powerplayCloseButtonSignal = new Signal();
        public Signal showPowerplayDlgButonSignal = new Signal();
        public Signal<bool> schedulerSubscription = new Signal<bool>();
        public Signal<AdPlacements> showRewardedAdInGameSignal = new Signal<AdPlacements>();

        public void InitPowerplay()
        {
            powerPlayCloseButton.onClick.AddListener(OnPowerplayCloseButtonClicked);
            powerPlayOnBtn.onClick.AddListener(OnPowerPlayButtonClicked);
            powerPlayWithRVBtn.onClick.AddListener(OnPowerPlayButtonClicked);
            showPowerplayDlgButton.onClick.AddListener(OnShowPowerplayDlgButtonClicked);
            rewardedVideoBtn.onClick.AddListener(OnPlayRewardedVideoClicked);
        }

        public void ShowPowerplay()
        {
            EnableModalBlocker();
            UpdateView();
            powerPlayDlg.SetActive(true);
        }

        public void HidePowerplay()
        {
            DisableModalBlocker();
            powerPlayDlg.SetActive(false);
        }

        public void OnEnablePowerMode()
        {
            audioService.Play(audioService.sounds.SFX_REWARD_UNLOCKED);
            SetupState(true, false);
        }

        public void OnParentShowPowerplay()
        {
            HidePowerplay();
        }

        public void CleanupPowerplay()
        {
            powerPlayCloseButton.onClick.RemoveAllListeners();
            powerPlayOnBtn.onClick.RemoveAllListeners();
            showPowerplayDlgButton.onClick.RemoveAllListeners();
        }

        private void UpdateView()
        {
            if (storeItem == null && storeSettingsModel.items.ContainsKey(powerPlayShortCode))
            {
                storeItem = storeSettingsModel.items[powerPlayShortCode];
            }

            if (storeItem == null)
            {
                return;
            }

            powerPlayFreeHintsText.text = $"Get {settingsModel.powerModeFreeHints} Free Hints";
            canSeeRewardedVideo = playerModel.gems < adsSettingsModel.minGemsRequiredforRV && playerModel.rvUnlockTimestamp > 0
                && !(adsSettingsModel.removeRVOnPurchase && playerModel.HasPurchased());
            coolDownTimeUTC = playerModel.rvUnlockTimestamp;
            
            SetupState(isPowerModeOn, canSeeRewardedVideo);
            
        }

        private void OnPowerplayCloseButtonClicked()
        {
            audioService.PlayStandardClick();
            powerplayCloseButtonSignal.Dispatch();
        }

        private void OnShowPowerplayDlgButtonClicked()
        {
            audioService.PlayStandardClick();
            showPowerplayDlgButonSignal.Dispatch();
        }

        private void OnPowerPlayButtonClicked()
        {
            audioService.PlayStandardClick();
            if (playerModel.gems >= storeItem.currency3Cost)
            {
                powerModeButtonClickedSignal.Dispatch();
            }
            else
            {
                SpotPurchaseMediator.analyticsContext = "cpu_in_game_power_mode";
                notEnoughGemsSignal.Dispatch();
            }
        }

        private void SetupState(bool powerModeEnabled, bool canSeeRewardedVideo)
        {
            bool showRewardedVideoPanel = canSeeRewardedVideo && !powerModeEnabled;

            powerPlayOnBtn.gameObject.SetActive(!showRewardedVideoPanel);
            powerPlayWithRVBtn.gameObject.SetActive(showRewardedVideoPanel);
            rewardedVideoBtn.gameObject.SetActive(showRewardedVideoPanel);

            if (showRewardedVideoPanel)
            {
                powerPlayGemIcon.enabled = false;
                powerPlayGemCost.enabled = false;
                powerPlayOnText.enabled = false;

                powerPlayWithRVTick.enabled = powerModeEnabled;
                powerPlayPlusWithRV.SetActive(!powerModeEnabled);
                gemIconWithRv.enabled = !powerModeEnabled;
                gemCostWithRv.enabled = !powerModeEnabled;
                powerPlayWithRVBtn.interactable = !powerModeEnabled;
                timerRunningTooltip.SetActive(false);
                videoNotAvailableTooltip.SetActive(false);
                if (!IsCoolDownComplete() && !isTimerRunning)
                { StartTimer(coolDownTimeUTC); }
            }
            else
            {
                powerPlayOnText.enabled = powerModeEnabled;
                powerPlayGemIcon.enabled = !powerModeEnabled;
                powerPlayGemCost.enabled = !powerModeEnabled;
                powerPlayTick.enabled = powerModeEnabled;
                powerPlayPlus.gameObject.SetActive(!powerModeEnabled);
                powerPlayGemCost.text = storeItem.currency3Cost.ToString();
                powerPlayOnBtn.interactable = !powerModeEnabled;
            }
            isPowerModeOn = powerModeEnabled;
        }

        public void SetupPowerplayImage(bool powerplayEnabled)
        {
            powerplayImage.enabled = powerplayEnabled;
            powerplayImage.sprite = powerplayEnabled ? powerplayActiveSprite : powerplayInActiveSprite;
            isPowerModeOn = powerplayEnabled;
            showPowerplayDlgButton.gameObject.SetActive(false);
        }

        public void EnableVideoAvailabilityTooltip()
        {
            videoNotAvailableTooltip.SetActive(true);
            Invoke("DisableVideoAvailabilityTooltip", 5);
        }

        public void DisableVideoAvailabilityTooltip()
        {
            videoNotAvailableTooltip.SetActive(false);
        }

        public void EnableTimerTooltip()
        {
            timerRunningTooltip.SetActive(true);
            Invoke("DisableTimerTooltip", 5);
        }

        public void DisableTimerTooltip()
        {
            timerRunningTooltip.SetActive(false);
        }

        private void OnPlayRewardedVideoClicked()
        {
            if (IsCoolDownComplete())
            {
                showRewardedAdInGameSignal.Dispatch(AdPlacements.Rewarded_cpu_in_game_power_mode);
            }
            else
            {
                EnableTimerTooltip();
            }
        }

        public bool IsCoolDownComplete()
        {
            return coolDownTimeUTC < serverClock.currentTimestamp;
        }

        public void StartTimer(long coolDownTime = 0)
        {
            if (!isTimerRunning)
            {
                coolDownTimeUTC = coolDownTime;
                UpdateTimerText();
                powerPlayAdTimer.SetActive(true);
                getRV.SetActive(false);
                schedulerSubscription.Dispatch(true);
                isTimerRunning = true;
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
            long timeLeft = coolDownTimeUTC - serverClock.currentTimestamp;
            if (timeLeft > 0)
            {
                timeLeft -= 1000;
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
            isTimerRunning = false;
            rewardedVideoBtn.interactable = false;
            DisableTimerTooltip();
        }

        public void UpdatePowerModeRVTimer(long timer, bool rvEnabled)
        {
            canSeeRewardedVideo = rvEnabled;
            coolDownTimeUTC = timer;
            SetupState(isPowerModeOn, canSeeRewardedVideo);
        }
    }
}
