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

namespace TurboLabz.InstantFramework
{
    public partial class HomeView : View
    {
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IFacebookService facebookService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }

        [Inject] public LoadFriendsSignal loadFriendsSignal { get; set; }
        [Inject] public ClearCommunitySignal clearCommunitySignal { get; set; }
        [Inject] public NewFriendSignal newFriendSignal { get; set; }
        [Inject] public SearchFriendSignal searchFriendSignal { get; set; }
        [Inject] public RefreshCommunitySignal refreshCommunitySignal { get; set; }
        [Inject] public UpdatePlayerNotificationCountSignal updatePlayerNotificationCountSignal { get; set; }

        private SpritesContainer defaultAvatarContainer;

        public Button quickMatchBtn;
        public Text quickMatchTitleTxt;
        public Text quickMatchDescriptionTxt;

        public Button playFriendsMatchBtn;
        public Text playFriendsMatchTitleTxt;
        public Text playFriendsMatchDescriptionTxt;

        public Button playComputerMatchBtn;
        public Text playComputerMatchTitleTxt;
        public Text playComputerMatchDescriptionTxt;
        public Text playComputerLevelTxt;

        public GameObject uiBlocker;

        public Text onlinePlayersCountLabel;

        [Header("Choose computer difficulty dialog")]
        public GameObject chooseComputerDifficultyDlg;
        public Button decStrengthButton;
        public Text prevStrengthLabel;
        public Text easyLabel;
        public Text hardLabel;
        public Text currentStrengthLabel;
        public Text nextStrengthLabel;
        public Button incStrengthButton;
        public Button computerDifficultyDlgStartGameButton;
        public Button computerDifficultyDlgCloseButton;

        public Signal reloadFriendsSignal = new Signal();
        public Signal<string, bool> playButtonClickedSignal = new Signal<string, bool>();
        public Signal<int> actionCountUpdatedSignal = new Signal<int>();
        public Signal playCPUButtonClickedSignal = new Signal();
        public Signal playMultiplayerButtonClickedSignal = new Signal();
        public Signal decStrengthButtonClickedSignal = new Signal();
        public Signal incStrengthButtonClickedSignal = new Signal();

        private string eloPrefix;
        private bool isCPUGameInProgress;

        public void Init()
        {
            defaultAvatarContainer = SpritesContainer.Load(GSBackendKeys.DEFAULT_AVATAR_ALTAS_NAME);
            eloPrefix = localizationService.Get(LocalizationKey.ELO_SCORE);

            quickMatchTitleTxt.text = localizationService.Get(LocalizationKey.CPU_MENU_PLAY_ONLINE);
            playFriendsMatchTitleTxt.text = localizationService.Get(LocalizationKey.CPU_MENU_PLAY_FRIENDS);
            playComputerMatchTitleTxt.text = localizationService.Get(LocalizationKey.CPU_MENU_PLAY_CPU);

            playComputerMatchDescriptionTxt.text = localizationService.Get(LocalizationKey.CPU_MENU_SINGLE_PLAYER_GAME);

            quickMatchBtn.onClick.AddListener(OnQuickMatchBtnClicked);
            playComputerMatchBtn.onClick.AddListener(OnPlayComputerMatchBtnClicked);

            decStrengthButton.onClick.AddListener(OnDecStrengthButtonClicked);
            incStrengthButton.onClick.AddListener(OnIncStrengthButtonClicked);
            computerDifficultyDlgStartGameButton.onClick.AddListener(OnComputerDifficultyDlgStartGameClicked);
            computerDifficultyDlgCloseButton.onClick.AddListener(OnComputerDifficultyDlgCloseClicked);
            easyLabel.text = localizationService.Get(LocalizationKey.EASY);
            hardLabel.text = localizationService.Get(LocalizationKey.HARD);

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

        public void UpdateView(HomeVO vo)
        {
            if (uiBlocker.activeSelf)
            {
                uiBlocker.SetActive(false);
            }

            UpdateStrength(vo);
            if (vo.inProgress)
            {
                playComputerLevelTxt.gameObject.SetActive(true);
                playComputerLevelTxt.text = localizationService.Get(LocalizationKey.PLAYING_LEVEL ) + vo.selectedStrength;
               
            }
            else
            {
                playComputerLevelTxt.gameObject.SetActive(false);
            }

            onlinePlayersCountLabel.text = "Active Players " + vo.onlineCount.ToString();

    }

    public void UpdateStrength(HomeVO vo)
        {
            isCPUGameInProgress = vo.inProgress;
            int selectedStrength = vo.selectedStrength;
            int minStrength = vo.minStrength;
            int maxStrength = vo.maxStrength;

            UpdateStrength(selectedStrength,minStrength,maxStrength);
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

        void OnQuickMatchBtnClicked()
        {
            Debug.Log("OnQuickMatchBtnClicked");
            playMultiplayerButtonClickedSignal.Dispatch();
        }

        void OnPlayComputerMatchBtnClicked()
        {
            Debug.Log("OnPlayComputerMatchBtnClicked");
            if (!isCPUGameInProgress)
            {
                chooseComputerDifficultyDlg.SetActive(true);
            }
            else
            {
                playCPUButtonClickedSignal.Dispatch();
            }
        }

        void OnComputerDifficultyDlgCloseClicked()
        {
            chooseComputerDifficultyDlg.SetActive(false) ;
        }

        void OnComputerDifficultyDlgStartGameClicked()
        {
            playCPUButtonClickedSignal.Dispatch();
        }

        public void Show()
        {
            gameObject.SetActive(true);
            chooseComputerDifficultyDlg.SetActive(false);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public bool IsVisible()
        {
            return gameObject.activeSelf;
        }
    }
}
