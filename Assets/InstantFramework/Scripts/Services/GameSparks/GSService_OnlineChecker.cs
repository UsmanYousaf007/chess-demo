/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections;
using UnityEngine;
using System;
using TurboLabz.TLUtils;
using GameSparks.Api.Requests;
using strange.extensions.promise.api;
using strange.extensions.signal.impl;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        private Coroutine onlineCheckerPingerCR = null;
        private long requestTimestamp = 0;
        private bool onlineCheckerPendingRequest = false;
        private const int onlineCheckerFrequence = 5;
        private int onlineCheckerSlowInternetThreshold = 3;
        private static Signal<bool, ConnectionSwitchType> internetReachabilitySignal = new Signal<bool, ConnectionSwitchType>();
        private bool onlineCheckerInternetAvailability = true;
        private bool onlineCheckerSlowInternet = false;

        public void OnlineCheckerStart()
        {
            if (onlineCheckerPingerCR != null)
            {
                OnlineCheckerStop();
            }

            if (onlineCheckerPingerCR == null)
            {
                onlineCheckerPingerCR = routineRunner.StartCoroutine(OnlineCheckerPingerCR());
            }
        }

        public void OnlineCheckerStop()
        {
            if (onlineCheckerPingerCR != null)
            {
                routineRunner.StopCoroutine(onlineCheckerPingerCR);
            }

            onlineCheckerPingerCR = null;
        }

        public void OnlineCheckerAddListener(Action<bool, ConnectionSwitchType> listener)
        {
            internetReachabilitySignal.AddListener(listener);
        }

        public void OnlineCheckerRemoveListener(Action<bool, ConnectionSwitchType> listener)
        {
            internetReachabilitySignal.RemoveListener(listener);
        }

        private IEnumerator OnlineCheckerPingerCR()
        {
            while (true)
            {
                if (onlineCheckerPendingRequest == false)
                {
                    requestTimestamp = TimeUtil.unixTimestampMilliseconds;
                    new GSOnlineCheckerPingRequest().Send().Then(OnlineCheckerCompleted);
                    onlineCheckerPendingRequest = true;
                }

                yield return new WaitForSecondsRealtime(onlineCheckerFrequence);
            }
        }

        private void OnlineCheckerCompleted(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                long t = TimeUtil.unixTimestampMilliseconds - requestTimestamp;
                onlineCheckerSlowInternet = t > (onlineCheckerSlowInternetThreshold * 1000);

                if (onlineCheckerInternetAvailability == false)
                {
                    internetReachabilitySignal.Dispatch(true, ConnectionSwitchType.FROM_DISCONNECTED_TO_CONNECTED);
                }
                onlineCheckerInternetAvailability = true;
            }
            else
            {
                if (onlineCheckerInternetAvailability == true)
                {
                    internetReachabilitySignal.Dispatch(false, ConnectionSwitchType.FROM_CONNECTED_TO_DISCONNECTED);
                }
                onlineCheckerInternetAvailability = false;
            }

            onlineCheckerPendingRequest = false;

        }
    }

    #region REQUEST

    public class GSOnlineCheckerPingRequest : GSFrameworkRequest
    {
        const string SHORT_CODE = "OnlineChecker";

        public IPromise<BackendResult> Send()
        {
            this.errorCode = BackendResult.ONLINECHECKER_REQUEST_FAILED;

            new LogEventRequest()  
                .SetEventKey(SHORT_CODE)
                .Send(OnRequestSuccess, OnRequestFailure, 5000);

            return promise;
        }
    }

    #endregion
}

