/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using strange.extensions.mediation.impl;
using System;
using GameSparks.Core;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    [CLSCompliant(false)]
    public class GDPRDlgMediator : Mediator
    {
        // View injection
        [Inject] public GDPRDlgView view { get; set; }

        //Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public GDPRDlgClosedSignal gdprDlgClosedSignal { get; set; }
        [Inject] public NotificationRecievedSignal notificationRecievedSignal { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }

        //Services
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IAdsService adsService { get; set; }

        public override void OnRegister()
        {
            view.InitOnce();
            view.showRegularAdsBtnClickedSignal.AddListener(OnShowRegularAdsBtnClicked);
            view.acceptAndCollectBtnClickedSignal.AddListener(OnAcceptAndCollectBtnClicked);
            view.onGDPRDlgClosedSignal.AddListener(OnGDPRDlgClosed);
        }


        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.GDPR_DLG)
            {
                view.Show();
                analyticsService.Event(AnalyticsEventId.gdpr);
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.GDPR_DLG)
            {
                view.Hide();
            }
        }

        [ListensTo(typeof(GetInitDataOnCompleteSignal))]
        public void OnServerDataAvailable()
        {
            view.OnServerDataAvailable();
        }

        public void OnShowRegularAdsBtnClicked()
        {
            SetPersonalisedAds(false);

            OnGDPRDlgClosed();

            analyticsService.Event(AnalyticsEventId.gdpr_player_interaction, AnalyticsContext.rejected);
        }

        public void OnAcceptAndCollectBtnClicked()
        {
            SetPersonalisedAds(true);

            SendInGameNotification();

            view.OnAcceptAndCollectBtnClickedPostProcessing();

            OnGDPRDlgClosed();

            analyticsService.Event(AnalyticsEventId.gdpr_player_interaction, AnalyticsContext.accepted);
        }

        private void SetPersonalisedAds(bool value)
        {
            var jsonData = new GSRequestData().AddString("rewardType", GSBackendKeys.ClaimReward.TYPE_PERSONALISED_ADS_GEM)
                                              .AddBoolean("consentFlag", value);
            view.backendService.ClaimReward(jsonData);

            adsService.CollectSensitiveData(value);
        }

        public void OnGDPRDlgClosed()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_LOBBY);
            gdprDlgClosedSignal.Dispatch();
        }

        private void SendInGameNotification()
        {
            NotificationVO notificationVO;

            notificationVO.isOpened = false;
            notificationVO.title = "Reward!";
            notificationVO.body = "+" + view.rewardsSettingsModel.personalisedAdsGemReward + " gems have been added to your inventory.";
            notificationVO.senderPlayerId = "undefined";
            notificationVO.challengeId = "undefined";
            notificationVO.matchGroup = "undefined";
            notificationVO.avatarId = "GemRwdThumb";
            notificationVO.avaterBgColorId = "undefined";
            notificationVO.profilePicURL = "undefined";
            notificationVO.isPremium = false;
            notificationVO.timeSent = 0;
            notificationVO.actionCode = "undefined";
            notificationVO.league = -1;

            notificationRecievedSignal.Dispatch(notificationVO);
            view.audioService.Play(view.audioService.sounds.SFX_REWARD_UNLOCKED);
        }
    }
}
