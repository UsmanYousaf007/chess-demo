/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential


using UnityEngine;
using UnityEngine.UI;

using strange.extensions.signal.impl;
using TurboLabz.Chess;
using TurboLabz.TLUtils;
using TurboLabz.InstantFramework;
using TMPro;
using TurboLabz.InstantGame;
using System.Collections;

namespace TurboLabz.CPU
{
    public partial class GameView
    {
        public Signal hintClickedSignal = new Signal();

        [Header("Hint")]
        public GameObject hintFromIndicator;
        public GameObject hintToIndicator;

        public Button hintButton;
        public Text hintLabel;
        public Image hintIcon;
        public Text hintCountLabel;
        public Image hintAdd;
        public GameObject hintThinking;
        public StrengthAnim strengthPanel;
        public GameObject strengthOnboardingTooltip;

        private bool isStrengthToolTipShown;

        public void InitHint()
        {
            //hintButtonLabel.text = localizationService.Get(LocalizationKey.CPU_GAME_HINT_BUTTON);
            hintButton.onClick.AddListener(HintButtonClicked);
            hintThinking.SetActive(false);
            strengthPanel.Hide();

            var originalScale = strengthPanel.stickerBg.transform.localScale;
            var vectorToScale = new Vector3(originalScale.x * scaleUniform, originalScale.y * scaleUniform, 1);
            strengthPanel.stickerBg.transform.localScale = vectorToScale;
        }

        public void OnParentShowHint()
        {
            HideHint();
            DisableHintButton();
        }

        public void RenderHint(HintVO vo)
        {
            int fromSquareIndex = RankFileMap.Map[vo.fromSquare.fileRank.rank, vo.fromSquare.fileRank.file];
            hintFromIndicator.transform.position = chessboardSquares[fromSquareIndex].position;
            hintFromIndicator.SetActive(true);

            int toSquareIndex = RankFileMap.Map[vo.toSquare.fileRank.rank, vo.toSquare.fileRank.file];
            hintToIndicator.transform.position = chessboardSquares[toSquareIndex].position;
            hintToIndicator.SetActive(true);

            audioService.Play(audioService.sounds.SFX_HINT);

            //UpdateHintCount(vo.availableHints);
            //DisableHintButton();

            hintThinking.SetActive(false);
            DisableModalBlocker();
            DisableHintButton();

            RestoreStepButtons();

            var strengthVO = new StrengthVO();
            strengthVO.strength = vo.strength;
            strengthVO.fromPosition = hintFromIndicator.transform.position;
            strengthVO.toPosition = hintToIndicator.transform.position;
            strengthVO.fromIndicator = hintFromIndicator;
            strengthVO.toIndicator = hintToIndicator;
            strengthVO.audioService = audioService;
            strengthVO.analyticsService = analyticsService;
            strengthVO.analyticsContext = AnalyticsContext.computer_match;
            strengthVO.activeSkinId = vo.skinId;
            strengthVO.pieceName = vo.piece;

            if (vo.piece.Contains("captured"))
            {
                strengthVO.pieceName = string.Format("{0}{1}", vo.piece[0], LastOpponentCapturedPiece.ToLower());
            }

            strengthPanel.ShowStrengthPanel(strengthVO);
            StartCoroutine(HideHint(4.0f));

        }

        public void CancelHint()
        {
            hintThinking.SetActive(false);
            DisableModalBlocker();
            //DisableHintButton();

            if (strengthPanel.gameObject.activeSelf)
            {
                analyticsService.Event(AnalyticsEventId.cancel_pow_move_meter, AnalyticsContext.computer_match);
            }

            strengthPanel.Hide();
            strengthOnboardingTooltip.SetActive(false);
        }

        public IEnumerator HideHint(float t)
        {
            yield return new WaitForSeconds(t);
            HideHint();
        }

        public void HideHint()
        {
            //hintFromIndicator.SetActive(false);
            //hintToIndicator.SetActive(false);
            hintThinking.SetActive(false);
        }

        private void HintButtonClicked()
        {
            if (hintAdd.gameObject.activeSelf)
            {
                //navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SUBSCRIPTION_DLG);
            }
            else
            {
                cancelHintSingal.Dispatch();
                hintThinking.SetActive(true);
                EnableModalBlocker(Colors.UI_BLOCKER_INVISIBLE_ALPHA);
                hintClickedSignal.Dispatch();

                StashStepButtons();

                if (isStrengthToolTipShown)
                {
                    isStrengthToolTipShown = false;
                    analyticsService.Event(AnalyticsEventId.tap_move_meter_after_tooltip, AnalyticsContext.computer_match);
                }

                if (InstantFramework.LobbyView.isStrengthTrainingShown)
                {
                    analyticsService.Event(AnalyticsEventId.tap_move_meter_after_training, AnalyticsContext.computer_match);
                }
                else
                {
                    analyticsService.Event(AnalyticsEventId.tap_pow_move_meter, AnalyticsContext.computer_match);
                }
            }
        }

        public void ToggleHintButton(bool isPlayerTurn)
        {
            if (isPlayerTurn)
            {
                EnableHintButton();
            }
            else
            {
                DisableHintButton();
            }
        }

        public void DisableHintButton()
        {
            hintButton.interactable = false;
            hintCountLabel.color = Colors.ColorAlpha(hintCountLabel.color, 0.5f);
            hintAdd.color = Colors.ColorAlpha(hintAdd.color, 0.5f);
            hintLabel.color = Colors.ColorAlpha(hintLabel.color, 0.5f);
            hintIcon.color = Colors.ColorAlpha(hintIcon.color, 0.5f);
        }

        public void EnableHintButton()
        {
            hintButton.interactable = true;
            hintCountLabel.color = Colors.ColorAlpha(hintCountLabel.color, 1f);
            hintAdd.color = Colors.ColorAlpha(hintAdd.color, 1f);
            hintLabel.color = Colors.ColorAlpha(hintLabel.color, 0.87f);
            hintIcon.color = Colors.ColorAlpha(hintIcon.color, 1f);
        }
    

        public void UpdateHintCount(int count)
        {
            if (playerModel.HasSubscription())
            {
                hintAdd.gameObject.SetActive(false);
                hintCountLabel.gameObject.SetActive(false);
            }
            else if (count <= 0)
            {
                hintAdd.gameObject.SetActive(true);
                hintCountLabel.gameObject.SetActive(false);
            }
            else
            {
                hintAdd.gameObject.SetActive(false);
                hintCountLabel.gameObject.SetActive(true);
            }

            hintCountLabel.text = count.ToString();
        }

        public void ShowStrengthOnboardingTooltip(bool show)
        {
            strengthOnboardingTooltip.SetActive(show);
            isStrengthToolTipShown |= show;
        }

    }
}
