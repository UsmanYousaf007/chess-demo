/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;
using UnityEngine;
using TurboLabz.TLUtils;
using System.Collections;
using GameSparks.Core;
using TurboLabz.CPU;
using HUF.Analytics.Runtime.API;

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
        [Inject] public LoadGameSignal loadCPUGameDataSignal { get; set; }
        [Inject] public PauseNotificationsSignal pauseNotificationsSignal { get; set; }

        // Services
        [Inject] public IAudioService audioService { get; set; }
		[Inject] public IShareService shareService { get; set; }
		[Inject] public IBackendService backendService { get; set; }
        [Inject] public IRoutineRunner routineRunner { get; set; }
        [Inject] public IAppsFlyerService appsFlyerService { get; set; }
        [Inject] public IAdsService adsService { get; set; }
        [Inject] public IVideoPlaybackService videoPlaybackService { get; set; }
        [Inject] public IRewardsService rewardsService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IDownloadablesModel downloadablesModel { get; set; }
        [Inject] public INotificationsModel notificationsModel { get; set; }

        bool gameSparksAvailable = false;

        private Coroutine wifiHealthCheckCR = null;
        private int wifiHealthWaitCounter = 0;
        private const int SLOW_WIFI_WARNING_THRESHOLD_SECONDS = 30;

        public override void Execute()
        {
            CommandBegin();

            modelsResetSignal.Dispatch();
            modelsLoadFromDiskSignal.Dispatch();
            pauseNotificationsSignal.Dispatch(true);

            ListenForKeyEvents();
			navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SPLASH);
			audioService.Init();
            appsFlyerService.Init();
            loadCPUGameDataSignal.Dispatch();
            adsService.Init();
            videoPlaybackService.Init();
        }

		void StartGameSparksAvailable(bool isAvailable)
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
                //if (playerModel.newUser)
                //{
                //    navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SKILL_LEVEL_DLG);
                //    RemoveListeners();
                //    CommandEnd();
                //}
                //else
                //{
                //    GotoReception();
                //}
            }
            else if (result != BackendResult.CANCELED)
			{
				backendErrorSignal.Dispatch(result);
                CommandEnd();
            }
		}

		void GotoReception()
		{
            initFacebookSignal.Dispatch();
            receptionSignal.Dispatch(false);
			RemoveListeners();

            CommandEnd();
		}

        void ListenForKeyEvents()
		{
            backendService.AddChatMessageListener();
            GS.GameSparksAvailable += StartGameSparksAvailable;
		}

		void RemoveListeners()
		{
			GS.GameSparksAvailable -= StartGameSparksAvailable;
		}

        IEnumerator CheckWifiHealthCR()
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

        void CommandBegin()
        {
            Retain();
            wifiHealthCheckCR = routineRunner.StartCoroutine(CheckWifiHealthCR());
        }

        void CommandEnd()
        {
            if (wifiHealthCheckCR != null) 
            {
                routineRunner.StopCoroutine(wifiHealthCheckCR);
                wifiHealthCheckCR = null;
            }

            Release();
        }
    }
}
