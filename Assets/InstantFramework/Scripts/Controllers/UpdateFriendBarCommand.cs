/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using strange.extensions.command.impl;
using TurboLabz.TLUtils;
using System;
using TurboLabz.Multiplayer;
using System.Collections.Generic;
using TurboLabz.Chess;


namespace TurboLabz.InstantFramework
{
    public class UpdateFriendBarCommand : Command
    {
        // Parameters
        [Inject] public string friendId { get; set; }

        // Dispatch signals
        [Inject] public UpdateFriendBarStatusSignal updateFriendBarStatusSignal { get; set; }

        // Models
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IChessboardModel chessboardModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        public override void Execute()
        {
            bool friendHasMatch = false;

            foreach (KeyValuePair<string, MatchInfo> entry in matchInfoModel.matches)
            {
                MatchInfo matchInfo = entry.Value;
                string opponentId = matchInfo.opponentPublicProfile.playerId;

                if (friendId == opponentId)
                {
                    Chessboard chessboard = chessboardModel.chessboards[entry.Key];
                    LongPlayStatusVO vo;
                    vo.playerId = friendId;
                    vo.lastActionTime = DateTime.UtcNow;
                    vo.longPlayStatus = LongPlayStatus.DEFAULT;

                    // NEW_CHALLENGE
                    if (matchInfo.acceptStatus == GSBackendKeys.Match.ACCEPT_STATUS_NEW)
                    {
                        if (matchInfo.challengerId == playerModel.id)
                        {
                            vo.longPlayStatus = (chessboard.isPlayerTurn) ? LongPlayStatus.PLAYER_TURN : LongPlayStatus.OPPONENT_TURN;
                        }
                        else
                        {
                            vo.longPlayStatus = LongPlayStatus.NEW_CHALLENGE;
                        }

                        vo.lastActionTime = chessboard.lastMoveTime;
                    }
                    // DECLINED
                    else if (matchInfo.acceptStatus == GSBackendKeys.Match.ACCEPT_STATUS_DECLINED &&
                        matchInfo.challengerId == playerModel.id)
                    {
                        vo.longPlayStatus = LongPlayStatus.DECLINED;
                    }
                    // PLAYER_TURN OR OPPONENT_TURN
                    else if (chessboard.gameEndReason == GameEndReason.NONE)
                    {
                        vo.longPlayStatus = (chessboard.isPlayerTurn) ? LongPlayStatus.PLAYER_TURN : LongPlayStatus.OPPONENT_TURN;
                        vo.lastActionTime = chessboard.lastMoveTime;
                    }
                    // WIN/DRAW/LOSE
                    else if (chessboard.gameEndReason != GameEndReason.DECLINED)
                    {
                        if (chessboard.gameEndReason == GameEndReason.CHECKMATE ||
                            chessboard.gameEndReason == GameEndReason.RESIGNATION ||
                            chessboard.gameEndReason == GameEndReason.TIMER_EXPIRED)
                        {
                            vo.longPlayStatus = (matchInfo.winnerId == playerModel.id) ? LongPlayStatus.PLAYER_WON : LongPlayStatus.OPPONENT_WON;
                        }
                        else
                        {
                            vo.longPlayStatus = LongPlayStatus.DRAW;
                        }
                    }

                    updateFriendBarStatusSignal.Dispatch(vo);
                    friendHasMatch = true;



                    break;
                }
            }

            if (!friendHasMatch)
            {
                LongPlayStatusVO vo;
                vo.longPlayStatus = LongPlayStatus.DEFAULT;
                vo.lastActionTime = DateTime.UtcNow;
                vo.playerId = friendId;

                updateFriendBarStatusSignal.Dispatch(vo);
            }


        }
    }
}
