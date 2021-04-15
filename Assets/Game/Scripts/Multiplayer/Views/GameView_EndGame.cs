/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential


using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;
using TurboLabz.Chess;
using TurboLabz.InstantGame;
using strange.extensions.promise.api;
using HUFEXT.CrossPromo.Runtime.API;
using TMPro;
using System.Collections.Generic;

namespace TurboLabz.Multiplayer
{
    public partial class GameView
    {
        [Header("End Game")]
        private bool gameEnded = false;

        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IBlurBackgroundService blurBackgroundService { get; set; }

        public Signal<string, bool, float> showWeeklyChampionshipResultsSignal = new Signal<string, bool, float>();


        public void ShowEndGame()
        {
            resultsCanvasGroup.alpha = 1;
            rewardsCanvasGroup.alpha = 1;

            HidePossibleMoves();
            HideOpponentConnectionMonitor();

            if (!ArePlayerMoveIndicatorsVisible())
            {
                HidePlayerToIndicator();
            }

            HideSafeMoveBorder();
            ShowViewBoardResultsPanel(false);

            preferencesModel.isRateAppDialogueShown = false;
            appInfoModel.gameMode = GameMode.NONE;

            UIDlgManager.Show(gameEndDlgContainer, Colors.BLUR_BG_BRIGHTNESS_DARK);

            gameEnded = true;
        }

        public void UpdateEndGame()
        {
            gameEnded = false;
        }

        public void ShowViewBoardResultsPanel(bool show)
        {
            viewBoardResultPanel.gameObject.SetActive(show);
        }

        public void HideGameEndDialog()
        {
            UIDlgManager.Hide(gameEndDlgContainer);
            HideResultsDialog();
            HideRewardsDialog();
        }

        private void ShowWeeklyChampionshipResults()
        {
            showWeeklyChampionshipResultsSignal.Dispatch(challengeId, playerWins, TRANSITION_DURATION);
        }
    }
}
