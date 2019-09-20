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

        public void NewGame()
        {
            plugin.NewGame();
            ChessAiPlugin.resultsReadySignal.RemoveAllListeners();
            ChessAiPlugin.resultsReadySignal.AddListener(ResultsReady);
        }

        public IPromise<FileRank, FileRank, string> GetAiMove(AiMoveInputVO vo)
        {
			MoveRequest request;
			request.promise = new Promise<FileRank, FileRank, string>();
			request.vo = vo;

            //routineRunner.StartCoroutine(ProcessQueue(request));
            //return aiMovePromise;


            aiMoveInputVO = request.vo;
            aiMovePromise = request.promise;
            routineRunner.StartCoroutine(GetAiResult());
            return aiMovePromise;
        }

        public IPromise<FileRank, FileRank, string> GetAiMoveStrength(AiMoveInputVO vo)
        {
            aiMoveStrengthPromise = new Promise<FileRank, FileRank, string>();
            aiMoveInputVO = vo;
            routineRunner.StartCoroutine(GetAiResult());
            return aiMoveStrengthPromise;
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
            if(aiMoveInputVO.isStrength)
            {
                int searchDepthRange = ChessAiConfig.SF_MAX_SEARCH_DEPTH - ChessAiConfig.SF_MIN_SEARCH_DEPTH;
                int searchDepthInt = ChessAiConfig.SF_MIN_SEARCH_DEPTH + Mathf.FloorToInt(aiMoveInputVO.playerStrengthPct * searchDepthRange);
                searchDepth = searchDepthInt.ToString();
                AiLog("isStrength searchDepth = " + searchDepth);
            }
            else
            {
                searchDepth = aiMoveInputVO.isHint ? ChessAiConfig.SF_MAX_SEARCH_DEPTH.ToString() : GetSearchDepth().ToString();
                AiLog("searchDepth = " + searchDepth);
            }

            
            plugin.GoDepth(searchDepth);

            while (!resultsReady)
            {
                yield return null;
            }

            ExecuteAiMove();
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
                GetBestMove();
            }
            else
            {
                SelectMove();
                aiMovePromise = null;
            }
            
        }

        private void GetBestMove()
        {
            var selectedMove = aiSearchResultMovesList[0];
            var from = chessService.GetFileRankLocation(selectedMove[0], selectedMove[1]);
            var to = chessService.GetFileRankLocation(selectedMove[2], selectedMove[3]);
            var piece = chessService.GetPieceNameAtLocation(from);

            //if piece is null then it means player had moved it in his last move
            //getting piece info from player's last move
            if (piece == null)
            {
                piece = aiMoveInputVO.squares[aiMoveInputVO.lastPlayerMove.to.file, aiMoveInputVO.lastPlayerMove.to.rank].piece.name;
            }

            aiMoveStrengthPromise.Dispatch(from, to, piece);
            aiMoveStrengthPromise = null;
        }

        private void GetMoveStrength()
        {
            double precentage = 0.0f;
            FileRank from = aiMoveInputVO.lastPlayerMove.from;
            FileRank to = aiMoveInputVO.lastPlayerMove.to;
            int totolMoveCount = aiSearchResultMovesList.Count - 1;

            if (totolMoveCount > 0)
            {
                string moveString = aiMoveInputVO.lastPlayerMove.MoveToString(from, to);
                int moveFoundIndex = -1;

                for (int i = 0; i < totolMoveCount; ++i)
                {
                    LogUtil.Log("j:" + i + " MOVES : " + aiSearchResultMovesList[i]);

                    if (string.Equals(moveString, aiSearchResultMovesList[i]))
                    {
                        moveFoundIndex = i;
                        break;
                    }
                }

                LogUtil.Log("moveString : " + moveString + " totolMoveCount : "+ totolMoveCount + " moveFoundIndex:"+ moveFoundIndex);

                if (moveFoundIndex >= 0)
                {
                    precentage = ((float)(totolMoveCount - moveFoundIndex) / (float)totolMoveCount);
                    precentage = Math.Round(precentage, 1) * 10;
                }
            }

            aiMoveStrengthPromise.Dispatch(from, to, ((float)precentage).ToString());
            aiMoveStrengthPromise = null;
        }
    }
}
