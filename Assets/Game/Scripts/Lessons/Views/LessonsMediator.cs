/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using GameAnalyticsSDK;
using strange.extensions.mediation.impl;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;

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
        [Inject] public PurchaseStoreItemSignal purchaseStoreItemSignal { get; set; }
        [Inject] public LoadSpotInventorySignal loadSpotInventorySignal { get; set; }

        //Analytics Service
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IPromotionsService promotionsService { get; set; }

        //Models
        [Inject] public IAppInfoModel appInfoModel { get; set; }
        [Inject] public ILessonsModel lessonsModel { get; set; }

        public override void OnRegister()
        {
            view.Init();
            view.backSignal.AddListener(OnBackPressed);
            view.playVideoSingal.AddListener(OnPlayVideo);
            view.unlockVideoSingal.AddListener(OnUnlockVideo);
            view.unlockAllLessonsSignal.AddListener(OnUnlockAllLessons);
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

            if (vo.showBanner)
            {
                analyticsService.Event(AnalyticsEventId.banner_shown, AnalyticsContext.unlock_all_lessons);
            }
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

            if (!lesson.vo.isLocked)
            {
                view.processing.SetActive(true);
                appInfoModel.isVideoLoading = true;
                loadVideoSignal.Dispatch(lesson.vo);
            }
            //else
            //{
            //    setSubscriptionContext.Dispatch($"lessons_{lesson.vo.section.ToLower().Replace(' ', '_')}");
            //    promotionsService.LoadSubscriptionPromotion();
            //}
        }

        private void OnUnlockVideo(LessonTile lesson)
        {
            view.audioService.PlayStandardClick();

            if (lesson.haveEnoughGemsToUnlock)
            {
                purchaseStoreItemSignal.Dispatch(lesson.vo.videoId, true);
            }
            else
            {
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SPOT_PURCHASE);
            }
        }

        [ListensTo(typeof(UpdatePurchasedStoreItemSignal))]
        public void OnSubscriptionPurchased(StoreItem item)
        {
            if (view.isActiveAndEnabled &&
               (item.kind.Equals(GSBackendKeys.ShopItem.SUBSCRIPTION_TAG) || item.key.Equals(GSBackendKeys.ShopItem.ALL_LESSONS_PACK)))
            {
                view.lessonsBanner.gameObject.SetActive(false);
                view.UnlockLessons();

                if (item.key.Equals(GSBackendKeys.ShopItem.ALL_LESSONS_PACK))
                {
                    analyticsService.Event(AnalyticsEventId.banner_purchased, AnalyticsContext.unlock_all_lessons);
                }
            }
        }

        [ListensTo(typeof(UpdatePlayerInventorySignal))]
        public void OnInventoryUpdated(PlayerInventoryVO inventory)
        {
            if (view.isActiveAndEnabled)
            {
                view.UpdateLessons();
                view.lessonsBanner.gameObject.SetActive(!inventory.allLessonsUnlocked);
            }
        }

        [ListensTo(typeof(PurchaseStoreItemResultSignal))]
        public void OnItemUnlocked(StoreItem item, PurchaseResult result)
        {
            if (result == PurchaseResult.PURCHASE_SUCCESS && view.isActiveAndEnabled)
            {
                view.UnlockLesson(item.key);
            }
        }

        private void OnUnlockAllLessons()
        {
            analyticsService.Event(AnalyticsEventId.banner_clicked, AnalyticsContext.unlock_all_lessons);
            purchaseStoreItemSignal.Dispatch(GSBackendKeys.ShopItem.ALL_LESSONS_PACK, true);
        }

        [ListensTo(typeof(ShowProcessingSignal))]
        public void OnShowProcessing(bool blocker, bool processing)
        {
            view.processing.SetActive(blocker);
        }
    }
}
