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
        [Inject] public VideoLessonVO lessonVO { get; set; }

        // Models
        [Inject] public IStoreSettingsModel storeSettingsModel { get; set; }
        [Inject] public ILessonsModel lessonsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        // Dispatch Signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public VideoLoadFailedSignal videoLoadFailedSignal { get; set; }
        [Inject] public UpdateVideoLessonViewSignal updateVideoLessonViewSignal { get; set; }

        // Services
        [Inject] public IAWSService aWSService { get; set; }
        [Inject] public IVideoPlaybackService videoPlaybackService { get; set; }

        private StoreIconsContainer iconsContainer;

        public override void Execute()
        {
            Retain();

            if (storeSettingsModel.items.ContainsKey(lessonVO.videoId))
            {
                aWSService.GetSignedUrl(storeSettingsModel.items[lessonVO.videoId].videoUrl).Then(OnURLReceived);
            }
            else
            {
                videoLoadFailedSignal.Dispatch();
            }
        }

        public void OnURLReceived(BackendResult result, string signedUrl)
        {
            if (result == BackendResult.SUCCESS)
            {
                videoPlaybackService.Prepare(signedUrl);
                var vo = new LessonPlayVO();
                var nextLesson = lessonsModel.GetNextLesson(lessonVO.videoId);

                if (storeSettingsModel.items.ContainsKey(lessonVO.videoId))
                {
                    var nextLessonVO = new VideoLessonVO();
                    iconsContainer = StoreIconsContainer.Load();
                    nextLessonVO.name = storeSettingsModel.items[nextLesson].displayName;
                    nextLessonVO.videoId = nextLesson;
                    nextLessonVO.icon = iconsContainer.GetSprite(lessonsModel.GetTopicId(nextLesson));
                    nextLessonVO.isLocked = !(playerModel.HasSubscription() || playerModel.OwnsVGood(nextLesson));
                    nextLessonVO.progress = (float)playerModel.GetVideoProgress(nextLesson) / 100f;
                    vo.nextLesson = nextLessonVO;
                }

                vo.currentLesson = lessonVO;
                updateVideoLessonViewSignal.Dispatch(vo);
            }
            else
            {
                videoLoadFailedSignal.Dispatch();
            }

            Release();
        }

    }
}
