/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;
using TurboLabz.InstantFramework;

namespace TurboLabz.InstantGame
{
    public class LoadVideoCommand : Command
    {
        // Params
        [Inject] public string videoId { get; set; }

        // Models
        [Inject] public IStoreSettingsModel storeSettingsModel { get; set; }

        // Dispatch Signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public VideoLoadFailedSignal videoLoadFailedSignal { get; set; }

        // Services
        [Inject] public IAWSService aWSService { get; set; }
        [Inject] public IVideoPlaybackService videoPlaybackService { get; set; }

        public override void Execute()
        {
            Retain();

            StoreItem videoItem = storeSettingsModel.GetVideoByShortCode(videoId);
            aWSService.GetSignedUrl(videoItem.videoUrl).Then(OnURLReceived);
        }

        public void OnURLReceived(BackendResult result, string signedUrl)
        {
            if (result == BackendResult.SUCCESS)
            {
                videoPlaybackService.Prepare(signedUrl);
            }
            else
            {
                videoLoadFailedSignal.Dispatch();
            }

            Release();
        }

    }
}
