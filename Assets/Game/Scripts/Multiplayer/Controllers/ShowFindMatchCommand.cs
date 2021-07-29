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
        [Inject] public ITournamentsModel tournamentsModel { get; set; }

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
            vo.gameMode = actionData.action;
            vo.powerMode = actionData.powerMode;
            vo.bettingCoins = actionData.betValue;

            vo.player.playerPic = picsModel.GetPlayerPic(playerModel.id);
            vo.player.avatarId = playerModel.avatarId;
            vo.player.avatarColorId = playerModel.avatarBgColorId;
            vo.player.isPremium = playerModel.HasSubscription();

            var leagueAssets = tournamentsModel.GetLeagueSprites(playerModel.league.ToString());
            vo.player.leagueBorder = leagueAssets != null ? leagueAssets.ringSprite : null;

            vo.opponent.playerId = null;

            if (actionData.action != FindMatchAction.ActionCode.Random1.ToString() && actionData.action != FindMatchAction.ActionCode.Random3.ToString()
                && actionData.action != FindMatchAction.ActionCode.Random.ToString() && actionData.action != FindMatchAction.ActionCode.Random10.ToString()
                && actionData.action != FindMatchAction.ActionCode.Random30.ToString() && actionData.action != FindMatchAction.ActionCode.RandomLong.ToString())
            {
                vo.opponent.playerId = actionData.opponentId;

                Friend friend = playerModel.GetFriend(actionData.opponentId);

                if (friend != null)
                {
                    vo.opponent.playerPic = friend.publicProfile.profilePicture;
                    vo.opponent.avatarId = friend.publicProfile.avatarId;
                    vo.opponent.avatarColorId = friend.publicProfile.avatarBgColorId;
                    vo.opponent.isPremium = friend.publicProfile.isSubscriber;
                    vo.opponent.leagueBorder = friend.publicProfile.leagueBorder;
                }
                else
                {
                    vo.opponent.playerPic = picsModel.GetPlayerPic(actionData.opponentId);
                    vo.opponent.avatarId = actionData.avatarId;
                    vo.opponent.avatarColorId = actionData.avatarBgColor;
                }
            }

            var joinedTournament = tournamentsModel.GetJoinedTournament(actionData.tournamentId);
            vo.isTournamentMatch = !string.IsNullOrEmpty(actionData.tournamentId);
            vo.isTicketSpent = joinedTournament != null && joinedTournament.matchesPlayedCount > 0;

            updateFindViewSignal.Dispatch(vo);
        }
    }
}
