/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using strange.extensions.command.impl;
using TurboLabz.TLUtils;
using System;
using TurboLabz.Chess;

namespace TurboLabz.InstantFramework
{
    public class StartLongMatchCommand : Command
    {
        // Parameters
        [Inject] public string challengeId { get; set; }

        // Dispatch signals
        [Inject] public StartGameSignal startGameSignal { get; set; }
        [Inject] public UpdateOpponentProfileSignal updateOpponentProfileSignal { get; set; }
        [Inject] public FriendBarBusySignal friendBarBusySignal { get; set; }

        // Models
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        // Services
        [Inject] public IChessAiService chessAiService { get; set; }

        public override void Execute()
        {
            matchInfoModel.activeChallengeId = challengeId;

            MatchInfo matchInfo = matchInfoModel.matches[challengeId];
            string opponentId = matchInfo.opponentPublicProfile.playerId;

            PublicProfile publicProfile =  playerModel.friends[opponentId].publicProfile;

            ProfileVO pvo = new ProfileVO();
            pvo.playerName = publicProfile.name;
            pvo.eloScore = publicProfile.eloScore;
            pvo.countryId = publicProfile.countryId;
            pvo.playerId = publicProfile.playerId;
            pvo.isOnline = publicProfile.isOnline;
            pvo.playerPic = publicProfile.profilePicture;
            pvo.avatarId = publicProfile.avatarId;
            pvo.avatarColorId = publicProfile.avatarBgColorId;
            pvo.isActive = publicProfile.isActive;
            pvo.isPremium = publicProfile.isSubscriber;
            pvo.leagueBorder = publicProfile.leagueBorder;
            pvo.trophies2 = publicProfile.trophies2;

            updateOpponentProfileSignal.Dispatch(pvo);

            friendBarBusySignal.Dispatch(opponentId, false, CreateLongMatchAbortReason.Unassigned);
            chessAiService.AiMoveRequestInit();
            startGameSignal.Dispatch();
        }
    }
}
