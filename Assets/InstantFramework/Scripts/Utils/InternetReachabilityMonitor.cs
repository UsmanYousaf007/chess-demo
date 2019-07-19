/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using System.Collections;
using strange.extensions.signal.impl;
using System;
using UnityEngine.Networking;
using Crosstales.OnlineCheck;

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

        public enum ConnectionSwitchType {
            NONE,
            FROM_CONNECTED_TO_DISCONNECTED,
            FROM_DISCONNECTED_TO_CONNECTED
        }

        private static void CheckInternetAccess()
        {
            isInternetReachable = OnlineCheck.isInternetAvailable;
        }

        private static IEnumerator InternetReachabilityCR()
        {
            while (true)
            {
                CheckInternetAccess();

                if (!isInternetReachable)
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
            prevInternetReachability = isInternetReachable;

            if (interneReachablilityCR == null)
            {
                interneReachablilityCR = normalRoutineRunner.StartCoroutine(InternetReachabilityCR());
            }
        }

        public static void StopMonitor()
        {
            if (interneReachablilityCR != null)
            {
                normalRoutineRunner.StopCoroutine(interneReachablilityCR);
                interneReachablilityCR = null;
            }
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
