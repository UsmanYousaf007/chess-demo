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
        [Inject] public FriendBar friendBar { get; set; }

        // Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateProfileDialogSignal updateProfileDialogSignal { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IPicsModel picsModel { get; set; }

        public override void Execute()
        {
        	TLUtils.LogUtil.LogNullValidation(opponentId, "opponentId");
        	
            if (opponentId == null) return;

            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_PROFILE_DLG);

            ProfileDialogVO vo = new ProfileDialogVO();

            vo.playerId = opponentId;
            vo.playerProfilePic = picsModel.GetPlayerPic(playerModel.id);
            vo.playerProfileName = playerModel.name;
            vo.playerAvatarColor = playerModel.avatarBgColorId;
            vo.playerAvatarId = playerModel.avatarId;
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
            else if (playerModel.search.ContainsKey(opponentId))
            {
                friend = playerModel.search[opponentId];
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
            vo.oppAvatarId = friend.publicProfile.avatarId;
            vo.oppAvatarColor = friend.publicProfile.avatarBgColorId;
            vo.oppElo = friend.publicProfile.eloScore;
            vo.oppCountryCode = friend.publicProfile.countryId;
            vo.oppPlayingSinceDate = friend.publicProfile.creationDateShort;
            vo.oppLastSeen = TimeUtil.DateTimeToRelativeTime(friend.publicProfile.lastSeenDateTime.ToLocalTime());

            vo.oppTotalGamesWon = friend.publicProfile.totalGamesWon;
            vo.oppTotalGamesLost = friend.publicProfile.totalGamesLost;

            vo.playerWinsCount = friend.gamesWon;
            vo.playerDrawsCount = friend.gamesDrawn;
            vo.opponentWinsCount = friend.gamesLost;
            vo.opponentDrawsCount = friend.gamesDrawn;
            vo.totalGamesCount = friend.gamesWon + friend.gamesLost + friend.gamesDrawn;
            vo.friendType = friend.friendType;
            vo.longPlayStatus = friendBar.longPlayStatus;
            vo.oppOnline = friend.publicProfile.isOnline;
            vo.oppActive = friend.publicProfile.isActive;
            vo.isOpponentPremium = friend.publicProfile.isSubscriber;

            updateProfileDialogSignal.Dispatch(vo);
        }
    }
}
