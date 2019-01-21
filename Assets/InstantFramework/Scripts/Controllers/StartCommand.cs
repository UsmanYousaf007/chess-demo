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

        // Services
        [Inject] public IAudioService audioService { get; set; }
		[Inject] public IShareService shareService { get; set; }
		[Inject] public IBackendService backendService { get; set; }
        [Inject] public IAdsService adsService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }

		bool gameSparksAvailable = false;

		public override void Execute()
		{
			Retain();

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
	}
}