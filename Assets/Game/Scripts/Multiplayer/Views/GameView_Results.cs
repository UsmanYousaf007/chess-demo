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

namespace TurboLabz.Multiplayer
{
    public partial class GameView
    {
        [Header("Declined Dialog")]
        public GameObject declinedDialog;
        public Text declinedHeading;
        public Text declinedReason;
        public Button declinedLobbyButton;
        public Text declinedLobbyButtonLabel;

		[Header("Results Dialog")]
        public GameObject resultsDialog;
        public Text resultsGameResultLabel;
        public Text resultsGameResultReasonLabel;
        public Text resultsFriendlyLabel;

        public Text resultsRatingTitleLabel;
        public Text resultsRatingValueLabel;
        public Text resultsRatingChangeLabel;

        public Button resultsCollectRewardButton;
        public Text resultsCollectRewardButtonLabel;
        public Button resultsCloseButton;
        public Text resultsCloseButtonLabel;

        public Image resultsAdTVImage;
        public Text resultsRewardCoinsLabel;
        public Image resultsVictoryRewardImage;
        public Image resultsDefeatRewardImage;
        public Text resultsEarnedLabel;

        public Button resultsSkipRewardButton;
        public Text resultsSkipRewardButtonLabel;

        public Signal resultsStatsButtonClickedSignal = new Signal();
        public Signal showAdButtonClickedSignal = new Signal();
        public Signal resultsDialogClosedSignal = new Signal();
        public Signal resultsDialogOpenedSignal = new Signal();
        public Signal backToLobbySignal = new Signal();
        public Signal backToFriendsSignal = new Signal();

        private const float RESULTS_DELAY_TIME = 1f;
        private const float RESULTS_SHORT_DELAY_TIME = 0.3f;
        private const float RESULTS_DIALOG_DURATION = 0.5f;
        private float resultsDialogHalfHeight;
        private float declinedDialogHalfHeight;

        private bool playerWins;
        private bool isDraw;
        private string adRewardType;
        private float animDelay;

