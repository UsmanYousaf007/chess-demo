/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public interface ITournamentsModel
    {
        DateTime lastFetchedTime { get; set; }

        List<JoinedTournamentData> joinedTournaments { get; set; }
        List<LiveTournamentData> openTournaments { get; set; }
        List<LiveTournamentData> upcomingTournaments { get; set; }

        long CalculateCurrentStartTime(long waitTimeSeconds, long durationSeconds, long firstStartTimeSeconds);
        long CalculateTournamentTimeLeftSeconds(JoinedTournamentData joinedTournament);
        long CalculateTournamentTimeLeftSeconds(LiveTournamentData liveTournament);
        bool isTournamentOpen(LiveTournamentData liveTournament);
    }
}

