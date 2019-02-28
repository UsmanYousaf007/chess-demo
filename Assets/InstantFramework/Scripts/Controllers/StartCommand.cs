/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;
using UnityEngine;
using TurboLabz.TLUtils;
using System.Collections;
using GameSparks.Core;
using GameAnalyticsSDK;

namespace TurboLabz.InstantFramework
{
	public class StartCommand : Command
	{
		// Dispatch Signals
		[Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
		[Inject] public ReceptionSignal receptionSignal { get; set; }
		[Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public InitFacebookSignal initFacebookSignal { get; set; }
        [Inject] public ModelsResetSignal modelsResetSignal { get; set; }
        [Inject] public ModelsLoadFromDiskSignal modelsLoadFromDiskSignal { get; set; }
        [Inject] public SplashWifiIsHealthySignal splashWifiIsHealthySignal { get; set; }

        // Services
        [Inject] public IAudioService audioService { get; set; }
		[Inject] public IShareService shareService { get; set; }
		[Inject] public IBackendService backendService { get; set; }
        [Inject] public IAdsService adsService { get; set; }
        [Inject] public IRoutineRunner routineRunner { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }

		bool gameSparksAvailable = false;

        private Coroutine wifiHealthCheckCR = null;
        private int wifiHealthWaitCounter = 0;
        private const int SLOW_WIFI_WARNING_THRESHOLD_SECONDS = 10;

        public override void Execute()
		{
            Retain();

            wifiHealthCheckCR = routineRunner.StartCoroutine(CheckWifiHealthCR());

            modelsResetSignal.Dispatch();
            modelsLoadFromDiskSignal.Dispatch();

			ListenForKeyEvents();
			navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SPLASH);
			audioService.Init();
            adsService.Init();
            GameAnalytics.Initialize();
		}

		void GameSparksAvailable(bool isAvailable)
        {
			gameSparksAvailable = isAvailable;
			ProcessStartup();
		}

		void ProcessStartup()
		{
            if (gameSparksAvailable)
			{
				if (GS.Authenticated)
                {
                    GotoReception();
				}
                // New guest account
				else
				{
					backendService.AuthGuest().Then(OnAuthGuest);
				}
			}
		}

		private void OnAuthGuest(BackendResult result)
		{
			if (result == BackendResult.SUCCESS)
			{
				GotoReception();
			}
            else if (result != BackendResult.CANCELED)
			{
				backendErrorSignal.Dispatch(result);
                Release();
            }
		}

		void GotoReception()
		{
            if (wifiHealthCheckCR != null)
            {
                routineRunner.StopCoroutine(wifiHealthCheckCR);
            }

            initFacebookSignal.Dispatch();
            receptionSignal.Dispatch();
			RemoveListeners();
            Release();
		}

        void ListenForKeyEvents()
		{
            backendService.AddChatMessageListener();
            GS.GameSparksAvailable += GameSparksAvailable;
		}

		void RemoveListeners()
		{
			GS.GameSparksAvailable -= GameSparksAvailable;
		}


        private IEnumerator CheckWifiHealthCR()
        {
            while (true)
            {
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    backendErrorSignal.Dispatch(BackendResult.NO_INTERNET_REACHABILITY);
                }

                if (wifiHealthWaitCounter > 0)
                {
                    splashWifiIsHealthySignal.Dispatch();
                }
                wifiHealthWaitCounter++;

                yield return new WaitForSecondsRealtime(SLOW_WIFI_WARNING_THRESHOLD_SECONDS);
            }
        }
    }
}