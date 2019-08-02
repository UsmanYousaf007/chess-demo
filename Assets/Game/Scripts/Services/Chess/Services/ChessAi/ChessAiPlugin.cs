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
using System.Runtime.InteropServices;
using TurboLabz.TLUtils;
using strange.extensions.signal.impl;
using UnityEngine;
using AOT;

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
        #elif UNITY_IOS
        public const string PLUGIN_NAME = "__Internal";
        #endif

        //#if !UNITY_IOS
        //[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        //#endif
        public delegate void UnityOutDelegate(string str);

       // #if UNITY_IOS
        [DllImport (PLUGIN_NAME)]
        public static extern void tl_setUnityOutFuncPtr(UnityOutDelegate callback);
       // #else
       // [DllImport (PLUGIN_NAME)]
      //  public static extern void tl_setUnityOutFuncPtr(IntPtr fp);
       // #endif

        [DllImport (PLUGIN_NAME)]
        private static extern void tl_echo(string arg);

        [DllImport (PLUGIN_NAME)]
        private static extern void tl_init();

        [DllImport (PLUGIN_NAME)]
        private static extern void tl_cmd(string arg);

        [DllImport (PLUGIN_NAME)]
        private static extern void tl_shutdown();

        private static bool isInitialized;

        public void NewGame()
        {
            if (!isInitialized)
            {
                // Set the callback to Unity function
                //#if UNITY_IOS
                tl_setUnityOutFuncPtr(UnityOutCallback);
               // #else
                //UnityOutDelegate callback_delegate = new UnityOutDelegate(UnityOutCallback);
                //IntPtr intptr_delegate = Marshal.GetFunctionPointerForDelegate(callback_delegate);
                //tl_setUnityOutFuncPtr(intptr_delegate);
                //#endif

                // Initialize the plugin once
                tl_init();
                isInitialized = true;
            }

            tl_cmd("ucinewgame");
            SetContempt(ChessAiConfig.SF_CONTEMPT);
            SetPonder(ChessAiConfig.SF_PONDER);
            SetMultiPV(ChessAiConfig.SF_MULTIPV);
            SetSkillLevel(ChessAiConfig.SF_SKILL_LEVEL);
            SetThreads(SystemInfo.processorCount.ToString());
        }

        public void Shutdown()
        {
            // Shutdown the plugin
            if (isInitialized)
            {
                isInitialized = false;
                tl_shutdown();
            }
        }

        public void SetPosition(string FEN)
        {
            tl_cmd("position fen " + FEN);
        }

        private void SetContempt(string contempt)
        {
            tl_cmd("setoption name Contempt value " + contempt);
        }

        private void SetPonder(string ponder)
        {
            tl_cmd("setoption name Ponder value " + ponder);
        }

        private void SetMultiPV(string multiPV)
        {
            tl_cmd("setoption name MultiPV value " + multiPV);
        }

        private void SetSkillLevel(string skill)
        {
            tl_cmd("setoption name Skill Level value " + skill);
        }

        private void SetSlowMover(string slowMoverAmt)
        {
            tl_cmd("setoption name Slow Mover value " + slowMoverAmt);
        }

        private void SetHash(string hash)
        {
            tl_cmd("setoption name Hash value " + hash);
        }

        private void SetThreads(string threads)
        {
            // DO NOT USE, CRASHES ON IOS. UNKNOWN BEHAVIOR ON ANDROID PHONES.
            // cmd("setoption name Threads value " + threads);
        }

        public void GoDepth(string depth)
        {
            results.aiBestMoveStr = null;
            results.aiSearchResultMovesStr = null;
            results.aiSearchResultScoresStr = null;

            tl_cmd("go depth " + depth);
        }

        public void Stop()
        {
            tl_cmd("stop");
        }

       // #if UNITY_IOS
        [MonoPInvokeCallback(typeof(UnityOutDelegate))]
        //#endif
        private static void UnityOutCallback(string str)
        {
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
