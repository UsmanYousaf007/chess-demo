/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using UnityEngine.UI;

using strange.extensions.mediation.impl;
using strange.extensions.signal.impl ;

using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using System;
using System.Collections;
using DG.Tweening;

namespace TurboLabz.InstantGame
{
    public class LobbyView : View
    {
        [Inject] public ILocalizationService localizationService { get; set; }

        [Inject] public IAudioService audioService { get; set; }

        // Scene references
        public Image cover;

        public Text inProgressLabel;
        public GameObject inProgressSticker;
        public GameObject setStrength;

        public Text strengthLabel;
        public Button decStrengthButton;
        public Text prevStrengthLabel;
        public Text currentStrengthLabel;
        public Text nextStrengthLabel;
        public Button incStrengthButton;

        public Button playMultiplayerButton;
        public Text playMultiplayerButtonLabel;

        public Button playFriendsButton;
        public Text playFriendsButtonLabel;
        public GameObject actionCounter;
        public Text actionCounterLabel;

        public Button playCPUButton;
        public Text playCPUButtonLabel;

        public GameObject adCounter;
        public Text adCounterLabel;
        public Text adBonusLabel;

        public InputField devFen;

        // View signals
        public Signal decStrengthButtonClickedSignal = new Signal();
        public Signal incStrengthButtonClickedSignal = new Signal();
        public Signal decDurationButtonClickedSignal = new Signal();
        public Signal incDurationButtonClickedSignal = new Signal();

        public Signal decPlayerColorButtonClickedSignal = new Signal();
        public Signal incPlayerColorButtonClickedSignal = new Signal();
		public Signal incThemeButtonClickedSignal = new Signal();
		public Signal decThemeButtonClickedSignal = new Signal();
        public Signal playCPUButtonClickedSignal = new Signal();
        public Signal playMultiplayerButtonClickedSignal = new Signal();
        public Signal playFriendsButtonClickedSignal = new Signal();
		public Signal themesButtonClickedSignal = new Signal();
        public Signal statsButtonClickedSignal = new Signal();

        public Signal<string> devFenValueChangedSignal = new Signal<string>();

        public void Init()
        {
            decStrengthButton.onClick.AddListener(OnDecStrengthButtonClicked);
            incStrengthButton.onClick.AddListener(OnIncStrengthButtonClicked);
            playMultiplayerButton.onClick.AddListener(OnPlayMultiplayerButtonClicked);
            playFriendsButton.onClick.AddListener(OnPlayFriendsButtonClicked);
            playCPUButton.onClick.AddListener(OnPlayCPUButtonClicked);
        
            devFen.onValueChanged.AddListener(OnDevFenValueChanged);

            strengthLabel.text = localizationService.Get(LocalizationKey.CPU_MENU_STRENGTH);
            inProgressLabel.text = localizationService.Get(LocalizationKey.CPU_MENU_IN_PROGRESS);
            playMultiplayerButtonLabel.text = localizationService.Get(LocalizationKey.CPU_MENU_PLAY_ONLINE);
            playFriendsButtonLabel.text = localizationService.Get(LocalizationKey.CPU_MENU_PLAY_FRIENDS);
            playCPUButtonLabel.text = localizationService.Get(LocalizationKey.CPU_MENU_PLAY_CPU);

			currentStrengthLabel.color = Colors.YELLOW;
			prevStrengthLabel.color = Colors.ColorAlpha (Colors.WHITE, Colors.DISABLED_TEXT_ALPHA);
			nextStrengthLabel.color = Colors.ColorAlpha (Colors.WHITE, Colors.DISABLED_TEXT_ALPHA);

            if (cover != null)
            {
                cover.gameObject.SetActive(true);
            }

            actionCounterLabel.text = "0";
        }

        public void CleanUp()
        {
            decStrengthButton.onClick.RemoveAllListeners();
            incStrengthButton.onClick.RemoveAllListeners();
            playMultiplayerButton.onClick.RemoveAllListeners();
            playFriendsButton.onClick.RemoveAllListeners();
            playCPUButton.onClick.RemoveAllListeners();
            devFen.onValueChanged.RemoveAllListeners();
        }

        public void UpdateView(LobbyVO vo)
        {
            UpdateStrength(vo);
			
            inProgressSticker.SetActive(vo.inProgress);
            setStrength.SetActive(!vo.inProgress);
		}


        public void UpdateStrength(LobbyVO vo)
        {
            int selectedStrength = vo.selectedStrength;
            int minStrength = vo.minStrength;
            int maxStrength = vo.maxStrength;

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

        public void Show()
        {
            gameObject.SetActive(true);

            // Clear up any texture memory that could be hanging post-game.
            Resources.UnloadUnusedAssets();

            if (cover != null)
            {
                DOTween.ToAlpha(()=> cover.color, x=> cover.color = x, 0f, 0.2f).OnComplete(DisableCover);
            }

            actionCounter.SetActive(actionCounterLabel.text != "0");

            if (Debug.isDebugBuild)
            {
                devFen.gameObject.SetActive(true);
            }
            else
            {
                devFen.gameObject.SetActive(false);
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);

            return;
        }

        public bool IsVisible()
        {
            return gameObject.activeSelf;
        }

        public void SetActionCount(int count)
        {
            actionCounter.SetActive(count > 0);
            actionCounterLabel.text = count.ToString();
        }

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


       
        private void OnPlayCPUButtonClicked()
        {
            playCPUButtonClickedSignal.Dispatch();
        }

        private void OnPlayMultiplayerButtonClicked()
        {
            playMultiplayerButtonClickedSignal.Dispatch();
        }

        private void OnPlayFriendsButtonClicked()
        {
            playFriendsButtonClickedSignal.Dispatch();
        }

        private void OnStatsButtonClicked()
        {
            statsButtonClickedSignal.Dispatch();
        }
            
        private void OnDevFenValueChanged(string fen)
        {
            devFenValueChangedSignal.Dispatch(fen);
        }

        private void DisableCover()
        {
            cover.gameObject.SetActive(false);
            cover = null;
        }


    }
}


