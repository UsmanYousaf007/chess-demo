﻿/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;
using TurboLabz.InstantGame;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public interface ILessonsModel
    {
        OrderedDictionary<string, string> topicsMapping { get; set; }
        OrderedDictionary<string, string> lessonsMapping { get; set; }
        int GetCompletedLessonsCount(string topic);
        bool HasWatchedAllLessons();
        string GetNextLesson(string currentLesson);
        string GetTopicId(string lesson);
        OrderedDictionary<string, List<TopicVO>> GetSectionsWithTopicVO(StoreIconsContainer iconsContainer);
        TopicVO lastViewedTopic { get; set; }
        IEnumerable<string> GetLessonsByTopicId(string topicId);
    }
}