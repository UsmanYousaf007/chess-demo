/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-17 15:42:00 UTC+05:00
///
/// @description
/// [add_description_here]

using strange.extensions.command.impl;

using TurboLabz.InstantFramework;
using TurboLabz.Chess;
using TurboLabz.Multiplayer;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class ShowFindMatchCommand : Command
    {
        // Parameters
        [Inject] public string actionJson { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IPicsModel picsModel { get; set; }

        // Dispatch Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateFindViewSignal updateFindViewSignal { get; set; }

        public override void Execute()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER);
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER_FIND_DLG);

            FindMatchActionData actionData = JsonUtility.FromJson<FindMatchActionData>(actionJson);
            FindViewVO vo = new FindViewVO();
            vo.player = new ProfileVO();
            vo.opponent = new ProfileVO();
            vo.timeoutSeconds = 15;

            vo.player.playerPic = picsModel.GetPlayerPic(playerModel.id);
            vo.player.avatarId = playerModel.avatarId;
            vo.player.avatarColorId = playerModel.avatarBgColorId;

            vo.opponent.playerId = null;

            if (actionData.action != "Random")
            {
                Friend friend = playerModel.GetFriend(actionData.opponentId);

                vo.opponent.playerId = actionData.opponentId;
                vo.opponent.playerPic = friend.publicProfile.profilePicture;
                vo.opponent.avatarId = friend.publicProfile.avatarId;
                vo.opponent.avatarColorId = friend.publicProfile.avatarBgColorId;
            }

            updateFindViewSignal.Dispatch(vo);
        }
    }
}
