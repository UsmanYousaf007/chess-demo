
using System;
using strange.extensions.command.impl;

namespace TurboLabz.InstantFramework
{
    public class LoadArenaCommand : Command
    {
        // Models
        [Inject] public ITournamentsModel tournamentsModel { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }

        // Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public UpdateTournamentsViewSignal updateTournamentsViewSignal { get; set; }

        public override void Execute()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_ARENA);
            var diffInSeconds = (DateTime.UtcNow - tournamentsModel.lastFetchedTime).TotalSeconds;
            if (diffInSeconds > GSSettings.TOURNAMENTS_FETCH_GAP_TIME)
            {
                Retain();
                backendService.TournamentsOpGetAllTournaments().Then(OnComplete);
            }
            else
            {
                updateTournamentsViewSignal.Dispatch();
            }
        }

        private void OnComplete(BackendResult result)
        {
            if (result != BackendResult.SUCCESS && result != BackendResult.CANCELED)
            {
                backendErrorSignal.Dispatch(result);
            }
            else
            {
                updateTournamentsViewSignal.Dispatch();
            }

            Release();
        }
    }
}
