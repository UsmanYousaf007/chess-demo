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
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using UnityEngine;

namespace TurboLabz.Multiplayer
{
    public partial class GameMediator
    {
        [Inject] public IBackendService backendService { get; set; }

        [Inject] public StartGameSignal startGameSignal { get; set; }

        [Inject] public IChessboardModel chessboardModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        [Inject] public ILocalizationService localizationService { get; set; }

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
                TLUtils.LogUtil.Log("OnWifiHealthUpdate: Ping caused a wifi health update", "cyan");
            }
        }

        private void OnInternetConnectedTicked(bool isConnected, InternetReachabilityMonitor.ConnectionSwitchType connectionSwitch)
        {
            //view.WifiHealthUpdate(isConnected);

            if (connectionSwitch == InternetReachabilityMonitor.ConnectionSwitchType.FROM_CONNECTED_TO_DISCONNECTED)
            {
                view.warningLabel.text = localizationService.Get(LocalizationKey.GM_WIFI_RECONNECTING);
                LogUtil.Log("Internet Disconnected", "cyan");
                if (matchInfoModel.activeChallengeId != null)
                {
                    GSFrameworkRequest.CancelRequestSession();
                    stopTimersSignal.Dispatch();
                    view.FlashClocks(true);
                }
            }
            else
            if (connectionSwitch == InternetReachabilityMonitor.ConnectionSwitchType.FROM_DISCONNECTED_TO_CONNECTED)
            {
                LogUtil.Log("Reconnect GS", "cyan");
                //GameSparks.Core.GS.Reconnect();

                if (matchInfoModel.activeChallengeId != null)
                {
                    backendService.SyncReconnectData(matchInfoModel.activeChallengeId).Then(OnSycReconnectionData);
                }
            }
        }

        private void OnSycReconnectionData(BackendResult backendResult)
        {
            if (backendResult == BackendResult.CANCELED)
            {
                LogUtil.Log("Restart match CANCELED!!..", "cyan");
                return;
            }

            LogUtil.Log("Restarting match!!..", "cyan");
            //stopTimersSignal.Dispatch();

            Chessboard activeChessboard = chessboardModel.chessboards[matchInfoModel.activeChallengeId];
            MatchInfo activeMatchInfo = matchInfoModel.activeMatch;
            activeChessboard.currentState = null;

            startGameSignal.Dispatch();
            SendReconnectionAck();

            view.WifiHealthUpdate(true);
            view.warningLabel.text = localizationService.Get(LocalizationKey.GM_WIFI_WARNING);
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
