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
    public class LessonsView : View
    {
        public Image topicIcon;
        public Image progressBar;
        public Text completedLabel;
        public Text totalLabel;
        public Text topicName;
        public GameObject completedObject;
        public Text backButtonLabel;
        public Button backButton;
        public Transform lessonTileContainer;
        public GameObject lessonTile;
        public GameObject processing;
        public ScrollRect scrollView;

        private GameObjectsPool lessonTilePool;
        private string lastTopicId = string.Empty;

        public Signal backSignal = new Signal();
        public Signal<VideoLessonVO> playVideoSingal = new Signal<VideoLessonVO>();

        //Services
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public ILocalizationService localizationService { get; set; }

        public void Init()
        {
            lessonTilePool = new GameObjectsPool(lessonTile);
            backButton.onClick.AddListener(OnBackButtonClicked);
            backButtonLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_BACK_TO_GAME);
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

        public void UpdateView(LessonsViewVO vo)
        {
            SetupTopic(vo.topicVO);

            foreach(var lessonVO in vo.lessons)
            {
                var lesson = lessonTilePool.GetObject();
                var lessonTile = lesson.GetComponent<LessonTile>();
                lessonTile.Init(lessonVO);
                lessonTile.button.onClick.RemoveAllListeners();
                lessonTile.button.onClick.AddListener(() => playVideoSingal.Dispatch(lessonVO));
                lesson.transform.SetParent(lessonTileContainer, false);
                lesson.SetActive(true);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(lessonTileContainer.GetComponent<RectTransform>());
        }

        private void ClearView()
        {
            foreach (var tile in lessonTileContainer.GetComponentsInChildren<LessonTile>())
            {
                lessonTilePool.ReturnObject(tile.gameObject);
            }
        }

        private void SetupTopic(TopicVO vo)
        {
            scrollView.verticalNormalizedPosition = lastTopicId.Equals(vo.name) ? scrollView.verticalNormalizedPosition : 1;
            topicIcon.sprite = vo.icon;
            topicIcon.SetNativeSize();
            topicName.text = vo.name;
            totalLabel.text = $"{vo.total} Lessons";

            var completedPercentage = (float)vo.completed / vo.total;
            var isCompleted = completedPercentage == 1;
            var fillAmount = .09f + (vo.completed * ((.91f - .09f) / vo.total));
            completedObject.SetActive(isCompleted);
            completedLabel.gameObject.SetActive(!isCompleted);
            progressBar.fillAmount = fillAmount;
            completedLabel.text = $"{(int)(completedPercentage * 100)}%";
            progressBar.color = isCompleted ? Colors.GLASS_GREEN : Colors.YELLOW;
            lastTopicId = vo.name;
        }

        private void OnBackButtonClicked()
        {
            audioService.PlayStandardClick();
            backSignal.Dispatch();
        }
    }
}
