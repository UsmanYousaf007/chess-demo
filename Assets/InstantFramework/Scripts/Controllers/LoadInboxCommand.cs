
using System;
using strange.extensions.command.impl;

namespace TurboLabz.InstantFramework
{
    public class LoadInboxCommand : Command
    {
        // Models
        [Inject] public IInboxModel inboxModel { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }

        // Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public InboxAddMessagesSignal inboxAddMessagesSignal { get; set; }
        [Inject] public InboxFetchingMessagesSignal inboxFetchingMessagesSignal { get; set; }

        public override void Execute()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_INBOX);

            var diffInSeconds = (DateTime.UtcNow - inboxModel.lastFetchedTime).TotalSeconds;
            if (diffInSeconds > GSSettings.TOURNAMENTS_FETCH_GAP_TIME)
            {
                inboxFetchingMessagesSignal.Dispatch(true);
                backendService.InBoxOpGet().Then(OnInboxGetComplete);
            }
            else
            {
                inboxAddMessagesSignal.Dispatch(inboxModel.items);
            }
        }

        public void OnInboxGetComplete(BackendResult backendResult)
        {
            inboxFetchingMessagesSignal.Dispatch(false);
        }
    }
}
