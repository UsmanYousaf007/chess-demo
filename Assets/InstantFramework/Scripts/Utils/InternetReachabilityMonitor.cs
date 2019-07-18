/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using System.Collections;
using strange.extensions.signal.impl;
using System;
using UnityEngine.Networking;

namespace TurboLabz.TLUtils
{
    public static class InternetReachabilityMonitor
    {
        private static readonly int INTERNET_REACHABILITY_STATUS_TICK_SECONDS = 1;
        private static Coroutine interneReachablilityCR;
        private static NormalRoutineRunner normalRoutineRunner = new NormalRoutineRunner();
        public static Signal<bool, ConnectionSwitchType> internetReachabilitySignal = new Signal<bool, ConnectionSwitchType>();
        public static bool prevInternetReachability = false;
        public static bool isInternetReachable = false;
        public static bool resultPending = false;

        public enum ConnectionSwitchType {
            NONE,
            FROM_CONNECTED_TO_DISCONNECTED,
            FROM_DISCONNECTED_TO_CONNECTED
        }

        private static void CheckInternet()
        {
            if (resultPending == false)
            {
                resultPending = true;
                normalRoutineRunner.StartCoroutine(PingURLCR("http://google.com"));
            }
        }

        private static IEnumerator PingURLCR(string url)
        {
            WWW www = new WWW(url);
            yield return www;

            if (string.IsNullOrEmpty(www.error)) // Success
            {
                isInternetReachable = true;
            }
            else // Failure
            {
                isInternetReachable = false;
            }
            resultPending = false;

            TLUtils.LogUtil.Log("INTERNET Ping= " + isInternetReachable, "cyan");
        }

        private static bool IsInternReachable()
        {
            TLUtils.LogUtil.Log("INTERNET = " + isInternetReachable, "cyan");
            return isInternetReachable;
        }

        private static IEnumerator InternetReachabilityCR()
        {
            while (true)
            {
                CheckInternet();

                if (!IsInternReachable())
                {
                    ConnectionSwitchType connectionSwitch = prevInternetReachability == true ? 
                        ConnectionSwitchType.FROM_CONNECTED_TO_DISCONNECTED : ConnectionSwitchType.NONE;
                    internetReachabilitySignal.Dispatch(false, connectionSwitch);
                    prevInternetReachability = false;
                }
                else
                {
                    ConnectionSwitchType connectionSwitch = prevInternetReachability == false ?
                        ConnectionSwitchType.FROM_DISCONNECTED_TO_CONNECTED : ConnectionSwitchType.NONE;
                    internetReachabilitySignal.Dispatch(true, connectionSwitch);
                    prevInternetReachability = true;
                }

                yield return new WaitForSecondsRealtime(INTERNET_REACHABILITY_STATUS_TICK_SECONDS);
            }
        }

        public static void StartMonitor()
        {
            prevInternetReachability = IsInternReachable();

            if (interneReachablilityCR == null)
            {
                interneReachablilityCR = normalRoutineRunner.StartCoroutine(InternetReachabilityCR());
            }
        }

        public static void StopMonitor()
        {
            normalRoutineRunner.StopCoroutine(interneReachablilityCR);
            interneReachablilityCR = null;
        }

        public static void AddListener(Action<bool, ConnectionSwitchType> signalListener)
        {
            internetReachabilitySignal.AddListener(signalListener);
        }

        public static void RemoveListener(Action<bool, ConnectionSwitchType> signalListener)
        {
            internetReachabilitySignal.RemoveListener(signalListener);
        }
    }
}
