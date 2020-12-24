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

        //Models
        [Inject] public ILeaguesModel leaguesModel { get; set; }
        [Inject] public ITournamentsModel tournamentsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }

        //Signals
        public Signal<string, long> playMultiplayerButtonClickedSignal = new Signal<string, long>();

        //Services
        [Inject] public ILocalizationService localizationService { get; set; }

        LeagueTierIconsContainer leagueTierIconsContainer;

        public void Init()
        {
            leagueTierIconsContainer = leagueTierIconsContainer == null ? LeagueTierIconsContainer.Load() : leagueTierIconsContainer;

            startGame3mButton.onClick.AddListener(delegate { OnStartGameBtnClicked(FindMatchAction.ActionCode.Random3.ToString()); });
            startGame5mButton.onClick.AddListener(delegate { OnStartGameBtnClicked(FindMatchAction.ActionCode.Random.ToString()); });
            startGame10mButton.onClick.AddListener(delegate { OnStartGameBtnClicked(FindMatchAction.ActionCode.Random10.ToString()); });
            startGame30mButton.onClick.AddListener(delegate { OnStartGameBtnClicked(FindMatchAction.ActionCode.Random30.ToString()); });
            startGame3mText.text = localizationService.Get(LocalizationKey.MIN3_GAME_TEXT);
            startGame5mText.text = localizationService.Get(LocalizationKey.MIN5_GAME_TEXT);
            startGame10mText.text = localizationService.Get(LocalizationKey.MIN10_GAME_TEXT);
            startGame30mText.text = localizationService.Get(LocalizationKey.MIN30_GAME_TEXT);

            powerPlayOnBtn.onClick.AddListener(OnPowerPlayButtonClicked);
        }

        public void Show()
        {
            this.gameObject.SetActive(true);
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);
        }

        void OnStartGameBtnClicked(string actionCode)
        {
            Debug.Log("OnQuickMatchBtnClicked");
            /*if (playerModel.coins >= settingsModel.bettingIncrements[bettingIndex])
            {
                playMultiplayerButtonClickedSignal.Dispatch(actionCode, settingsModel.bettingIncrements[bettingIndex]);
            }*/
        }

        void OnPowerPlayButtonClicked()
        {
            //TODO set power play to true here
            powerPlayTick.enabled = true;
            powerPlayOnBtn.interactable = false;
            powerPlayPlus.enabled = false;
        }
    }
}
