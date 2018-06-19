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

using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using strange.extensions.signal.impl;

using TurboLabz.TLUtils;
using TurboLabz.InstantFramework;
using TurboLabz.Chess;
using TurboLabz.InstantGame;

namespace TurboLabz.CPU
{
    public partial class GameView
    {
		[Header("Results Dialog")]

        public GameObject resultsDialog;
        public Text resultsDialogHeading;
        public Text resultsDialogReason;
		public Button resultsExitButton;
        public Text resultsExitButtonLabel;
        public Button resultsCloseButton;
        public Text resultsCloseButtonLabel;
        public Button resultsDialogButton;

        public Signal resultsExitButtonClickedSignal = new Signal();
		public Signal resultsStatsButtonClickedSignal = new Signal();
        public Signal showAdButtonClickedSignal = new Signal();
        public Signal enterPlaybackSignal = new Signal();
        public Signal resultsDialogButtonClickedSignal = new Signal();

        private const float RESULTS_DELAY_TIME = 1f;
        private const float RESULTS_SHORT_DELAY_TIME = 0.3f;
        private const float RESULTS_DIALOG_DURATION = 0.5f;
        private float resultsDialogHalfHeight;

        private bool playerWins;
        private bool isDraw;

        public void InitResults()
        {
            resultsExitButton.onClick.AddListener(OnResultsExitButtonClicked);
            resultsCloseButton.onClick.AddListener(OnResultsClosed);
            resultsDialogButton.onClick.AddListener(OnResultsDialogButtonClicked);
		
            resultsExitButtonLabel.text = localizationService.Get(LocalizationKey.CPU_RESULTS_EXIT_BUTTON);
            resultsCloseButtonLabel.text = localizationService.Get(LocalizationKey.CPU_RESULTS_CLOSE_BUTTON);
		
            resultsDialogHalfHeight = resultsDialog.GetComponent<RectTransform>().rect.height / 2f;
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
            resultsDialogButton.gameObject.SetActive(false);

            DisableUndoButton();
            DisableMenuButton();
            DisableHintButton();
            DeactivateThink();
            HidePossibleMoves();

            if (!ArePlayerMoveIndicatorsVisible())
            {
                HidePlayerToIndicator();
            }
        }

        public void HideResultsDialog()
        {
            resultsDialog.SetActive(false);
        }

		public void UpdateResultsDialog(GameEndReason gameEndReason, bool playerWins, int statResult)
        {
            HideMenu();
            DisableInteraction();
            EnableModalBlocker();

            this.playerWins = playerWins;
            isDraw = false;
            float animDelay = RESULTS_DELAY_TIME;

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
                resultsDialogReason.text = localizationService.Get(LocalizationKey.GM_RESULT_DIALOG_REASON_RESIGNATION);

                if (!playerWins)
                {
                    animDelay = RESULTS_SHORT_DELAY_TIME;
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
            else
            {
                Assertions.Assert(false, "Unknown game end reason.");
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
        }

        public bool IsResultsDialogVisible()
        {
            return resultsDialog.activeSelf;
        }

        public void EnableResultsDialogButton()
        {
            DisableModalBlocker();
            EnableMenuButton();
            resultsDialogButton.gameObject.SetActive(true);
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

        private bool IsResultsDialogActive()
        {
            return resultsDialog.activeSelf;
        }
			
        private void OnResultsExitButtonClicked()
        {
            resultsExitButtonClickedSignal.Dispatch();
        }

        private void OnAdsButtonClicked()
        {
            showAdButtonClickedSignal.Dispatch();
        }

        private void OnResultsClosed()
        {
            enterPlaybackSignal.Dispatch();
        }

        private void OnResultsDialogButtonClicked()
        {
            resultsDialogButtonClickedSignal.Dispatch();
        }

		private void OnResultsStatsButtonClicked()
		{
			resultsStatsButtonClickedSignal.Dispatch();
		}
    }
}
