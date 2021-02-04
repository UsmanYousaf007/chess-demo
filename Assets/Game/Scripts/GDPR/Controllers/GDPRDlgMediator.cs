/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using strange.extensions.mediation.impl;
using System;

using GameSparks.Core;
using HUFEXT.GenericGDPR.Runtime.API;
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

            view.GemsAddedAnimation();

            analyticsService.Event(AnalyticsEventId.gdpr_player_interaction, AnalyticsContext.accepted);
        }

        private void SetPersonalisedAds(bool value)
        {
            var jsonData = new GSRequestData().AddString("rewardType", GSBackendKeys.ClaimReward.TYPE_PERSONALISED_ADS_GEM)
                                              .AddBoolean("consentFlag", value);
            view.backendService.ClaimReward(jsonData);

            HGenericGDPR.IsPersonalizedAdsAccepted = value;
            adsService.CollectSensitiveData(HGenericGDPR.IsPersonalizedAdsAccepted);
        }

        public void OnGDPRDlgClosed()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_LOBBY);
            gdprDlgClosedSignal.Dispatch();
        }
    }
}
