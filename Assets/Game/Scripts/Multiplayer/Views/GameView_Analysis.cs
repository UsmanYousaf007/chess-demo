using System.Collections.Generic;
using DanielLochner.Assets.SimpleScrollSnap;
using TurboLabz.Chess;
using UnityEngine;

namespace TurboLabz.Multiplayer
{
    public partial class GameView
    {
        [Header("Analysis")]
        public SimpleScrollSnap movesContainer;
        public GameObject analysisMoveView;
        public Sprite moveAnalysisBlunder;
        public Sprite moveAnalysisMistake;
        public Sprite moveAnalysisPerfect;

        private void OnParentShowAnalysis()
        {
            playerInfoPanel.SetActive(true);
            analysisPanel.SetActive(false);

            if (movesContainer.Panels != null)
            {
                for (int i = 0; i < movesContainer.Panels.Length; i++)
                {
                    movesContainer.Remove(i);
                }
            }
        }

        public void UpdateAnalysisView(List<MoveAnalysis> moves)
        {
            int i = 1;
            foreach (var move in moves)
            {
                if (move.bestMove != null)
                {
                    var moveVO = analysisMoveView.GetComponent<AnalysisMoveView>();

                    moveVO.SetupMove($"{i}.", move.playerMove.ToShortString(),
                        move.moveQuality == MoveQuality.BLUNDER ? moveAnalysisBlunder :
                        move.moveQuality == MoveQuality.MISTAKE ? moveAnalysisMistake :
                        move.moveQuality == MoveQuality.PERFECT ? moveAnalysisPerfect : null);

                    movesContainer.Add(analysisMoveView, i - 1);

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
