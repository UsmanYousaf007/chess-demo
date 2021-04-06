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
        public Image BlurBg;
        private bool gameEnded = false;

        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IBlurBackgroundService blurBackgroundService { get; set; }


        public void ShowEndGame()
        {
            blurBackgroundService.BlurBackground(BlurBg, 5, Colors.BLUR_BG_BRIGHTNESS_DARK);

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

            if ((playerWins || isDraw) && !gameEnded && isRankedGame)
            {
                StartEndAnimationSequence();
            }
            else
            {
                //ShowResultsDialog();
                //Invoke("ScaleInResultsDialog", animDelay);
                AnimateResultsDlg();
            }
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

        private void SetBlurBg(bool val)
        {
            BlurBg.enabled = true;
            BlurBg.gameObject.SetActive(val);
        }
    }
}
