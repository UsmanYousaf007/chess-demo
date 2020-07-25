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
using System;
using GameAnalyticsSDK;

namespace TurboLabz.Multiplayer
{
    public partial class GameView
    {
        public Signal hintClickedSignal = new Signal();

        [Inject] public CancelHintSingal cancelHintSingal { get; set; }

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
            try
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

                var strengthVO = new StrengthVO();
                strengthVO.strength = vo.strength;
                strengthVO.fromPosition = hintFromIndicator.transform.position;
                strengthVO.toPosition = hintToIndicator.transform.position;
                strengthVO.fromIndicator = hintFromIndicator;
                strengthVO.toIndicator = hintToIndicator;
                strengthVO.audioService = audioService;
                strengthVO.analyticsService = analyticsService;
                strengthVO.activeSkinId = vo.skinId;
                strengthVO.pieceName = vo.piece;

                if (isLongPlay)
                {
                    strengthVO.analyticsContext = AnalyticsContext.long_match;
                }
                else
                {
                    strengthVO.analyticsContext = AnalyticsContext.quick_match;
                }

                if (vo.piece.Contains("captured"))
                {
                    strengthVO.pieceName = string.Format("{0}{1}", vo.piece[0], LastOpponentCapturedPiece.ToLower());
                }

                strengthPanel.ShowStrengthPanel(strengthVO);
                StartCoroutine(HideHint(4.0f));
            }
            catch (Exception e)
            {
                if(hintFromIndicator == null)
                {
                    GameAnalytics.NewErrorEvent(GAErrorSeverity.Error, "GameView_Hint_RenderHint_hintFromIndicator_" + e.StackTrace);
                }
                else if (hintToIndicator == null)
                {
                    GameAnalytics.NewErrorEvent(GAErrorSeverity.Error, "GameView_Hint_RenderHint_hintToIndicator_" + e.StackTrace);
                }
                else if (audioService == null)
                {
                    GameAnalytics.NewErrorEvent(GAErrorSeverity.Error, "GameView_Hint_RenderHint_audioService_" + e.StackTrace);
                }
                else if (hintThinking == null)
                {
                    GameAnalytics.NewErrorEvent(GAErrorSeverity.Error, "GameView_Hint_RenderHint_hintThinking_" + e.StackTrace);
                }
                else if (strengthPanel == null)
                {
                    GameAnalytics.NewErrorEvent(GAErrorSeverity.Error, "GameView_Hint_RenderHint_strengthPanel_" + e.StackTrace);
                }
                else if (audioService.sounds == null)
                {
                    GameAnalytics.NewErrorEvent(GAErrorSeverity.Error, "GameView_Hint_RenderHint_audioService_sounds_" + e.StackTrace);
                }
                else if (vo.fromSquare == null)
                {
                    GameAnalytics.NewErrorEvent(GAErrorSeverity.Error, "GameView_Hint_RenderHint_vo_fromSquare_" + e.StackTrace);
                }
                else if (vo.toSquare == null)
                {
                    GameAnalytics.NewErrorEvent(GAErrorSeverity.Error, "GameView_Hint_RenderHint_vo_toSquare_" + e.StackTrace);
                }
            }
        }

        public void CancelHint()
        {
            hintThinking.SetActive(false);
            DisableModalBlocker();
            //DisableHintButton();
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
            hintFromIndicator.SetActive(false);
            hintToIndicator.SetActive(false);
            hintThinking.SetActive(false);
        }

        private void HintButtonClicked()
        {
            if (hintAdd.gameObject.activeSelf)
            {
                var matchType = isLongPlay ? "classic" : isTenMinGame ? "10m" : isOneMinGame ? "1m" : isThirtyMinGame ? "30m" : "5m";
                setSubscriptionContext.Dispatch($"{matchType}_move_meter");
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SUBSCRIPTION_DLG);
                OnParentHideAdBanner();
                subscriptionDlgClosedSignal.AddOnce(OnParentShowAdBanner);
                EnableModalBlocker();
            }
            else
            {
                cancelHintSingal.Dispatch();
                hintThinking.SetActive(true);
                EnableModalBlocker(Colors.UI_BLOCKER_INVISIBLE_ALPHA);
                hintClickedSignal.Dispatch();
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
            //if (isLongPlay && !isRankedGame)
            //{
                hintButton.interactable = true;
                hintCountLabel.color = Colors.ColorAlpha(hintCountLabel.color, 1f);
                hintAdd.color = Colors.ColorAlpha(hintAdd.color, 1f);
                hintLabel.color = Colors.ColorAlpha(hintLabel.color, 0.87f);
                hintIcon.color = Colors.ColorAlpha(hintIcon.color, 1f);
            //}
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
