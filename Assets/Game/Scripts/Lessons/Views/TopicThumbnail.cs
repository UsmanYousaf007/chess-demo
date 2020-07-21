using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace TurboLabz.InstantGame
{
    public class TopicThumbnail : MonoBehaviour
    {
        [SerializeField] private VideoLessonTopic topic;
        [SerializeField] private Sprite icon;

        private Button topicButton;

        public void Init(UnityAction onTopicButtonClicked)
        {
            topicButton.onClick.AddListener(onTopicButtonClicked);
        }
    }
}
