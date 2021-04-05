﻿using System.Collections.Generic;
using Picker;
using strange.extensions.signal.impl;
using TurboLabz.Chess;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

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

        public Signal<List<MoveAnalysis>> onAnalysiedMoveSelectedSignal = new Signal<List<MoveAnalysis>>();

        private List<MoveAnalysis> analysiedMoves;

        private void OnParentShowAnalysis()
        {
            playerInfoPanel.SetActive(true);
            analysisPanel.SetActive(false);
        }

        public void InitAnalysis()
        {
            pickerSrollRect.onSelectItem.AddListener(OnMoveSelected);

            var originalScale = analysisPlayerMovePivotHolder.localScale;
            var vectorToScale = new Vector3(originalScale.x * scaleUniform, originalScale.y * scaleUniform, 1);
            analysisPlayerMovePivotHolder.localScale = vectorToScale;
        }

        public void UpdateAnalysisView(List<MoveAnalysis> moves, bool isLocked = false)
        {
            analysiedMoves = moves;
            ClearMovesList();
            SetupMovesList(isLocked);
            ResetCapturedPieces();
            playerInfoPanel.SetActive(false);
            analysisPanel.SetActive(true);
            uiBlocker.SetActive(false);
            SetBlurBg(false);
            HideOpponentToIndicator();
            HideOpponentFromIndicator();
            HidePlayerToIndicator();
            HidePlayerFromIndicator();
        }

        private void ClearMovesList()
        {
            movesSpinnerParent.SetActive(false);

            foreach (Transform move in movesContainer)
            {
                Destroy(move.gameObject);
            }
        }

        private void SetupMovesList(bool isLocked)
        {
            int i = 1;
            foreach (var move in analysiedMoves)
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
            }

            movesSpinnerParent.SetActive(true);
        }

        private Sprite GetMoveQualitySprite(MoveQuality quality)
        {
            return quality == MoveQuality.BLUNDER ? moveAnalysisBlunder :
                   quality == MoveQuality.MISTAKE ? moveAnalysisMistake :
                   quality == MoveQuality.PERFECT ? moveAnalysisPerfect : null;
        }

        private void OnMoveSelected(GameObject obj)
        {
            var move = obj.GetComponent<AnalysisMoveView>();
            var moveNumber = move.normal.moveNumber.text;
            var moveIndex = int.Parse(moveNumber.Remove(moveNumber.Length - 1));
            var selectedMoves = moveAnalysisList.GetRange(0, moveAnalysisList.Count - analysiedMoves.Count + moveIndex);

            onAnalysiedMoveSelectedSignal.Dispatch(selectedMoves);
            ShowSelectedMoveAnalysis(!move.IsLocked);
            OnAnalysisBoardUpdated(moveIndex, move.IsLocked);
        }

        private void OnAnalysisBoardUpdated(int moveIndex, bool isLocked)
        {
            var analysiedMove = analysiedMoves[moveIndex - 1];
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
            var showStrengthPanelOnRight = playerMoveToPosition.x < 0;
            var showStrengthPanel = analysiedMove.moveQuality != MoveQuality.PERFECT && !isLocked;

            //Player move indicators
            hindsightFromIndicator.transform.position = playerMoveFromPosition;
            hindsightToIndicator.transform.position = playerMoveToPosition;
            hindsightFromIndicator.SetActive(true);
            hindsightToIndicator.SetActive(true);

            //Placing and rotating arrow head
            analysisArrowHead.rectTransform.position = bestMoveToScreenPosition;
            analysisArrowHead.transform.localEulerAngles = new Vector3(0, 0, angle);

            //Drawing the line from square to arrow head
            analysisLine.Draw(bestMoveFromPosition, mainCamera.ScreenToWorldPoint(analysisLineEndPivot.position), 30.0f);

            //Placing pivot holder for strength and move quality
            analysisPlayerMovePivotHolder.position = playerMoveToPosition;
            var leftPivotPosition = mainCamera.WorldToScreenPoint(analysisPlayerMoveLeftPivot.position);
            var rightPivotPosition = mainCamera.WorldToScreenPoint(analysisPlayerMoveRightPivot.position);

            //Setting move quality sprite
            analysisMoveQuality.sprite = GetMoveQualitySprite(analysiedMove.moveQuality);
            analysisMoveQuality.enabled = analysiedMove.moveQuality != MoveQuality.NORMAL;

            //Setting strength value
            analysisStrengthLabel.text = $"{Mathf.RoundToInt(analysiedMove.strength * 100)}%";

            //Placing strength panel and quality icon
            FlipAnalysisStrengthPanel(showStrengthPanelOnRight ? 1 : -1);
            analysisStrengthPanel.transform.position = showStrengthPanelOnRight ? rightPivotPosition : leftPivotPosition;
            analysisMoveQuality.transform.position = showStrengthPanelOnRight && showStrengthPanel ? leftPivotPosition : rightPivotPosition;
            analysisStrengthPanel.SetActive(showStrengthPanel);
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
        }
    }
}
