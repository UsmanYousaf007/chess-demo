using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

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

        //Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }
        [Inject] public IStoreSettingsModel storeSettingsModel { get; set; }

        //Services
        [Inject] public IAudioService audioService { get; set; }

        //Signals
        public Signal powerModeButtonClickedSignal = new Signal();
        public Signal notEnoughGemsSignal = new Signal();
        public Signal closeButtonSignal = new Signal();
        public Signal<bool> conutinueButtonSignal = new Signal<bool>();

        private StoreItem storeItem;
        private bool isPowerModeOn;

        public void Init()
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
            powerPlayOnBtn.onClick.AddListener(OnPowerPlayButtonClicked);
            continueButton.onClick.AddListener(OnContinueButtonClicked);
        }

        public void Show()
        {
            UpdateView();
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void OnEnablePowerMode()
        {
            audioService.Play(audioService.sounds.SFX_REWARD_UNLOCKED);
            SetupState(true);
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

            freeHintsText.text = $"Get {settingsModel.powerModeFreeHints} Free Hints";
            SetupState(isPowerModeOn);
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
                powerModeButtonClickedSignal.Dispatch();
            }
            else
            {
                notEnoughGemsSignal.Dispatch();
            }
        }

        private void SetupState(bool powerModeEnabled)
        {
            onText.enabled = powerModeEnabled;
            gemIcon.enabled = !powerModeEnabled;
            gemCost.enabled = !powerModeEnabled;
            powerPlayTick.enabled = powerModeEnabled;
            powerPlayPlus.SetActive(!powerModeEnabled);
            gemCost.text = storeItem.currency3Cost.ToString();
            powerPlayOnBtn.interactable = !powerModeEnabled;
            isPowerModeOn = powerModeEnabled;
            powerPlayOnBtn.interactable = !powerModeEnabled;
        }
    }
}
