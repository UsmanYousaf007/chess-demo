/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;
using UnityEngine;
using UnityEngine.UI;

namespace TurboLabz.InstantGame
{
    public class TopicsView : View
    {
        public Text startLessonLabel;
        public Image nextLessonIcon;
        public Text nextLessonName;
        public Button nextLessonButton;
        public Transform categoryContainer;
        public GameObject topicCategory;
        public GameObject topicTile;
        public Text backButtonLabel;
        public Button backButton;
        public GameObject processing;

        private GameObjectsPool categoryPool;
        private GameObjectsPool topicTilePool;
        private string nextLessonId = string.Empty;

        public Signal<string> nextLessonSignal = new Signal<string>();
        public Signal backSignal = new Signal();
        public Signal<TopicVO> loadTopicSignal = new Signal<TopicVO>();

        //Services
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public ILocalizationService localizationService { get; set; }

        public void Init()
        {
            categoryPool = new GameObjectsPool(topicCategory);
            topicTilePool = new GameObjectsPool(topicTile);

            nextLessonButton.onClick.AddListener(OnNextLessonClicked);
            backButton.onClick.AddListener(OnBackButtonClicked);

            startLessonLabel.text = localizationService.Get(LocalizationKey.LESSONS_START);
            backButtonLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_BACK_TO_GAME);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void UpdateView(TopicsViewVO vo)
        {
            nextLessonIcon.sprite = vo.nextLesson.icon;
            nextLessonIcon.SetNativeSize();
            nextLessonName.text = vo.nextLesson.name;
            nextLessonId = vo.nextLesson.videoId;

            foreach (var section in vo.sections)
            {
                var sectionObj = categoryPool.GetObject();
                sectionObj.transform.SetParent(categoryContainer, false);
                sectionObj.GetComponent<TopicCategory>().Init(section.Key, section.Value, topicTilePool, loadTopicSignal);
                sectionObj.SetActive(true);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(categoryContainer.GetComponent<RectTransform>());
        }

        private void OnNextLessonClicked()
        {
            audioService.PlayStandardClick();
            nextLessonSignal.Dispatch(nextLessonId);
        }

        private void OnBackButtonClicked()
        {
            audioService.PlayStandardClick();
            backSignal.Dispatch();
        }
    }
}
