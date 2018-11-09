/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;
using UnityEngine;
using TurboLabz.TLUtils;
using System.Collections;
using GameSparks.Core;

namespace TurboLabz.InstantFramework
{
	public class StartCommand : Command
	{
		// Dispatch Signals
		[Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
		[Inject] public ReceptionSignal receptionSignal { get; set; }
		[Inject] public BackendErrorSignal backendErrorSignal { get; set; }

		// Services
		[Inject] public IFacebookService facebookService { get; set; }
		[Inject] public IAudioService audioService { get; set; }
		[Inject] public IShareService shareService { get; set; }
		[Inject] public IBackendService backendService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }

		bool gameSparksAvailable = false;
        bool gsAuthenticated = false;

		public override void Execute()
		{
			Retain();

			ListenForKeyEvents();
			navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SPLASH);
			audioService.Init();
			shareService.Init();
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
                    gsAuthenticated = true;
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
            facebookService.Init().Then(OnFacebookInit);
            receptionSignal.Dispatch();
			RemoveListeners();
		}

        void OnFacebookInit(FacebookResult result)
        {
            // Existing facebook account
            if ((result == FacebookResult.SUCCESS) && gsAuthenticated && facebookService.isLoggedIn())
            {
                bool existingPlayer = true;
                backendService.AuthFacebook(facebookService.GetAccessToken(), existingPlayer).Then(OnFacebookAuthComplete);
            }
            else
            {
                Release();
            }
        }

        private void OnFacebookAuthComplete(BackendResult result)
        {
            if (result != BackendResult.SUCCESS)
            {
                backendErrorSignal.Dispatch(result);
            }

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