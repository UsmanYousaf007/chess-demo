using strange.extensions.signal.impl;
using TMPro;
using TurboLabz.InstantFramework;
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
        public Button rewardedVideoBtn;

        private StoreItem storeItem;
        private bool isPowerModeOn;

        //Models
        [Inject] public ISettingsModel settingsModel { get; set; }
        [Inject] public IStoreSettingsModel storeSettingsModel { get; set; }

        //Signals
        public Signal powerModeButtonClickedSignal = new Signal();
        public Signal powerplayCloseButtonSignal = new Signal();
        public Signal showPowerplayDlgButonSignal = new Signal();

        public void InitPowerplay()
        {
            powerPlayCloseButton.onClick.AddListener(OnPowerplayCloseButtonClicked);
            powerPlayOnBtn.onClick.AddListener(OnPowerPlayButtonClicked);
            showPowerplayDlgButton.onClick.AddListener(OnShowPowerplayDlgButtonClicked);
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
            SetupState(true);
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
            SetupState(isPowerModeOn);
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

        private void SetupState(bool powerModeEnabled)
        {
            powerPlayWithRVBtn.gameObject.SetActive(false);
            rewardedVideoBtn.gameObject.SetActive(false);

            powerPlayOnText.enabled = powerModeEnabled;
            powerPlayGemIcon.enabled = !powerModeEnabled;
            powerPlayGemCost.enabled = !powerModeEnabled;
            powerPlayTick.enabled = powerModeEnabled;
            powerPlayPlus.gameObject.SetActive(!powerModeEnabled);
            powerPlayGemCost.text = storeItem.currency3Cost.ToString();
            powerPlayOnBtn.interactable = !powerModeEnabled;
            isPowerModeOn = powerModeEnabled;
        }

        public void SetupPowerplayImage(bool powerplayEnabled)
        {
            powerplayImage.enabled = powerplayEnabled;
            powerplayImage.sprite = powerplayEnabled ? powerplayActiveSprite : powerplayInActiveSprite;
            isPowerModeOn = powerplayEnabled;
            showPowerplayDlgButton.gameObject.SetActive(false);
        }
    }
}
