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
        }

        private void OnFindMatch(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                PublicProfile opponentPublicProfile = matchInfoModel.opponentPublicProfile;

                if (opponentPublicProfile.usingFacebookAuth)
                {
                    facebookService.GetSocialPic(opponentPublicProfile.facebookAuthId).Then(OnGetOpponentProfilePicture);
                }
                else
                {
                    MatchFound();
                }
            }
            else
            {
                backendErrorSignal.Dispatch(result);
                Release();
            }
        }

        private void OnGetOpponentProfilePicture(FacebookResult result, Sprite sprite)
        {
            if (result == FacebookResult.SUCCESS)
            {
                matchInfoModel.opponentPublicProfile.profilePicture = sprite;
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
            getGameStartTimeSignal.Dispatch();
            Release();
        }
    }
}
