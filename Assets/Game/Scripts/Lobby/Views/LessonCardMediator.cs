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
        [Inject] public ShowAdSignal showAdSignal { get; set; }
        [Inject] public LoadTopicsViewSignal loadTopicsViewSignal { get; set; }
        [Inject] public SetSubscriptionContext setSubscriptionContext { get; set; }
        [Inject] public LoadVideoSignal loadVideoSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IAppInfoModel appInfoModel { get; set; }

        //Services
        [Inject] public IPromotionsService promotionsService { get; set; }

        public override void OnRegister()
        {
            view.Init();
            view.OnStartButtonClickedSignal.AddListener(OnStartBtnClicked);
        }

        private void OnStartBtnClicked(VideoLessonVO vo)
        {
            //loadTopicsViewSignal.Dispatch();
            if (vo.isLocked)
            {
                setSubscriptionContext.Dispatch($"lessons_{vo.section.ToLower().Replace(' ', '_')}");
                promotionsService.LoadSubscriptionPromotion();
            }
            else
            {
                view.processing.SetActive(true);
                appInfoModel.isVideoLoading = true;
                //navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_TOPICS_VIEW);
                loadVideoSignal.Dispatch(vo);
            }
        }

        //Listeners
        [ListensTo(typeof(UpdateTopiscViewSignal))]
        public void OnUpdateView(TopicsViewVO vo)
        {
            view.UpdateView(vo);
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
    }
}