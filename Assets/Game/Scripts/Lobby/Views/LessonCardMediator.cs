/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using UnityEngine;
using UnityEngine.UI;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TurboLabz.TLUtils;
using System;
using TMPro;
using System.Collections.Generic;
using System.Text;
using TurboLabz.InstantGame;
using TurboLabz.CPU;
using System.Collections;

namespace TurboLabz.InstantFramework
{
    [CLSCompliant(false)]
    public class LessonCardMediator : Mediator
    {
        // View injection
        [Inject] public LessonCardView view { get; set; }

        //Dispatch signals
        [Inject] public LoadVideoSignal loadVideoSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public PurchaseStoreItemSignal purchaseStoreItemSignal { get; set; }
        [Inject] public LoadTopicsViewSignal loadTopicsViewSignal { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IAppInfoModel appInfoModel { get; set; }

        //Services
        [Inject] public IPromotionsService promotionsService { get; set; }

        public override void OnRegister()
        {
            view.Init();
            view.startButtonClickedSignal.AddListener(OnStartBtnClicked);
            view.viewAllButtonClickedSignal.AddListener(OnViewAllButtonClickedSignal);
            view.buyLessonClickedSingal.AddListener(OnUnlockVideo);
        }

        private void OnStartBtnClicked(VideoLessonVO vo)
        {
            if (vo.isLocked)
            {
                view.lessonLocked.SetActive(true);
            }
            else
            {
                view.processing.SetActive(true);
                appInfoModel.isVideoLoading = true;
                loadVideoSignal.Dispatch(vo);
            }
        }

        private void OnViewAllButtonClickedSignal()
        {
            loadTopicsViewSignal.Dispatch();
        }

        private void OnUnlockVideo(string videoId)
        {
            if (playerModel.gems >= view.LessonCost)
            {
                purchaseStoreItemSignal.Dispatch(videoId, true);
            }
            else
            {
                SpotPurchaseMediator.analyticsContext = "lesson";
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SPOT_PURCHASE);
            }
        }

        //Listeners
        [ListensTo(typeof(UpdateLessonCardSignal))]
        public void OnUpdateView(VideoLessonVO vo, bool allLessonsUnclocked)
        {
            view.UpdateView(vo, allLessonsUnclocked);
        }

        [ListensTo(typeof(UpdatePurchasedStoreItemSignal))]
        public void OnSubscriptionPurchased(StoreItem item)
        {
            if (item.kind.Equals(GSBackendKeys.ShopItem.SUBSCRIPTION_TAG) || item.key.Equals(GSBackendKeys.ShopItem.ALL_LESSONS_PACK))
            {
                view.UnlockNextLesson();
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

        [ListensTo(typeof(PurchaseStoreItemResultSignal))]
        public void OnItemUnlocked(StoreItem item, PurchaseResult result)
        {
            if (result == PurchaseResult.PURCHASE_SUCCESS && view.isActiveAndEnabled && item.key.Equals(view.NextLessonId))
            {
                view.UnlockNextLesson();
                view.PlayNextLesson();
            }
        }
    }
}