        public void InitResults()
        {
            // Button listeners
            resultsCollectRewardButton.onClick.AddListener(OnResultsCollectRewardButtonClicked);
            declinedLobbyButton.onClick.AddListener(OnResultsCollectRewardButtonClicked);
            resultsCloseButton.onClick.AddListener(OnResultsClosed);
            resultsSkipRewardButton.onClick.AddListener(OnResultsSkipRewardButtonClicked);

            // Text Labels
            resultsCollectRewardButtonLabel.text = localizationService.Get(LocalizationKey.RESULTS_COLLECT_REWARD_BUTTON);
            resultsCloseButtonLabel.text = localizationService.Get(LocalizationKey.RESULTS_CLOSE_BUTTON);
            resultsRatingTitleLabel.text = localizationService.Get(LocalizationKey.ELO_SCORE);
            resultsFriendlyLabel.text = localizationService.Get(LocalizationKey.FRIENDLY_GAME_CAPTION);
            resultsEarnedLabel.text = localizationService.Get(LocalizationKey.RESULTS_EARNED);
            resultsSkipRewardButtonLabel.text = localizationService.Get(LocalizationKey.RESULTS_SKIP_REWARD_BUTTON);

            declinedHeading.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_HEADING_DECLINED);
            declinedReason.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_PLAYER_DECLINED);
            declinedLobbyButtonLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_RESULTS_BACK);
		
            resultsDialogHalfHeight = resultsDialog.GetComponent<RectTransform>().rect.height / 2f;
            declinedDialogHalfHeight = declinedDialog.GetComponent<RectTransform>().rect.height / 2f;
        }

        public void CleanupResults()
        {
            resultsCollectRewardButton.onClick.RemoveAllListeners();
            declinedLobbyButton.onClick.RemoveAllListeners();
            resultsCloseButton.onClick.RemoveAllListeners();
            resultsSkipRewardButton.onClick.RemoveAllListeners();
        }

        public void OnParentShowResults()
        {
            HideResultsDialog();
        }

        public void ShowResultsDialog()
        {
            EnableModalBlocker();
            resultsDialog.SetActive(true);

            HidePossibleMoves();

            if (!ArePlayerMoveIndicatorsVisible())
            {
                HidePlayerToIndicator();
            }
        }

        public void HideResultsDialog()
        {
            resultsDialog.SetActive(false);
            declinedDialog.SetActive(false);
        }

        private void UpdateResultRatingSection(bool isRanked, int currentEloScore, int eloScoreDelta)
        {
            resultsFriendlyLabel.gameObject.SetActive(false);
            resultsRatingTitleLabel.gameObject.SetActive(true);
            resultsRatingValueLabel.gameObject.SetActive(false);
            resultsRatingChangeLabel.gameObject.SetActive(false);

            if (!isRanked)
            {
                resultsFriendlyLabel.gameObject.SetActive(true);
                return;
            }

            // Ranked Game
            resultsRatingValueLabel.gameObject.SetActive(true);
            resultsRatingChangeLabel.gameObject.SetActive(true);
            resultsRatingChangeLabel.gameObject.SetActive(false);

            resultsRatingValueLabel.text = currentEloScore.ToString();

            if (eloScoreDelta > 0)
            {
                resultsRatingChangeLabel.text = "(+" + eloScoreDelta + ")";
                resultsRatingChangeLabel.color = Colors.GREEN;
            }
            else if (eloScoreDelta < 0)
            {
                resultsRatingChangeLabel.text = "(" + eloScoreDelta + ")";
                resultsRatingChangeLabel.color = Colors.RED;
            }
        }

        private void UpdateGameEndReasonSection(GameEndReason gameEndReason)
        {
            switch (gameEndReason)
            {
                case GameEndReason.TIMER_EXPIRED:
                    resultsGameResultReasonLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_TIMER_EXPIRED);
                    break;

                case GameEndReason.CHECKMATE:
                    resultsGameResultReasonLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_CHECKMATE);
                    break;

                case GameEndReason.RESIGNATION:
                    if (!playerWins)
                    {
                        resultsGameResultReasonLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_RESIGNATION_PLAYER);
                        animDelay = RESULTS_SHORT_DELAY_TIME;
                    }
                    else
                    {
                        resultsGameResultReasonLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_RESIGNATION_OPPONENT);
                    }
                    break;

                case GameEndReason.STALEMATE:
                    isDraw = true;
                    resultsGameResultReasonLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_STALEMATE);
                    break;

                case GameEndReason.DRAW_BY_INSUFFICIENT_MATERIAL:
                    isDraw = true;
                    resultsGameResultReasonLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_DRAW_BY_INSUFFICIENT_MATERIAL);
                    break;

                case GameEndReason.DRAW_BY_FIFTY_MOVE_RULE_WITH_MOVE: 
                case GameEndReason.DRAW_BY_FIFTY_MOVE_RULE_WITHOUT_MOVE:
                    isDraw = true;
                    resultsGameResultReasonLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_DRAW_BY_FIFTY_MOVE_RULE);
                    break;

                case GameEndReason.DRAW_BY_THREEFOLD_REPEAT_RULE_WITH_MOVE:
                case GameEndReason.DRAW_BY_THREEFOLD_REPEAT_RULE_WITHOUT_MOVE:
                    isDraw = true;
                    resultsGameResultReasonLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_DRAW_BY_THREEFOLD_REPEAT_RULE);
                    break;

                case GameEndReason.PLAYER_DISCONNECTED:
                    resultsGameResultReasonLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_PLAYER_DISCONNECTED);
                    break;

                default:
                    resultsGameResultReasonLabel.text = "Unknown Reason";
                    break;
            }
        }

        private void UpdateGameResultHeadingSection()
        {
            
            if (isDraw)
            {
                resultsGameResultLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_HEADING_DRAW);
                resultsGameResultLabel.color = Colors.YELLOW;
            }
            else
            {
                if (playerWins)
                {
                    resultsGameResultLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_HEADING_WIN);
                    resultsGameResultLabel.color = Colors.GREEN;
                }
                else
                {
                    resultsGameResultLabel.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_HEADING_LOSE);
                    resultsGameResultLabel.color = Colors.RED;
                }
            }
        }

        public void UpdateResultsDialog(ResultsVO vo)
        {
            DisableInteraction();
            EnableModalBlocker();

            if (vo.reason == GameEndReason.DECLINED)
            {
                HandleDeclinedDialog();
                return;
            }

            playerWins = vo.playerWins;
            isDraw = false;
            animDelay = RESULTS_DELAY_TIME;
            GameEndReason gameEndReason = vo.reason;

            UpdateResultRatingSection(vo.isRanked, vo.currentEloScore, vo.eloScoreDelta);
            UpdateGameEndReasonSection(vo.reason);
            UpdateGameResultHeadingSection();

            resultsDialog.transform.localPosition = new Vector3(0f, Screen.height + resultsDialogHalfHeight, 0f);
            Invoke("AnimateResultsDialog", animDelay);

            // TODO: move this call to the clock partial class
            if (gameEndReason == GameEndReason.TIMER_EXPIRED)
            {
                if (playerWins)
                {
                    ExpireOpponentTimer();
                }
                else
                {
                    ExpirePlayerTimer();
                }
            }

            resultsVictoryRewardImage.gameObject.SetActive(playerWins);
            resultsDefeatRewardImage.gameObject.SetActive(!playerWins);
            resultsAdTVImage.gameObject.SetActive(!vo.removeAds);

            // Reward
            resultsRewardCoinsLabel.text = "+" + vo.rewardCoins;
            adRewardType = vo.adRewardType;
        }

        private void AnimateResultsDialog()
        {
            resultsDialog.transform.DOLocalMove(Vector3.zero, RESULTS_DIALOG_DURATION).SetEase(Ease.OutBack);

            if (isDraw || !playerWins)
            {
                audioService.Play(audioService.sounds.SFX_DEFEAT);
            }
            else
            {
                audioService.Play(audioService.sounds.SFX_VICTORY);
            }
        }

        public bool IsResultsDialogVisible()
        {
            return resultsDialog.activeSelf;
        }

        private void HandleDeclinedDialog()
        {
            resultsDialog.SetActive(false);
            declinedDialog.SetActive(true);
            declinedDialog.transform.localPosition = new Vector3(0f, Screen.height + declinedDialogHalfHeight, 0f);
            Invoke("AnimateDeclinedDialog", RESULTS_SHORT_DELAY_TIME);
        }

        private void AnimateDeclinedDialog()
        {
            declinedDialog.transform.DOLocalMove(Vector3.zero, RESULTS_DIALOG_DURATION).SetEase(Ease.OutBack);
            audioService.Play(audioService.sounds.SFX_DEFEAT);
        }

        private bool IsResultsDialogActive()
        {
            return resultsDialog.activeSelf;
        }
			
        private void OnResultsCollectRewardButtonClicked()
        {
            showAdSignal.Dispatch(AdType.RewardedVideo, adRewardType);

            if (isLongPlay)
            {
                backToFriendsSignal.Dispatch();
            }
            else
            {
                backToLobbySignal.Dispatch();
            }
        }

        private void OnResultsClosed()
        {
            uiBlocker.SetActive(false);
            resultsDialogClosedSignal.Dispatch();
        }

        public void OnResultsSkipRewardButtonClicked()
        {
            showAdSignal.Dispatch(AdType.Interstitial, GSBackendKeys.ClaimReward.NONE);

            if (isLongPlay)
            {
                backToFriendsSignal.Dispatch();
            }
            else
            {
                backToLobbySignal.Dispatch();
            }
        }
    }
}
