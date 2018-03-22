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

using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public partial class GSService
    {
        private enum PingerState
        {
            STOPPED = -1,
            PINGING
        }

        // Dispatch signals
        [Inject] public ClockSyncedSignal clockSyncedSignal { get; set; }

        // Utils
        [Inject] public IRoutineRunner routineRunner { get; set; }
        [Inject] public IServerClock serverClock { get; set; }

        private PingerState state = PingerState.STOPPED;
        private IEnumerator startPingerCR;
        private bool isClockSyncedSignalDispatched;
        private int initialPingCount;

        [PostConstruct]
        public void ConstructConnectionMonitor()
        {
            serverClock.latencySampleCount = GSSettings.LATENCY_SAMPLE_COUNT;
        }

        private void StartPinger()
        {
            Assertions.Assert(state == PingerState.STOPPED, "Pinger must not already be running!");

            state = PingerState.PINGING;
            startPingerCR = StartPingerCR();
            routineRunner.StartCoroutine(startPingerCR);
        }

        private void StopPinger()
        {
            Assertions.Assert(state == PingerState.PINGING, "The pinger must already be running!");

            routineRunner.StopCoroutine(startPingerCR);
            startPingerCR = null;
            state = PingerState.STOPPED;

            // Clean up and reset server clock.
            serverClock.Reset();
            initialPingCount = 0;
            isClockSyncedSignalDispatched = false;
        }

        private IEnumerator StartPingerCR()
        {
            while (true)
            {
                new GSPingRequest().Send(OnPingSuccess).Then(OnPing);

                float frequency = GSSettings.PINGER_FREQUENCY;

                if (initialPingCount < GSSettings.INITIAL_PING_COUNT)
                {
                    frequency = GSSettings.INITIAL_PINGER_FREQUENCY;
                    ++initialPingCount;
                }

                yield return new WaitForSecondsRealtime(frequency);
            }
        }

        private void OnPingSuccess(LogEventResponse response)
        {
            // Cache client recipt timestamp at the very top to get the true
            // receipt time.
            long clientReceiptTimestamp = TimeUtil.unixTimestampMilliseconds;
            long clientSendTimestamp = response.ScriptData.GetLong(GSEventData.Ping.ATT_KEY_CLIENT_SEND_TIMESTAMP).Value;
            long serverReceiptTimestamp = response.ScriptData.GetLong(GSEventData.Ping.ATT_KEY_SERVER_RECEIPT_TIMESTAMP).Value;

            serverClock.CalculateLatency(clientSendTimestamp, serverReceiptTimestamp, clientReceiptTimestamp);
        }

        private void OnPing(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                if (!isClockSyncedSignalDispatched)
                {
                    clockSyncedSignal.Dispatch(result);
                    isClockSyncedSignalDispatched = true;
                }
            }
            else
            {
                backendErrorSignal.Dispatch(result);
            }
        }
    }
}
