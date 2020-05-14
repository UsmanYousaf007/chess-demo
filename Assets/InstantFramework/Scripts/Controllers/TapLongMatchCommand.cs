/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using UnityEngine;
using strange.extensions.command.impl;
using TurboLabz.TLUtils;
using System.Collections.Generic;


namespace TurboLabz.InstantFramework
{
    public class TapLongMatchCommand : Command
    {
        // Parameters
        [Inject] public string opponentId { get; set; }
        [Inject] public bool isRanked { get; set; }

        // Dispatch signals
        [Inject] public CreateLongMatchSignal createLongMatchSignal { get; set; }
        [Inject] public StartLongMatchSignal startLongMatchSignal { get; set; }
        [Inject] public MatchAnalyticsSignal matchAnalyticsSignal { get; set; }

        // Models
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        public override void Execute()
        {
            matchInfoModel.activeLongMatchOpponentId = opponentId;

            string challengeId = GetChallengeId();

            MatchAnalyticsVO matchAnalyticsVO = new MatchAnalyticsVO();
            matchAnalyticsVO.context = AnalyticsContext.start_attempt;
            matchAnalyticsVO.matchType = "classic";
            matchAnalyticsVO.eventID = AnalyticsEventId.match_find;
            var friend = playerModel.GetFriend(opponentId);
            matchAnalyticsVO.friendType = friend.friendType;
            matchAnalyticsSignal.Dispatch(matchAnalyticsVO);


            if (challengeId == null)
            {
                createLongMatchSignal.Dispatch(opponentId, isRanked);
            }
            else
            {
                startLongMatchSignal.Dispatch(challengeId);
            }
        }

        private string GetChallengeId()
        {
            foreach (KeyValuePair<string, MatchInfo> entry in matchInfoModel.matches)
            {
                if (entry.Value.opponentPublicProfile.playerId == opponentId)
                {
                    return entry.Key;
                }
            }

            return null;
        }
    }
}
