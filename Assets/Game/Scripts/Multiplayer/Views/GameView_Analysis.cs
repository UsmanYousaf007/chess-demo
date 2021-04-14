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
        public Sprite moveAnalysisNormal;
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
        public Transform analysisArrowTooltipPivot;

        public Image gameAnalysisLogo;
        public Text analysisDebugText;

        public Signal<List<MoveAnalysis>> onAnalysiedMoveSelectedSignal = new Signal<List<MoveAnalysis>>();
        public Signal<MatchAnalysis, StoreItem, bool> showGetFullAnalysisDlg = new Signal<MatchAnalysis, StoreItem, bool>();

        private bool landingFirstTime;
        private bool animateMovesDial;
        private bool isMovesDialAnimating;
        private GameObject moveSelectGO;

        private bool isGameAnalysisEnabled;

        private void OnParentShowAnalysis()
        {
            playerInfoPanel.SetActive(true);
            analysisPanel.SetActive(false);
            gameAnalysisLogo.enabled = false;
            SetGameAnalysisBottomBar(false);
            animateMovesDial = true;
            isGameAnalysisEnabled = false;
            analysisDebugText.gameObject.SetActive(false);
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
            isGameAnalysisEnabled = !isLocked;
            EmptyScores();
            opponentProfileView.championshipTrophiesContainer.SetActive(false);
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
            if (moveAnalysisList.Count == 0)
            {
                return;
            }

            var playerColor = moveAnalysisList[0].isPlayerMove ? ChessColor.WHITE : ChessColor.BLACK;

            int i = 1;
            int j = 1;
            foreach (var move in moveAnalysisList)
            {
                var moveView = Instantiate(analysisMoveView, movesContainer);
                var moveVO = moveView.GetComponent<AnalysisMoveView>();

                moveVO.SetupMove(i,
                    i % 2 == 1 ? $"{j}." : string.Empty,
                    move.playerMove.ToShortString(),
                    GetMoveQualitySprite(move.moveQuality),
                    GetPieceSprite(move.playerMove.piece.name),
                    move.playerAdvantage,
                    playerColor,
                    isLocked
                    );
                i++;
                if (i % 2 == 0) j++;

                scrollRectAlphaHandler.AddScrollItem(moveView);
            }

            scrollRectAlphaHandler.OnScroll(Vector2.zero);
            movesSpinnerParent.SetActive(true);

            if (animateMovesDial)
            {
                StartCoroutine(AnimateMovesDial(isLocked));
            }
            else
            {
                pickerSrollRect.ScrollToItemAtIndex(moveAnalysisList.Count - 1, true);
            }
        }

        private Sprite GetMoveQualitySprite(MoveQuality quality, bool showNormal = false)
        {
            return quality == MoveQuality.BLUNDER ? moveAnalysisBlunder :
                   quality == MoveQuality.MISTAKE ? moveAnalysisMistake :
                   quality == MoveQuality.PERFECT ? moveAnalysisPerfect :
                   quality == MoveQuality.NORMAL && showNormal ? moveAnalysisNormal : null;
        }

        private string GetMoveQualityText(MoveQuality quality)
        {
            return quality == MoveQuality.BLUNDER ? "Blunder" :
                   quality == MoveQuality.MISTAKE ? "Mistake" :
                   quality == MoveQuality.PERFECT ? "Perfect" : "Ok";
        }

        private void OnMoveSelected(GameObject obj)
        {
            if (obj == null)
            {
                return;
            }

            if (isMovesDialAnimating)
            {
                moveSelectGO = obj;
                return;
            }

            var move = obj.GetComponent<AnalysisMoveView>();
            var moveIndex = move.MoveNumber;
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
            var showQualityOnRight = playerMoveToPosition.x < 0;
            //var showStrengthPanel = analysiedMove.moveQuality != MoveQuality.PERFECT && !isLocked;

            //Player move indicators
            hindsightFromIndicator.transform.position = playerMoveFromPosition;
            hindsightToIndicator.transform.position = playerMoveToPosition;
            hindsightFromIndicator.SetActive(true);
            hindsightToIndicator.SetActive(true);

            //Placing and rotating arrow head
            analysisArrowHead.rectTransform.position = bestMoveToScreenPosition;
            arrowTooltip.transform.position = analysisArrowTooltipPivot.position;
            analysisArrowHead.transform.localEulerAngles = new Vector3(0, 0, angle);

            //Drawing the line from square to arrow head
            analysisLine.Draw(bestMoveFromPosition, mainCamera.ScreenToWorldPoint(analysisLineEndPivot.position), 33.0f);

            //Placing pivot holder for strength and move quality
            analysisPlayerMovePivotHolder.position = playerMoveToPosition;
            var leftPivotPosition = mainCamera.WorldToScreenPoint(analysisPlayerMoveLeftPivot.position);
            var rightPivotPosition = mainCamera.WorldToScreenPoint(analysisPlayerMoveRightPivot.position);
            analysisMoveQuality.transform.position = showQualityOnRight ? rightPivotPosition : leftPivotPosition;

            //Setting move quality sprite
            analysisMoveQuality.sprite = GetMoveQualitySprite(analysiedMove.moveQuality, true);
            moveQualityTooltipText.text = GetMoveQualityText(analysiedMove.moveQuality);

            //Setting strength value
            //analysisStrengthLabel.text = $"{Mathf.RoundToInt(analysiedMove.strength * 100)}%";

            //Placing strength panel and quality icon
            //FlipAnalysisStrengthPanel(showStrengthPanelOnRight ? 1 : -1);
            //analysisStrengthPanel.transform.position = showStrengthPanelOnRight ? rightPivotPosition : leftPivotPosition;
            //analysisStrengthPanel.SetActive(showStrengthPanel);

            analysisDebugText.gameObject.SetActive(false);
            var moveScore = analysiedMove.playerScoreDebug == 0 ? "not found" : analysiedMove.playerScoreDebug.ToString();
            var delta = analysiedMove.playerScoreDebug == 0 ? "NaN" : (analysiedMove.bestScore - analysiedMove.playerScoreDebug).ToString();
            analysisDebugText.text = $"BS:{analysiedMove.bestScore}, MS:{moveScore}, D:{delta}, AS:{analysiedMove.playerScore}";
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

        private IEnumerator AnimateMovesDial(bool isLocked)
        {
            var scrollFromIndex = isLocked ? Mathf.Max(moveAnalysisList.Count - 6, 0) : Mathf.Min(moveAnalysisList.Count - 1, 5);
            var scrollToIndex = isLocked ? moveAnalysisList.Count - 1 : 0;
            ShowSelectedMoveAnalysis(false);
            animateMovesDial = false;
            isMovesDialAnimating = true;
            analysisMovesSpinnerDragHandler.enabled = false;
            pickerSrollRect.ScrollToItemAtIndex(scrollFromIndex, true);
            yield return new WaitForEndOfFrame();
            pickerSrollRect.autoScrollSeconds = 2.3f;
            pickerSrollRect.ScrollToItemAtIndex(scrollToIndex);
            yield return new WaitForSeconds(pickerSrollRect.autoScrollSeconds);
            analysisMovesSpinnerDragHandler.enabled = true;
            isMovesDialAnimating = false;
            OnMoveSelected(moveSelectGO);
        }

        public void LastMoveAnalysied(MoveAnalysis moveAnalysis)
        {
            moveAnalysisList.Last().playerAdvantage = moveAnalysis.playerAdvantage;
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
