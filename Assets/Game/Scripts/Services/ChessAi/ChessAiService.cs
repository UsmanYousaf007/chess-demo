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

using System;
using System.Collections;
using System.Runtime.InteropServices;

using UnityEngine;

using strange.extensions.promise.api;
using strange.extensions.promise.impl;

using TurboLabz.Common;

namespace TurboLabz.Chess
{
    public partial class ChessAiService : IChessAiService
    {
        #if UNITY_EDITOR
        public const string PLUGIN_NAME = "osx-ai";
        #elif UNITY_ANDROID
        public const string PLUGIN_NAME = "android-ai";
        #endif

        [DllImport (PLUGIN_NAME)]
        public static extern void setUnityOutFuncPtr( IntPtr fp );

        [DllImport (PLUGIN_NAME)]
        private static extern void echo( string arg );

        [DllImport (PLUGIN_NAME)]
        private static extern void init();

        [DllImport (PLUGIN_NAME)]
        private static extern void cmd( string arg );

        [DllImport (PLUGIN_NAME)]
        private static extern void shutdown();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void UnityOutDelegate(string str);

        [Inject] public IRoutineRunner routineRunner { get; set; }
        [Inject] public IChessService chessService { get; set; }

        public bool isInitialized { get; set; }

        private IPromise<FileRank, FileRank, string> aiMovePromise; 
        private static string aiSearchResultMovesStr;
        private static string aiSearchResultScoresStr;
        private static string aiBestMoveStr;
        private static bool aiResultsReady = false;

        private AiMoveInputVO aiMoveInputVO;
        private AiOverrideStrength overrideStrength;

        public void NewGame()
        {
            if (!isInitialized)
            {
                // Set the callback to Unity function
                UnityOutDelegate callback_delegate = new UnityOutDelegate(UnityOutCallback);
                IntPtr intptr_delegate = Marshal.GetFunctionPointerForDelegate(callback_delegate);
                setUnityOutFuncPtr(intptr_delegate);

                // Initialize the plugin once
                init();
                isInitialized = true;
            }

            cmd("ucinewgame");
            SetSkill(0);
        }

        public IPromise<FileRank, FileRank, string> GetAiMove(AiMoveInputVO vo)
        {
            return ExecGetAiMove(vo, AiOverrideStrength.NONE);
        }

        public IPromise<FileRank, FileRank, string> GetAiMove(AiMoveInputVO vo, AiOverrideStrength overrideStrength)
        {
            return ExecGetAiMove(vo, overrideStrength);
        }

        public IPromise<FileRank, FileRank, string> ExecGetAiMove(AiMoveInputVO vo, AiOverrideStrength overrideStrength)
        {
            // Save inputs
            this.aiMoveInputVO = vo;
            this.overrideStrength = overrideStrength;

            // Setup move search parameters. Near the end game, the AI looks very
            // abnormal if it does not make the optimal move. Therefore we set the
            // move search count to 1 which ensures that the Ai makes the best move
            // possible. This is mainly due to how the player perceives the game
            // on an empty chessboard, i.e, stupid moves look very stupid.
            if (ReachedEndGame())
            {
                SetAnalysisMovesCount(1);
            }
            else
            {
                SetAnalysisMovesCount(MOVES_TO_GENERATE);
            }

            SetContempt(0); // Do not change this, we don't know what it does at other values.

            // Execute the search
            // We set a strong search depth for one minute games because the ai engine
            // exhibits focused aggressive behaviour with that search depth. Meaning instead
            // of beating around the bush, it will go straight to attack your prime pieces
            // which is the general way players approach a 1 min game. This is especially
            // important to counter time hackers that make random moves so that the opponent's
            // clock runs out. TODO: verify whether this comment is true 
            if (vo.timeControl == AiTimeControl.ONE_MINUTE)
            {
                cmd("go depth " + ONE_MIN_SEARCH_DEPTH);
            }
            else
            {
                cmd("go depth " + DEFAULT_SEARCH_DEPTH);
            }

            // Read the results
            aiMovePromise = new Promise<FileRank, FileRank, string>();
            routineRunner.StartCoroutine(GetAiResult(aiMoveInputVO.timeControl, aiMoveInputVO.aiMoveNumber));
            return aiMovePromise;
        }

        public void Shutdown()
        {
            // Shutdown the plugin
            shutdown();
            isInitialized = false;
        }

        public void SetPosition(string FEN)
        {
            cmd("position fen " + FEN);
        }

