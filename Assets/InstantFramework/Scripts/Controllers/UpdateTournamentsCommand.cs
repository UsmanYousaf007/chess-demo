/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///

using System;
using strange.extensions.command.impl;

namespace TurboLabz.InstantFramework
{
    public class UpdateTournamentsCommand : Command
    {
        // Models
        [Inject] public ITournamentsModel tournamentsModel { get; set; }

        // dispatch signals
        [Inject] public UpdateTournamentsViewSignal updateTournamentsViewSignal { get; set; }
        [Inject] public UpdateTournamentLeaderboardSignal updateTournamentLeaderboardSignal { get; set; }
        [Inject] public TournamentOpFailedSignal opFailedSignal { get; set; }

        // services
        [Inject] public IBackendService backendService { get; set; }

        public override void Execute()
        {
            Retain();

            tournamentsModel.updating = true;

            backendService.TournamentsOpUpdateTournaments().Then(OnGetComplete);

        }

        private void OnGetComplete(BackendResult result)
        {
            tournamentsModel.updating = false;

            if (result != BackendResult.SUCCESS)
            {
                opFailedSignal.Dispatch();
            }

            updateTournamentsViewSignal.Dispatch();
            updateTournamentLeaderboardSignal.Dispatch("");

            Release();
        }
    }
}
