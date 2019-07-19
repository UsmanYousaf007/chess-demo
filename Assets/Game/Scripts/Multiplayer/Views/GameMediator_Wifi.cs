/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-03-17 12:36:54 UTC+05:00
/// 
/// @description
/// [add_description_here]
using System;
using GameSparks.Core;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using UnityEngine;

namespace TurboLabz.Multiplayer
{
    public partial class GameMediator
    {
        // Dispatch Signals
        [Inject] public StartGameSignal startGameSignal { get; set; }

        // Models
        [Inject] public IChessboardModel chessboardModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IAppInfoModel appInfoModel { get; set; }

        // Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }

        public void OnRegisterWifi()
        {
            view.InitWifi();
        }

        [ListensTo(typeof(WifiIsHealthySignal))]
        public void OnWifiHealthUpdate(bool isHealthy)
        {
            if (matchInfoModel.activeChallengeId != null)
            {
                view.WifiHealthUpdate(isHealthy);
            }
        }

        private void OnInternetConnectedTicked(bool isConnected, InternetReachabilityMonitor.ConnectionSwitchType connectionSwitch)
        {
            if (connectionSwitch == InternetReachabilityMonitor.ConnectionSwitchType.FROM_CONNECTED_TO_DISCONNECTED)
            {
                if (matchInfoModel.activeChallengeId != null)
                {
                    TLUtils.LogUtil.Log("Match disconnected", "cyan");
                    GSFrameworkRequest.CancelRequestSession();
                    stopTimersSignal.Dispatch();
                    view.FlashClocks(true);

                    appInfoModel.reconnectTimeStamp = TimeUtil.unixTimestampMilliseconds;
                }
            }
            else
            if (connectionSwitch == InternetReachabilityMonitor.ConnectionSwitchType.FROM_DISCONNECTED_TO_CONNECTED)
            {
                if (matchInfoModel.activeChallengeId != null)
                {
                    TLUtils.LogUtil.Log("Match reconnecting..", "cyan");
                    backendService.SyncReconnectData(matchInfoModel.activeChallengeId).Then(OnSycReconnectionData);
                }
            }
        }

        private void OnSycReconnectionData(BackendResult backendResult)
        {
            if (backendResult == BackendResult.CANCELED)
            {
                TLUtils.LogUtil.Log("Match: Canceled OnSycReconnectionData!", "cyan");
                return;
            }

            // Record analytics
            TimeSpan totalSeconds = TimeSpan.FromMilliseconds(TimeUtil.unixTimestampMilliseconds - appInfoModel.reconnectTimeStamp);
            analyticsService.Event(AnalyticsEventId.disconnection_time, AnalyticsParameter.count, totalSeconds.Seconds);
            TLUtils.LogUtil.Log("Reconnection Time Seconds = " + totalSeconds.Seconds, "cyan");

            if (backendResult == BackendResult.REQUEST_TIMEOUT)
            {
                TLUtils.LogUtil.Log("Match: Go back to match without data sync!", "cyan");
                view.WifiHealthUpdate(true);
                view.FlashClocks(false);
                return;
            }

            TLUtils.LogUtil.Log("Match reconnected", "cyan");

            Chessboard activeChessboard = chessboardModel.chessboards[matchInfoModel.activeChallengeId];
            MatchInfo activeMatchInfo = matchInfoModel.activeMatch;
            activeChessboard.currentState = null;

            startGameSignal.Dispatch();
            SendReconnectionAck();

            view.WifiHealthUpdate(true);
            view.FlashClocks(false);
        }

        private void SendReconnectionAck()
        {
            // Send ack on resume for quick match. This ensures that even if watchdog ping was missed
            // during disconnection, this ack will resume proper watchdog operation on server
            string challengeId = matchInfoModel.activeChallengeId;
            if (matchInfoModel.activeMatch != null && !matchInfoModel.activeMatch.isLongPlay)
            {
                string challengedId = matchInfoModel.activeMatch.challengedId;
                string challengerId = matchInfoModel.activeMatch.challengerId;
                bool isPlayerTurn = chessboardModel.chessboards[challengeId].isPlayerTurn;

                string currentTurnPlayerId;
                bool isPlayerChallenger = challengerId == playerModel.id;
                if (isPlayerTurn)
                {
                    currentTurnPlayerId = isPlayerChallenger == true ? challengerId : challengedId;
                }
                else
                {
                    currentTurnPlayerId = isPlayerChallenger == true ? challengedId : challengerId;
                }

                backendService.MatchWatchdogPingAck(currentTurnPlayerId, challengerId, challengedId, challengeId);
            }
        }
    }
}
