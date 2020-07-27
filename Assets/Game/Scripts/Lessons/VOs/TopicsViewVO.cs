/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantGame
{
    public class TopicsViewVO
    {
        public bool allLessonsWatched;
        public VideoLessonVO nextLesson;
        public OrderedDictionary<string, List<TopicVO>> sections;
    }
}
