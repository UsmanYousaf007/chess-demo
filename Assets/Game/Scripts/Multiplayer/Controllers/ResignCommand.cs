using System.Collections.Generic;
using strange.extensions.command.impl;
using TurboLabz.InstantFramework;

namespace TurboLabz.Multiplayer
{
    public class ResignCommand : Command
    {
        // Parameters
        [Inject] public string opponentId { get; set; }

        // Dispatch signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public SyncReconnectDataSignal syncReconnectDataSignal { get; set; }
        [Inject] public MatchAnalyticsSignal matchAnalyticsSignal { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }

        // Models
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IChessboardModel chessboardModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        private string challengeId = null;

        public override void Execute()
        {
            Retain();

            if (opponentId == "")
            {
                challengeId = matchInfoModel.activeChallengeId;
            }
            else
            {
                challengeId = GetChallengeId();
            }

            preferencesModel.resignCount++;
            backendService.PlayerResign(challengeId).Then(OnResign);
        }

        private void OnResign(BackendResult result)
        {
            if (result != BackendResult.SUCCESS && result != BackendResult.CANCELED)
            {
                if (challengeId == matchInfoModel.activeChallengeId)
                {
                    syncReconnectDataSignal.Dispatch(challengeId);
                }
            }
            else
            {
                //Analytics
                var matchInfo = matchInfoModel.matches[challengeId];
                if (matchInfo.isLongPlay)
                {
                    var chessboard = chessboardModel.chessboards[challengeId];
                    if (chessboard.moveList.Count < 2)
                    {
                        var analyticsVO = new MatchAnalyticsVO();
                        analyticsVO.context = AnalyticsContext.cancelled;
                        analyticsVO.matchType = "classic";
                        analyticsVO.eventID = AnalyticsEventId.match_find;
                        var opponentId = matchInfo.opponentPublicProfile.playerId;

                        if (playerModel.friends.ContainsKey(opponentId))
                        {
                            var friendType = playerModel.friends[opponentId].friendType;
                            if (friendType.Equals(GSBackendKeys.Friend.TYPE_SOCIAL))
                            {
                                analyticsVO.friendType = "friends_facebook";

                            }
                            else if (friendType.Equals(GSBackendKeys.Friend.TYPE_FAVOURITE) ||
                                     friendType.Equals(GSBackendKeys.Friend.TYPE_COMMUNITY))
                            {
                                analyticsVO.friendType = "friends_community";
                            }
                        }
                        else
                        {
                            analyticsVO.friendType = "community";
                        }

                        matchAnalyticsSignal.Dispatch(analyticsVO);
                    }
                }
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
