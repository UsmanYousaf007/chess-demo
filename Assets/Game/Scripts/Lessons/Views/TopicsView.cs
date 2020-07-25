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
        public Image nextLessonIcon;
        public Text nextLessonName;
        public Button[] nextLessonButtons;
        public Image nextLessonProgress;
        public Transform categoryContainer;
        public GameObject topicCategory;
        public GameObject topicTile;
        public Text backButtonLabel;
        public Button backButton;
        public GameObject processing;
        public GameObject nextLessonSection;
        public GameObject lessonsCompletedSection;
        public Text lessonCompletedTitle;
        public Text lessonCompletedDescription;

        private GameObjectsPool categoryPool;
        private GameObjectsPool topicTilePool;
        private VideoLessonVO lessonVO;

        public Signal<VideoLessonVO> nextLessonSignal = new Signal<VideoLessonVO>();
        public Signal backSignal = new Signal();
        public Signal<TopicVO> loadTopicSignal = new Signal<TopicVO>();

        //Services
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public ILocalizationService localizationService { get; set; }

        public void Init()
        {
            categoryPool = new GameObjectsPool(topicCategory);
            topicTilePool = new GameObjectsPool(topicTile);

            foreach (var nextLessonButton in nextLessonButtons)
            {
                nextLessonButton.onClick.AddListener(OnNextLessonClicked);
            }
            backButton.onClick.AddListener(OnBackButtonClicked);

            backButtonLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_BACK_TO_GAME);
            lessonCompletedTitle.text = localizationService.Get(LocalizationKey.LESSONS_COMPLETED_TITLE);
            lessonCompletedDescription.text = localizationService.Get(LocalizationKey.LESSONS_COMPLETED_DESCRIPTION);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            ClearView();
        }

        public void UpdateView(TopicsViewVO vo)
        {
            nextLessonSection.SetActive(!vo.allLessonsWatched);
            lessonsCompletedSection.SetActive(vo.allLessonsWatched);
            nextLessonIcon.sprite = vo.nextLesson.icon;
            nextLessonName.text = vo.nextLesson.name;
            nextLessonProgress.fillAmount = vo.nextLesson.progress;
            lessonVO = vo.nextLesson;

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
            nextLessonSignal.Dispatch(lessonVO);
        }

        private void OnBackButtonClicked()
        {
            audioService.PlayStandardClick();
            backSignal.Dispatch();
        }

        private void ClearView()
        {
            foreach (var category in categoryContainer.GetComponentsInChildren<TopicCategory>())
            {
                categoryPool.ReturnObject(category.gameObject);
                category.Reset();
            }
        }
    }
}
