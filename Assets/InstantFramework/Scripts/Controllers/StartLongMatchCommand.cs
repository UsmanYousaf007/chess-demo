/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using strange.extensions.command.impl;
using TurboLabz.TLUtils;
using System;

namespace TurboLabz.InstantFramework
{
    public class StartLongMatchCommand : Command
    {
        // Parameters
        [Inject] public string challengeId { get; set; }

        // Dispatch signals
        [Inject] public StartGameSignal startGameSignal { get; set; }
        [Inject] public UpdateOpponentProfileSignal updateOpponentProfileSignal { get; set; }

        // Models
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        public override void Execute()
        {
            matchInfoModel.activeChallengeId = challengeId;

            MatchInfo matchInfo = matchInfoModel.matches[challengeId];
            PublicProfile publicProfile = matchInfo.opponentPublicProfile;

            ProfileVO pvo = new ProfileVO();
            pvo.playerName = publicProfile.name;
            pvo.eloScore = publicProfile.eloScore;
            pvo.countryId = publicProfile.countryId;

            string opponentId = matchInfo.opponentPublicProfile.playerId;

            if (playerModel.friends.ContainsKey(opponentId))
            {
                pvo.playerPic = playerModel.friends[opponentId].publicProfile.profilePicture;
            }
            else if (playerModel.community.ContainsKey(opponentId))
            {
                pvo.playerPic = playerModel.community[opponentId].publicProfile.profilePicture;
            }

            updateOpponentProfileSignal.Dispatch(pvo);

            startGameSignal.Dispatch();
        }
    }
}
