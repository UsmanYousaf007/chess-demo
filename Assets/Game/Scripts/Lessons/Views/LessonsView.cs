/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

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
        public GameObject gridRow;
        public Transform emptyTile;

        private GameObjectsPool lessonTilePool;
        private GameObjectsPool gridRowPool;
        private string lastTopicId = string.Empty;
        private List<LessonTile> lessonTiles;

        public Signal backSignal = new Signal();
        public Signal<LessonTile> playVideoSingal = new Signal<LessonTile>();
        public Signal<LessonTile> unlockVideoSingal = new Signal<LessonTile>();

        //Services
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public ILocalizationService localizationService { get; set; }

        public void Init()
        {
            lessonTilePool = new GameObjectsPool(lessonTile);
            gridRowPool = new GameObjectsPool(gridRow);
            backButton.onClick.AddListener(OnBackButtonClicked);
            backButtonLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_BACK_TO_GAME);
            lessonTiles = new List<LessonTile>();
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

            int i = 0;
            GameObject lessonContainer = null;

            foreach (var lessonVO in vo.lessons)
            {
                if (i % 2 == 0)
                {
                    lessonContainer = gridRowPool.GetObject();
                    lessonContainer.SetActive(true);
                    lessonContainer.transform.SetParent(lessonTileContainer, false);
                }

                var lesson = lessonTilePool.GetObject();
                var lessonTile = lesson.GetComponent<LessonTile>();
                lessonTile.Init(lessonVO);
                lessonTile.button.onClick.RemoveAllListeners();
                lessonTile.button.onClick.AddListener(() => playVideoSingal.Dispatch(lessonTile));
                lessonTile.unlockBtn.onClick.RemoveAllListeners();
                lessonTile.unlockBtn.onClick.AddListener(() => unlockVideoSingal.Dispatch(lessonTile));
                lesson.transform.SetParent(lessonContainer.transform, false);
                lesson.SetActive(true);
                lessonTiles.Add(lessonTile);
                i++;

                if (i == vo.lessons.Count)
                {
                    emptyTile.SetParent( i % 2 == 0 ? transform : lessonContainer.transform, false);
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(lessonTileContainer.GetComponent<RectTransform>());
        }

        private void ClearView()
        {
            foreach (var tile in lessonTileContainer.GetComponentsInChildren<LessonTile>())
            {
                lessonTilePool.ReturnObject(tile.gameObject);
            }

            foreach (var row in lessonTileContainer.GetComponentsInChildren<HorizontalLayoutGroup>())
            {
                gridRowPool.ReturnObject(row.gameObject);
            }

            lessonTiles.Clear();
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

        public void UnlockLessons()
        {
            foreach (var lesson in lessonTiles)
            {
                lesson.Unlock();
            }
        }

        public void UpdateLessons()
        {
            foreach (var lesson in lessonTiles)
            {
                lesson.SetupUnlockButton();
            }
        }

        public void UnlockLesson(string lessonId)
        {
            var lesson = (from lessonTile in lessonTiles
                          where lessonTile.vo.videoId.Equals(lessonId)
                          select lessonTile).FirstOrDefault();

            if (lesson != null)
            {
                lesson.Unlock();
                audioService.Play(audioService.sounds.SFX_REWARD_UNLOCKED);
            }
        }
    }
}
