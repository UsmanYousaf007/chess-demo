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
                    vo.longPlayStatus = LongPlayStatus.NEW_CHALLENGE;
                    vo.lastActionTime = DateTime.UtcNow;
                    vo.playerId = opponentId;

                    updateFriendBarStatusSignal.Dispatch(vo);
                    friendHasMatch = true;

                    break;
                }
            }

            if (!friendHasMatch)
            {
                LongPlayStatusVO vo;
                vo.longPlayStatus = LongPlayStatus.NONE;
                vo.lastActionTime = DateTime.UtcNow;
                vo.playerId = friendId;

                updateFriendBarStatusSignal.Dispatch(vo);
            }
        }
    }
}
