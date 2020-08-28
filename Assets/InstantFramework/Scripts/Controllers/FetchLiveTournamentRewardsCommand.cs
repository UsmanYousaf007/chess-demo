/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///

using System;
using strange.extensions.command.impl;

namespace TurboLabz.InstantFramework
{
    public class FetchLiveTournamentRewardsCommand : Command
    {
        // parameter
        [Inject] public string tournamentShortCode { get; set; }

        // Models
        [Inject] public ITournamentsModel tournamentsModel { get; set; }
        
        // dispatch signals
        [Inject] public FetchLiveTournamentRewardsSuccessSignal fetchSuccessSignal { get; set; }
        [Inject] public TournamentOpFailedSignal opFailedSignal { get; set; }

        // services
        [Inject] public IBackendService backendService { get; set; }

        public override void Execute()
        {
            Retain();

            LiveTournamentData openTournament = tournamentsModel.GetOpenTournament(tournamentShortCode);
            if (openTournament != null)
            {
                var lastFetchedTimeDelta = (DateTime.UtcNow - openTournament.lastFetchedTime).TotalSeconds;
                if (lastFetchedTimeDelta >= GSSettings.TOURNAMENTS_FETCH_GAP_TIME)
                {
                    backendService.TournamentsOpGetLiveRewards(tournamentShortCode).Then(OnGetComplete);
                }
                else
                {
                    fetchSuccessSignal.Dispatch(tournamentShortCode);
                }
            }
            else
            {
                OnGetComplete(BackendResult.TOURNAMENTS_OP_FAILED);
            }
        }

        private void OnGetComplete(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                fetchSuccessSignal.Dispatch(tournamentShortCode);
            }
            else
            {
                opFailedSignal.Dispatch();
            }

            Release();
        }
    }
}
