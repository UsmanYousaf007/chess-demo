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
using System;

namespace TurboLabz.Chess
{
    public partial class ChessAiService : IChessAiService
    {
        [Inject] public IRoutineRunner routineRunner { get; set; }
        [Inject] public IChessService chessService { get; set; }

        struct MoveRequest
        {
            public IPromise<FileRank, FileRank, string> promise;
            public AiMoveInputVO vo;
        }

        public bool Busy
        {
            get
            {
                return aiMovePromise != null;
            }
        }

        private IPromise<FileRank, FileRank, string> aiMovePromise;
        private IPromise<FileRank, FileRank, string> aiMoveStrengthPromise;
        private AiMoveInputVO aiMoveInputVO;
        private ChessAiPlugin plugin = new ChessAiPlugin();
        private bool resultsReady;

        public void NewGame(string multiPV = ChessAiConfig.SF_MULTIPV)
        {
            plugin.NewGame(multiPV);
            ChessAiPlugin.resultsReadySignal.RemoveAllListeners();
            ChessAiPlugin.resultsReadySignal.AddListener(ResultsReady);
        }

        public IPromise<FileRank, FileRank, string> GetAiMove(AiMoveInputVO vo)
        {
            return AddToQueue(_GetAiMove, vo);
        }

        public IPromise<FileRank, FileRank, string> GetAiMoveStrength(AiMoveInputVO vo)
        {
            return AddToQueue(_GetAiMoveStrength, vo);
        }

        private void _GetAiMove(AiMoveInputVO vo)
        {
			MoveRequest request;
			request.promise = new Promise<FileRank, FileRank, string>();
			request.vo = vo;
            SetPosition(vo.fen);
            aiMoveInputVO = request.vo;
            routineRunner.StartCoroutine(GetAiResult());
        }

        private void _GetAiMoveStrength(AiMoveInputVO vo)
        {
            NewGame();
            SetPosition(vo.fen);
            aiMoveInputVO = vo;
            routineRunner.StartCoroutine(GetAiResult());
        }

        //private IEnumerator ProcessQueue(MoveRequest request)
        //{
        //	while (Busy)
        //	{
        //		yield return null;
        //	}
        //
        //	aiMoveInputVO = request.vo;
        //	aiMovePromise = request.promise;
        //	routineRunner.StartCoroutine(GetAiResult());
        //}

        private IEnumerator GetAiResult()
        {
            // Clear the results flag
            resultsReady = false;
            string searchDepth;

            // Execute the move
            searchDepth = (aiMoveInputVO.isStrength || aiMoveInputVO.isHint) ? ChessAiConfig.SF_MAX_SEARCH_DEPTH.ToString() :
                aiMoveInputVO.analyse ? ChessAiConfig.SF_ANALYSIS_SEARCH_DEPTH.ToString() : GetSearchDepth().ToString();

            AiLog("searchDepth = " + searchDepth);
            
            plugin.GoDepth(searchDepth);

            while (!resultsReady)
            {
                yield return null;
            }

            // TODO: Not the ideal design but we want to wait a frame here
            // since executeaimove can theoretically null the original promise returned in the GetAiMove
            // if it all happens in the same frame. So we just wait for one frame here so that the
            // promise returned is never null.
            yield return null;

            ExecuteAiMove();
            taskIsReadyToExecute = true;
            ExecuteQueue();
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

            if (aiMoveInputVO.isStrength)
            {
                GetMoveStrength();
            }
            else if (aiMoveInputVO.isHint)
            {
                //GetBestMove();
                GetHint();
            }
            else if (aiMoveInputVO.analyse)
            {
                GetMoveAnalysis();
            }
            else
            {
                SelectMove();
                aiMovePromise = null;
            }
            
        }
    }
}
