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

        public override void Execute()
        {
            chessboardEventSignal.Dispatch(ChessboardEvent.GAME_STARTED);

            Chessboard activeChessboard = chessboardModel.chessboards[matchInfoModel.activeChallengeId];
            MatchInfo matchInfo = matchInfoModel.activeMatch;

            if (matchInfo.isLongPlay &&
                matchInfo.acceptStatus == GSBackendKeys.Match.ACCEPT_STATUS_NEW &&
                matchInfo.challengedId == playerModel.id)
            {
                chessboardEventSignal.Dispatch(ChessboardEvent.GAME_ACCEPT_REQUESTED);
            }

            // PREPARE CHAT
            ChatVO vo = new ChatVO();
            vo.chatMessages = chatModel.GetChat(matchInfoModel.activeMatch.opponentPublicProfile.playerId);
            vo.opponentName = matchInfoModel.activeMatch.opponentPublicProfile.name;
            vo.playerId = playerModel.id;
            vo.playerProfilePic = playerModel.profilePic;

            // Handle long match pictures
            if (matchInfoModel.activeMatch.isLongPlay)
            {
                string opponentId = matchInfoModel.activeLongMatchOpponentId;

                if (playerModel.friends.ContainsKey(opponentId))
                {
                    vo.opponentProfilePic = playerModel.friends[opponentId].publicProfile.profilePicture;
                }
                else if (playerModel.community.ContainsKey(opponentId))
                {
                    vo.opponentProfilePic = playerModel.community[opponentId].publicProfile.profilePicture;
                }
            }

            //vo.hasUnreadMessages = chatModel.hasUnreadMessages.ContainsKey(matchInfoModel.activeMatch.opponentPublicProfile.playerId);
            enableGameChatSignal.Dispatch(vo);
        }
    }
}
