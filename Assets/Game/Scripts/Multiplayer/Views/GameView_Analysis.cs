using System.Collections.Generic;
using Picker;
using strange.extensions.signal.impl;
using TurboLabz.Chess;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TurboLabz.InstantFramework;
using System.Collections;

namespace TurboLabz.Multiplayer
{
    public partial class GameView
    {
        [Header("Analysis")]
        public Transform movesContainer;
        public PickerScrollRect pickerSrollRect;
        public GameObject analysisMoveView;
        public Sprite moveAnalysisBlunder;
        public Sprite moveAnalysisMistake;
        public Sprite moveAnalysisPerfect;
        public DrawLine analysisLine;
        public Image analysisArrowHead;
        public Transform analysisLineEndPivot;
        public GameObject analysisStrengthPanel;
        public Text analysisStrengthLabel;
        public Transform analysisPlayerMovePivotHolder;
        public Transform analysisPlayerMoveLeftPivot;
        public Transform analysisPlayerMoveRightPivot;
        public Image analysisMoveQuality;
        public GameObject movesSpinnerParent;
        public AnalysisMovesSpinnerDragHandler spinnerDragHandler;
        public ScrollRectAlphaHandler scrollRectAlphaHandler;
        public AnalysisMovesSpinnerDragHandler analysisMovesSpinnerDragHandler;

        public Button strengthBtn;
        public Button arrowBtn;
        public Button moveQualityBtn;

        public GameObject strengthTooltip;
        public GameObject arrowTooltip;
        public GameObject moveQualityTooltip;
        public Text moveQualityTooltipText;
        public Text arrowTooltipText;

        public Image gameAnalysisLogo;

        public Signal<List<MoveAnalysis>> onAnalysiedMoveSelectedSignal = new Signal<List<MoveAnalysis>>();
        public Signal<MatchAnalysis, StoreItem, bool> showGetFullAnalysisDlg = new Signal<MatchAnalysis, StoreItem, bool>();

        private bool landingFirstTime;
        private bool animateMovesDial;
        private bool isMovesDialAnimating;
        private GameObject moveSelectGO;

        private void OnParentShowAnalysis()
        {
            playerInfoPanel.SetActive(true);
            analysisPanel.SetActive(false);
            gameAnalysisLogo.enabled = false;
            SetGameAnalysisBottomBar(false);
            animateMovesDial = true;
        }

        public void InitAnalysis()
        {
            pickerSrollRect.onSelectItem.AddListener(OnMoveSelected);
            pickerSrollRect.onValueChanged.AddListener(scrollRectAlphaHandler.OnScroll);

            var originalScale = analysisPlayerMovePivotHolder.localScale;
            var vectorToScale = new Vector3(originalScale.x * scaleUniform, originalScale.y * scaleUniform, 1);
            analysisPlayerMovePivotHolder.localScale = vectorToScale;

            strengthBtn.onClick.AddListener(OnClickedStrength);
            arrowBtn.onClick.AddListener(OnClickedArrow);
            moveQualityBtn.onClick.AddListener(OnClickedMoveQuality);

            spinnerDragHandler.audioService = audioService;
        }

        public void UpdateAnalysisView(bool isLocked = false)
        {
            ClearMovesList();
            SetupMovesList(isLocked);
            ResetCapturedPieces();
            playerInfoPanel.SetActive(false);
            analysisPanel.SetActive(true);
            uiBlocker.SetActive(false);
            HideGameEndDialog();
            HideOpponentToIndicator();
            HideOpponentFromIndicator();
            HidePlayerToIndicator();
            HidePlayerFromIndicator();
            ShowViewBoardResultsPanel(true);
            matchTypeObject.SetActive(false);
            opponentClockContainer.SetActive(false);
            landingFirstTime = isLocked;
            SetGameAnalysisBottomBar(!isLocked);
        }

        private void ClearMovesList()
        {
            movesSpinnerParent.SetActive(false);
            scrollRectAlphaHandler.Reset();

            foreach (Transform move in movesContainer)
            {
                Destroy(move.gameObject);
            }
        }

