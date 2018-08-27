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
    public class LongMatchReadyCommand : Command
    {
        // Parameters
        [Inject] public MatchIdVO matchId { get; set; }

        // Dispatch signals
        [Inject] public StartGameSignal startGameSignal { get; set; }

        // Services
        [Inject] public IFacebookService facebookService { get; set; }

        // Models
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }

        public override void Execute()
        {
            Retain();

            if (matchId.opponentId == matchInfoModel.activeLongMatchOpponentId)
            {
                matchInfoModel.activeChallengeId = matchId.challengeId;
                startGameSignal.Dispatch();
            }

            //showFindMatchSignal.Dispatch();
            //backendService.FindMatch().Then(OnFindMatch);

            //findMatchCompleteSignal.AddOnce(OnFindMatchComplete);
        }

        /*

        private void OnFindMatch(BackendResult result)
        {
            if (result == BackendResult.CANCELED)
            {
                Release();
            }
            else if (result != BackendResult.SUCCESS)
            {
              //  backendErrorSignal.Dispatch(result);
                Release();
            }
        }

        private void OnFindMatchComplete(string challengeId)
        {
            matchInfoModel.activeChallengeId = challengeId;
            PublicProfile opponentPublicProfile = matchInfoModel.activeMatch.opponentPublicProfile;

            if (opponentPublicProfile.facebookUserId != null)
            {
                facebookService.GetSocialPic(opponentPublicProfile.facebookUserId, opponentPublicProfile.playerId).Then(OnGetOpponentProfilePicture);
            }
            else
            {
                MatchFound();
            }
        }

        private void OnGetOpponentProfilePicture(FacebookResult result, Sprite sprite, string facebookUserId)
        {
            if (result == FacebookResult.SUCCESS)
            {
                matchInfoModel.activeMatch.opponentPublicProfile.profilePicture = sprite;
            }
            else
            {
                // In case of a failure we just don't set the profile picture.
                LogUtil.LogWarning("Unable to get the profile picture. FacebookResult: " + result);
            }

            MatchFound();
        }

        private void MatchFound()
        {
            // Create and dispatch opponent profile with the match found signal
            PublicProfile publicProfile = matchInfoModel.activeMatch.opponentPublicProfile;

            ProfileVO pvo = new ProfileVO();
            pvo.playerPic = publicProfile.profilePicture;
            pvo.playerName = publicProfile.name;
            pvo.eloScore = publicProfile.eloScore;
            pvo.countryId = publicProfile.countryId;

            //matchFoundSignal.Dispatch(pvo);
            //getGameStartTimeSignal.Dispatch();

            Release();
        }
        */
    }
}
