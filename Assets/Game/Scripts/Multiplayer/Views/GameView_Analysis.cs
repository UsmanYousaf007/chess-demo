using System.Collections.Generic;
using Picker;
using strange.extensions.signal.impl;
using TurboLabz.Chess;
using UnityEngine;

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
        }

        public void UpdateAnalysisView(List<MoveAnalysis> moves)
        {
            analysiedMoves = moves;
            ClearMovesList();
            SetupMovesList();
            ResetCapturedPieces();
            playerInfoPanel.SetActive(false);
            analysisPanel.SetActive(true);
            uiBlocker.SetActive(false);
        }

        private void ClearMovesList()
        {
            foreach (Transform move in movesContainer)
            {
                Destroy(move.gameObject);
            }
        }

        private void SetupMovesList()
        {
            int i = 1;
            foreach (var move in analysiedMoves)
            {
                var moveView = Instantiate(analysisMoveView, movesContainer);
                var moveVO = moveView.GetComponent<AnalysisMoveView>();

                moveVO.SetupMove($"{i}.", move.playerMove.ToShortString(),
                    move.moveQuality == MoveQuality.BLUNDER ? moveAnalysisBlunder :
                    move.moveQuality == MoveQuality.MISTAKE ? moveAnalysisMistake :
                    move.moveQuality == MoveQuality.PERFECT ? moveAnalysisPerfect : null);
                i++;
            }
        }

        private void OnMoveSelected(GameObject obj)
        {
            var move = obj.GetComponent<AnalysisMoveView>();
            var moveNumber = move.normal.moveNumber.text;
            var moveIndex = int.Parse(moveNumber.Remove(moveNumber.Length - 1));
            var selectedMoves = GetSelectedMoves(moveIndex);

            onAnalysiedMoveSelectedSignal.Dispatch(selectedMoves);
        }

        private List<MoveAnalysis> GetSelectedMoves(int moveIndex)
        {
            var selectedMoves = new List<MoveAnalysis>();

            for (int i = 0; i < moveIndex; i++)
            {
                selectedMoves.Add(analysiedMoves[i]);
            }

            return selectedMoves;
        }
    }
}
