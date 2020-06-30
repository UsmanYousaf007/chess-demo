/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TurboLabz.TLUtils;
using GameSparks.Core;
using UnityEngine.Analytics;
using GameAnalyticsSDK;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        [Inject] public GameDisconnectingSignal gameDisconnectingSignal { get; set; }
        [Inject] public AppEventSignal appEventSignal { get; set;  }
        [Inject] public ModelsSaveToDiskSignal modelsSaveToDiskSignal { get; set; }
        [Inject] public ModelsResetSignal modelsResetSignal { get; set; }
        [Inject] public ModelsLoadFromDiskSignal modelsLoadFromDiskSignal { get; set; }
        [Inject] public ResumeMatchSignal resumeMatchSignal { get; set; }
        [Inject] public ChessboardBlockerEnableSignal chessboardBlockerEnableSignal { get; set; }
        [Inject] public ReconnectViewEnableSignal reconnectViewEnableSignal { get; set; }
        [Inject] public SyncReconnectDataSignal syncReconnectData { get; set; }

        private NavigatorViewId prevViewId;
        private BackendResult authBackendResult;

        public void MonitorConnectivity(bool enable)
        {
            GS.GameSparksAvailable -= MonitorGameSparksAvailable;
            GS.Instance.GameSparksAuthenticated -= MonitorGameSparksAuthenticated;

            if (enable)
            {
                GS.GameSparksAvailable += MonitorGameSparksAvailable;
            }
        }

        private void MonitorGameSparksAuthenticated(string id)
        {
            GS.Instance.GameSparksAuthenticated -= MonitorGameSparksAuthenticated;
            ProcessHardReconnection(BackendResult.SUCCESS);
        }

        void ProcessHardReconnection(BackendResult r)
        {
            // Stop GS connection monitoring.
            MonitorConnectivity(false);

            // Reset all models
            modelsResetSignal.Dispatch();
            // Load saved models (perfs etc)
            modelsLoadFromDiskSignal.Dispatch();

            // Begin processing hard reconnect
            resumeMatchSignal.Dispatch(prevViewId);
        }

        public IEnumerator SwithOffIsResumeCR()
        {
            yield return new WaitForSeconds(0.5f);
            appInfoModel.isResumeGS = false;
        }

        public void ScheduleSwitchOffResumeGS()
        {
            routineRunner.StartCoroutine(SwithOffIsResumeCR());
        }

        void ReportDisconnectAnalytics()
        {
            bool isResumeGS = appInfoModel.isResumeGS;

            // Log Forced Disconnect (app returning from background)
            if (appInfoModel.isResumeGS)
            {
                appInfoModel.isResumeGS = false;
                analyticsService.Event(AnalyticsEventId.gs_disconneced, AnalyticsContext.return_from_background);
            }
            // Log Disconnect during a multiplayer game
            else if (matchInfoModel.activeMatch != null)
            {
                if (matchInfoModel.activeMatch.isBotMatch)
                {
                    analyticsService.Event(AnalyticsEventId.gs_disconneced, AnalyticsContext.bot_match);
                }
                else if (matchInfoModel.activeMatch.isLongPlay)
                {
                    analyticsService.Event(AnalyticsEventId.gs_disconneced, AnalyticsContext.long_match);
                }
                else if (matchInfoModel.activeMatch.isTenMinGame)
                {
                    analyticsService.Event(AnalyticsEventId.gs_disconneced, AnalyticsContext.tenmin_match);
                }
                else if (matchInfoModel.activeMatch.isOneMinGame)
                {
                    analyticsService.Event(AnalyticsEventId.gs_disconneced, AnalyticsContext.onemin_match);
                }
                else if (matchInfoModel.activeMatch.isThirtyMinGame)
                {
                    analyticsService.Event(AnalyticsEventId.gs_disconneced, AnalyticsContext.thirtymin_match);
                }
                else
                {
                    analyticsService.Event(AnalyticsEventId.gs_disconneced, AnalyticsContext.quick_match);
                }
            }
            // Log Disconnect during a CPU game
            else if (navigatorModel.currentViewId == NavigatorViewId.CPU ||
                navigatorModel.currentViewId == NavigatorViewId.CPU_EXIT_DLG ||
                navigatorModel.currentViewId == NavigatorViewId.CPU_RESULTS_DLG)
            {
                analyticsService.Event(AnalyticsEventId.gs_disconneced, AnalyticsContext.cpu_match);
            }
            // Log Disconnect during matchmaking
            else if (navigatorModel.currentViewId == NavigatorViewId.MULTIPLAYER_FIND_DLG)
            {
                analyticsService.Event(AnalyticsEventId.gs_disconneced, AnalyticsContext.matchmaking);
            }
            // Log Disconnect during other screens
            else
            {
                analyticsService.Event(AnalyticsEventId.gs_disconneced, AnalyticsContext.not_in_game);
            }

            if (isResumeGS == false)
            {
                analyticsService.Event(AnalyticsEventId.gs_disconnected_unique);
                GameAnalytics.NewErrorEvent(GAErrorSeverity.Info, "[GS Unforced Disconnect] " + GameSparks.SocketStats.lastError);
            }
        }

        private void OnAuthComplete(BackendResult result)
        {
            authBackendResult = result;

            if (result != BackendResult.SUCCESS)
            {
                GS.Instance.GameSparksAuthenticated -= MonitorGameSparksAuthenticated;
                backendErrorSignal.Dispatch(result);
            }
        }

        void MonitorGameSparksAvailable(bool isAvailable)
        {
            if (isAvailable)
            {
                LogUtil.Log("GS Connected", "red");

                GS.Instance.GameSparksAuthenticated += MonitorGameSparksAuthenticated;

                string fbAccessToken = facebookService.GetAccessToken();
                if (fbAccessToken == null)
                {
                    AuthGuest().Then(OnAuthComplete);
                }
                else
                {
                    AuthFacebook(fbAccessToken, true).Then(OnAuthComplete);
                }
            }
            else
            {
                LogUtil.Log("GS Disconnected", "red");

                ReportDisconnectAnalytics();

                GS.Reset();

                // Avoid soft reconnect processing
                OnlineCheckerStop();

                if (appInfoModel.isReconnecting == DisconnectStates.FALSE)
                {
                    appInfoModel.reconnectTimeStamp = TimeUtil.unixTimestampMilliseconds;
                }

                appInfoModel.isReconnecting = DisconnectStates.LONG_DISCONNET;
                analyticsService.Event(AnalyticsEventId.reconnection_shown, AnalyticsContext.gs_disconnect);

                reconnectViewEnableSignal.Dispatch(true);
                chessboardBlockerEnableSignal.Dispatch(true);

                // Stop the pinger
                StopPinger();

                // Reconnect processing depends on last view
                prevViewId = navigatorModel.currentViewId;
                // Remove pending requests processing
                if (appInfoModel.syncInProgress == false)
                {
                    GSFrameworkRequest.CancelRequestSession();
                }
                // Data Sync was cancelled 
                appInfoModel.syncInProgress = false;
                // Dispatch signal that we are in reconnection
                gameDisconnectingSignal.Dispatch();
                // Save models to disk to reload when coming back from background
                modelsSaveToDiskSignal.Dispatch();
                // Avoid processing gs messages that arrive right after gs becomes available
                // because models will be reset and those messages will no longer be valid. The
                // state will be obtained from full init data.
                RemoveChallengeListeners();
            }
        }
    }
}
