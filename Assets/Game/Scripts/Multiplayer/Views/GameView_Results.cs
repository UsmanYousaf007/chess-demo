/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using strange.extensions.signal.impl;

using TurboLabz.TLUtils;
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
        public Text resultsDialogHeading;
        public Text resultsDialogReason;
		public Button resultsExitButton;
        public Text resultsExitButtonLabel;
        public Button resultsCloseButton;
        public Text resultsCloseButtonLabel;

        public Text ratingLabel;
        public Text ratingValue;
        public Text ratingDelta;
        public Text friendlyCaption;

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

        public void InitResults()
        {
            resultsExitButton.onClick.AddListener(OnResultsExitButtonClicked);
            declinedLobbyButton.onClick.AddListener(OnResultsExitButtonClicked);

            resultsCloseButton.onClick.AddListener(OnResultsClosed);
		
            resultsCloseButtonLabel.text = localizationService.Get(LocalizationKey.CPU_RESULTS_CLOSE_BUTTON);
            ratingLabel.text = localizationService.Get(LocalizationKey.ELO_SCORE);
            friendlyCaption.text = localizationService.Get(LocalizationKey.FRIENDLY_GAME_CAPTION);

            declinedHeading.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_HEADING_DECLINED);
            declinedReason.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_PLAYER_DECLINED);
            declinedLobbyButtonLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_RESULTS_BACK);
		
            resultsDialogHalfHeight = resultsDialog.GetComponent<RectTransform>().rect.height / 2f;
            declinedDialogHalfHeight = declinedDialog.GetComponent<RectTransform>().rect.height / 2f;

        }

        public void CleanupResults()
        {
            resultsCloseButton.onClick.RemoveAllListeners();
            resultsExitButton.onClick.RemoveAllListeners();
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

        public void UpdateResultsDialog(ResultsVO vo)
        {
            DisableInteraction();
            EnableModalBlocker();

            if (vo.reason == GameEndReason.DECLINED)
            {
                HandleDeclinedDialog();
                return;
            }

            if (isLongPlay)
            {
                resultsExitButtonLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_RESULTS_BACK).ToUpper();
            }
            else
            {
                resultsExitButtonLabel.text = localizationService.Get(LocalizationKey.CPU_RESULTS_EXIT_BUTTON);
            }

            friendlyCaption.gameObject.SetActive(false);
            ratingLabel.gameObject.SetActive(false);
            ratingValue.gameObject.SetActive(false);
            ratingDelta.gameObject.SetActive(false);

            if (vo.isRanked)
            {
                ratingLabel.gameObject.SetActive(true);
                ratingValue.gameObject.SetActive(true);
                ratingDelta.gameObject.SetActive(true);

                ratingValue.text = vo.currentEloScore.ToString();

                if (vo.eloScoreDelta > 0)
                {
                    ratingDelta.text = "(+" + vo.eloScoreDelta + ")";
                    ratingDelta.color = Colors.GREEN;
                }
                else if (vo.eloScoreDelta == 0)
                {
                    ratingDelta.gameObject.SetActive(false);
                }
                else
                {
                    ratingDelta.text = "(" + vo.eloScoreDelta + ")";
                    ratingDelta.color = Colors.RED;
                }
            }
            else
            {
                friendlyCaption.gameObject.SetActive(true);
            }

            this.playerWins = vo.playerWins;
            isDraw = false;
            float animDelay = RESULTS_DELAY_TIME;
            GameEndReason gameEndReason = vo.reason;

            if (gameEndReason == GameEndReason.TIMER_EXPIRED)
            {
                resultsDialogReason.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_TIMER_EXPIRED);
            }
            else if (gameEndReason == GameEndReason.CHECKMATE)
            {
                resultsDialogReason.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_CHECKMATE);
            }
            else if (gameEndReason == GameEndReason.RESIGNATION)
            {
                if (!playerWins)
                {
                    resultsDialogReason.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_RESIGNATION_PLAYER);
                    animDelay = RESULTS_SHORT_DELAY_TIME;
                }
                else
                {
                    resultsDialogReason.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_RESIGNATION_OPPONENT);
                }
            }
            else if (gameEndReason == GameEndReason.STALEMATE)
            {
                isDraw = true;
                resultsDialogReason.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_STALEMATE);
            }
            else if (gameEndReason == GameEndReason.DRAW_BY_INSUFFICIENT_MATERIAL)
            {
                isDraw = true;
                resultsDialogReason.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_DRAW_BY_INSUFFICIENT_MATERIAL);
            }
            else if (gameEndReason == GameEndReason.DRAW_BY_FIFTY_MOVE_RULE_WITH_MOVE ||
                     gameEndReason == GameEndReason.DRAW_BY_FIFTY_MOVE_RULE_WITHOUT_MOVE)
            {
                isDraw = true;
                resultsDialogReason.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_DRAW_BY_FIFTY_MOVE_RULE);
            }
            else if (gameEndReason == GameEndReason.DRAW_BY_THREEFOLD_REPEAT_RULE_WITH_MOVE ||
                     gameEndReason == GameEndReason.DRAW_BY_THREEFOLD_REPEAT_RULE_WITHOUT_MOVE)
            {
                isDraw = true;
                resultsDialogReason.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_DRAW_BY_THREEFOLD_REPEAT_RULE);
            }
            else if (gameEndReason == GameEndReason.PLAYER_DISCONNECTED)
            {
                resultsDialogReason.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_PLAYER_DISCONNECTED);
            }
            else
            {
                resultsDialogReason.text = "Unknown Reason";
            }

            if (isDraw)
            {
                resultsDialogHeading.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_HEADING_DRAW);
                resultsDialogHeading.color = Colors.YELLOW;
            }
            else
            {
                if (playerWins)
                {
                    resultsDialogHeading.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_HEADING_WIN);
                    resultsDialogHeading.color = Colors.GREEN;
                }
                else
                {
                    resultsDialogHeading.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_HEADING_LOSE);
                    resultsDialogHeading.color = Colors.RED;
                }
            }

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

            // Reward
            resultsExitButtonLabel.text = localizationService.Get(LocalizationKey.GM_EXIT_BUTTON_COLLECT_REWARD) + " +" + vo.rewardCoins;
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
			
        private void OnResultsExitButtonClicked()
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

        private void OnAdsButtonClicked()
        {
            showAdButtonClickedSignal.Dispatch();
        }


    }
}
