/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;
using strange.extensions.mediation.api;
using TurboLabz.Multiplayer;
using TurboLabz.TLUtils;
using TurboLabz.Chess;

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
        [Inject] public UpdateOfferDrawSignal updateOfferDrawSignal { get; set; }

        // Services
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IFacebookService facebookService { get; set; }
        [Inject] public IChessAiService chessAiService { get; set; }


        public override void Execute()
        {
            //chessboardEventSignal.Dispatch(ChessboardEvent.GAME_STARTED);

            // In case of an ended quick match game results event would have cleared out the board and activeChallengeId.
            if (matchInfoModel.activeChallengeId == null)
            {
                return;
            }

            chessAiService.AiMoveRequestInit();

            MatchInfo matchInfo = matchInfoModel.activeMatch;

            OnboardingTooltipCommand.oldOpponentScore = 0;
            OnboardingTooltipCommand.oldPlayerScore = 0;

            if (!preferencesModel.isLobbyLoadedFirstTime)
            {
                preferencesModel.isLobbyLoadedFirstTime = true;
            }

            if (matchInfo.isLongPlay &&
                matchInfo.acceptStatus == GSBackendKeys.Match.ACCEPT_STATUS_NEW &&
                matchInfo.challengedId == playerModel.id)
            {
                chessboardEventSignal.Dispatch(ChessboardEvent.GAME_ACCEPT_REQUESTED);
            }

            string opponentId = matchInfoModel.activeMatch.opponentPublicProfile.playerId;

            OfferDrawVO offerDrawVO = new OfferDrawVO();
            offerDrawVO.status = matchInfoModel.activeMatch.drawOfferStatus;
            offerDrawVO.offeredBy = matchInfoModel.activeMatch.drawOfferedBy;
            offerDrawVO.opponentId = opponentId;
            offerDrawVO.challengeId = matchInfoModel.activeChallengeId;
            updateOfferDrawSignal.Dispatch(offerDrawVO);

            Friend opponentProfile = playerModel.GetFriend(opponentId);

            // PREPARE CHAT TODO: Switch over to central player profile management system
            ChatVO vo = new ChatVO();
            vo.playerId = playerModel.id;
            vo.opponentId = opponentId;
            vo.hasUnreadMessages = chatModel.hasUnreadMessages.ContainsKey(opponentId);
            vo.unreadMessagesCount = 0;

            if (vo.hasUnreadMessages)
            {
                vo.unreadMessagesCount = chatModel.hasUnreadMessages[opponentId];
            }

            vo.isChatEnabled = true;

            if (opponentProfile != null && opponentProfile.lastMatchTimestamp <= 0)
            {
                vo.isChatEnabled = !matchInfo.isLongPlay ||
                                    matchInfo.acceptStatus == GSBackendKeys.Match.ACCEPT_STATUS_ACCEPTED ||
                                    opponentProfile.friendType == Friend.FRIEND_TYPE_SOCIAL;
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
