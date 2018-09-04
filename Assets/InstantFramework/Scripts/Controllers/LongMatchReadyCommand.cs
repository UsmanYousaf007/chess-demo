/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-11 13:42:52 UTC+05:00
///
/// @description
/// [add_description_here]

using UnityEngine;
using strange.extensions.command.impl;
using TurboLabz.TLUtils;
using System;
using TurboLabz.Multiplayer;


namespace TurboLabz.InstantFramework
{
    public class LongMatchReadyCommand : Command
    {
        // Parameters
        [Inject] public MatchIdVO matchId { get; set; }

        // Dispatch signals
        [Inject] public StartLongMatchSignal startLongMatchSignal { get; set; }
        [Inject] public UpdateFriendBarSignal updateFriendBarSignal { get; set; }

        // Models
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IChessboardModel chessboardModel { get; set; }

        public override void Execute()
        {
            LogUtil.Log("Long match ready...", "white");


            // Is the opponent on the block list? If so, exit
            if (playerModel.blocked.ContainsKey(matchId.opponentId))
            {
                return;
            }

            MatchInfo matchInfo = matchInfoModel.matches[matchId.challengeId];
            DateTime startTime = TimeUtil.ToDateTime(matchInfo.gameStartTimeMilliseconds);

            LongPlayStatusVO vo = new LongPlayStatusVO();
            vo.lastActionTime = startTime;

            // If you didn't start this match then this person has challenged you
            if (matchId.opponentId != matchInfoModel.activeLongMatchOpponentId)
            {
                vo.longPlayStatus = LongPlayStatus.NEW_CHALLENGE;
            }
            // else set it to the person who's turn it is
            else
            {
                Chessboard chessboard = chessboardModel.chessboards[matchId.challengeId];
                vo.longPlayStatus = (chessboard.isPlayerTurn) ?
                    LongPlayStatus.PLAYER_TURN : LongPlayStatus.OPPONENT_TURN;
            }

            vo.playerId = matchId.opponentId;
            updateFriendBarSignal.Dispatch(vo);

            // Launch the game if you tapped a player
            if (matchId.opponentId == matchInfoModel.activeLongMatchOpponentId)
            {
                startLongMatchSignal.Dispatch(matchId.challengeId);
            }
        }
    }
}
