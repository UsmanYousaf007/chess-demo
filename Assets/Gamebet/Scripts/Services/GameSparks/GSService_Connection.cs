/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-21 21:18:09 UTC+05:00
/// 
/// @description
/// [add_description_here]

using GameSparks.Core;
using strange.extensions.promise.api;

using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public partial class GSService
    {
        public ConnectionState connectionState { get; private set; }

        private bool isGSInitialized = false;
        private GSRequestSession requestSession = new GSRequestSession();
        private GSExtInternetReachabilityMonitor internetReachabilityMonitor = new GSExtInternetReachabilityMonitor();

        [PostConstruct]
        public void ConstructConnection()
        {
            connectionState = ConnectionState.DISCONNECTED;
        }

        public IPromise<BackendResult> Connect()
        {
            Assertions.Assert(connectionState == ConnectionState.DISCONNECTED, "Backend must be disconnected!");

            connectionState = ConnectionState.CONNECTING;
            internetReachabilityMonitor.Start(OnInternetReachabilityFailure);

            return new GSConnectRequest().Send(isGSInitialized).Then(OnConnect);
        }

        // This method must not be called before calling Connect().
        public void Disconnect()
        {
            Assertions.Assert(connectionState != ConnectionState.DISCONNECTED, "Backend must not be already disconnected!");

            internetReachabilityMonitor.Stop();

            if (isAuthenticated)
            {
                StopPostAuthProcesses();
            }

            GS.Disconnect();

            requestSession.EndSession();
            connectionState = ConnectionState.DISCONNECTED;
        }

        private void OnInternetReachabilityFailure()
        {
            backendErrorSignal.Dispatch(BackendResult.NO_INTERNET_REACHABILITY);
        }

        // We also look for the successful connection promise here so that we
        // can start our additional ping monitor.
        private void OnConnect(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                connectionState = ConnectionState.CONNECTED;
                isGSInitialized = true;

                if (isAuthenticated)
                {
                    StartPostAuthProcesses();
                }
            }
        }

        private void StartPostAuthProcesses()
        {
            StartPinger();
            AddMessageListeners();
        }

        private void StopPostAuthProcesses()
        {
            RemoveMessageListeners();
            StopPinger();
        }
    }
}
