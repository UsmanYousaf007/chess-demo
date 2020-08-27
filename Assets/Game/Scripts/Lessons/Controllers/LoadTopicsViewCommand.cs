/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;
using TurboLabz.InstantFramework;

namespace TurboLabz.InstantGame
{
    public class LoadTopicsViewCommand : Command
    {
        //Dispatch Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateTopiscViewSignal updateTopiscViewSignal { get; set; }

        //Models
        [Inject] public IMetaDataModel metaDataModel { get; set; }
        [Inject] public ILessonsModel lessonsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        private StoreIconsContainer iconsContainer;

        public override void Execute()
        {
            iconsContainer = StoreIconsContainer.Load();
            var nextLesson = lessonsModel.GetNextLesson(playerModel.lastWatchedVideo);
            var vo = new TopicsViewVO();

            if (string.IsNullOrEmpty(nextLesson))
            {
                vo.allLessonsWatched = true;
            }
            else if (metaDataModel.store.items.ContainsKey(nextLesson))
            {
                var nextLessonVO = new VideoLessonVO();
                nextLessonVO.name = metaDataModel.store.items[nextLesson].displayName;
                nextLessonVO.videoId = nextLesson;
                nextLessonVO.icon = iconsContainer.GetSprite(lessonsModel.GetTopicId(nextLesson));
                nextLessonVO.isLocked = !(playerModel.HasSubscription() || playerModel.OwnsVGood(nextLesson) || playerModel.OwnsVGood(GSBackendKeys.ShopItem.ALL_LESSONS_PACK));
                nextLessonVO.progress = (float)playerModel.GetVideoProgress(nextLesson) / 100f;
                nextLessonVO.overallIndex = lessonsModel.lessonsMapping.IndexOf(nextLesson);
                nextLessonVO.section = lessonsModel.topicsMapping[lessonsModel.lessonsMapping[nextLesson]];
                vo.nextLesson = nextLessonVO;
            }

            vo.sections = lessonsModel.GetSectionsWithTopicVO(iconsContainer);
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_TOPICS_VIEW);
            updateTopiscViewSignal.Dispatch(vo);
        }
    }
}
