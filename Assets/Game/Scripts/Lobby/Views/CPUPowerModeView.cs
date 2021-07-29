using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class CPUPowerModeView : View
    {
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
        public Button continueButton;

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

        private bool canSeeRewardedVideo;
        private long coolDownTimeUTC;
        private bool isTimerRunning;
        public IServerClock serverClock;

        //Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }
        [Inject] public IStoreSettingsModel storeSettingsModel { get; set; }
        [Inject] public IAdsSettingsModel adsSettingsModel { get; set; }

        //Services
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }

        //Signals
        public Signal powerModeButtonClickedSignal = new Signal();
        public Signal notEnoughGemsSignal = new Signal();
        public Signal closeButtonSignal = new Signal();
        public Signal<bool> conutinueButtonSignal = new Signal<bool>();
        public Signal<bool> schedulerSubscription = new Signal<bool>();
        public Signal<AdPlacements> showRewardedAdSignal = new Signal<AdPlacements>();

        private StoreItem storeItem;
        private bool isPowerModeOn;

        public void Init()
        {
            UIDlgManager.Setup(gameObject);

            closeButton.onClick.AddListener(OnCloseButtonClicked);
            powerPlayOnBtn.onClick.AddListener(OnPowerPlayButtonClicked);
            powerPlayWithRVBtn.onClick.AddListener(OnPowerPlayButtonClicked);
            continueButton.onClick.AddListener(OnContinueButtonClicked);
            rewardedVideoBtn.onClick.AddListener(OnPlayRewardedVideoClicked);
        }

        public void Show()
        {  
            UpdateView();
            UIDlgManager.Show(gameObject);
        }

        public void Hide()
        {
            UIDlgManager.Hide(gameObject);
        }

        public void OnEnablePowerMode()
        {
            audioService.Play(audioService.sounds.SFX_REWARD_UNLOCKED);
            SetupState(true, canSeeRewardedVideo);
            analyticsService.Event(AnalyticsEventId.consumable_used, AnalyticsContext.cpu_pre_game_power_mode);
        }

        private void UpdateView()
        {
            if (storeItem == null && storeSettingsModel.items.ContainsKey(shortCode))
            {
                storeItem = storeSettingsModel.items[shortCode];
            }

            if (storeItem == null)
            {
                return;
            }
            canSeeRewardedVideo =  playerModel.gems < adsSettingsModel.minGemsRequiredforRV && playerModel.rvUnlockTimestamp > 0
                && !(adsSettingsModel.removeRVOnPurchase && playerModel.HasPurchased());
            coolDownTimeUTC = playerModel.rvUnlockTimestamp;
            freeHintsText.text = $"Get {settingsModel.powerModeFreeHints} Free Hints";
            SetupState(isPowerModeOn, canSeeRewardedVideo);
        }

        private void OnContinueButtonClicked()
        {
            audioService.PlayStandardClick();
            conutinueButtonSignal.Dispatch(isPowerModeOn);
            isPowerModeOn = false;
        }

        private void OnCloseButtonClicked()
        {
            audioService.PlayStandardClick();
            closeButtonSignal.Dispatch();
        }

        private void OnPowerPlayButtonClicked()
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
            bool showRewardedVideoPanel = canSeeRV && !powerModeEnabled;

            powerPlayOnBtn.gameObject.SetActive(!showRewardedVideoPanel);
            powerPlayWithRVBtn.gameObject.SetActive(showRewardedVideoPanel);
            rewardedVideoBtn.gameObject.SetActive(showRewardedVideoPanel);

            if (showRewardedVideoPanel)
            {
                gemIcon.enabled = false;
                gemCost.enabled = false;

                bool isCoolDownComplete = IsCoolDownComplete();
                powerPlayWithRVTick.enabled = powerModeEnabled;
                powerPlayPlusWithRV.SetActive(!powerModeEnabled);
                gemIconWithRv.enabled = !powerModeEnabled;
                gemCostWithRv.enabled = !powerModeEnabled;
                powerPlayWithRVBtn.interactable = !powerModeEnabled;
                timerRunningTooltip.SetActive(false);
                videoNotAvailableTooltip.SetActive(false);
                if (!isCoolDownComplete && !isTimerRunning)
                { StartTimer(coolDownTimeUTC); }
            }
            else
            {
                powerPlayTick.enabled = powerModeEnabled;
                powerPlayPlus.SetActive(!powerModeEnabled);
                onText.enabled = powerModeEnabled;
                gemIcon.enabled = !powerModeEnabled;
                gemCost.enabled = !powerModeEnabled;
                gemCost.text = storeItem.currency3Cost.ToString();
                powerPlayOnBtn.interactable = !powerModeEnabled;
            }

            isPowerModeOn = powerModeEnabled;

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
                showRewardedAdSignal.Dispatch(AdPlacements.Rewarded_cpu_pregame_power_mode);
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

        public void UpdateRVTimer(long timer, bool rvEnabled)
        {
            canSeeRewardedVideo = rvEnabled;
            coolDownTimeUTC = timer;
            SetupState(isPowerModeOn, canSeeRewardedVideo);
        }
    }
}
