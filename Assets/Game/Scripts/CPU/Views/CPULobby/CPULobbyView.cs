/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-03 16:10:44 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;
using UnityEngine.UI;

using strange.extensions.mediation.impl;
using strange.extensions.signal.impl ;

using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using System;
using System.Collections;

namespace TurboLabz.InstantChess
{
    public class CPULobbyView : View
    {
        [Inject] public ILocalizationService localizationService { get; set; }

        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IShareService shareService { get; set; }

        // Scene references
        public Text strengthLabel;
        public Button decStrengthButton;
        public Text prevStrengthLabel;
        public Text currentStrengthLabel;
        public Text nextStrengthLabel;
        public Button incStrengthButton;

        public Text durationLabel;
        public Button decDurationButton;
        public Text currentDurationLabel;
        public GameObject infinityIcon;
        public Button incDurationButton;

        public Text playerColorLabel;
        public Button decPlayerColorButton;
        public GameObject whiteKing;
        public GameObject blackKing;
        public GameObject randomKing;
        public Button incPlayerColorButton;

        public Text themesLabel;
        public Button decThemeButton;
        public Text currentThemeLabel;
        public Button incThemeButton;

        public Button playButton;
        public Text playButtonLabel;

        public Button themesButton;
        public Text themesButtonLabel;

        public Button freeBucksButton;
        public Text freeBucksButtonLabel;

		public Button addBucksButton;
		public Text playerBucks;

        public GameObject freeBucksRewardDlg;
        public Button freeBucksRewardOkButton;
        public Text freeBucksRewardOkButtonLabel;
        public Text freeBucksRewardTitle;
        public Text freeBucksRewardAmount;

        public Button audioIsOnButton;
        public Button audioIsOffButton;
        public Button statsButton;
        public Button shareAppButton;

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
        public Signal playButtonClickedSignal = new Signal();
		public Signal themesButtonClickedSignal = new Signal();
        public Signal freeBucksButtonClickedSignal = new Signal();
        public Signal freeBucksRewardOkButtonClickedSignal = new Signal();
        public Signal statsButtonClickedSignal = new Signal();
        public Signal shareAppButtonClickedSignal = new Signal();
        public Signal<string> devFenValueChangedSignal = new Signal<string>();
        public Signal freeBucksUpdateAdsSignal = new Signal();
		public Signal addBucksButtonClickedSignal = new Signal();

        private Coroutine waitCR;
        private Coroutine waitForAdsAvailabilityCR;
        private const int ADS_AVAILABILITY_RETRY_SECONDS = 3;

        public void Init()
        {
            decStrengthButton.onClick.AddListener(OnDecStrengthButtonClicked);
            incStrengthButton.onClick.AddListener(OnIncStrengthButtonClicked);
            decDurationButton.onClick.AddListener(OnDecDurationButtonClicked);
            incDurationButton.onClick.AddListener(OnIncDurationButtonClicked);
            decPlayerColorButton.onClick.AddListener(OnDecPlayerColorButtonClicked);
            incPlayerColorButton.onClick.AddListener(OnIncPlayerColorButtonClicked);
			incThemeButton.onClick.AddListener(OnIncThemeButtonClicked);
			decThemeButton.onClick.AddListener(OnDecThemeButtonClicked);
            playButton.onClick.AddListener(OnPlayButtonClicked);
			themesButton.onClick.AddListener(OnThemesButtonClicked);
            freeBucksButton.onClick.AddListener(OnFreeBucksButtonClicked);
            statsButton.onClick.AddListener(OnStatsButtonClicked);
            audioIsOnButton.onClick.AddListener(OnAudioIsOnButtonClicked);
            audioIsOffButton.onClick.AddListener(OnAudioIsOffButtonClicked);
            shareAppButton.onClick.AddListener(OnShareAppButtonClicked);
            freeBucksRewardOkButton.onClick.AddListener(OnFreeBucksRewardOkButtonClicked);
			addBucksButton.onClick.AddListener(OnAddBucksButtonClicked);

            devFen.onValueChanged.AddListener(OnDevFenValueChanged);

            strengthLabel.text = localizationService.Get(LocalizationKey.CPU_MENU_STRENGTH);
            durationLabel.text = localizationService.Get(LocalizationKey.CPU_MENU_DURATION);
            playerColorLabel.text = localizationService.Get(LocalizationKey.CPU_MENU_PLAYER_COLOR);
			themesLabel.text = localizationService.Get(LocalizationKey.CPU_MENU_THEME);
            playButtonLabel.text = localizationService.Get(LocalizationKey.CPU_MENU_PLAY);
            themesButtonLabel.text = localizationService.Get(LocalizationKey.CPU_MENU_THEMES);
            freeBucksRewardOkButtonLabel.text = localizationService.Get(LocalizationKey.CPU_FREE_BUCKS_REWARD_OK);
            freeBucksRewardTitle.text = localizationService.Get(LocalizationKey.CPU_FREE_BUCKS_REWARD_TITLE);


			currentStrengthLabel.color = Colors.YELLOW;
			currentDurationLabel.color = Colors.YELLOW;
			currentThemeLabel.color = Colors.YELLOW;
			prevStrengthLabel.color = Colors.ColorAlpha (Colors.WHITE_TEXT, Colors.DISABLED_TEXT_ALPHA);
			nextStrengthLabel.color = Colors.ColorAlpha (Colors.WHITE_TEXT, Colors.DISABLED_TEXT_ALPHA);

            RefreshAudioButtons();
        }

