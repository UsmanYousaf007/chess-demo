/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;
using System.Collections.Generic;

namespace TurboLabz.InstantFramework 
{
    public class AcceptCommand : Command
    {
        // Parameters
        [Inject] public string opponentId { get; set; }

        // Dispatch signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public UpdateFriendBarSignal updateFriendBarSignal { get; set; }
        [Inject] public SortFriendsSignal sortFriendsSignal { get; set; }
        [Inject] public StartLongMatchSignal startLongMatchSignal { get; set; }
        [Inject] public MatchAnalyticsSignal matchAnalyticsSignal { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IAppsFlyerService appsFlyerService { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }

        // Models
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }

        string challengeId;

        public override void Execute()
        {
            Retain();
            challengeId = GetChallengeId();
            backendService.Accept(challengeId).Then(OnAccept);
        }

        private void OnAccept(BackendResult result)
        {
            if (result != BackendResult.CANCELED && result != BackendResult.SUCCESS)
            {
                backendErrorSignal.Dispatch(result);
            }

            if (result == BackendResult.SUCCESS)
            {
                MatchInfo matchInfo = matchInfoModel.matches[challengeId];
                matchInfo.acceptedThisSession = true;
                matchInfo.acceptStatus = GSBackendKeys.Match.ACCEPT_STATUS_ACCEPTED;
                updateFriendBarSignal.Dispatch(playerModel.friends[opponentId], opponentId);
                sortFriendsSignal.Dispatch();
                startLongMatchSignal.Dispatch(challengeId);

                //Analytics
                preferencesModel.gameStartCount++;
                hAnalyticsService.LogMultiplayerGameEvent(AnalyticsEventId.game_started.ToString(), "gameplay", "long_match", challengeId);
                appsFlyerService.TrackLimitedEvent(AnalyticsEventId.game_started, preferencesModel.gameStartCount);

                MatchAnalyticsVO matchAnalyticsVO = new MatchAnalyticsVO();
                matchAnalyticsVO.context = AnalyticsContext.accepted;
                matchAnalyticsVO.matchType = "classic";
                matchAnalyticsVO.eventID = AnalyticsEventId.match_find;

                if (playerModel.friends.ContainsKey(opponentId))
                {
                    var friendType = playerModel.friends[opponentId].friendType;
                    if (friendType.Equals(GSBackendKeys.Friend.TYPE_SOCIAL))
                    {
                        matchAnalyticsVO.friendType = "friends_facebook";

                    }
                    else if (friendType.Equals(GSBackendKeys.Friend.TYPE_FAVOURITE))
                    {
                        matchAnalyticsVO.friendType = "friends_community";
                    }
                    else
                    {
                        matchAnalyticsVO.friendType = "community";
                    }
                }
                else
                {
                    matchAnalyticsVO.friendType = "community";
                }

                matchAnalyticsSignal.Dispatch(matchAnalyticsVO);
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
