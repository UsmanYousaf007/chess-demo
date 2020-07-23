﻿/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.mediation.impl;
using TurboLabz.InstantFramework;

namespace TurboLabz.InstantGame
{
    public class TopicsMediator : Mediator
    {
        // View injection
        [Inject] public TopicsView view { get; set; }

        //Dispatch Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public LoadVideoSignal loadVideoSignal { get; set; }
        [Inject] public LoadLessonsViewSignal loadLessonsViewSignal { get; set; }

        public override void OnRegister()
        {
            view.Init();
            view.backSignal.AddListener(OnBackPressed);
            view.nextLessonSignal.AddListener(OnNextLesson);
            view.loadTopicSignal.AddListener(OnTopicClicked);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.TOPICS_VIEW)
            {
                view.Show();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.TOPICS_VIEW)
            {
                view.Hide();
            }
        }

        [ListensTo(typeof(UpdateTopiscViewSignal))]
        public void OnUpdateView(TopicsViewVO vo)
        {
            view.UpdateView(vo);
        }

        [ListensTo(typeof(VideoEventSignal))]
        public void OnLessonVideoReady(VideoEvent videoEvent)
        {
            if (view.isActiveAndEnabled)
            {
                view.processing.SetActive(false);

                if (videoEvent == VideoEvent.ReadyToPlay)
                {
                    navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_LESSON_VIDEO);
                }
            }
        }

        [ListensTo(typeof(VideoLoadFailedSignal))]
        public void OnLessonLoadFailed()
        {
            if (view.isActiveAndEnabled)
            {
                view.processing.SetActive(false);
            }
        }

        private void OnBackPressed()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }

        private void OnNextLesson(string lessonId, bool isLocked)
        {
            if (isLocked)
            {
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SUBSCRIPTION_DLG);
            }
            else
            {
                view.processing.SetActive(true);
                loadVideoSignal.Dispatch(lessonId);
            }
        }

        private void OnTopicClicked(TopicVO vo)
        {
            view.audioService.PlayStandardClick();
            loadLessonsViewSignal.Dispatch(vo);
        }
    }
}