        public void CleanUp()
        {
            decStrengthButton.onClick.RemoveAllListeners();
            incStrengthButton.onClick.RemoveAllListeners();
            decDurationButton.onClick.RemoveAllListeners();
            incDurationButton.onClick.RemoveAllListeners();
            decPlayerColorButton.onClick.RemoveAllListeners();
            incPlayerColorButton.onClick.RemoveAllListeners();
            playButton.onClick.RemoveAllListeners();
            statsButton.onClick.RemoveAllListeners();
            devFen.onValueChanged.RemoveAllListeners();

            audioIsOnButton.onClick.RemoveAllListeners();
            audioIsOffButton.onClick.RemoveAllListeners();
            shareAppButton.onClick.RemoveAllListeners();
        }

        public void UpdateView(CPULobbyVO vo)
        {
            UpdateStrength(vo);
            UpdateDuration(vo);
            UpdatePlayerColor(vo);
			UpdateTheme(vo);
			UpdatePlayerBucks(vo.playerBucks);
		}

        public void UpdateStrength(CPULobbyVO vo)
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

        public void UpdateDuration(CPULobbyVO vo)
        {
            int duration = vo.durationMinutes[vo.selectedDurationIndex];

            if (duration == 0)
            {
                infinityIcon.SetActive(true);
                currentDurationLabel.gameObject.SetActive(false);
            }
            else
            {
                infinityIcon.SetActive(false);
                currentDurationLabel.gameObject.SetActive(true);
                currentDurationLabel.text = 
                    localizationService.Get(LocalizationKey.GM_ROOM_DURATION, vo.durationMinutes[vo.selectedDurationIndex]);
            }

            if (vo.selectedDurationIndex == 0)
            {
                decDurationButton.interactable = false;
                incDurationButton.interactable = true;
            }
            else if (vo.selectedDurationIndex == (vo.durationMinutes.Length - 1))
            {
                decDurationButton.interactable = true;
                incDurationButton.interactable = false;
            }
            else
            {
                decDurationButton.interactable = true;
                incDurationButton.interactable = true;
            }
        }

        public void UpdatePlayerColor(CPULobbyVO vo)
        {
            randomKing.SetActive(false);
            whiteKing.SetActive(false);
            blackKing.SetActive(false);

            if (vo.selectedPlayerColorIndex == 0)
            {
                whiteKing.SetActive(true);
                incPlayerColorButton.interactable = true;
                decPlayerColorButton.interactable = false;
            }
            else if (vo.selectedPlayerColorIndex == 1)
            {
                blackKing.SetActive(true);
                incPlayerColorButton.interactable = true;
                decPlayerColorButton.interactable = true;
            }
            else if (vo.selectedPlayerColorIndex == 2)
            {
                randomKing.SetActive(true);
                incPlayerColorButton.interactable = false;
                decPlayerColorButton.interactable = true;
            }
        }

		public void UpdateTheme(CPULobbyVO vo)
		{
			currentThemeLabel.text = vo.activeSkinDisplayName;
		}

		public void UpdatePlayerBucks(int bucks)
		{
			playerBucks.text = bucks.ToString();
		}

        public void UpdateAds(AdsVO vo)
        {
            freeBucksButton.interactable = false;

            if (waitCR != null)
            {
                StopCoroutine(waitCR);
                waitCR = null;
            }

            if (vo.state == AdsState.AVAILABLE)
            {
                freeBucksButtonLabel.text = localizationService.Get(LocalizationKey.CPU_FREE_BUCKS_BUTTON_GET, vo.bucks);
                freeBucksButton.interactable = true;
            }
            else if (vo.state == AdsState.NOT_AVAILABLE)
            {
                freeBucksButtonLabel.text = localizationService.Get(LocalizationKey.CPU_FREE_BUCKS_BUTTON_NOT_AVAILABLE);
                waitForAdsAvailabilityCR = StartCoroutine(WaitForAdsAvailabilityCR());
            }
            else if (vo.state == AdsState.WAIT)
            {
                TimeSpan wait = TimeSpan.FromMilliseconds(vo.waitMs);
                waitCR = StartCoroutine(WaitCR(wait));
            }
        }

