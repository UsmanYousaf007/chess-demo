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

using UnityEngine;

using GameSparks.Api.Responses;

using TurboLabz.TLUtils;
using System.Collections.Generic;
using GameSparks.Core;
using TurboLabz.InstantFramework;
using System;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        [Inject] public WifiIsHealthySignal wifiIsHealthySignal { get; set; }

        private int initialPingCount;
        private long sendTime;
        private Coroutine wifiHealthCheckCR;

        public void StartPinger()
        {
            
            routineRunner.StartCoroutine(StartPingerCR());
        }

        private IEnumerator StartPingerCR()
        {
            while (true)
            {
                RestartHealthCheckMonitor();
                new GSPingRequest(OnPingSuccess).Send();

                float frequency = GSSettings.PINGER_FREQUENCY;

                if (initialPingCount < (GSSettings.INITIAL_PING_COUNT - 1))
                {
                    frequency = GSSettings.INITIAL_PINGER_FREQUENCY;
                    ++initialPingCount;
                }

                yield return new WaitForSecondsRealtime(frequency);
            }
        }

        private void OnPingSuccess(LogEventResponse response)
        {
            StopHealthCheckMonitor();
            float secondsElapsed = (TimeUtil.unixTimestampMilliseconds - sendTime)/1000f;
            if (secondsElapsed < GSSettings.SLOW_WIFI_WARNING_THRESHOLD)
            {
                wifiIsHealthySignal.Dispatch(true);
            }

            // Cache client recipt timestamp at the very top to get the true
            // receipt time.
            long clientReceiptTimestamp = TimeUtil.unixTimestampMilliseconds;
            long clientSendTimestamp = response.ScriptData.GetLong(GSBackendKeys.CLIENT_SEND_TIMESTAMP).Value;
            long serverReceiptTimestamp = response.ScriptData.GetLong(GSBackendKeys.SERVER_RECEIPT_TIMESTAMP).Value;

            serverClock.CalculateLatency(clientSendTimestamp, serverReceiptTimestamp, clientReceiptTimestamp);
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
}

#region REQUEST
    public class GSPingRequest : GSLogEventRequest
    {
        const string SHORT_CODE = "Ping";
        const string ATT_CLIENT_SEND_TIMESTAMP = "clientSendTimestamp";

        public GSPingRequest(Action<LogEventResponse> onSuccess)
        {
            // Set your request parameters here
            key = SHORT_CODE;
            request.SetEventAttribute(ATT_CLIENT_SEND_TIMESTAMP, TimeUtil.unixTimestampMilliseconds);
            errorCode = BackendResult.PING_REQUEST_FAILED;

            // Do not modify below
            this.onSuccess = onSuccess;
        }
    }

#endregion