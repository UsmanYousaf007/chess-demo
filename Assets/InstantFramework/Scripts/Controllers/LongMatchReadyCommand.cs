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
            // Is the opponent on the block list? If so, exit
            if (playerModel.blocked.ContainsKey(matchId.opponentId))
            {
                return;
            }

            // A friend or visible community member starts a new game with you
            if (playerModel.friends.ContainsKey(matchId.opponentId) ||
                playerModel.community.ContainsKey(matchId.opponentId))
            {
                
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
                vo.longPlayStatus = (chessboard.currentTurnPlayerId == matchId.opponentId) ?
                    LongPlayStatus.OPPONENT_TURN : LongPlayStatus.PLAYER_TURN;
            }

            vo.playerId = matchId.opponentId;
            updateFriendBarSignal.Dispatch(vo);

            LogUtil.Log("LONG MATCH READY: " + matchId.challengeId, "red");

            // Launch the game if you tapped a player
            if (matchId.opponentId == matchInfoModel.activeLongMatchOpponentId)
            {
                startLongMatchSignal.Dispatch(matchId.challengeId);
            }
        }

    }
}
