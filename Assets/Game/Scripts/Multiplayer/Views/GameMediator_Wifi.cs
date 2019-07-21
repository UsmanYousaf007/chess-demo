﻿/// @license Propriety <http://license.url>
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
        /*
         * Disconnection
         * Internet Reachability module dispatches the registered handler which has a parameter to tell if there was a switch over 
         * to connection or disconnection. A disconnection triggers the set up for a match disconnection.
         * 
         * Soft Reconnect
         * A soft reconnect occurs before GS triggers in-availability. When internet becomes unreachable GS may not trigger in-availability right away.
         * The Internet Reachability module calls GS.Reconnect when internet becomes available. GameSparks incoming pending messages will be recieved when GS.Reconnect
         * completes its processing. These will include challenge messages such as TurnTaken. On reconnect SyncReconnectData is called to retrieve the latest state
         * of the active challenge and it is then applied.        
         * 
         * Note: [This following method did not work and needs further investigation!] The game mediator however will get its registered handler trigger right away on 
         * internet availability. If it is the player's turn, the match state is updated immediatedly. After a challenge message is processed it dispatches the 
         * ChallengeMessageProcessedSignal which is listened to by this game mediator. On a message processessed signal, if the game is in reconnection state, then the match state is updated.
         * 
         * Hard Reconnect
         * The hard reconnection occurs if GS triggers in-availability. The hard reconnect is handled by the GS_Service_MonitorConnectivity module. In hard recoonect,
         * all models are reset and GetInitData is initiated when GS triggers availability. The game goes through the Reception state and if a match is active then this 
         * match is started in it's state received from the server.
         * 
        */

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

        // private bool matchReconnection = false;

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

        /*
        [ListensTo(typeof(ChallengeMessageProcessedSignal))]
        public void ChallengeMessagedProcessed(string challengeId)
        {
            if (challengeId == matchInfoModel.activeChallengeId && matchReconnection)
            {
                LogUtil.Log("ChallengeMessagedProcessed called..", "cyan");
                ReconnectMatch();
                //matchReconnection = false;
            }
        }
        */

        private void OnInternetConnectedTicked(bool isConnected, InternetReachabilityMonitor.ConnectionSwitchType connectionSwitch)
        {
            if (connectionSwitch == InternetReachabilityMonitor.ConnectionSwitchType.FROM_CONNECTED_TO_DISCONNECTED)
            {
                if (matchInfoModel.activeChallengeId != null)
                {
                    TLUtils.LogUtil.Log("Match disconnected Id: " + matchInfoModel.activeChallengeId, "cyan");
                    GSFrameworkRequest.CancelRequestSession();
                    stopTimersSignal.Dispatch();
                    view.FlashClocks(true);
                    appInfoModel.reconnectTimeStamp = TimeUtil.unixTimestampMilliseconds;
                    //matchReconnection = true;
                }
            }
            else
            if (connectionSwitch == InternetReachabilityMonitor.ConnectionSwitchType.FROM_DISCONNECTED_TO_CONNECTED)
            {
                view.WifiHealthUpdate(true);

                if (matchInfoModel.activeChallengeId != null)
                {
                    LogUtil.Log("Match reconnecting..", "cyan");
                    //if (chessboardModel.chessboards[matchInfoModel.activeChallengeId].isPlayerTurn)
                    //{
                    //    ReconnectMatch();
                    //}
                    //else
                    //{
                        backendService.SyncReconnectData(matchInfoModel.activeChallengeId).Then(OnSycReconnectionData);
                    //}
                }
            }
        }

        private void OnSycReconnectionData(BackendResult backendResult)
        {
            if (backendResult == BackendResult.SUCCESS)
            {
                ReconnectMatch();
            }
            else
            {
                LogUtil.Log("Match: May be Canceled or Timed Out OnSycReconnectionData!", "cyan");
            }
        }

        private void ReconnectMatch()
        {
            LogUtil.Log("Match reconnected Id: " + matchInfoModel.activeChallengeId, "cyan");

            Chessboard activeChessboard = chessboardModel.chessboards[matchInfoModel.activeChallengeId];
            MatchInfo activeMatchInfo = matchInfoModel.activeMatch;
            activeChessboard.currentState = null;

            startGameSignal.Dispatch();
            SendReconnectionAck();
            view.FlashClocks(false);
            //matchReconnection = false;

            // Record analytics
            TimeSpan totalSeconds = TimeSpan.FromMilliseconds(TimeUtil.unixTimestampMilliseconds - appInfoModel.reconnectTimeStamp);
            analyticsService.Event(AnalyticsEventId.disconnection_time, AnalyticsParameter.count, totalSeconds.Seconds);
            LogUtil.Log("Reconnection Time Seconds = " + totalSeconds.Seconds, "cyan");
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
