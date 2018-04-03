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
            ChessAiPlugin.resultsReadySignal.RemoveAllListeners();
            ChessAiPlugin.resultsReadySignal.AddListener(ResultsReady);
        }

        public IPromise<FileRank, FileRank, string> GetAiMove(AiMoveInputVO vo)
        {
            // Save inputs
            this.aiMoveInputVO = vo;

            // Clear the results flag
            resultsReady = false;

            // Execute the search
            // We set a strong search depth for one minute games because the ai engine
            // exhibits focused aggressive behaviour with that search depth. Meaning instead
            // of beating around the bush, it will go straight to attack your prime pieces
            // which is the general way players approach a 1 min game. This is especially
            // important to counter time hackers that make random moves so that the opponent's
            // clock runs out. TODO: verify whether this actually works correctly
            if (vo.timeControl == AiTimeControl.ONE_MINUTE)
            {
                plugin.GoDepth(ChessAiConfig.SF_ONE_MIN_SEARCH_DEPTH);
            }
            else
            {
                plugin.GoDepth(ChessAiConfig.SF_DEFAULT_SEARCH_DEPTH);
            }

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
            // lets wait and then execute
            float delay;

            if (aiMoveInputVO.isHint)
            {
                delay = AiMoveTimes.M_HINT;
            }
            else
            {
                delay = AiMoveTimes.M_CPU;
            }

            yield return new WaitForSecondsRealtime(delay);

            while (!resultsReady)
            {
                yield return null;
            }

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
