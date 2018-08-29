/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;
using TurboLabz.TLUtils;
using System.Text;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class ShowProfileDialogCommand : Command
    {
        // Signal parameters
        [Inject] public string opponentId { get; set; }

        // Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateProfileDialogSignal updateProfileDialogSignal { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IPicsModel picsModel { get; set; }

        public override void Execute()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_PROFILE_DLG);

            ProfileDialogVO vo = new ProfileDialogVO();

            vo.playerId = opponentId;
            vo.playerProfilePic = playerModel.profilePic;
            vo.playerProfileName = playerModel.name;
            vo.playerElo = playerModel.eloScore;
            vo.playerCountryCode = playerModel.countryId;

            Friend friend = null;
            if (playerModel.friends.ContainsKey(opponentId))
            {
                friend = playerModel.friends[opponentId];
                vo.isCommunity = false;
            }
            else if (playerModel.community.ContainsKey(opponentId))
            {
                friend = playerModel.community[opponentId];
                vo.isCommunity = true;
            }
            else
            {
                Assertions.Assert(false, "Invalid opponent id");
                return;
            }

            Sprite pic = friend.publicProfile.profilePicture;

            vo.oppProfilePic = pic;
            vo.oppProfileName = friend.publicProfile.name;
            vo.oppElo = friend.publicProfile.eloScore;
            vo.oppCountryCode = friend.publicProfile.countryId;

            vo.playerWinsCount = friend.gamesWon;
            vo.playerDrawsCount = friend.gamesDrawn;
            vo.opponentWinsCount = friend.gamesLost;
            vo.opponentDrawsCount = friend.gamesDrawn;
            vo.totalGamesCount = friend.gamesWon + friend.gamesLost + friend.gamesDrawn;

            updateProfileDialogSignal.Dispatch(vo);
        }
    }
}
