/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-11 13:42:52 UTC+05:00
///
/// @description
/// [add_description_here]

using UnityEngine;
using strange.extensions.command.impl;
using TurboLabz.TLUtils;


namespace TurboLabz.InstantFramework
{
    public class FindMatchCommand : Command
    {
        // Dispatch signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public ShowFindMatchSignal showFindMatchSignal { get; set; }
        [Inject] public GetGameStartTimeSignal getGameStartTimeSignal { get; set; }
        [Inject] public MatchFoundSignal matchFoundSignal { get; set; }
        [Inject] public UpdateOpponentProfileSignal updateOpponentProfileSignal { get; set; }

        // Listen to signal
        [Inject] public FindMatchCompleteSignal findMatchCompleteSignal { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public IFacebookService facebookService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }

        public override void Execute()
        {
            Retain();
            showFindMatchSignal.Dispatch();
            backendService.FindMatch().Then(OnFindMatch);

            findMatchCompleteSignal.AddOnce(OnFindMatchComplete);
        }

        private void OnFindMatch(BackendResult result)
        {
            if (result == BackendResult.CANCELED)
            {
                Release();
            }
            else if (result != BackendResult.SUCCESS)
            {
                backendErrorSignal.Dispatch(result);
                Release();
            }
        }

        private void OnFindMatchComplete(string challengeId)
        {
            matchInfoModel.activeChallengeId = challengeId;
            MatchFound();

            PublicProfile opponentPublicProfile = matchInfoModel.activeMatch.opponentPublicProfile;
            if (opponentPublicProfile.facebookUserId != null)
            {
                facebookService.GetSocialPic(opponentPublicProfile.facebookUserId, opponentPublicProfile.playerId).Then(OnGetOpponentProfilePicture);
            }
        }

        private void OnGetOpponentProfilePicture(FacebookResult result, Sprite sprite, string facebookUserId)
        {
            // Todo: create a separate signal for just updating the opponent picture.
            if (result == FacebookResult.SUCCESS)
            {
                matchInfoModel.activeMatch.opponentPublicProfile.profilePicture = sprite;
                ProfileVO pvo = GetOpponentProfile();
                updateOpponentProfileSignal.Dispatch(pvo);
            }

            Release();
        }

        private void MatchFound()
        {
            // Create and dispatch opponent profile with the match found signal
            ProfileVO pvo = GetOpponentProfile();

            matchFoundSignal.Dispatch(pvo);
            updateOpponentProfileSignal.Dispatch(pvo);

            getGameStartTimeSignal.Dispatch();

            if (matchInfoModel.activeMatch.opponentPublicProfile.facebookUserId == null)
            {
                Release();
            }
        }

        private ProfileVO GetOpponentProfile()
        {
            PublicProfile publicProfile = matchInfoModel.activeMatch.opponentPublicProfile;

            ProfileVO pvo = new ProfileVO();
            pvo.playerPic = publicProfile.profilePicture;
            pvo.playerName = publicProfile.name;
            pvo.eloScore = publicProfile.eloScore;
            pvo.countryId = publicProfile.countryId;
            pvo.playerId = publicProfile.playerId;
            pvo.isOnline = true;

            return pvo;
        }
    }
}
