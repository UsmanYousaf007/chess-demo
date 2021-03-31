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

namespace TurboLabz.Multiplayer
{
    public partial class GameView
    {
        [Header("End Game Rewards Dialog")]
        public GameObject rewardsDialog;
        public GameObject mirrorPanel;

        public Image rewardsVictoryRewardImg;
        public Image rewardsBetReversedImg;

        public TMP_Text titleTxt;

        public Text rewardsBetReversedLabel;
        public Text rewardsEarnedCoinsLabel;
        public Text rewardsEarnedStarsLabel;
        public GameObject rewardsCoins;
        public GameObject rewardsStars;

        public Image rewardsPowerplayImage;
        public Sprite rewardsPowerPlayOnSprite;
        public Sprite rewardsPowerPlayOffSprite;

        public CanvasGroup rewardsCanvasGroup;

        public void ShowRewardsDialog()
        {
            if (!isRankedGame || (isRankedGame && !playerWins && !isDraw))
                return;
            //BlurBg.enabled = true;
            rewardsDialog.SetActive(true);
            //Invoke("AnimateRewardsDialog", REWARDS_DELAY_TIME);
        }

        #region Setup

        /*public void StartGameEndFlow()
        {
            //EnableModalBlocker(Colors.UI_BLOCKER_DARK_ALPHA);
            //BlurBg.enabled = true;
            HidePossibleMoves();
            HideOpponentConnectionMonitor();

            if (!isRankedGame || (isRankedGame && !playerWins && !isDraw))
            {
                ShowResultsDialog();
            }
            else
            {
                ShowRewardsDialog();
            }

            if (!ArePlayerMoveIndicatorsVisible())
            {
                HidePlayerToIndicator();
            }

            HideSafeMoveBorder();
            ShowViewBoardResultsPanel(false);

            preferencesModel.isRateAppDialogueShown = false;
            appInfoModel.gameMode = GameMode.NONE;
        }*/

        private void SetupRewardsLayout()
        {
            rewardsBetReversedImg.gameObject.SetActive(isDraw && isRankedGame);
            rewardsVictoryRewardImg.gameObject.SetActive(playerWins && isRankedGame);

            titleTxt.text = (isDraw && isRankedGame)? "DRAW" : "YOU WIN";

            rewardsCoins.gameObject.SetActive(isDraw || playerWins && isRankedGame);
            rewardsStars.gameObject.SetActive(playerWins && isRankedGame);

            rewardsPowerplayImage.gameObject.SetActive(playerWins && isRankedGame);
        }

        private void UpdateRewards(bool powerMode)
        {
            rewardsPowerplayImage.enabled = powerMode;
            rewardsPowerplayImage.sprite = powerMode ? powerPlayOnSprite : powerPlayOffSprite;
        }

        public void UpdateRewardsDialog(ResultsVO vo)
        {
            if (!playerWins && isRankedGame && !isDraw)
                return;

            if (!animationPlayed)
            {
                var coinsRewarded = playerWins ? vo.betValue * vo.coinsMultiplyer : vo.betValue;
                _winAnimationSequence.Reset((long)coinsRewarded, vo.earnedStars, vo.powerMode == true ? vo.earnedStars : 0, playerWins, vo.isRanked);
            }

            mirrorPanel.transform.localPosition = new Vector3(0f, Screen.height + resultsDialogHalfHeight, 0f);
            SetupRewardsLayout();
            UpdateRewards(vo.powerMode);
        }
        #endregion

        #region Animations
        

        #endregion


    }
}
