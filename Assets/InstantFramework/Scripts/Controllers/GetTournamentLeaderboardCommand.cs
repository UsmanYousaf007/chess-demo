/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///

using System;
using strange.extensions.command.impl;

namespace TurboLabz.InstantFramework
{
    public class GetTournamentLeaderboardCommand : Command
    {
        // parameter
        [Inject] public string tournamentId { get; set; }
        [Inject] public bool update { get; set; } // Not used anymore.

        // dispatch signals
        [Inject] public UpdateTournamentLeaderboardSignal getLeaderboardSuccessSignal { get; set; }
        [Inject] public TournamentOpFailedSignal opFailedSignal { get; set; }
        [Inject] public UpdateTournamentsViewSignal updateTournamentsViewSignal { get; set; }

        // Models
        [Inject] public ITournamentsModel tournamentsModel { get; set; }

        // services
        [Inject] public IBackendService backendService { get; set; }

        public override void Execute()
        {
            Retain();

            JoinedTournamentData joinedTournament = tournamentsModel.GetJoinedTournament(tournamentId);
            if (joinedTournament != null)
            {
                var lastFetchedTimeDelta = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - joinedTournament.lastFetchedTimeUTCSeconds;
                if (lastFetchedTimeDelta >= GSSettings.LEADERBOARDS_FETCH_GAP_TIME)
                {
                    backendService.TournamentsOpGetLeaderboard(tournamentId, update).Then(OnGetComplete);
                }
                else
                {
                    getLeaderboardSuccessSignal.Dispatch(tournamentId);
                }
            }
            else
            {
                backendService.TournamentsOpGetLeaderboard(tournamentId, update).Then(OnGetComplete);
            }

        }

        private void OnGetComplete(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                updateTournamentsViewSignal.Dispatch();
                getLeaderboardSuccessSignal.Dispatch(tournamentId);
            }
            else
            {
                opFailedSignal.Dispatch();
            }

            Release();
        }
    }
}
