/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public interface IMatchInfoModel
    {
        Dictionary<string, MatchInfo> matches { get; set; }
        MatchInfo CreateMatch(string challengeId);
        string activeChallengeId { get; set; }
        MatchInfo activeMatch { get; }
        string activeLongMatchOpponentId { get; set; }
        List<string> unregisteredChallengeIds { get; set; }
        bool createLongMatchAborted { get; set; }
    }
}