        /// <summary>
        /// Sets the contempt.
        /// </summary>
        /// <param name="contempt">Min: -100, Max: 100. Positive values of contempt 
        /// favor more "risky" play, while negative values will favor draws. Zero 
        /// is neutral.</param>
        private void SetContempt(int contempt)
        {
            Assertions.Assert((contempt >= -100 && contempt <= 100), "Invalid contempt value.");
            cmd("setoption name Contempt value " + contempt);
        }

        /// <summary>
        /// Sets the skill.
        /// </summary>
        /// <param name="skill">Min: 0, Max: 20. How well you want the AI to play.
        /// At level 0, the AI will make dumb moves. Level 20 is best/strongest play.</param>
        private void SetSkill(int skill)
        {
            Assertions.Assert((skill >= 0 && skill <= 19), "Invalid skill value.");
            cmd("setoption name Skill Level value " + skill);
        }

        private void SetAnalysisMovesCount(int analysisMovesCount)
        {
            Assertions.Assert((analysisMovesCount >= 1 && analysisMovesCount <= 500), "Invalid analysisMovesCount value:" + analysisMovesCount);
            cmd("setoption name MultiPV value " + analysisMovesCount);
        }

        private static void UnityOutCallback(string str)
        {
            if (str.Contains("moveOptions"))
            {
                aiSearchResultMovesStr = str;
            }
            else if (str.Contains("moveScores"))
            {
                aiSearchResultScoresStr = str;
            }
            else if (str.Contains("bestmove"))
            {
                aiBestMoveStr = str;
            }

            if (aiSearchResultMovesStr != null &&
                aiSearchResultScoresStr != null &&
                aiBestMoveStr != null)
            {
                aiResultsReady = true;
            }
        }

        private IEnumerator GetAiResult(AiTimeControl timeControl, int aiMoveNumber)
        {
            while (!aiResultsReady)
            {
                yield return null;
            }

            float delay = aiMoveInputVO.defaultMoveDelay;

            if (Debug.isDebugBuild && aiMoveInputVO.overrideSpeed == AiOverrideSpeed.FAST)
            {
                delay = AiMoveTimes.M_FAST;
            }
            else if (Debug.isDebugBuild && aiMoveInputVO.overrideSpeed == AiOverrideSpeed.SLOW)
            {
                delay = AiMoveTimes.M_SLOW;
            }
            else if (timeControl == AiTimeControl.CPU)
            {
                delay = AiMoveTimes.M_CPU;
            }
            else if (timeControl == AiTimeControl.HINT)
            {
                delay = AiMoveTimes.M_FAST;
            }
            else
            {
                // lets figure out how many seconds we want to wait...
                float [][] moveTimes = null;

                if (timeControl == AiTimeControl.ONE_MINUTE)
                {
                    moveTimes = AiMoveTimes.M_1;
                }
                else if (timeControl == AiTimeControl.THREE_MINUTE)
                {
                    moveTimes = AiMoveTimes.M_3;
                }
                else if (timeControl == AiTimeControl.FIVE_MINUTE)
                {
                    moveTimes = AiMoveTimes.M_5;
                }
                else if (timeControl == AiTimeControl.TEN_MINUTE) 
                {
                    moveTimes = AiMoveTimes.M_10;
                }
                else
                {
                    // This area is supposed to be for debugging purposes.
                    LogUtil.LogWarning("Unsupported time control: timeControl value is '" + timeControl + "'");
                    moveTimes = new float[][] { new float[] { 1, 2 } };
                }

                float moveTimeDelayMin = 0;
                float moveTimeDelayMax = 0;

                if (aiMoveNumber > moveTimes.Length)
                {
                    moveTimeDelayMin = moveTimes[moveTimes.Length - 1][0];
                    moveTimeDelayMax = moveTimes[moveTimes.Length - 1][1];
                }
                else
                {
                    moveTimeDelayMin = moveTimes[aiMoveNumber - 1][0];
                    moveTimeDelayMax = moveTimes[aiMoveNumber - 1][1];
                }

                // pick a random time from the min max
                delay = UnityEngine.Random.Range(moveTimeDelayMin, moveTimeDelayMax);
            }

            // lets wait and then execute
            yield return new WaitForSecondsRealtime(delay);

            ExecuteAiMove();
        }

        private void ExecuteAiMove()
        {
            SelectMove();

            aiBestMoveStr = null;
            aiSearchResultMovesStr = null;
            aiSearchResultScoresStr = null;
            aiMovePromise = null;
            aiResultsReady = false;
        }
    }
}
