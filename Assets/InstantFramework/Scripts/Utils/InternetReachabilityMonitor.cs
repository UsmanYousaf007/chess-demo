/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using System.Collections;
using strange.extensions.signal.impl;

namespace TurboLabz.TLUtils
{
    public static class InternetReachabilityMonitor
    {
        private static readonly int INTERNET_REACHABILITY_STATUS_TICK_SECONDS = 1;
        private static Coroutine interneReachablilityCR;
        private static NormalRoutineRunner normalRoutineRunner = new NormalRoutineRunner();
        public static Signal<bool> internetReachabilitySignal = new Signal<bool>();
        public static bool prevInternetReachability = false;

        private static IEnumerator InternetReachabilityCR()
        {
            while (true)
            {
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    internetReachabilitySignal.Dispatch(false);
                    prevInternetReachability = false;
                }
                else
                {
                    internetReachabilitySignal.Dispatch(true);
                    prevInternetReachability = true;
                }

                yield return new WaitForSecondsRealtime(INTERNET_REACHABILITY_STATUS_TICK_SECONDS);
            }
        }

        public static void StartMonitor()
        {
            prevInternetReachability = Application.internetReachability != NetworkReachability.NotReachable;

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
    }
}
