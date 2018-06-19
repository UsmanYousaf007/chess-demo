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
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public IFacebookService facebookService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }

        public override void Execute()
        {
            Retain();
            backendService.FindMatch().Then(OnFindMatch);
        }

        private void OnFindMatch(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                LogUtil.Log("Found a match command....! ", "cyan");

                /*
                PublicProfile opponentPublicProfile = matchInfoModel.opponentPublicProfile;

                if (opponentPublicProfile.hasExternalAuth)
                {
                    string opponentId = opponentPublicProfile.externalAuthentications[ExternalAuthType.FACEBOOK].id;
                    facebookService.GetProfilePicture(opponentId).Then(OnGetOpponentProfilePicture);
                }
                else
                {
                    RunPostMatchFoundProcesses();
                }*/
            }
            else
            {
                backendErrorSignal.Dispatch(result);

            }

            Release();
        }
        /*
        private void OnGetOpponentProfilePicture(FacebookResult result, Texture2D texture)
        {
            // In case of a failure we just don't set the profile picture.
            if (result == FacebookResult.SUCCESS)
            {
                Sprite sprite = Sprite.Create(texture,
                                              new Rect(0, 0, texture.width, texture.height),
                                              new Vector2(0.5f, 0.5f));
                sprite.name = texture.name;
                PublicProfile opponentPublicProfile = matchInfoModel.opponentPublicProfile;
                opponentPublicProfile.profilePicture = sprite;
                matchInfoModel.opponentPublicProfile = opponentPublicProfile;
            }
            else
            {
                LogUtil.LogWarning("Unable to get the profile picture. FacebookResult: " + result);
            }

            RunPostMatchFoundProcesses();
        }
        */
        /*
        private void RunPostMatchFoundProcesses()
        {
            // Update model since the bet has been placed on the server.
            RoomSetting roomInfo = roomSettingsModel.settings[matchInfoModel.roomId];
            playerModel.currency1 -= roomInfo.wager;

            PostMatchmakingVO vo;
            vo.currency1 = playerModel.currency1;
            vo.opponentPublicProfile = matchInfoModel.opponentPublicProfile;
            vo.eloStatsWin = matchInfoModel.eloStatsWin;
            vo.eloStatsLose = matchInfoModel.eloStatsLose;
            vo.eloStatsDraw = matchInfoModel.eloStatsDraw;

            updateMatchmakingViewPostMatchFoundSignal.Dispatch(vo);

            Release();
        }
        */
    }
}
