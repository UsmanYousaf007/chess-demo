using strange.extensions.command.impl;

namespace TurboLabz.InstantFramework
{
    public class SaveLastWatchedVideoCommand : Command
    {
        // Params
        [Inject] public string videoId { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }

        // Signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }

        public override void Execute()
        {
            Retain();

            backendService.SetLastWatchedVideo(videoId).Then(OnComplete);
        }

        private void OnComplete(BackendResult result)
        {
            if (result != BackendResult.SUCCESS && result != BackendResult.CANCELED)
            {
                backendErrorSignal.Dispatch(result);
            }

            Release();
        }
    }
}
