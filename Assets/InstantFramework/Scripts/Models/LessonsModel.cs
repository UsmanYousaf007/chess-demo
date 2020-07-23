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
        public OrderedDictionary<string, OrderedDictionary<string, List<string>>> lessonsMapping { get; set; }
        public int totalVideos { get; set; }

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
            totalVideos = 0;
            lessonsMapping = new OrderedDictionary<string, OrderedDictionary<string, List<string>>>();
        }

        public int GetCompletedLessonsCount(string section, string topic)
        {
            int count = 0;

            foreach (var lesson in lessonsMapping[section][topic])
            {
                if (playerModel.isVideoFullyWatched(lesson))
                {
                    count++;
                }
            }

            return count;
        }

        public bool HasWatchedAllLessons()
        {
            int totalWatchedVideos = 0;

            foreach(var section in lessonsMapping)
            {
                foreach (var topic in section.Value)
                {
                    totalWatchedVideos += GetCompletedLessonsCount(section.Key, topic.Key);
                }
            }

            return totalWatchedVideos == totalVideos;
        }

        public string GetNextLesson(string currentLesson)
        {
            if (string.IsNullOrEmpty(currentLesson))
            {
                return lessonsMapping.First().Value.First().Value.First();
            }

            var found = false;
            foreach (var section in lessonsMapping)
            {
                if (found)
                {
                    return section.Value.First().Value.First();
                }

                foreach (var topic in section.Value)
                {
                    if (found)
                    {
                        return topic.Value.First();
                    }

                    for (int i = 0; i < topic.Value.Count; i++)
                    {
                        if (topic.Value[i].Equals(currentLesson))
                        {
                            if (i + 1 < topic.Value.Count)
                            {
                                return topic.Value[i + 1];
                            }
                            else
                            {
                                found = true;
                            }
                        }
                    }
                }
            }

            if (!HasWatchedAllLessons())
            {
                foreach (var section in lessonsMapping)
                {
                    foreach (var topic in section.Value)
                    {
                        foreach (var lesson in topic.Value)
                        {
                            if (!playerModel.isVideoFullyWatched(lesson))
                            {
                                return lesson;
                            }
                        }
                    }
                }
            }

            return string.Empty;
        }

        public string GetTopicId(string lessonId)
        {
            foreach (var section in lessonsMapping)
            {
                foreach (var topic in section.Value)
                {
                    foreach (var lesson in topic.Value)
                    {
                        if (lesson.Equals(lessonId))
                        {
                            return GSBackendKeys.GetLessonKey(topic.Key);
                        }
                    }
                }
            }

            return "PieceMovement";
        }

        public OrderedDictionary<string, List<TopicVO>> GetSectionsWithTopicVO(StoreIconsContainer iconsContainer)
        {
            var rv = new OrderedDictionary<string, List<TopicVO>>();
            foreach (var section in lessonsMapping)
            {
                var topicList = new List<TopicVO>();
                foreach (var topic in section.Value)
                {
                    var vo = new TopicVO();
                    vo.name = topic.Key;
                    vo.total = topic.Value.Count;
                    vo.completed = GetCompletedLessonsCount(section.Key, topic.Key);
                    vo.icon = iconsContainer.GetSprite(GSBackendKeys.GetLessonKey(topic.Key));
                    vo.section = section.Key;
                    topicList.Add(vo);
                }
                rv.Add(section.Key, topicList);
            }
            return rv;
        }
    }
}
