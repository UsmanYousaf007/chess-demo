
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

        public override void Execute()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_INBOX);

            var diffInSeconds = (DateTime.UtcNow - inboxModel.lastFetchedTime).TotalSeconds;
            if (diffInSeconds > GSSettings.TOURNAMENTS_FETCH_GAP_TIME)
            {
                Retain();
                backendService.InBoxOpGet();//.Then(OnComplete);
            }
            else
            {
                inboxAddMessagesSignal.Dispatch(inboxModel.items);
            }
        }

        /*
        private void OnComplete(BackendResult result)
        {
            if (result != BackendResult.SUCCESS && result != BackendResult.CANCELED)
            {
                backendErrorSignal.Dispatch(result);
            }
            else
            {
                inboxAddMessagesSignal.Dispatch(inboxModel.items);
            }

            Release();
        }
        */
    }
}
