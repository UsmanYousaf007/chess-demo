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
        private  static Coroutine pingUrlCR = null;

        public enum ConnectionSwitchType {
            NONE,
            FROM_CONNECTED_TO_DISCONNECTED,
            FROM_DISCONNECTED_TO_CONNECTED
        }

        private static string PING_URL = "http://www.google.com";

        private static void CheckInternetAccess()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                isInternetReachable = false;
            }
        }

        private static IEnumerator PingURLCR(string url)
        {
            while (true)
            {
                using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
                {
                    // Request and wait for the desired page.
                    yield return webRequest.SendWebRequest();

                    string[] pages = url.Split('/');
                    int page = pages.Length - 1;

                    if (webRequest.isNetworkError)
                    {
                        isInternetReachable = false;
                    }
                    else
                    {
                        isInternetReachable = true;
                    }
                }

                yield return new WaitForSeconds(3);
            }
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

            if (pingUrlCR == null)
            {
                pingUrlCR = normalRoutineRunner.StartCoroutine(PingURLCR(PING_URL));
            }
        }

        public static void StopMonitor()
        {
            if (interneReachablilityCR != null)
            {
                normalRoutineRunner.StopCoroutine(interneReachablilityCR);
                interneReachablilityCR = null;
            }

            if (pingUrlCR == null)
            {
                normalRoutineRunner.StopCoroutine(pingUrlCR);
                pingUrlCR = null;
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
