/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-16 10:42:33 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System.Collections;

using UnityEngine;

using strange.extensions.promise.api;
using strange.extensions.promise.impl;

using TurboLabz.TLUtils;
using System.Collections.Generic;

namespace TurboLabz.Chess
{
    public partial class ChessAiService : IChessAiService
    {
        [Inject] public IRoutineRunner routineRunner { get; set; }
        [Inject] public IChessService chessService { get; set; }

        private IPromise<FileRank, FileRank, string> aiMovePromise; 
        private AiMoveInputVO aiMoveInputVO;
        private ChessAiPlugin plugin = new ChessAiPlugin();
        private bool resultsReady;

        public void NewGame()
        {
            plugin.NewGame();
            ChessAiPlugin.resultsReadySignal.RemoveAllListeners();
            ChessAiPlugin.resultsReadySignal.AddListener(ResultsReady);
        }

        public IPromise<FileRank, FileRank, string> GetAiMove(AiMoveInputVO vo)
        {
            // Save inputs
            this.aiMoveInputVO = vo;

            // Clear the results flag
            resultsReady = false;

            // Execute the move
            string searchDepth = vo.isHint ? ChessAiConfig.SF_MAX_SEARCH_DEPTH.ToString() : GetSearchDepth().ToString();
            AiLog("searchDepth = " + searchDepth);
            plugin.GoDepth(searchDepth);

            // Read the results
            aiMovePromise = new Promise<FileRank, FileRank, string>();
            routineRunner.StartCoroutine(GetAiResult());
            return aiMovePromise;
        }

        public void Shutdown()
        {
            // Shutdown the plugin
            plugin.Shutdown();
        }

        public void SetPosition(string FEN)
        {
            plugin.SetPosition(FEN);
        }

        private void ResultsReady()
        {
            resultsReady = true;
        }

        private IEnumerator GetAiResult()
        {
            float startTime = Time.time;

            while (!resultsReady)
            {
                yield return null;
            }

            float delay = 0f;

            if (aiMoveInputVO.aiMoveDelay == AiMoveDelay.CPU)
            {
                delay = AiMoveTimes.M_CPU;
            }
            else if (aiMoveInputVO.aiMoveDelay == AiMoveDelay.ONLINE_5M)
            {
                int index = Mathf.Min(aiMoveInputVO.aiMoveNumber, AiMoveTimes.M_5.Length - 1);
                float[] delayRange = AiMoveTimes.M_5[index];
                delay = Random.Range(delayRange[0], delayRange[1]);
            }
                
            float timeElapsed = Time.time - startTime;
            delay -= timeElapsed;
            delay = Mathf.Max(0, delay);
            yield return new WaitForSecondsRealtime(delay);

            ExecuteAiMove();
        }

        private void ExecuteAiMove()
        {
            // Read the scores returned
            aiSearchResultScoresList = new List<string>(ChessAiPlugin.results.aiSearchResultScoresStr.Split(','));
            aiSearchResultScoresList.RemoveAt(0); // Gets rid of the label
            scores = new List<int>();

            foreach (string score in aiSearchResultScoresList)
            {
                scores.Add(int.Parse(score));
            }

            // Read the moves returned
            aiSearchResultMovesList = new List<string>(ChessAiPlugin.results.aiSearchResultMovesStr.Split(','));
            aiSearchResultMovesList.RemoveAt(0); // Gets rid of the label

            SelectMove();

            aiMovePromise = null;
        }
    }
}
