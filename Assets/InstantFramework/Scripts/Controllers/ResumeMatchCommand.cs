/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using strange.extensions.command.impl;
using TurboLabz.TLUtils;
using System;
using TurboLabz.Multiplayer;
using TurboLabz.CPU;
using GameSparks.Core;

namespace TurboLabz.InstantFramework
{
    public class ResumeMatchCommand : Command
    {
        // Params
        [Inject] public NavigatorViewId prevViewId { get; set; }

        // Dispatch signals
        [Inject] public StartGameSignal startGameSignal { get; set; }
        [Inject] public StartCPUGameSignal startCPUGameSignal { get; set; }
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public ReceptionSignal receptionSignal { get; set; }
        [Inject] public GetInitDataCompleteSignal getInitDataCompleteSignal { get; set; }
        [Inject] public GetInitDataFailedSignal getInitDataFailedSignal { get; set; }
        [Inject] public LoadLobbySignal loadLobbySignal { get; set; }
        [Inject] public Multiplayer.StopTimersSignal stopTimersSignal { get; set; }
        [Inject] public ReconnectionCompleteSignal reconnectionCompeteSignal { get; set; }
        [Inject] public ChessboardBlockerEnableSignal chessboardBlockerEnableSignal { get; set; }
        [Inject] public ReconnectViewEnableSignal reconnectViewEnableSignal { get; set; }
        [Inject] public GetInitDataSignal getInitDataSignal { get; set; }
        [Inject] public ToggleBannerSignal toggleBannerSignal { get; set; }
        [Inject] public RefreshFriendsSignal refreshFriendsSignal { get; set; }
        [Inject] public RefreshCommunitySignal refreshCommunitySignal { get; set; }

        // Models
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public Multiplayer.IChessboardModel chessboardModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IAppInfoModel appInfoModel { get; set; }
        [Inject] public INavigatorModel navigatorModel { get; set; }

        // services
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IFacebookService facebookService { get; set; }
        [Inject] public ISignInWithAppleService signInWithAppleService { get; set; }

        private BackendResult authBackendResult;

        public override void Execute()
        {
            Retain();

            getInitDataCompleteSignal.AddListener(OnGetInitDataComplete);
            getInitDataFailedSignal.AddListener(OnGetInitDataFailed);
            getInitDataSignal.Dispatch(true);
        }

        private void OnAuthComplete(BackendResult result)
        {
            authBackendResult = result;

            if (result != BackendResult.SUCCESS)
            {
                GS.Instance.GameSparksAuthenticated -= ResumeGameSparksAuthenticated;
                backendErrorSignal.Dispatch(result);
            }
        }

        private void AuthenticateOnFail()
        {
            GS.Instance.GameSparksAuthenticated += ResumeGameSparksAuthenticated;

            string fbAccessToken = facebookService.GetAccessToken();
            if (fbAccessToken == null)
            {
                backendService.AuthGuest().Then(OnAuthComplete);
            }
            else
            {
                backendService.AuthFacebook(fbAccessToken, true).Then(OnAuthComplete);
            }
        }

        private void ResumeGameSparksAuthenticated(string id)
        {
            GS.Instance.GameSparksAuthenticated -= ResumeGameSparksAuthenticated;
            getInitDataSignal.Dispatch(true);
        }

        private void OnGetInitDataFailed(BackendResult result)
        {
            if (result == BackendResult.NOT_AUTHORIZED)
            {
                AuthenticateOnFail();
            }
            else
            if (result != BackendResult.CANCELED)
            {
                TLUtils.LogUtil.Log("ResumeMatchCommand::OnGetInitDataFailed() GetInitData failed!");
                getInitDataSignal.Dispatch(true);
            }
        }

        private void OnGetInitDataComplete()
        {
            backendService.AddChallengeListeners();

            if (navigatorModel.currentViewId == NavigatorViewId.CPU)
            {
                stopTimersSignal.Dispatch();
                startCPUGameSignal.Dispatch();
                toggleBannerSignal.Dispatch(true);
            }
            else if (navigatorModel.currentViewId == NavigatorViewId.MULTIPLAYER_RESULTS_DLG)
            {
                LogUtil.Log("Ignore recover match on result screen. NavigatorViewId.MULTIPLAYER_RESULTS_DLG", "cyan");
                // do nothing

                // Todo: condition needs to be game ended but still on board view
            }
            else if (matchInfoModel.activeChallengeId == null)
            {
                LogUtil.Log("Ignore recover match for view on completed game. NavigatorViewId.MULTIPLAYER", "cyan");
                // do nothing

                // Todo: condition needs to be game ended but still on board view
            }
            else if (matchInfoModel.activeChallengeId != null && !chessboardModel.isValidChallenge(matchInfoModel.activeChallengeId))
            {
                LogUtil.Log("Ignore recover match for view on Ivalid game. NavigatorViewId.MULTIPLAYER", "cyan");
            }
            else if (matchInfoModel.activeChallengeId != null)
            {
                stopTimersSignal.Dispatch();
                startGameSignal.Dispatch();
                toggleBannerSignal.Dispatch(true);

                // Record analytics
                TimeSpan totalSeconds = TimeSpan.FromMilliseconds(TimeUtil.unixTimestampMilliseconds - appInfoModel.reconnectTimeStamp);
                analyticsService.Event(AnalyticsEventId.disconnection_time, AnalyticsParameter.count, totalSeconds.Seconds);
                TLUtils.LogUtil.Log("ResumeMatchCommand() Reconnection Time Seconds = " + totalSeconds.Seconds, "cyan");

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
            else
            {
                loadLobbySignal.Dispatch();
            }

            CommandEnd();
        }

        private void CommandEnd()
        {
            reconnectionCompeteSignal.Dispatch();
            getInitDataCompleteSignal.RemoveListener(OnGetInitDataComplete);
            getInitDataFailedSignal.RemoveListener(OnGetInitDataFailed);

            prevViewId = NavigatorViewId.NONE;
            appInfoModel.syncInProgress = false;

            // Resume GS connection monitoring
            backendService.MonitorConnectivity(true);

            // Start the pinger
            backendService.StartPinger();

            reconnectViewEnableSignal.Dispatch(false);
            chessboardBlockerEnableSignal.Dispatch(false);

            // Restart the reachability monitor
            backendService.OnlineCheckerStart();

            appInfoModel.isReconnecting = DisconnectStates.FALSE;

            refreshFriendsSignal.Dispatch();
            refreshCommunitySignal.Dispatch(true);

            Release();
        }
    }
}
