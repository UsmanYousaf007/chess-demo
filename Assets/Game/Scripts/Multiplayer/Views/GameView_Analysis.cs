using System.Collections.Generic;
using Picker;
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

        private void OnParentShowAnalysis()
        {
            playerInfoPanel.SetActive(true);
            analysisPanel.SetActive(false);

            foreach (Transform move in movesContainer)
            {
                Destroy(move.gameObject);
            }
        }

        public void UpdateAnalysisView(List<MoveAnalysis> moves)
        {
            int i = 1;
            foreach (var move in moves)
            {
                if (move.bestMove != null)
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

            ResetCapturedPieces();
            playerInfoPanel.SetActive(false);
            analysisPanel.SetActive(true);
            uiBlocker.SetActive(false);
        }
    }
}
