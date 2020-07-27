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

namespace TurboLabz.CPU
{
    public partial class GameView
    {
        public Signal hindsightClickedSignal = new Signal();

        [Inject] public CancelHintSingal cancelHintSingal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public SubscriptionDlgClosedSignal subscriptionDlgClosedSignal { get; set; }
        [Inject] public SetSubscriptionContext setSubscriptionContext { get; set; }


        [Header("Hindsight")]
        public GameObject hindsightFromIndicator;
        public GameObject hindsightToIndicator;
        public Button hindsightButton;
        public Text hindsightLabel;
        public Image hindsightIcon;
        public Text hindsightCountLabel;
        public Image hindsightAdd;
        public GameObject hindsightThinking;
        private bool hindsightPreviousState;
        public CoachView coachView;
        public GameObject coachOnboardingTooltip;

        private bool isCoachToolTipShown;

        public void InitHindsight()
        {
            //hintButtonLabel.text = localizationService.Get(LocalizationKey.CPU_GAME_HINT_BUTTON);
            hindsightButton.onClick.AddListener(HindsightButtonClicked);
            hindsightThinking.SetActive(false);

            var originalScale = coachView.stickerBg.transform.localScale;
            var vectorToScale = new Vector3(originalScale.x * scaleUniform, originalScale.y * scaleUniform, 1);
            coachView.stickerBg.transform.localScale = vectorToScale;
        }

        public void OnParentShowHindsight()
        {
            HideHindsight();
        }

        public void RenderHindsight(HintVO vo)
        {
            int fromSquareIndex = RankFileMap.Map[vo.fromSquare.fileRank.rank, vo.fromSquare.fileRank.file];
            hindsightFromIndicator.transform.position = chessboardSquares[fromSquareIndex].position;
			//hindsightFromIndicator.SetActive(true);

			int toSquareIndex = RankFileMap.Map[vo.toSquare.fileRank.rank, vo.toSquare.fileRank.file];
            hindsightToIndicator.transform.position = chessboardSquares[toSquareIndex].position;
			//hindsightToIndicator.SetActive(true);

			//audioService.Play(audioService.sounds.SFX_HINT);

            hindsightThinking.SetActive(false);
            DisableModalBlocker();
            DisableHindsightButton();
            
            RestoreStepButtons();

            var coachVO = new CoachVO();
            coachVO.fromPosition = hindsightFromIndicator.transform.position;
            coachVO.toPosition = hindsightToIndicator.transform.position;
            coachVO.moveFrom = vo.fromSquare.fileRank.GetAlgebraicLocation();
            coachVO.moveTo = vo.toSquare.fileRank.GetAlgebraicLocation();
            coachVO.pieceName = vo.piece;
            coachVO.activeSkinId = vo.skinId;
            coachVO.isBestMove = vo.didPlayerMadeBestMove;
            coachVO.audioService = audioService;
            coachVO.analyticsService = analyticsService;
            coachVO.analyticsContext = AnalyticsContext.computer_match;

                if (vo.piece.Contains("captured"))
            {
                coachVO.pieceName = string.Format("{0}{1}", vo.piece[0], LastOpponentCapturedPiece.ToLower());
            }

            coachView.Show(coachVO);
            Invoke("HideHindsight", 4);
        }

        public void CancelHindsight()
        {
            hindsightThinking.SetActive(false);
            DisableModalBlocker();
            //DisableHindsightButton();
            coachView.Hide();
            coachOnboardingTooltip.SetActive(false);
        }

        public void HideHindsight()
        {
            hindsightFromIndicator.SetActive(false);
            hindsightToIndicator.SetActive(false);
            hindsightThinking.SetActive(false);
        }

        private void HindsightButtonClicked()
        {
            if (hindsightAdd.gameObject.activeSelf)
            {
                setSubscriptionContext.Dispatch("cpu_coach");
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SUBSCRIPTION_DLG);
                OnParentHideAdBanner();
                subscriptionDlgClosedSignal.AddOnce(OnParentShowAdBanner);
                EnableModalBlocker();
            }
            else
            {
                cancelHintSingal.Dispatch();
                hindsightThinking.SetActive(true);
                EnableModalBlocker(Colors.UI_BLOCKER_INVISIBLE_ALPHA);
                //coachView.ShowAnalyzing();
                hindsightClickedSignal.Dispatch();
                analyticsService.Event(AnalyticsEventId.power_ups_used, AnalyticsContext.coach);
                StashStepButtons();
            }
        }

        public void ToggleHindsightButton(bool on)
        {
            if (on)
            {
                EnableHindsightButton();
            }
            else
            {
                DisableHindsightButton();
            }
        }

        public void StashHindsightButton()
        {
            hindsightPreviousState = hindsightButton.interactable;
            DisableHindsightButton();
        }

        public void RestoreHindsightButton()
        {
            if (hindsightPreviousState)
            {
                EnableHindsightButton();
            }
        }

        public void DisableHindsightButton()
        {
            hindsightButton.interactable = false;
            hindsightCountLabel.color = Colors.ColorAlpha(hindsightCountLabel.color, 0.5f);
            hindsightAdd.color = Colors.ColorAlpha(hindsightAdd.color, 0.5f);
            hindsightLabel.color = Colors.ColorAlpha(hindsightLabel.color, 0.5f);
            hindsightIcon.color = Colors.ColorAlpha(hindsightIcon.color, 0.5f);
        }

        private void EnableHindsightButton()
        {
            hindsightButton.interactable = true;
            hindsightCountLabel.color = Colors.ColorAlpha(hindsightCountLabel.color, 1f);
            hindsightAdd.color = Colors.ColorAlpha(hindsightAdd.color, 1f);
            hindsightLabel.color = Colors.ColorAlpha(hindsightLabel.color, 0.87f);
            hindsightIcon.color = Colors.ColorAlpha(hindsightIcon.color, 1f);
        }

        public void UpdateHindsightCount(int count)
        {
            if (playerModel.HasSubscription())
            {
                hindsightAdd.gameObject.SetActive(false);
                hindsightCountLabel.gameObject.SetActive(false);
            }
            else if (count <= 0)
            {
                hindsightAdd.gameObject.SetActive(true);
                hindsightCountLabel.gameObject.SetActive(false);
            }
            else
            {
                hindsightAdd.gameObject.SetActive(false);
                hindsightCountLabel.gameObject.SetActive(true);
            }

            hindsightCountLabel.text = count.ToString();
        }

        public void ShowCoachOnboardingTooltip(bool show)
        {
            coachOnboardingTooltip.SetActive(show);
            isCoachToolTipShown |= show;
        }
    }
}