        private void SetupMovesList(bool isLocked)
        {
            int i = 1;
            foreach (var move in moveAnalysisList)
            {
                var moveView = Instantiate(analysisMoveView, movesContainer);
                var moveVO = moveView.GetComponent<AnalysisMoveView>();
                moveVO.SetupMove($"{i}.",
                    move.playerMove.ToShortString(),
                    GetMoveQualitySprite(move.moveQuality),
                    GetPieceSprite(move.playerMove.piece.name),
                    move.whiteScore,
                    move.blackScore,
                    isLocked);
                i++;

                scrollRectAlphaHandler.AddScrollItem(moveView);
            }

            scrollRectAlphaHandler.OnScroll(Vector2.zero);
            movesSpinnerParent.SetActive(true);

            if (animateMovesDial)
            {
                StartCoroutine(AnimateMovesDial());
            }
            else
            {
                pickerSrollRect.ScrollToItemAtIndex(moveAnalysisList.Count - 1, true);
            }
        }

        private Sprite GetMoveQualitySprite(MoveQuality quality)
        {
            return quality == MoveQuality.BLUNDER ? moveAnalysisBlunder :
                   quality == MoveQuality.MISTAKE ? moveAnalysisMistake :
                   quality == MoveQuality.PERFECT ? moveAnalysisPerfect : null;
        }

        private string GetMoveQualityText(MoveQuality quality)
        {
            return quality == MoveQuality.BLUNDER ? "Blunder" :
                   quality == MoveQuality.MISTAKE ? "Mistake" :
                   quality == MoveQuality.PERFECT ? "Perfect" : string.Empty;
        }

        private void OnMoveSelected(GameObject obj)
        {
            if (isMovesDialAnimating)
            {
                moveSelectGO = obj;
                return;
            }

            var move = obj.GetComponent<AnalysisMoveView>();
            var moveNumber = move.normal.moveNumber.text;
            var moveIndex = int.Parse(moveNumber.Remove(moveNumber.Length - 1));
            var selectedMoves = moveAnalysisList.GetRange(0, moveIndex);
            var isLastMove = moveIndex == moveAnalysisList.Count;

            gameAnalysisLogo.enabled = !isLastMove;
            ShowViewBoardResultsPanel(isLastMove);
            ShowSelectedMoveAnalysis(!move.IsLocked);

            if (move.IsLocked)
            {
                if (!landingFirstTime)
                {
                    showGetFullAnalysisDlg.Dispatch(matchAnalysis, fullGameAnalysisStoreItem, freeGameAnalysisAvailable);
                    pickerSrollRect.ScrollToItemAtIndex(moveAnalysisList.Count - 1, true);
                }

                landingFirstTime = false;
            }
            else
            {
                onAnalysiedMoveSelectedSignal.Dispatch(selectedMoves);
                OnAnalysisBoardUpdated(moveIndex, move.IsLocked);
            }
        }

