/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections;
using UnityEngine;
using strange.extensions.command.impl;
using TurboLabz.TLUtils;
using TurboLabz.Multiplayer;
using System.Collections.Generic;

namespace TurboLabz.InstantFramework 
{
    public class DeclineCommand : Command
    {
        // Parameters
        [Inject] public string opponentId { get; set; }

        // Dispatch signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public UnregisterSignal unregisterSignal { get; set; }
        [Inject] public MatchAnalyticsSignal matchAnalyticsSignal { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }

        // Models
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        string challengeId;

        public override void Execute()
        {
            Retain();
            challengeId = GetChallengeId();
            backendService.Decline(challengeId).Then(OnDecline);
        }

        private void OnDecline(BackendResult result)
        {
            if (result != BackendResult.CANCELED && result != BackendResult.SUCCESS)
            {
                backendErrorSignal.Dispatch(result);
            }

            if (result == BackendResult.SUCCESS)
            {
                unregisterSignal.Dispatch(challengeId);

                var analyticsVO = new MatchAnalyticsVO();
                analyticsVO.context = AnalyticsContext.rejected;
                analyticsVO.matchType = "classic";
                analyticsVO.eventID = AnalyticsEventId.match_find;

                if (playerModel.friends.ContainsKey(opponentId))
                {
                    var friendType = playerModel.friends[opponentId].friendType;
                    if (friendType.Equals(GSBackendKeys.Friend.TYPE_SOCIAL))
                    {
                        analyticsVO.friendType = "friends_facebook";

                    }
                    else if (friendType.Equals(GSBackendKeys.Friend.TYPE_FAVOURITE))
                    {
                        analyticsVO.friendType = "friends_community";
                    }
                    else
                    {
                        analyticsVO.friendType = "community";
                    }
                }
                else
                {
                    analyticsVO.friendType = "community";
                }

                matchAnalyticsSignal.Dispatch(analyticsVO);
            }

            Release();
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
