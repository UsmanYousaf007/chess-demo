/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.mediation.impl;
using TurboLabz.InstantFramework;

namespace TurboLabz.InstantGame
{
    public class LessonsMediator : Mediator
    {
        // View injection
        [Inject] public LessonsView view { get; set; }

        //Dispatch Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public LoadVideoSignal loadVideoSignal { get; set; }
        [Inject] public SetSubscriptionContext setSubscriptionContext { get; set; }

        //Analytics Service
        [Inject] public IAnalyticsService analyticsService { get; set; }

        public override void OnRegister()
        {
            view.Init();
            view.backSignal.AddListener(OnBackPressed);
            view.playVideoSingal.AddListener(OnPlayVideo);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.LESSONS_VIEW)
            {
                view.Show();
                analyticsService.ScreenVisit(AnalyticsScreen.lessons_videos);
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.LESSONS_VIEW)
            {
                view.Hide();
            }
        }

        [ListensTo(typeof(UpdateLessonsViewSignal))]
        public void OnUpdateView(LessonsViewVO vo)
        {
            view.UpdateView(vo);
        }

        [ListensTo(typeof(VideoEventSignal))]
        public void OnLessonVideoReady(VideoEvent videoEvent)
        {
            if (view.isActiveAndEnabled)
            {
                if (videoEvent == VideoEvent.ReadyToPlay)
                {
                    navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_LESSON_VIDEO);
                }

                view.processing.SetActive(false);
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

        private void OnPlayVideo(VideoLessonVO vo)
        {
            view.audioService.PlayStandardClick();

            if (vo.isLocked)
            {
                setSubscriptionContext.Dispatch($"lessons_{vo.section.ToLower().Replace(' ', '_')}");
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SUBSCRIPTION_DLG);
            }
            else
            {
                view.processing.SetActive(true);
                loadVideoSignal.Dispatch(vo);
            }
        }
    }
}
