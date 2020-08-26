/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public interface ITournamentsModel
    {
        List<JoinedTournamentData> joinedTournaments { get; set; }
        List<LiveTournamentData> openTournaments { get; set; }
        List<LiveTournamentData> upcomingTournaments { get; set; }

        void calculateCurrentStartTime(LiveTournamentData liveTournament);
        bool isTournamentOpen(LiveTournamentData liveTournament);
        long GetWaitTimeInSeconds(LiveTournamentData liveTournament);
    }
}

