/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-22 12:55:25 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System.Collections;
using System;
using UnityEngine;
using GameSparks.Api.Responses;
using TurboLabz.TLUtils;
using TurboLabz.InstantFramework;

using GameSparks.Api.Requests;
using strange.extensions.promise.api;
using GameSparks.Core;
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        [Inject] public WifiIsHealthySignal wifiIsHealthySignal { get; set; }

        private int initialPingCount;
        private long sendTime;
        private Coroutine wifiHealthCheckCR;
        private Coroutine pingerCR;

        public void StartPinger()
        {
            if (pingerCR == null)
            {
                pingerCR = routineRunner.StartCoroutine(StartPingerCR());
            }
        }

        public void StopPinger()
        {
            if (pingerCR != null)
            {
                routineRunner.StopCoroutine(pingerCR);
            }
            pingerCR = null;
        }

        private IEnumerator StartPingerCR()
        {
            while (true)
            {
                RestartHealthCheckMonitor();
                bool opCommunityPublicStatus = (!(initialPingCount < (GSSettings.INITIAL_PING_COUNT - 1))) && (appInfoModel.gameMode == GameMode.NONE);
                new GSPingRequest().Send(OnPingSuccess, TimeUtil.unixTimestampMilliseconds, opCommunityPublicStatus);

                float frequency = GSSettings.PINGER_FREQUENCY;

                if (initialPingCount < (GSSettings.INITIAL_PING_COUNT - 1))
                {
                    frequency = GSSettings.INITIAL_PINGER_FREQUENCY;
                    ++initialPingCount;
                }

                yield return new WaitForSecondsRealtime(frequency);
            }
        }

        private void UpdateClockLatency(LogEventResponse response)
        {
            // Cache client recipt timestamp at the very top to get the true
            // receipt time.
            long clientReceiptTimestamp = TimeUtil.unixTimestampMilliseconds;
            long clientSendTimestamp = response.ScriptData.GetLong(GSBackendKeys.CLIENT_SEND_TIMESTAMP).Value;
            long serverReceiptTimestamp = response.ScriptData.GetLong(GSBackendKeys.SERVER_RECEIPT_TIMESTAMP).Value;

            serverClock.CalculateLatency(clientSendTimestamp, serverReceiptTimestamp, clientReceiptTimestamp);
        }

        private void UpdateCommunityPublicStatus(LogEventResponse response)
        {
            if (!response.ScriptData.ContainsKey("communityPublicStatus"))
            {
                return;
            }

            GSData statusList = response.ScriptData.GetGSData("communityPublicStatus");
            foreach (KeyValuePair<string, object> obj in statusList.BaseData)
            {
                GSData player = (GSData)obj.Value;
                string playerId = obj.Key;
                bool isOnline = player.GetBoolean("isOnline").Value;

                PublicProfile publicProfile = playerModel.community[playerId].publicProfile;
                ProfileVO pvo = new ProfileVO();
                pvo.playerPic = publicProfile.profilePicture;
                pvo.playerName = publicProfile.name;
                pvo.eloScore = publicProfile.eloScore;
                pvo.countryId = publicProfile.countryId;
                pvo.playerId = publicProfile.playerId;
                pvo.avatarColorId = publicProfile.avatarBgColorId;
                pvo.avatarId = publicProfile.avatarId;
                pvo.isOnline = isOnline;
                pvo.isActive = publicProfile.isActive;

                updtateFriendOnlineStatusSignal.Dispatch(pvo);
            }
        }

        private void OnPingSuccess(object r)
        {
            LogEventResponse response = (LogEventResponse)r;
            StopHealthCheckMonitor();
            float secondsElapsed = (TimeUtil.unixTimestampMilliseconds - sendTime)/1000f;
            if (secondsElapsed < GSSettings.SLOW_WIFI_WARNING_THRESHOLD)
            {
                wifiIsHealthySignal.Dispatch(true);
            }

            UpdateClockLatency(response);

            UpdateCommunityPublicStatus(response);
        }
            
        private void RestartHealthCheckMonitor()
        {
            StopHealthCheckMonitor();
            wifiHealthCheckCR = routineRunner.StartCoroutine(CheckWifiHealthCR());
            sendTime = TimeUtil.unixTimestampMilliseconds;
        }

        private void StopHealthCheckMonitor()
        {
            if (wifiHealthCheckCR != null)
            {
                routineRunner.StopCoroutine(wifiHealthCheckCR);
                wifiHealthCheckCR = null;
            }
        }

        private IEnumerator CheckWifiHealthCR()
        {
            yield return new WaitForSecondsRealtime(GSSettings.SLOW_WIFI_WARNING_THRESHOLD);
            wifiIsHealthySignal.Dispatch(false);
        }
    }

    #region REQUEST




    public class GSPingRequest : GSFrameworkRequest
    {
        const string SHORT_CODE = "Ping";

        public IPromise<BackendResult> Send(Action<object> onSuccess, long clientSendTimestamp, bool opCommunityPublicStatus)
        {
            this.onSuccess = onSuccess;
            this.errorCode = BackendResult.PING_REQUEST_FAILED;

            string requestParams = BuildParams(clientSendTimestamp, opCommunityPublicStatus);

            new LogEventRequest()  
                .SetEventKey(SHORT_CODE)
                .SetEventAttribute("params", requestParams)
                .Send(OnRequestSuccess, OnRequestFailure);

            return promise;
        }

        [Serializable]
        private struct RequestParams
        {
            public long clientSendTimestamp;
            public bool opCommunityPublicStatus;
        }

        private string BuildParams(long clientSendTimestamp, bool opCommunityPublicStatus)
        {
            RequestParams requestParams;
            requestParams.clientSendTimestamp = clientSendTimestamp;
            requestParams.opCommunityPublicStatus = opCommunityPublicStatus;
            return JsonUtility.ToJson(requestParams);
        }
    }

    #endregion
}

    