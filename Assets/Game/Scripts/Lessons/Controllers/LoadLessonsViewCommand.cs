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
        //Parameters
        [Inject] public TopicVO topicVO { get; set; }

        //Dispatch Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateLessonsViewSignal updateTopiscViewSignal { get; set; }

        //Models
        [Inject] public IMetaDataModel metaDataModel { get; set; }
        [Inject] public ILessonsModel lessonsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        public override void Execute()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_LESSONS_VIEW);

            var lessonsList = new List<VideoLessonVO>();
            var lessons = lessonsModel.lessonsMapping[topicVO.section][topicVO.name];

            foreach (var lesson in lessons)
            {
                if (metaDataModel.store.items.ContainsKey(lesson))
                {
                    var lessonVO = new VideoLessonVO();
                    lessonVO.name = metaDataModel.store.items[lesson].displayName;
                    lessonVO.videoId = lesson;
                    lessonVO.icon = topicVO.icon;
                    lessonVO.isLocked = !(playerModel.HasSubscription() || playerModel.OwnsVGood(lesson));
                    lessonVO.progress = (float)playerModel.GetVideoProgress(lesson)/100f;
                    lessonsList.Add(lessonVO);
                }
            }

            var vo = new LessonsViewVO();
            vo.topicVO = topicVO;
            vo.lessons = lessonsList;
            updateTopiscViewSignal.Dispatch(vo);
        }
    }
}
