/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
using strange.extensions.command.impl;

namespace TurboLabz.InstantFramework
{
    public class GetAllTournamentsCommand : Command
    {
        // dispatch signals
        [Inject] public GetTournamentsSuccessSignal getTournamentsSuccessSignal { get; set; }
        [Inject] public TournamentOpFailedSignal opFailedSignal { get; set; }

        // services
        [Inject] public IBackendService backendService { get; set; }

        public override void Execute()
        {
            Retain();

            backendService.TournamentsOpGetAllTournaments().Then(OnGetComplete);
        }

        private void OnGetComplete(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                getTournamentsSuccessSignal.Dispatch();
            }
            else
            {
                opFailedSignal.Dispatch();
            }

            Release();
        }
    }
}
