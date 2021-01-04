using strange.extensions.signal.impl;
using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;
using UnityEngine;
using UnityEngine.UI;

namespace TurboLabz.Multiplayer
{
    public partial class GameView
    {
        [Header("Special Hint")]
        public Button specialHintButton;
        public GameObject specialHintThinkinig;
        public CoachView specialHintView;
        public Text specialHintGemsCost;
        public Image specialHintGemsBg;
        public string specialHintShortCode;
        public GameObject specialHintToolTip;
        public Text specialHintTooltipText;
        public Text specialHintCountText;
        public GameObject specialHintCountContainer;

        private bool haveEnoughHints;
        private bool haveEnoughGemsForHint;
        private bool canUseSpecialHint;
        private StoreItem hintsStoreItem;
        private int hintsAllowedPerGame;
        private int hintCount;
        public GameObject specialFreeHintContainer;

        public Signal<VirtualGoodsTransactionVO> specialHintClickedSignal = new Signal<VirtualGoodsTransactionVO>();
        public Signal<VirtualGoodsTransactionVO> notEnoughSpecialHintsSingal = new Signal<VirtualGoodsTransactionVO>();

        public void InitSpecialHint()
        {
            var originalScale = specialHintView.stickerBg.transform.localScale;
            var vectorToScale = new Vector3(originalScale.x * scaleUniform, originalScale.y * scaleUniform, 1);
            specialHintView.stickerBg.transform.localScale = vectorToScale;
            specialHintButton.onClick.AddListener(SpecialHintButtonClicked);
        }

        public void OnParentShowSpecialHint()
        {
            DisableSpecialHintButton();
            specialHintThinkinig.SetActive(false);
            specialHintView.Hide();
            specialHintToolTip.SetActive(false);
        }

        public void SetupSpecialHintButton(SpecialHintVO vo)
        {
            hintsStoreItem = vo.specialHintStoreItem;
            canUseSpecialHint = vo.isAvailable;
            hintsAllowedPerGame = vo.hintsAllowedPerGame;
            hintCount = vo.hintCount;
            SetupSpecialHintButton();
            specialHintTooltipText.text = $"*Only {hintsAllowedPerGame} {localizationService.Get(LocalizationKey.GM_SPECIAL_HINT_NOT_AVAILABLE)}";
            specialHintToolTip.SetActive(!canUseSpecialHint);

            if (canUseSpecialHint && !playerModel.HasSubscription())
            {
                analyticsService.Event(AnalyticsEventId.booster_shown, AnalyticsContext.hint);
            }
        }

        public void UpdateSpecialHintButton(int hintUsedCount, bool updateHintCount)
        {
            if (updateHintCount)
            {
                hintCount--;
                hintCount = hintCount < 0 ? 0 : hintCount;
            }

            canUseSpecialHint = hintUsedCount < hintsAllowedPerGame;
            SetupSpecialHintButton();
        }

        public void SetupSpecialHintButton()
        {
            if (hintsStoreItem == null)
            {
                return;
            }

            specialHintGemsCost.text = hintsStoreItem.currency3Cost.ToString();
            haveEnoughHints = playerModel.HasSubscription() || preferencesModel.freeDailyHint == FreePowerUpStatus.NOT_CONSUMED;
            haveEnoughGemsForHint = playerModel.gems >= hintsStoreItem.currency3Cost;
            specialHintGemsBg.gameObject.SetActive(true);
            specialHintButton.image.color = Colors.ColorAlpha(specialHintButton.image.color, canUseSpecialHint ? 1 : 0.5f);
            specialHintCountText.color = Colors.ColorAlpha(specialHintCountText.color, canUseSpecialHint ? 1 : 0.5f);
            specialHintCountText.text = hintCount.ToString();
            specialHintCountContainer.SetActive(!playerModel.HasSubscription());
            specialFreeHintContainer.SetActive(preferencesModel.freeDailyHint == FreePowerUpStatus.NOT_CONSUMED);
        }

        public void ToggleSpecialHintButton(bool on)
        {
            if (on)
            {
                EnableSpecialHintButton();
            }
            else
            {
                DisableSpecialHintButton();
            }
        }

        private void DisableSpecialHintButton()
        {
            specialHintButton.interactable = false;
        }

        private void EnableSpecialHintButton()
        {
            specialHintButton.interactable = true;
        }

        public void RenderSpecialHint(HintVO vo)
        {
            int fromSquareIndex = RankFileMap.Map[vo.fromSquare.fileRank.rank, vo.fromSquare.fileRank.file];
            hindsightFromIndicator.transform.position = chessboardSquares[fromSquareIndex].position;

            int toSquareIndex = RankFileMap.Map[vo.toSquare.fileRank.rank, vo.toSquare.fileRank.file];
            hindsightToIndicator.transform.position = chessboardSquares[toSquareIndex].position;

            specialHintThinkinig.SetActive(false);
            DisableModalBlocker();
            DisableSpecialHintButton();

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
            coachVO.downloadablesModel = downloadablesModel;

            if (vo.piece.Contains("captured"))
            {
                coachVO.pieceName = string.Format("{0}{1}", vo.piece[0], LastOpponentCapturedPiece.ToLower());
            }

            specialHintView.Show(coachVO);
            specialHintToolTip.SetActive(!canUseSpecialHint);
        }

        public void CancelSpecialHint()
        {
            specialHintThinkinig.SetActive(false);
            DisableModalBlocker();
            specialHintView.Hide();
        }

        private void SpecialHintButtonClicked()
        {
            if (canUseSpecialHint)
            {
                if (haveEnoughHints)
                {
                    var transactionVO = new VirtualGoodsTransactionVO();
                    transactionVO.consumeItemShortCode = "premium";
                    transactionVO.consumeQuantity = 1;
                    ProcessHint(transactionVO);
                }
                else if (haveEnoughGemsForHint)
                {
                    var transactionVO = new VirtualGoodsTransactionVO();
                    transactionVO.consumeItemShortCode = GSBackendKeys.PlayerDetails.GEMS;
                    transactionVO.consumeQuantity = hintsStoreItem.currency3Cost;
                    transactionVO.buyItemShortCode = specialHintShortCode;
                    transactionVO.buyQuantity = 1;
                    ProcessHint(transactionVO);
                }
                else
                {
                    EnableModalBlocker(Colors.UI_BLOCKER_INVISIBLE_ALPHA);
                    notEnoughGemsSignal.Dispatch();
                }
            }
        }

        public void ProcessHint(VirtualGoodsTransactionVO vo)
        {
            cancelHintSingal.Dispatch();
            specialHintThinkinig.SetActive(true);
            EnableModalBlocker(Colors.UI_BLOCKER_INVISIBLE_ALPHA);
            specialHintClickedSignal.Dispatch(vo);
        }
    }
}
