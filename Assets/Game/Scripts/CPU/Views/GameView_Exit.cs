/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-06 17:37:45 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;
using UnityEngine.UI;

using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;

namespace TurboLabz.CPU
{
    public partial class GameView
    {
        public Button menuButton;
        public Button resignButton;
        public Button continueButton;
        public Button saveAndExitButton;
        public Button backToLobbyButton;
        public Button closeButton;

        public Signal menuButtonClickedSignal = new Signal();
        public Signal resignButtonClickedSignal = new Signal();
        public Signal continueButtonClickedSignal = new Signal();
        public Signal saveAndExitButtonClickedSignal = new Signal();
        public Signal returnToLobbySignal = new Signal();
        public Signal showResultsDlgSignal = new Signal();

        public GameObject gameMenu;

        public Text titleLabel;
        public Text resignButtonLabel;
        public Text continueButtonLabel;
        public Text saveAndExitButtonLabel;
        public Text backToLobbyButtonLabel;
        public Image backToLobbyButtonUnderline;

        private bool showAdOnBack;

        public void InitMenu()
        {
            UIDlgManager.Setup(gameMenu);

            menuButton.onClick.AddListener(OnMenuButtonClicked);
            resignButton.onClick.AddListener(OnResignButtonClicked);
            continueButton.onClick.AddListener(OnContinueButtonClicked);
            saveAndExitButton.onClick.AddListener(OnSaveAndExitButtonClicked);
            backToLobbyButton.onClick.AddListener(OnBackClicked);
            closeButton.onClick.AddListener(OnContinueButtonClicked);

            titleLabel.text = localizationService.Get(LocalizationKey.CPU_GAME_EXIT_DLG_TITLE);
            resignButtonLabel.text = localizationService.Get(LocalizationKey.CPU_GAME_RESIGN_BUTTON);
            continueButtonLabel.text = localizationService.Get(LocalizationKey.CPU_GAME_CONTINUE_BUTTON);
            saveAndExitButtonLabel.text = localizationService.Get(LocalizationKey.CPU_GAME_SAVE_AND_EXIT_CAP);
            backToLobbyButtonLabel.text = localizationService.Get(LocalizationKey.CPU_GAME_SAVE_AND_EXIT);

        }

        public void OnParentShowMenu()
        {
            HideMenu();
        }

        public void CleanupMenu() 
        {
            menuButton.onClick.RemoveAllListeners();
            continueButton.onClick.RemoveAllListeners();
            resignButton.onClick.RemoveAllListeners();
            closeButton.onClick.RemoveAllListeners();
        }

        public void ShowMenu()
        {
            UIDlgManager.Show(gameMenu);
            DisableMenuButton();
        }

        public void HideMenu()
        {
            UIDlgManager.Hide(gameMenu);
            EnableMenuButton();
        }

        public void OnEscapeClicked()
        {
            // Cannot escape the promo dialog or the results dialog
            if (IsPromoActive() || IsResultsDialogActive())
            {
                return;
            }

            if (gameMenu.activeSelf)
            {
                HideMenu();
            }
            else
            {
                OnMenuButtonClicked();
            }
        }

        public bool IsMenuButtonActive()
        {
            return gameMenu.activeSelf;
        }

        public void ToggleMenuButton(bool isPlayerTurn)
        {
            if (isPlayerTurn)
            {
                EnableMenuButton();
                RestoreStepButtons();
                //RestoreHindsightButton();
            }
            else
            {
                DisableMenuButton();
                StashStepButtons();
                //StashHindsightButton();
            }
        }

        public void DisableMenuButton()
        {
            menuButton.interactable = false;
            backToLobbyButton.interactable = false;
            backToLobbyButtonLabel.color = Colors.DISABLED_WHITE;
            backToLobbyButtonUnderline.color = Colors.DISABLED_WHITE;
        }

        public void EnableMenuButton()
        {
            menuButton.interactable = true;
            backToLobbyButton.interactable = true;
            backToLobbyButtonLabel.color = Colors.WHITE_150;
            backToLobbyButtonUnderline.color = Colors.WHITE_150;
        }

        void OnMenuButtonClicked()
        {
            audioService.PlayStandardClick();

            if (menuOpensResultsDlg)
            {
                showResultsDlgSignal.Dispatch();
            }
            else
            {
                menuButtonClickedSignal.Dispatch();
            }
        }

        void OnResignButtonClicked()
        {
            resignButtonClickedSignal.Dispatch();
        }

        void OnContinueButtonClicked()
        {
            continueButtonClickedSignal.Dispatch();
        }

        void OnBackClicked()
        {
            audioService.PlayStandardClick();
            if (menuOpensResultsDlg)
            {
                showResultsDlgSignal.Dispatch();
            }
            else
            {
                OnSaveAndExitButtonClicked();
            }
        }

        void OnSaveAndExitButtonClicked()
        {
            saveAndExitButtonClickedSignal.Dispatch();

            if (showAdOnBack)
            {
                ResultAdsVO vo = new ResultAdsVO();
                vo.adsType = AdType.Interstitial;
                vo.rewardType = GSBackendKeys.ClaimReward.NONE;
                vo.challengeId = "";
                vo.playerWins = false;
                vo.placementId = AdPlacements.Interstitial_endgame;
                playerModel.adContext = AnalyticsContext.interstitial_endgame;
                showAdSignal.Dispatch(vo, false);

                showAdOnBack = false;
            }
            else
            {
                returnToLobbySignal.Dispatch();
            }
        }
    }
}