        private void OnAnalysisBoardUpdated(int moveIndex, bool isLocked)
        {
            var analysiedMove = moveAnalysisList[moveIndex - 1];
            var mainCamera = Camera.main;

            var playerMoveFromIndex = RankFileMap.Map[analysiedMove.playerMove.from.rank, analysiedMove.playerMove.from.file];
            var playerMoveToIndex = RankFileMap.Map[analysiedMove.playerMove.to.rank, analysiedMove.playerMove.to.file];
            var playerMoveFromPosition = chessboardSquares[playerMoveFromIndex].position;
            var playerMoveToPosition = chessboardSquares[playerMoveToIndex].position;

            var bestMoveFromIndex = RankFileMap.Map[analysiedMove.bestMove.from.rank, analysiedMove.bestMove.from.file];
            var bestMoveToIndex = RankFileMap.Map[analysiedMove.bestMove.to.rank, analysiedMove.bestMove.to.file];
            var bestMoveFromPosition = chessboardSquares[bestMoveFromIndex].position;
            var bestMoveToPosition = chessboardSquares[bestMoveToIndex].position;

            var angle = Mathf.Atan2(bestMoveFromPosition.y - bestMoveToPosition.y, bestMoveFromPosition.x - bestMoveToPosition.x) * Mathf.Rad2Deg;
            var bestMoveToScreenPosition = mainCamera.WorldToScreenPoint(bestMoveToPosition);
            //var showStrengthPanelOnRight = playerMoveToPosition.x < 0;
            //var showStrengthPanel = analysiedMove.moveQuality != MoveQuality.PERFECT && !isLocked;
            var arrowTooltipValue = analysiedMove.moveQuality == MoveQuality.PERFECT ? "perfect" : "better";

            //Player move indicators
            hindsightFromIndicator.transform.position = playerMoveFromPosition;
            hindsightToIndicator.transform.position = playerMoveToPosition;
            hindsightFromIndicator.SetActive(true);
            hindsightToIndicator.SetActive(true);

            //Placing and rotating arrow head
            analysisArrowHead.rectTransform.position = bestMoveToScreenPosition;
            analysisArrowHead.transform.localEulerAngles = new Vector3(0, 0, angle);
            arrowTooltip.transform.localEulerAngles = new Vector3(0, 0, -angle);

            //Drawing the line from square to arrow head
            analysisLine.Draw(bestMoveFromPosition, mainCamera.ScreenToWorldPoint(analysisLineEndPivot.position), 30.0f);

            //Placing pivot holder for strength and move quality
            analysisPlayerMovePivotHolder.position = playerMoveToPosition;
            //var leftPivotPosition = mainCamera.WorldToScreenPoint(analysisPlayerMoveLeftPivot.position);
            var rightPivotPosition = mainCamera.WorldToScreenPoint(analysisPlayerMoveRightPivot.position);

            //Setting move quality sprite
            analysisMoveQuality.sprite = GetMoveQualitySprite(analysiedMove.moveQuality);
            moveQualityTooltipText.text = GetMoveQualityText(analysiedMove.moveQuality);
            analysisMoveQuality.enabled = analysiedMove.moveQuality != MoveQuality.NORMAL;
            arrowTooltipText.text = $"This was a {arrowTooltipValue} move";

            //Setting strength value
            analysisStrengthLabel.text = $"{Mathf.RoundToInt(analysiedMove.strength * 100)}%";

            //Placing strength panel and quality icon
            //FlipAnalysisStrengthPanel(showStrengthPanelOnRight ? 1 : -1);
            //analysisStrengthPanel.transform.position = showStrengthPanelOnRight ? rightPivotPosition : leftPivotPosition;
            analysisMoveQuality.transform.position = rightPivotPosition;
            //analysisStrengthPanel.SetActive(showStrengthPanel);
        }

        private void FlipAnalysisStrengthPanel(float scale)
        {
            var flipVector = new Vector3(scale, 1, 1);
            analysisStrengthPanel.transform.localScale = flipVector;
            analysisStrengthLabel.transform.localScale = flipVector;
        }

        private Sprite GetPieceSprite(string piece)
        {
            var pieceGO = (from p in pieces
                           where p.name.Equals(piece)
                           select p).FirstOrDefault();

            return pieceGO.GetComponent<SpriteRenderer>().sprite;
        }

        private void ShowSelectedMoveAnalysis(bool show)
        {
            analysisArrowHead.gameObject.SetActive(show);
            analysisLine.gameObject.SetActive(show);
            analysisMoveQuality.gameObject.SetActive(show);
            analysisStrengthPanel.SetActive(false);
        }

        private IEnumerator AnimateMovesDial()
        {
            ShowSelectedMoveAnalysis(false);
            animateMovesDial = false;
            isMovesDialAnimating = true;
            analysisMovesSpinnerDragHandler.enabled = false;
            pickerSrollRect.ScrollToItemAtIndex(Mathf.Max(moveAnalysisList.Count - 6, 0), true);
            yield return new WaitForEndOfFrame();
            pickerSrollRect.autoScrollSeconds = 2.3f;
            pickerSrollRect.ScrollToItemAtIndex(moveAnalysisList.Count - 1);
            yield return new WaitForSeconds(pickerSrollRect.autoScrollSeconds);
            analysisMovesSpinnerDragHandler.enabled = true;
            isMovesDialAnimating = false;
            OnMoveSelected(moveSelectGO);
        }

        #region Button Listeners

        private void OnClickedArrow()
        {
            arrowTooltip.SetActive(true);
        }

        private void OnClickedStrength()
        {
            strengthTooltip.SetActive(true);
        }

        private void OnClickedMoveQuality()
        {
            moveQualityTooltip.SetActive(true);
        }

        #endregion

    }
}
