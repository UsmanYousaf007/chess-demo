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
        public Image powerPlayPlus;
        public TMP_Text onText;
        public Image gemIcon;
        public TMP_Text gemCost;
        public string shortCode;
        public Button closeButton;

        //Models
        [Inject] public ILeaguesModel leaguesModel { get; set; }
        [Inject] public ITournamentsModel tournamentsModel { get; set; }
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
            gameObject.SetActive(true);
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

            SetupState(false);
        }

        void OnStartGameBtnClicked(string actionCode)
        {
            Debug.Log("OnQuickMatchBtnClicked");
            if (playerModel.coins >= betValue)
            {
                playMultiplayerButtonClickedSignal.Dispatch(actionCode, isPowerModeOn);
            }
            else
            {
                notEnoughCoinsSignal.Dispatch();
            }
        }

        void OnPowerPlayButtonClicked()
        {
            if (playerModel.gems >= storeItem.currency3Cost)
            {
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
            powerPlayPlus.enabled = !powerModeEnabled;
            gemCost.text = storeItem.currency3Cost.ToString();
            powerPlayOnBtn.interactable = !powerModeEnabled;
            isPowerModeOn = powerModeEnabled;
        }

        public void OnEnablePowerMode()
        {
            SetupState(true);
        }

        private void OnCloseButtonClicked()
        {
            audioService.PlayStandardClick();
            closeButtonSignal.Dispatch();
        }
    }
}
