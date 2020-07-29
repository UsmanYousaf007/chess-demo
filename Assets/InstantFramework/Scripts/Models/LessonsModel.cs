/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;
using TurboLabz.TLUtils;
using System.Linq;
using TurboLabz.InstantGame;

namespace TurboLabz.InstantFramework
{
    public class LessonsModel : ILessonsModel
    {
        public OrderedDictionary<string, string> topicsMapping { get; set; }
        public OrderedDictionary<string, string> lessonsMapping { get; set; }
        public TopicVO lastViewedTopic { get; set; }

        // Listen to signals
        [Inject] public ModelsResetSignal modelsResetSignal { get; set; }

        //Models
        [Inject] public IPlayerModel playerModel { get; set; }

        [PostConstruct]
        public void PostConstruct()
        {
            modelsResetSignal.AddListener(Reset);
        }

        private void Reset()
        {
            lastViewedTopic = new TopicVO();
            topicsMapping = new OrderedDictionary<string, string>();
            lessonsMapping = new OrderedDictionary<string, string>();
        }

        public int GetCompletedLessonsCount(string topic)
        {
            return (from lesson in lessonsMapping
                    where lesson.Value.Equals(topic) && playerModel.isVideoFullyWatched(lesson.Key)
                    select lesson).Count();
        }

        public bool HasWatchedAllLessons()
        {
            int totalWatchedVideos = (from lesson in lessonsMapping
                                      where playerModel.isVideoFullyWatched(lesson.Key)
                                      select lesson).Count();

            return totalWatchedVideos == lessonsMapping.Count;
        }

        public string GetNextLesson(string currentLesson)
        {
            if (string.IsNullOrEmpty(currentLesson))
            {
                return lessonsMapping.First().Key;
            }

            var currentLessonIndex = lessonsMapping.IndexOf(currentLesson);

            if (currentLessonIndex + 1 < lessonsMapping.Count)
            {
                return lessonsMapping.ElementAt(currentLessonIndex + 1).Key;
            }

            if (!HasWatchedAllLessons())
            {
                return (from lesson in lessonsMapping
                        where !playerModel.isVideoFullyWatched(lesson.Key)
                        select lesson.Key).First();
            }

            return string.Empty;
        }

        public string GetTopicId(string lessonId)
        {
            return GSBackendKeys.GetLessonKey(lessonsMapping[lessonId]);
        }

        private int GetTotalLessonsCount(string topic)
        {
            return (from lesson in lessonsMapping
                    where lesson.Value.Equals(topic)
                    select lesson).Count();
        }

        public OrderedDictionary<string, List<TopicVO>> GetSectionsWithTopicVO(StoreIconsContainer iconsContainer)
        {
            var rv = new OrderedDictionary<string, List<TopicVO>>();

            foreach (var topic in topicsMapping)
            {
                var vo = new TopicVO();
                vo.name = topic.Key;
                vo.total = GetTotalLessonsCount(topic.Key);
                vo.completed = GetCompletedLessonsCount(topic.Key);
                vo.icon = iconsContainer.GetSprite(GSBackendKeys.GetLessonKey(topic.Key));
                vo.section = topic.Value;

                if (rv.ContainsKey(topic.Value))
                {
                    rv[topic.Value].Add(vo);
                }
                else
                {
                    var list = new List<TopicVO>();
                    list.Add(vo);
                    rv.Add(topic.Value, list);
                }
            }

            return rv;
        }

        public IEnumerable<string> GetLessonsByTopicId(string topicId)
        {
            return from lesson in lessonsMapping
                   where lesson.Value.Equals(topicId)
                   select lesson.Key;
        }
    }
}
