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
using TurboLabz.Chess;
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
        [Inject] public RunTimeControlSignal runTimeControlSignal { get; set; }
        [Inject] public ReconnectViewEnableSignal reconnectViewEnableSignal { get; set; }


        // Models
        [Inject] public IChessboardModel chessboardModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IAppInfoModel appInfoModel { get; set; }

        // Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }

        private bool matchReconnection = false;

        public void OnRegisterWifi()
        {
            view.InitWifi();
        }

        [ListensTo(typeof(GameDisconnectingSignal))]
        public void OnGameDisconnecting()
        {
            if (gameObject.activeSelf && matchInfoModel.activeChallengeId != null)
            {
                stopTimersSignal.Dispatch();
                view.FlashClocks(true);
            }
        }

        [ListensTo(typeof(ReconnectionCompleteSignal))]
        public void OnReconnectionComplete()
        {
            if (gameObject.activeSelf)
            {
                view.FlashClocks(false);
            }
        }

        [ListensTo(typeof(ChallengeMessageProcessedSignal))]
        public void ChallengeMessagedProcessed(string challengeId)
        {
            if ((appInfoModel.gameMode != GameMode.NONE) &&
                (challengeId == matchInfoModel.activeChallengeId) && 
                (matchReconnection == true) &&
                (appInfoModel.isReconnecting != DisconnectStates.LONG_DISCONNET))
            {
                LogUtil.Log("ChallengeMessagedProcessed called...", "cyan");
                //stopTimersSignal.Dispatch();
                //ReconnectMatch();

                stopTimersSignal.Dispatch();
                RunTimeControlVO vo;
                vo.pauseAfterSwap = false;
                vo.waitingForOpponentToAccept = false;
                vo.playerJustAcceptedOnPlayerTurn = false;
                runTimeControlSignal.Dispatch(vo);
            }

            matchReconnection = false;
        }

        // Failed server requests will trigger this signal
        [ListensTo(typeof(SyncReconnectDataSignal))]
        public void SyncReconnectData(string challengeId)
        {
            backendService.OnlineCheckerStop();

            view.EnableSynchMovesDlg(true);

            OnInternetConnectedTicked(false, ConnectionSwitchType.FROM_CONNECTED_TO_DISCONNECTED);
            OnInternetConnectedTicked(true, ConnectionSwitchType.FROM_DISCONNECTED_TO_CONNECTED);

            backendService.OnlineCheckerStart();
        }

        // Chess board needs to be synched on app resume to keep clocks in synch with server
        // Only meant for app going into background and back
        [ListensTo(typeof(AppEventSignal))]
        public void OnAppEvent(AppEvent evt)
        {
            if (!gameObject.activeSelf ||
                evt != AppEvent.RESUMED ||
                appInfoModel.syncInProgress ||
                matchInfoModel.activeChallengeId == null ||
                !chessboardModel.isValidChallenge(matchInfoModel.activeChallengeId) ||
                appInfoModel.isReconnecting == DisconnectStates.LONG_DISCONNET)
            {
                return;
            }

            LogUtil.Log("OnAppEvent called for match app returning from background..", "cyan");
            matchReconnection = true;
        }

        private void OnSlowInternet(bool isSlowInternet)
        {
            if (matchInfoModel.activeChallengeId != null)
            {
                view.WifiHealthUpdate(!isSlowInternet);
            }
        }

        private void OnInternetConnectedTicked(bool isConnected, ConnectionSwitchType connectionSwitch)
        {
            // No need to sync an ended match
            if (matchInfoModel.activeChallengeId == null )
            {
                return;
            }

            if (appInfoModel.isReconnecting == DisconnectStates.LONG_DISCONNET)
            {
                TLUtils.LogUtil.Log("Skip match soft reconnect");
                return;
            }

            if (connectionSwitch == ConnectionSwitchType.FROM_CONNECTED_TO_DISCONNECTED)
            {
                if (matchInfoModel.activeChallengeId != null)
                {
                    appInfoModel.syncInProgress = true;
                    reconnectViewEnableSignal.Dispatch(true);
                    view.chessboardBlocker.SetActive(true);
                    TLUtils.LogUtil.Log("Match disconnected Id: " + matchInfoModel.activeChallengeId, "cyan");
                    GSFrameworkRequest.CancelRequestSession();
                    stopTimersSignal.Dispatch();
                    view.FlashClocks(true);
                    appInfoModel.reconnectTimeStamp = TimeUtil.unixTimestampMilliseconds;
                    matchReconnection = false;
                }
            }
            else if (connectionSwitch == ConnectionSwitchType.FROM_DISCONNECTED_TO_CONNECTED)
            {
                if (matchInfoModel.activeChallengeId != null)
                {
                    if (appInfoModel.isReconnecting != DisconnectStates.LONG_DISCONNET)
                    {
                        LogUtil.Log("Match reconnecting..", "cyan");
                        backendService.SyncReconnectData(matchInfoModel.activeChallengeId).Then(OnSycReconnectionData);
                    }
                }
            }
        }

        private void OnSycReconnectionData(BackendResult backendResult)
        {
            if (backendResult == BackendResult.SUCCESS)
            {
                if (appInfoModel.isReconnecting != DisconnectStates.LONG_DISCONNET)
                {
                    ReconnectMatch();
                }
            }
            else
            {
                LogUtil.Log("Match: May be Canceled or Timed Out OnSycReconnectionData!", "cyan");
            }

            view.EnableSynchMovesDlg(false);
            appInfoModel.syncInProgress = false;

            if (appInfoModel.isReconnecting != DisconnectStates.LONG_DISCONNET)
            {
                reconnectViewEnableSignal.Dispatch(false);
                view.FlashClocks(false);
                view.chessboardBlocker.SetActive(false);
            }
        }

        private void ReconnectMatch()
        {
            LogUtil.Log("Match reconnected Id: " + matchInfoModel.activeChallengeId, "cyan");

            // Reset the match if it has not ended
            if (matchInfoModel.activeChallengeId != null && chessboardModel.isValidChallenge(matchInfoModel.activeChallengeId))
            {
                Chessboard activeChessboard = chessboardModel.chessboards[matchInfoModel.activeChallengeId];
                MatchInfo activeMatchInfo = matchInfoModel.activeMatch;
                activeChessboard.currentState = null;

                startGameSignal.Dispatch();
                SendReconnectionAck();
            }

            view.FlashClocks(false);
            // Record analytics
            TimeSpan totalSeconds = TimeSpan.FromMilliseconds(TimeUtil.unixTimestampMilliseconds - appInfoModel.reconnectTimeStamp);
            analyticsService.Event(AnalyticsEventId.disconnection_time, AnalyticsParameter.count, totalSeconds.Seconds);
            LogUtil.Log("ReconnectMatch() Reconnection Time Seconds = " + totalSeconds.Seconds, "cyan");
            view.chessboardBlocker.SetActive(false);
        }

        private void SendReconnectionAck()
        {
            // Send ack on resume for quick match. This ensures that even if watchdog ping was missed
            // during disconnection, this ack will resume proper watchdog operation on server
            string challengeId = matchInfoModel.activeChallengeId;
            if (matchInfoModel.activeMatch != null && !matchInfoModel.activeMatch.isLongPlay && !matchInfoModel.activeMatch.isBotMatch)
            {
                string challengedId = matchInfoModel.activeMatch.challengedId;
                string challengerId = matchInfoModel.activeMatch.challengerId;
                bool isPlayerTurn = chessboardModel.chessboards[challengeId].isPlayerTurn;
                int moveCount = chessboardModel.chessboards[challengeId].moveList.Count;

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

                backendService.MatchWatchdogPingAck(currentTurnPlayerId, challengerId, challengedId, challengeId, moveCount);
            }
        }
    }
}
