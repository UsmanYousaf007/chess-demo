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

namespace TurboLabz.InstantFramework
{
    [CLSCompliant(false)]
    public class SelectTimeModeView : View
    {
        public Image BlurBg;

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

        //Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }
        [Inject] public IStoreSettingsModel storeSettingsModel { get; set; }

        //Signals
        public Signal<string, bool> playMultiplayerButtonClickedSignal = new Signal<string, bool>();
        public Signal powerModeButtonClickedSignal = new Signal();
        public Signal notEnoughCoinsSignal = new Signal();
        public Signal notEnoughGemsSignal = new Signal();
        public Signal closeButtonSignal = new Signal();

        //Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IBlurBackgroundService blurBackgroundService { get; set; }

        private StoreItem storeItem;
        private bool isPowerModeOn;
        private long betValue;

        public void Init()
        {
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
        }

        public void Show()
        {
            // Blur background and enable this dialog
            blurBackgroundService.BlurBackground(BlurBg, 6, Colors.BLUR_BG_BRIGHTNESS_NORMAL, gameObject);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void UpdateView(long betValue)
        {
            this.betValue = betValue;

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
                powerModeButtonClickedSignal.Dispatch();
            }
            else
            {
                notEnoughGemsSignal.Dispatch();
            }
        }

        void SetupState(bool powerModeEnabled)
        {
            onText.enabled = powerModeEnabled;
            gemIcon.enabled = !powerModeEnabled;
            gemCost.enabled = !powerModeEnabled;
            powerPlayTick.enabled = powerModeEnabled;
            powerPlayPlus.SetActive(!powerModeEnabled);
            gemCost.text = storeItem.currency3Cost.ToString();
            powerPlayOnBtn.interactable = !powerModeEnabled;
            isPowerModeOn = powerModeEnabled;
        }

        public void OnEnablePowerMode()
        {
            audioService.Play(audioService.sounds.SFX_REWARD_UNLOCKED);
            SetupState(true);
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

    }
}
