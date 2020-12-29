
using System;
using strange.extensions.command.impl;

namespace TurboLabz.InstantFramework
{
    public class LoadRewardsCommand : Command
    {
        // Models
        [Inject] public IInboxModel inboxModel { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }

        // Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public InboxAddMessagesSignal inboxAddMessagesSignal { get; set; }

        public override void Execute()
        {
            var diffInSeconds = (DateTime.UtcNow - inboxModel.lastFetchedTime).TotalSeconds;
            if (diffInSeconds > GSSettings.LEADERBOARDS_FETCH_GAP_TIME)
            {
                backendService.InBoxOpGet();
            }
            else
            {
                inboxAddMessagesSignal.Dispatch();
            }
        }
    }
}
