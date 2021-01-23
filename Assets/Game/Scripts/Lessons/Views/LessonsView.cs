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
using GameAnalyticsSDK;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantGame
{
    public class LessonsView : View
    {
        public Text backButtonLabel;
        public Button backButton;
        public Transform lessonTileContainer;
        public GameObject lessonTile;
        public GameObject processing;
        public ScrollRect scrollView;
        public GameObject gridRow;
        public Transform emptyTile;
        public Button lessonsBanner;

        private GameObjectsPool lessonTilePool;
        private GameObjectsPool gridRowPool;
        private string lastTopicId = string.Empty;
        private List<LessonTile> lessonTiles;

        public Signal backSignal = new Signal();
        public Signal<LessonTile> playVideoSingal = new Signal<LessonTile>();
        public Signal<LessonTile> unlockVideoSingal = new Signal<LessonTile>();
        public Signal unlockAllLessonsSignal = new Signal();
        [Inject] public ShowBottomNavSignal showBottomNavSignal { get; set; }

        //Services
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }

        //Models
        [Inject] public IPreferencesModel preferencesModel { get; set; }

        public void Init()
        {
            lessonTilePool = new GameObjectsPool(lessonTile);
            gridRowPool = new GameObjectsPool(gridRow);
            backButton.onClick.AddListener(OnBackButtonClicked);
            lessonsBanner.onClick.AddListener(OnLessonsBannerClicked);
            backButtonLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_BACK_TO_GAME);
            lessonTiles = new List<LessonTile>();
        }

        public void Show()
        {
            showBottomNavSignal.Dispatch(false);
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            ClearView();
        }

        public void UpdateView(LessonsViewVO vo)
        {
            //SetupTopic(vo.topicVO);
            lessonsBanner.gameObject.SetActive(vo.showBanner);

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
                lessonTile.button.onClick.AddListener(() =>
                {
                    //if (lessonVO.isLocked)
                    //{
                    //    unlockVideoSingal.Dispatch(lessonTile);
                    //}
                    //else
                    //{
                        playVideoSingal.Dispatch(lessonTile);
                    //}
                });
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

            //if (vo.showBanner)
            //{
            //    analyticsService.Event(AnalyticsEventId.booster_shown, AnalyticsContext.key);
            //}
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
                analyticsService.Event($"lesson_{lesson.vo.overallIndex}", AnalyticsContext.unlocked);
                audioService.Play(audioService.sounds.SFX_REWARD_UNLOCKED);
                analyticsService.ResourceEvent(GAResourceFlowType.Sink, GSBackendKeys.PlayerDetails.GEMS, lesson.vo.storeItem.currency3Cost, "lesson_unlocked", lesson.vo.overallIndex.ToString());
            }
        }

        private void OnLessonsBannerClicked()
        {
            audioService.PlayStandardClick();
            unlockAllLessonsSignal.Dispatch();
        }
    }
}
