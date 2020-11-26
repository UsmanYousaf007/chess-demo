
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
        [Inject] public IDailyRewardsService dailyRewardsService { get; set; }

        // Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public InboxAddMessagesSignal inboxAddMessagesSignal { get; set; }

        public override void Execute()
        {
            //navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_INBOX);
            dailyRewardsService.LoadDailyRewards();

            var diffInSeconds = (DateTime.UtcNow - inboxModel.lastFetchedTime).TotalSeconds;
            if (diffInSeconds > GSSettings.TOURNAMENTS_FETCH_GAP_TIME)
            {
                backendService.InBoxOpGet();
            }
            else
            {
                inboxAddMessagesSignal.Dispatch(inboxModel.items);
            }
        }
    }
}
