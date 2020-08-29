/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
using strange.extensions.command.impl;

namespace TurboLabz.InstantFramework
{
    public class GetTournamentLeaderboardCommand : Command
    {
        // parameter
        [Inject] public string tournamentId { get; set; }
        [Inject] public bool update { get; set; }

        // dispatch signals
        [Inject] public UpdateTournamentLeaderboardSignal getLeaderboardSuccessSignal { get; set; }
        [Inject] public TournamentOpFailedSignal opFailedSignal { get; set; }

        // services
        [Inject] public IBackendService backendService { get; set; }

        public override void Execute()
        {
            Retain();

            backendService.TournamentsOpGetLeaderboard(tournamentId, update).Then(OnGetComplete);
        }

        private void OnGetComplete(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
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
