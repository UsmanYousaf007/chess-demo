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

using TurboLabz.TLUtils;
using strange.extensions.signal.impl;

namespace TurboLabz.Chess
{
    public class ChessAiPlugin
    {
        public static Signal resultsReadySignal = new Signal();
        public static AiResults results;
        public struct AiResults
        {
            public string aiSearchResultMovesStr;
            public string aiSearchResultScoresStr;
            public string aiBestMoveStr;
        }



        #if UNITY_EDITOR
        public const string PLUGIN_NAME = "macos-ai";
        #elif UNITY_ANDROID
        public const string PLUGIN_NAME = "android-ai";
        #endif

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void UnityOutDelegate(string str);

        [DllImport (PLUGIN_NAME)]
        public static extern void setUnityOutFuncPtr(IntPtr fp);

        [DllImport (PLUGIN_NAME)]
        private static extern void echo(string arg);

        [DllImport (PLUGIN_NAME)]
        private static extern void init();

        [DllImport (PLUGIN_NAME)]
        private static extern void cmd(string arg);

        [DllImport (PLUGIN_NAME)]
        private static extern void shutdown();

        private static bool isInitialized;

        public  ChessAiPlugin()
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
            SetContempt(ChessAiConfig.SF_CONTEMPT);
            SetPonder(ChessAiConfig.SF_PONDER);
            SetMultiPV(ChessAiConfig.SF_MULTIPV);
            SetSkillLevel(ChessAiConfig.SF_SKILL_LEVEL);
            SetSlowMover(ChessAiConfig.SF_SLOW_MOVER);
        }

        public void Shutdown()
        {
            // Shutdown the plugin
            if (isInitialized)
            {
                shutdown();
                isInitialized = false;
            }
        }

        public void SetPosition(string FEN)
        {
            cmd("position fen " + FEN);
        }

        private void SetContempt(string contempt)
        {
            cmd("setoption name Contempt value " + contempt);
        }

        private void SetPonder(string ponder)
        {
            cmd("setoption name Ponder value " + ponder);
        }

        private void SetMultiPV(string multiPV)
        {
            cmd("setoption name MultiPV value " + multiPV);
        }

        private void SetSkillLevel(string skill)
        {
            cmd("setoption name Skill Level value " + skill);
        }

        private void SetSlowMover(string slowMoverAmt)
        {
            cmd("setoption name Slow Mover value " + slowMoverAmt);
        }

        public void GoDepth(string depth)
        {
            results.aiBestMoveStr = null;
            results.aiSearchResultMovesStr = null;
            results.aiSearchResultScoresStr = null;

            cmd("go depth " + depth);
        }

        private static void UnityOutCallback(string str)
        {
            LogUtil.Log("Unity out: " + str, "cyan");

            if (str.Contains("moveOptions"))
            {
                results.aiSearchResultMovesStr = str;
            }
            else if (str.Contains("moveScores"))
            {
                results.aiSearchResultScoresStr = str;
            }
            else if (str.Contains("bestmove"))
            {
                results.aiBestMoveStr = str;
            }

            if (results.aiSearchResultMovesStr != null &&
                results.aiSearchResultScoresStr != null &&
                results.aiBestMoveStr != null)
            {
                resultsReadySignal.Dispatch();
            }
        }
    }
}
