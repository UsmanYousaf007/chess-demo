/// @license Propriety <http://license.url>
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
        [Inject] public SetSubscriptionContext setSubscriptionContext { get; set; }

        //Services
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IPromotionsService promotionsService { get; set; }

        //Models
        [Inject] public IAppInfoModel appInfoModel { get; set; }

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
                analyticsService.ScreenVisit(AnalyticsScreen.lessons_topics);
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

        private void OnNextLesson(VideoLessonVO vo)
        {
            if (vo.isLocked)
            {
                setSubscriptionContext.Dispatch($"lessons_{vo.section.ToLower().Replace(' ', '_')}");
                promotionsService.LoadSubscriptionPromotion();
            }
            else
            {
                view.processing.SetActive(true);
                appInfoModel.isVideoLoading = true;
                loadVideoSignal.Dispatch(vo);
            }
        }

        private void OnTopicClicked(TopicVO vo)
        {
            view.audioService.PlayStandardClick();
            loadLessonsViewSignal.Dispatch(vo);
        }

        [ListensTo(typeof(UpdatePurchasedStoreItemSignal))]
        public void OnSubscriptionPurchased(StoreItem item)
        {
            if (view.isActiveAndEnabled &&
               (item.kind.Equals(GSBackendKeys.ShopItem.SUBSCRIPTION_TAG) || item.key.Equals(GSBackendKeys.ShopItem.ALL_LESSONS_PACK)))
            {
                view.UnlockNextLesson();
            }
        }
    }
}
