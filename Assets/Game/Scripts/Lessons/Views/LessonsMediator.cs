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

        //Models
        [Inject] public IAppInfoModel appInfoModel { get; set; }

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
                    view.processing.SetActive(false);
                    appInfoModel.isVideoLoading = false;
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
                appInfoModel.isVideoLoading = false;
            }
        }

        private void OnBackPressed()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }

        private void OnPlayVideo(LessonTile lesson)
        {
            view.audioService.PlayStandardClick();

            if (lesson.vo.isLocked)
            {
                setSubscriptionContext.Dispatch($"lessons_{lesson.vo.section.ToLower().Replace(' ', '_')}");
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SUBSCRIPTION_DLG);
            }
            else
            {
                view.processing.SetActive(true);
                appInfoModel.isVideoLoading = true;
                loadVideoSignal.Dispatch(lesson.vo);
            }
        }

        [ListensTo(typeof(UpdatePurchasedStoreItemSignal))]
        public void OnSubscriptionPurchased(StoreItem item)
        {
            if (view.isActiveAndEnabled &&
               (item.key.Contains("Subscription") || item.key.Equals(GSBackendKeys.ShopItem.ALL_LESSONS_PACK)))
            {
                view.UnlockLessons();
            }
        }
    }
}
