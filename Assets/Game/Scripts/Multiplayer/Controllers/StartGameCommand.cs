/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-15 17:46:40 UTC+05:00
/// 
/// @description
/// [add_description_here]

using strange.extensions.command.impl;
using strange.extensions.mediation.api;
using TurboLabz.Multiplayer;

namespace TurboLabz.InstantFramework
{
    public class StartGameCommand : Command
    {
        // Dispatch signals
        [Inject] public ChessboardEventSignal chessboardEventSignal { get; set; }

        // Models
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IChessboardModel chessboardModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

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
        }
    }
}
