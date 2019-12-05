using strange.extensions.command.impl;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class ShowInGameProfileCommand : Command
    {
        // Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateProfileDialogSignal updateProfileDialogSignal { get; set; }

        //Models
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        public override void Execute()
        {
            var activeMatch = matchInfoModel.activeMatch == null ? matchInfoModel.lastCompletedMatch : matchInfoModel.activeMatch;
            Friend opponentProfile = playerModel.GetFriend(activeMatch.opponentPublicProfile.playerId);
            PublicProfile opponentPublicProfile = null;

            //if opponentProfile is null then it means match is made using random quick match
            //getting opponent profile from active match
            if (opponentProfile == null)
            {
                opponentPublicProfile = activeMatch.opponentPublicProfile;
            }
            else
            {
                opponentPublicProfile = opponentProfile.publicProfile;
            }

            if (opponentPublicProfile == null) return;

            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_PROFILE_DLG);

            ProfileDialogVO vo = new ProfileDialogVO();
            vo.playerId = opponentPublicProfile.playerId;
            vo.oppProfilePic = opponentPublicProfile.profilePicture;
            vo.oppProfileName = opponentPublicProfile.name;
            vo.oppAvatarId = opponentPublicProfile.avatarId;
            vo.oppAvatarColor = opponentPublicProfile.avatarBgColorId;
            vo.oppElo = opponentPublicProfile.eloScore;
            vo.oppCountryCode = opponentPublicProfile.countryId;
            vo.oppPlayingSinceDate = opponentPublicProfile.creationDateShort;
            vo.oppLastSeen = TimeUtil.DateTimeToRelativeTime(opponentPublicProfile.lastSeenDateTime.ToLocalTime());
            vo.oppTotalGamesWon = opponentPublicProfile.totalGamesWon;
            vo.oppTotalGamesLost = opponentPublicProfile.totalGamesLost;
            vo.oppOnline = opponentPublicProfile.isOnline;
            vo.oppActive = opponentPublicProfile.isActive;
            vo.inGame = true;
            vo.isBot = activeMatch.isBotMatch;

            var friend = playerModel.GetFriend(opponentPublicProfile.playerId);

            if (friend != null)
            {
                vo.playerWinsCount = friend.gamesWon;
                vo.playerDrawsCount = friend.gamesDrawn;
                vo.opponentWinsCount = friend.gamesLost;
                vo.opponentDrawsCount = friend.gamesDrawn;
                vo.totalGamesCount = friend.gamesWon + friend.gamesLost + friend.gamesDrawn;
                vo.friendType = friend.friendType;
            }

            updateProfileDialogSignal.Dispatch(vo);
        }
    }
}
