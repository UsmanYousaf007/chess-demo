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

            FindMatchAction.ActionData actionData = JsonUtility.FromJson<FindMatchAction.ActionData>(actionJson);
            FindViewVO vo = new FindViewVO();
            vo.player = new ProfileVO();
            vo.opponent = new ProfileVO();
            vo.timeoutSeconds = 30;

            vo.player.playerPic = picsModel.GetPlayerPic(playerModel.id);
            vo.player.avatarId = playerModel.avatarId;
            vo.player.avatarColorId = playerModel.avatarBgColorId;
            vo.player.isPremium = playerModel.HasSubscription();

            vo.opponent.playerId = null;

            if (actionData.action != FindMatchAction.ActionCode.Random.ToString() && actionData.action != FindMatchAction.ActionCode.RandomLong.ToString()
                && actionData.action != FindMatchAction.ActionCode.Random10.ToString() && actionData.action != FindMatchAction.ActionCode.Random1.ToString())
            {
                vo.opponent.playerId = actionData.opponentId;

                Friend friend = playerModel.GetFriend(actionData.opponentId);

                if (friend != null)
                {
                    vo.opponent.playerPic = friend.publicProfile.profilePicture;
                    vo.opponent.avatarId = friend.publicProfile.avatarId;
                    vo.opponent.avatarColorId = friend.publicProfile.avatarBgColorId;
                    vo.opponent.isPremium = friend.publicProfile.isSubscriber;

                    // Updating this here so the friend can be added in recent played list.
                    // This is updated on server by itself when the match starts.
                    friend.removedFromRecentPlayed = false;
                }
                else
                {
                    vo.opponent.playerPic = picsModel.GetPlayerPic(actionData.opponentId);
                    vo.opponent.avatarId = actionData.avatarId;
                    vo.opponent.avatarColorId = actionData.avatarBgColor;
                }
            }

            updateFindViewSignal.Dispatch(vo);
        }
    }
}
