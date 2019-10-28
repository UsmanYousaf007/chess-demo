/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;
using strange.extensions.mediation.api;
using TurboLabz.Multiplayer;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class StartGameCommand : Command
    {
        // Dispatch signals
        [Inject] public ChessboardEventSignal chessboardEventSignal { get; set; }
        [Inject] public EnableGameChatSignal enableGameChatSignal { get; set; }

        // Models
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IChessboardModel chessboardModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IChatModel chatModel { get; set; }
        [Inject] public IPicsModel picsModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }

        // Services
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IFacebookService facebookService { get; set; }

        public override void Execute()
        {
            //chessboardEventSignal.Dispatch(ChessboardEvent.GAME_STARTED);

            // In case of an ended quick match game results event would have cleared out the board and activeChallengeId.
            if (matchInfoModel.activeChallengeId == null)
            {
                return;
            }

            Chessboard activeChessboard = chessboardModel.chessboards[matchInfoModel.activeChallengeId];

            //reset onboarding tooltip pref
            //if active chessboard's movelist is empty or one move then it means its a new game
            if (activeChessboard.moveList.Count <= 1)
            {
                preferencesModel.isCoachTooltipShown = false;
                preferencesModel.isStrengthTooltipShown = false;
            }
            OnboardingTooltipCommand.oldOpponentScore = 0;
            OnboardingTooltipCommand.oldPlayerScore = 0;

            MatchInfo matchInfo = matchInfoModel.activeMatch;

            if (matchInfo.isLongPlay &&
                matchInfo.acceptStatus == GSBackendKeys.Match.ACCEPT_STATUS_NEW &&
                matchInfo.challengedId == playerModel.id)
            {
                chessboardEventSignal.Dispatch(ChessboardEvent.GAME_ACCEPT_REQUESTED);
            }

            string opponentId = matchInfoModel.activeMatch.opponentPublicProfile.playerId;

            // PREPARE CHAT TODO: Switch over to central player profile management system
            ChatVO vo = new ChatVO();
            vo.chatMessages = chatModel.GetChat(opponentId);
            vo.opponentName = matchInfoModel.activeMatch.opponentPublicProfile.name;
            vo.playerId = playerModel.id;

            if (playerModel.friends.ContainsKey(opponentId))
            {
                vo.opponentName = playerModel.friends[opponentId].publicProfile.name;
            }
            else if (playerModel.community.ContainsKey(opponentId))
            {
                vo.opponentName = playerModel.community[opponentId].publicProfile.name;
            }

            vo.playerProfilePic = picsModel.GetPlayerPic(playerModel.id);
            vo.avatarBgColorId = playerModel.avatarBgColorId;
            vo.avatarId = playerModel.avatarId;

            vo.oppAvatarBgColorId = matchInfoModel.activeMatch.opponentPublicProfile.avatarBgColorId;
            vo.oppAvatarId = matchInfoModel.activeMatch.opponentPublicProfile.avatarId;
            vo.opponentId = opponentId;
            vo.opponentProfilePic = picsModel.GetPlayerPic(opponentId);

            if (vo.opponentProfilePic == null)
            {
                // Try your best to grab the picture
                if (playerModel.friends.ContainsKey(opponentId))
                {
                    vo.opponentProfilePic = playerModel.friends[opponentId].publicProfile.profilePicture;
                }
                else if (playerModel.community.ContainsKey(opponentId))
                {
                    vo.opponentProfilePic = playerModel.community[opponentId].publicProfile.profilePicture;
                }

                if (vo.opponentProfilePic == null)
                {
                    vo.opponentProfilePic = matchInfoModel.activeMatch.opponentPublicProfile.profilePicture;
                }
            }

            vo.hasUnreadMessages = chatModel.hasUnreadMessages.ContainsKey(opponentId);
            vo.unreadMessagesCount = 0;

            if (vo.hasUnreadMessages)
            {
                vo.unreadMessagesCount = chatModel.hasUnreadMessages[opponentId];
            }

            chessboardEventSignal.Dispatch(ChessboardEvent.GAME_STARTED);

            enableGameChatSignal.Dispatch(vo);
            chatModel.hasEngagedChat = false;

            // Analytics
            if (matchInfo.isLongPlay)
            {
                analyticsService.ScreenVisit(AnalyticsScreen.long_match);
            }
            else
            {
                analyticsService.ScreenVisit(AnalyticsScreen.quick_match, facebookService.isLoggedIn(), matchInfo.isBotMatch);
            }
        }
    }
}