        IEnumerator WaitCR(TimeSpan wait)
        {
            string availableText = localizationService.Get(LocalizationKey.CPU_FREE_BUCKS_BUTTON_AVAILABLE) + " ";

            while (true)
            {
                if (wait.TotalSeconds <= 0)
                {
                    freeBucksUpdateAdsSignal.Dispatch();
                    waitCR = null;
                    yield break;
                }

                freeBucksButtonLabel.text = availableText +
                string.Format("{0:D2}:{1:D2}:{2:D2}", wait.Hours, wait.Minutes, wait.Seconds);

                yield return new WaitForSeconds(1);
                wait = wait.Subtract(TimeSpan.FromSeconds(1));
            }
        }

        IEnumerator WaitForAdsAvailabilityCR()
        {
            yield return new WaitForSeconds(ADS_AVAILABILITY_RETRY_SECONDS);
            freeBucksUpdateAdsSignal.Dispatch();
        }

        public void Show()
        {
            gameObject.SetActive(true);

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

            if (waitCR != null)
            {
                StopCoroutine(waitCR);
                waitCR = null;
            }

            if (waitForAdsAvailabilityCR != null)
            {
                StopCoroutine(waitForAdsAvailabilityCR);
                waitForAdsAvailabilityCR = null;
            }

            return;
        }

        public void ShowFreeBucksRewardDlg()
        {
            freeBucksRewardDlg.SetActive(true);
        }

        public void UpdateFreeBucksReward(int amt)
        {
            freeBucksRewardAmount.text = amt.ToString();
        }

        public void HideFreeBucksRewardDlg()
        {
            freeBucksRewardDlg.SetActive(false);
        }

        public bool IsVisible()
        {
            return gameObject.activeSelf;
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

        private void OnDecDurationButtonClicked()
        {
            decDurationButtonClickedSignal.Dispatch();
            audioService.Play(audioService.sounds.SFX_STEP_CLICK);
        }

        private void OnIncDurationButtonClicked()
        {
            incDurationButtonClickedSignal.Dispatch();
            audioService.Play(audioService.sounds.SFX_STEP_CLICK);
        }

        private void OnDecPlayerColorButtonClicked()
        {
            decPlayerColorButtonClickedSignal.Dispatch();
            audioService.Play(audioService.sounds.SFX_STEP_CLICK);
        }

        private void OnIncPlayerColorButtonClicked()
        {
            incPlayerColorButtonClickedSignal.Dispatch();
            audioService.Play(audioService.sounds.SFX_STEP_CLICK);
        }

		private void OnIncThemeButtonClicked()
		{
			incThemeButtonClickedSignal.Dispatch();
			audioService.Play(audioService.sounds.SFX_STEP_CLICK);
		}

		private void OnDecThemeButtonClicked()
		{
			decThemeButtonClickedSignal.Dispatch();
			audioService.Play(audioService.sounds.SFX_STEP_CLICK);
		}

        private void OnPlayButtonClicked()
        {
            playButtonClickedSignal.Dispatch();
        }

		private void OnThemesButtonClicked()
		{
			themesButtonClickedSignal.Dispatch();
		}

        private void OnFreeBucksButtonClicked()
        {
            freeBucksButtonClickedSignal.Dispatch();
        }

        private void OnFreeBucksRewardOkButtonClicked()
        {
            freeBucksRewardOkButtonClickedSignal.Dispatch();
        }

        private void OnStatsButtonClicked()
        {
            statsButtonClickedSignal.Dispatch();
        }

        private void OnAudioIsOnButtonClicked()
        {
            audioService.ToggleAudio(false);
            RefreshAudioButtons();
        }

        private void OnAudioIsOffButtonClicked()
        {
            audioService.ToggleAudio(true);
            audioService.PlayStandardClick();
            RefreshAudioButtons();
        }

        private void RefreshAudioButtons()
        {
            audioIsOnButton.gameObject.SetActive(audioService.IsAudioOn());
            audioIsOffButton.gameObject.SetActive(!audioService.IsAudioOn());
        }

        private void OnShareAppButtonClicked()
        {
            shareAppButtonClickedSignal.Dispatch();
        }

		private void OnAddBucksButtonClicked()
		{
			addBucksButtonClickedSignal.Dispatch();
		}

        private void OnDevFenValueChanged(string fen)
        {
            devFenValueChangedSignal.Dispatch(fen);
        }
    }
}
