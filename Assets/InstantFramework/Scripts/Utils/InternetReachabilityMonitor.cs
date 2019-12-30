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
        public static Signal<bool, ConnectionSwitchType> internetReachabilitySignal = new Signal<bool, ConnectionSwitchType>();
        public static bool enableDispatches = true;

        public enum ConnectionSwitchType {
            NONE,
            FROM_CONNECTED_TO_DISCONNECTED,
            FROM_DISCONNECTED_TO_CONNECTED
        }

        private static void InternetReachabilityHandler(bool isInternetReachable)
        {
            if (enableDispatches)
            {
                ConnectionSwitchType connectionSwitch = isInternetReachable ?
                    ConnectionSwitchType.FROM_DISCONNECTED_TO_CONNECTED : ConnectionSwitchType.FROM_CONNECTED_TO_DISCONNECTED;

                internetReachabilitySignal.Dispatch(isInternetReachable, connectionSwitch);
            }
        }

        public static void EnableDispatches(bool enable)
        {
            enableDispatches = enable;
        }

        public static void StartMonitor()
        {
            OnlineCheck.Refresh();
            OnlineCheck.OnOnlineStatusChange += InternetReachabilityHandler;
        }

        public static void StopMonitor()
        {
            OnlineCheck.OnOnlineStatusChange -= InternetReachabilityHandler;
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
