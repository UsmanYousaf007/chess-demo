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

using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public class MatchmakingCommand : Command
    {
        // Command parameters
        [Inject] public FindMatchVO findMatchVO { get; set; }

        // Dispatch signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateMatchmakingViewPreMatchFoundSignal updateMatchmakingViewPreMatchFoundSignal { get; set; }
        [Inject] public UpdateMatchmakingViewPostMatchFoundSignal updateMatchmakingViewPostMatchFoundSignal { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public IFacebookService facebookService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IRoomSettingsModel roomSettingsModel { get; set; }

        public override void Execute()
        {
            Retain();

            if (findMatchVO.findMatchInLastPlayedRoom)
            {
                Assertions.Assert(matchInfoModel.roomId != null, "Cannot find match. matchInfoModel.roomId is null!");
                // If finidng match in the last played room keep
                // matchInfoModel.roomId the same as it was before i.e. don't
                // assign any value to it.
            }
            else
            {
                Assertions.Assert(findMatchVO.roomId != null, "Cannot find match. findMatchVO.roomId is null!");
                matchInfoModel.roomId = findMatchVO.roomId;
            }

            PreMatchmakingVO vo;
            vo.currency1 = playerModel.currency1;
            vo.currency2 = playerModel.currency2;
            vo.roomInfo = roomSettingsModel.settings[matchInfoModel.roomId];
            vo.playerPublicProfile = playerModel.publicProfile;
            vo.playerModel = playerModel;

            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MATCH_MAKING);
            updateMatchmakingViewPreMatchFoundSignal.Dispatch(vo);
            backendService.FindMatch(matchInfoModel.roomId).Then(OnFindMatch);
        }

        private void OnFindMatch(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                PublicProfile opponentPublicProfile = matchInfoModel.opponentPublicProfile;

                if (opponentPublicProfile.hasExternalAuth)
                {
                    string opponentId = opponentPublicProfile.externalAuthentications[ExternalAuthType.FACEBOOK].id;
                    facebookService.GetProfilePicture(opponentId).Then(OnGetOpponentProfilePicture);
                }
                else
                {
                    RunPostMatchFoundProcesses();
                }
            }
            else
            {
                backendErrorSignal.Dispatch(result);
                Release();
            }
        }

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

        private void RunPostMatchFoundProcesses()
        {
            // Update model since the bet has been placed on the server.
            RoomSetting roomInfo = roomSettingsModel.settings[matchInfoModel.roomId];
            playerModel.currency1 -= roomInfo.wager;

            PostMatchmakingVO vo;
            vo.currency1 = playerModel.currency1;
            vo.opponentPublicProfile = matchInfoModel.opponentPublicProfile;

            updateMatchmakingViewPostMatchFoundSignal.Dispatch(vo);

            Release();
        }
    }
}
