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
    public class CPUCardView : View
    {
        public TMP_Text subTitle;
        public TMP_Text chooseDifficultyText;

        public Button startGameButton;

        public TMP_Text easyLabel;
        public Button decStrengthButton;
        public TMP_Text prevStrengthLabel;

        public TMP_Text currentStrengthLabel;

        public TMP_Text hardLabel;
        public TMP_Text nextStrengthLabel;
        public Button incStrengthButton;

        public TMP_Text playComputerLevelTxt;
        public GameObject chooseDifficulty;

        private bool isCPUGameInProgress;

        //Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        public Signal decStrengthButtonClickedSignal = new Signal();
        public Signal incStrengthButtonClickedSignal = new Signal();
        public Signal playCPUButtonClickedSignal = new Signal();

        //Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }

        public void Init()
        {
            decStrengthButton.onClick.AddListener(OnDecStrengthButtonClicked);
            incStrengthButton.onClick.AddListener(OnIncStrengthButtonClicked);
            startGameButton.onClick.AddListener(OnStartGameButtonClicked);
            easyLabel.text = localizationService.Get(LocalizationKey.EASY);
            hardLabel.text = localizationService.Get(LocalizationKey.HARD);
        }

        public void UpdateView(LobbyVO vo)
        {
            UpdateStrength(vo);
            if (vo.inProgress)
            {
                playComputerLevelTxt.text = localizationService.Get(LocalizationKey.PLAYING_LEVEL) + vo.selectedStrength;
                playComputerLevelTxt.gameObject.SetActive(true);
                chooseDifficulty.SetActive(false);
            }
            else
            {
                playComputerLevelTxt.gameObject.SetActive(false);
                chooseDifficulty.SetActive(true);
            }
        }

        public void UpdateStrength(LobbyVO vo)
        {
            isCPUGameInProgress = vo.inProgress;
            int selectedStrength = vo.selectedStrength;
            int minStrength = vo.minStrength;
            int maxStrength = vo.maxStrength;

            UpdateStrength(selectedStrength, minStrength, maxStrength);
        }

        private void UpdateStrength(int selectedStrength, int minStrength, int maxStrength)
        {
            currentStrengthLabel.text = selectedStrength.ToString();

            if (selectedStrength == minStrength)
            {
                prevStrengthLabel.text = "";
                nextStrengthLabel.text = (selectedStrength + 1).ToString();
                incStrengthButton.interactable = true;
                decStrengthButton.interactable = false;
            }
            else if (selectedStrength == maxStrength)
            {
                prevStrengthLabel.text = (selectedStrength - 1).ToString();
                nextStrengthLabel.text = "";
                incStrengthButton.interactable = false;
                decStrengthButton.interactable = true;
            }
            else
            {
                prevStrengthLabel.text = (selectedStrength - 1).ToString();
                nextStrengthLabel.text = (selectedStrength + 1).ToString();
                incStrengthButton.interactable = true;
                decStrengthButton.interactable = true;
            }
        }

        //Button Listeners
        void OnDecStrengthButtonClicked()
        {
            decStrengthButtonClickedSignal.Dispatch();
            audioService.Play(audioService.sounds.SFX_STEP_CLICK);
        }

        private void OnIncStrengthButtonClicked()
        {
            incStrengthButtonClickedSignal.Dispatch();
            audioService.Play(audioService.sounds.SFX_STEP_CLICK);
        }

        void OnStartGameButtonClicked()
        {
            playCPUButtonClickedSignal.Dispatch();
        }
    }
}
