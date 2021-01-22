/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;
using strange.extensions.command.impl;
using TurboLabz.InstantFramework;

namespace TurboLabz.InstantGame
{
    public class LoadLessonsViewCommand : Command
    {
        //Dispatch Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateLessonsViewSignal updateTopiscViewSignal { get; set; }

        //Models
        [Inject] public IMetaDataModel metaDataModel { get; set; }
        [Inject] public ILessonsModel lessonsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        public override void Execute()
        {
            var iconsContainer = StoreIconsContainer.Load();
            var lessonsList = new List<VideoLessonVO>();
            var lessons = lessonsModel.lessonsMapping;
            int i = 0;

            foreach (var lesson in lessons)
            {
                if (metaDataModel.store.items.ContainsKey(lesson.Key))
                {
                    i++;
                    var lessonVO = new VideoLessonVO();
                    lessonVO.storeItem = metaDataModel.store.items[lesson.Key];
                    lessonVO.name = lessonVO.storeItem.displayName;
                    lessonVO.indexInTopic = i;
                    lessonVO.videoId = lesson.Key;
                    lessonVO.icon = iconsContainer.GetSprite(GSBackendKeys.GetLessonKey(lesson.Value));
                    lessonVO.isLocked = !(playerModel.HasSubscription() || playerModel.OwnsVGood(lesson.Key) || playerModel.OwnsVGood(GSBackendKeys.ShopItem.ALL_LESSONS_PACK));
                    lessonVO.progress = (float)playerModel.GetVideoProgress(lesson.Key)/100f;
                    lessonVO.overallIndex = lessonsModel.lessonsMapping.IndexOf(lesson.Key);
                    lessonVO.section = lessonsModel.topicsMapping[lessonsModel.lessonsMapping[lesson.Key]]; ;
                    lessonVO.playerModel = playerModel;
                    lessonsList.Add(lessonVO);
                }
            }

            var vo = new LessonsViewVO();
            vo.lessons = lessonsList;
            vo.showBanner = !playerModel.OwnsAllLessons();
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_LESSONS_VIEW);
            updateTopiscViewSignal.Dispatch(vo);
        }
    }
}
