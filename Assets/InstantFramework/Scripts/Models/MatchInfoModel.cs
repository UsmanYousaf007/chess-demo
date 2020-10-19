/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using System.Collections.Generic;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public enum CreateLongMatchAbortReason
    {
        Unassigned,
        CreateFailed,
        LimitReached,
        SelfLimitReached,
        Pending,
        Blocked
    };

    public class MatchInfo
    {
        public long gameStartTimeMilliseconds { get; set; }
        public PublicProfile opponentPublicProfile { get; set; }
        public string botId { get; set; }
        public float botDifficulty { get; set; }
        public EndGameResult endGameResult { get; set; }
        public int playerEloScoreDelta { get; set; }
        public string acceptStatus { get; set; }
        public string challengerId { get; set; }
        public string challengedId { get; set; }
        public bool isLongPlay { get; set; }
        public string winnerId { get; set; }
        public double gameDurationMs { get; set; }
        public long createTimeMs { get; set; }
        public bool isRanked { get; set; }
        public bool acceptedThisSession { get; set; }
        public int playerPowerupUsedCount { get; set; }
        public int opponentPowerupUsedCount { get; set; }
        public bool isTenMinGame { get; set; }
        public string drawOfferStatus { get; set; }
        public string drawOfferedBy { get; set; }
        public bool isOneMinGame { get; set; }
        public bool isThirtyMinGame { get; set; }
        public string gameEndReason { get; set; }
        public bool isTournamentMatch { get; set; }
        public int tournamentMatchScore { get; set; }
        public int tournamentMatchWinTimeBonus { get; set; }

        public bool isBotMatch
        {
            get
            {
                return (botId != null);
            }
        }

        public MatchInfo()
        {
            gameStartTimeMilliseconds = 0;
            opponentPublicProfile = new PublicProfile();
            botId = null;
            botDifficulty = 0;
            endGameResult = EndGameResult.NONE;
            playerEloScoreDelta = 0;
            acceptStatus = null;
            challengedId = null;
            challengedId = null;
            isLongPlay = false;
            gameDurationMs = 0;
            createTimeMs = 0;
            isRanked = false;
            acceptedThisSession = false;
            playerPowerupUsedCount = 0;
            opponentPowerupUsedCount = 0;
            isTenMinGame = false;
            drawOfferStatus = null;
            drawOfferedBy = null;
            isOneMinGame = false;
            gameEndReason = string.Empty;
            isThirtyMinGame = false;
            isTournamentMatch = false;
            tournamentMatchScore = 0;
            tournamentMatchWinTimeBonus = 0;
    }
    }

    public class MatchInfoModel : IMatchInfoModel
    {
        // Listen to signals
        [Inject] public ModelsResetSignal modelsResetSignal { get; set; }

        public OrderedDictionary<string, MatchInfo> matches { get; set; }
        public string activeChallengeId { get; set; }
        public string activeLongMatchOpponentId { get; set; }
        public List<string> unregisteredChallengeIds { get; set; }
        public bool createLongMatchAborted { get; set; }
        public CreateLongMatchAbortReason createLongMatchAbortReason { get; set; }
        public MatchInfo lastCompletedMatch { get; set; }
        public MatchInfo activeMatch 
        { 
            get
            {
                if (activeChallengeId == null ||
                    !matches.ContainsKey(activeChallengeId))
                {
                    return null;
                }

                return matches[activeChallengeId];   
            }
        }

        [PostConstruct]
        public void PostConstruct()
        {
            modelsResetSignal.AddListener(Reset);
        }

        private void Reset()
        {
            matches = new OrderedDictionary<string, MatchInfo>();
            activeLongMatchOpponentId = null;
            unregisteredChallengeIds = new List<string>();
            createLongMatchAborted = false;

            // Retain the activeChallengeId for resume game
            // Do not set activeChallengeId to null here.
        }

        public MatchInfo CreateMatch(string challengeId)
        {
            MatchInfo matchInfo = new MatchInfo();
            matches.Add(challengeId, matchInfo);
            return matchInfo;
        }
    }
}
