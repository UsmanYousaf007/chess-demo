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
using TurboLabz.TLUtils;

namespace TurboLabz.Multiplayer
{
    public partial class GameView
    {
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IChessboardModel chessboardModel { get; set; }

        [Header("Exit")]
        public Button menuButton;
        public Button resignButton;
        public Button continueButton;
        public Button closeButton;

        public Signal menuButtonClickedSignal = new Signal();
        public Signal resignButtonClickedSignal = new Signal();
        public Signal continueButtonClickedSignal = new Signal();

        public GameObject gameMenu;

        public Text exitTitleLabel;
        public Text resignButtonLabel;
        public Text continueButtonLabel;

        public void InitMenu()
        {
            menuButton.onClick.AddListener(OnMenuButtonClicked);
            resignButton.onClick.AddListener(OnResignButtonClicked);
            continueButton.onClick.AddListener(OnContinueButtonClicked);
            closeButton.onClick.AddListener(OnContinueButtonClicked);

            exitTitleLabel.text = localizationService.Get(LocalizationKey.CPU_GAME_EXIT_DLG_TITLE);
            resignButtonLabel.text = localizationService.Get(LocalizationKey.CPU_GAME_RESIGN_BUTTON);
            continueButtonLabel.text = localizationService.Get(LocalizationKey.CPU_GAME_CONTINUE_BUTTON);
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
            EnableModalBlocker();
            gameMenu.SetActive(true);

            DisableMenuButton();
        }

        public void HideMenu()
        {
            DisableModalBlocker();
            gameMenu.SetActive(false);

            EnableMenuButton();
        }

        public void OnEscapeClicked()
        {
            if (appInfoModel.isReconnecting != DisconnectStates.FALSE)
            {
                return;
            }

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

        public void DisableMenuButton()
        {
            menuButton.interactable = false;
        }

        public void EnableMenuButton()
        {
            menuButton.interactable = true;
        }

        void OnMenuButtonClicked()
        {
            if (appInfoModel.isReconnecting != DisconnectStates.FALSE)
            {
                return;
            }

            if (matchInfoModel.activeChallengeId == null)
            {
                resultsDialogOpenedSignal.Dispatch();
            }
            else if (matchInfoModel.activeMatch == null || matchInfoModel.activeMatch.endGameResult != EndGameResult.NONE)
            {
                resultsDialogOpenedSignal.Dispatch();
            }
            else
            {
                menuButtonClickedSignal.Dispatch();
            }
        }

        void OnResignButtonClicked()
        {
            if ((appInfoModel.isReconnecting == DisconnectStates.FALSE) && !appInfoModel.syncInProgress)
            {
                analyticsService.Event(AnalyticsEventId.tap_resign_game);
                resignButtonClickedSignal.Dispatch();
                EnableModalBlocker();
            }
        }

        void OnContinueButtonClicked()
        {
            continueButtonClickedSignal.Dispatch();
        }
    }
}
