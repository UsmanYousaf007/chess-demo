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
        [Header("Rewards Dialog")]
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

        public WinResultAnimSequence _winAnimationSequence;
        private bool animationPlayed = false;

        private float rewardsDialogHalfHeight;
        private float animRewardsDelay;
        private const float REWARDS_DIALOG_DURATION = 0.5f;
        private const float REWARDS_DELAY_TIME = 1f;
        private const float REWARDS_TO_RESULTS_DELAY_TIME = 1f;

        #region Button Listeners

        #endregion

        public void ShowRewardsDialog()
        {
            rewardsDialog.SetActive(true);
            Invoke("AnimateRewardsDialog", animRewardsDelay);
        }

        #region Setup

        public void StartGameEndFlow()
        {
            EnableModalBlocker(Colors.UI_BLOCKER_DARK_ALPHA);

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
        }

        private void SetupRewardsLayout()
        {
            rewardsBetReversedImg.gameObject.SetActive(isDraw && isRankedGame);
            rewardsVictoryRewardImg.gameObject.SetActive(playerWins && isRankedGame);

            titleTxt.text = (isDraw && isRankedGame)? "Draw" : "You Win";

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

            animRewardsDelay = REWARDS_DELAY_TIME;

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
        private void OnMirrorPanelAnimationComplete()
        {
            if (playerWins && !animationPlayed && isRankedGame)
            {
                _winAnimationSequence.PlayAnimation().Then(() => OnAnimationRewardsComplete());
                animationPlayed = true;
            }
        }

        private void OnAnimationRewardsComplete()
        {
            animationPlayed = true;
            rewardsDialog.SetActive(false);
            Invoke("ShowResultsDialog", REWARDS_TO_RESULTS_DELAY_TIME);
        }

        private void AnimateRewardsDialog()
        {
            mirrorPanel.transform.DOLocalMove(Vector3.zero, REWARDS_DIALOG_DURATION).SetEase(Ease.OutBack).OnComplete(OnMirrorPanelAnimationComplete);

            if (isDraw || !playerWins)
            {
                audioService.Play(audioService.sounds.SFX_DEFEAT);
            }
            else
            {
                audioService.Play(audioService.sounds.SFX_VICTORY);
            }
        }

        #endregion


    }
}
