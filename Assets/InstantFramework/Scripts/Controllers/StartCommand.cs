/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Noor + Faraz <noor@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-07 17:57:19 UTC+05:00
/// 
/// @description
/// This is the entry point to the game.

using strange.extensions.command.impl;
using UnityEngine;
using TurboLabz.TLUtils;
using System.Collections;
//using Facebook.Unity;
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

		// Listen to Signals
		[Inject] public SplashAnimCompleteSignal splashAnimCompleteSignal { get; set; }

		bool gameSparksAvailable;
		bool splashAnimComplete;
		bool facebookInitialized;

		public override void Execute()
		{
			Screen.sleepTimeout = SleepTimeout.NeverSleep;
			Input.multiTouchEnabled = Settings.MULTI_TOUCH_ENABLED;
			Application.targetFrameRate = Settings.TARGET_FRAME_RATE;

            LogUtil.Log("START COMMAND");

			Retain();

			ListenForKeyEvents();
			navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SPLASH);
			facebookService.Init().Then(OnFacebookInit);
			audioService.Init();
			shareService.Init();
		}

		void GameSparksAvailable(bool isAvailable)
		{
			gameSparksAvailable = isAvailable;
			ProcessStartup();
		}

		void OnFacebookInit(FacebookResult result)
		{
			if (result == FacebookResult.SUCCESS)
			{
				facebookInitialized = true;
				ProcessStartup();
			}
		}

		void SplashAnimComplete()
		{
			splashAnimComplete = true;
			ProcessStartup();
		}

		void ProcessStartup()
		{
			if (gameSparksAvailable &&
				splashAnimComplete &&
				facebookInitialized)
			{
				if (GS.Authenticated)
				{
					GotoReception();
				}
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
			else
			{
				backendErrorSignal.Dispatch(result);
			}

			Release();
		}

		void GotoReception()
		{
			receptionSignal.Dispatch();
			RemoveListeners();
			Release();
		}

		void ListenForKeyEvents()
		{
			GS.GameSparksAvailable += GameSparksAvailable;
			splashAnimCompleteSignal.AddListener(SplashAnimComplete);
		}

		void RemoveListeners()
		{
			GS.GameSparksAvailable -= GameSparksAvailable;
			splashAnimCompleteSignal.RemoveAllListeners();
		}
	}
}