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

        private GameObjectsPool lessonTilePool;

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

            var i = 0;
            foreach(var lessonVO in vo.lessons)
            {
                i++;
                var lesson = lessonTilePool.GetObject();
                var lessonTile = lesson.GetComponent<LessonTile>();
                lessonVO.name = $"{i}. {lessonVO.name}";
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
            topicIcon.sprite = vo.icon;
            topicIcon.SetNativeSize();
            topicName.text = vo.name;
            totalLabel.text = $"{vo.total} Lessons";

            var completedPercentage = (float)vo.completed / (float)vo.total;
            var isCompleted = completedPercentage == 1;
            completedObject.SetActive(isCompleted);
            completedLabel.gameObject.SetActive(!isCompleted);

            progressBar.fillAmount = completedPercentage;
            completedLabel.text = $"{completedPercentage * 100}%";
            progressBar.color = isCompleted ? Colors.GLASS_GREEN : Colors.YELLOW;
        }

        private void OnBackButtonClicked()
        {
            audioService.PlayStandardClick();
            backSignal.Dispatch();
        }
    }
}
