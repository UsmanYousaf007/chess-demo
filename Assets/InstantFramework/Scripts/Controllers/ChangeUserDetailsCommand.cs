/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections;
using UnityEngine;
using strange.extensions.command.impl;
using TurboLabz.TLUtils;
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
	public class ChangeUserDetailsCommand : Command
	{
		// Parameters
		[Inject] public string displayName { get; set; }

		// Dispatch signals
		[Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public UpdateProfileSignal updateProfileSignal { get; set; }
        [Inject] public UpdateCareerCardSignal updateCareerCardSignal { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }
		[Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IFacebookService facebookService { get; set; }
        [Inject] public ISignInWithAppleService signInWithAppleService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IPicsModel picsModel { get; set; }
        [Inject] public ITournamentsModel tournamentsModel { get; set; }

        string challengeId;

		public override void Execute()
		{		
            backendService.ChangeUserDetails(displayName).Then(OnSuccess);
        }

		private void OnSuccess(BackendResult result)
		{
			if (result != BackendResult.CANCELED && result != BackendResult.SUCCESS)
			{
				backendErrorSignal.Dispatch(result);
			}

			if (result == BackendResult.SUCCESS)
			{
                DispatchProfileSignal();
            }

		}

        private void DispatchProfileSignal()
        {
            ProfileVO pvo = new ProfileVO();
            pvo.playerPic = picsModel.GetPlayerPic(playerModel.id);
            pvo.playerName = playerModel.name;
            pvo.eloScore = playerModel.eloScore;
            pvo.countryId = playerModel.countryId;
            pvo.isFacebookLoggedIn = facebookService.isLoggedIn();
            pvo.isAppleSignedIn = signInWithAppleService.IsSignedIn();
            pvo.isAppleSignInSupported = signInWithAppleService.IsSupported();
            pvo.playerId = playerModel.id;
            pvo.avatarId = playerModel.avatarId;
            pvo.avatarColorId = playerModel.avatarBgColorId;
            var leagueAssets = tournamentsModel.GetLeagueSprites(playerModel.league.ToString());
            pvo.leagueBorder = leagueAssets != null ? leagueAssets.ringSprite : null;

            if (pvo.isFacebookLoggedIn && pvo.playerPic == null)
            {
                pvo.playerPic = picsModel.GetPlayerPic(playerModel.id);
            }

            updateProfileSignal.Dispatch(pvo);
            updateCareerCardSignal.Dispatch();
        }

    }
}
