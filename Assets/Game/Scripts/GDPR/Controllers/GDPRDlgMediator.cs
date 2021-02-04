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
using GameSparks.Core;

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

        public override void OnRegister()
        {
            view.InitOnce();
            view.showRegularAdsBtnClickedSignal.AddListener(OnShowRegularAdsBtnClicked);
            view.acceptAndCollectBtnClickedSignal.AddListener(OnAcceptAndCollectBtnClicked);        
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
            var jsonData = new GSRequestData().AddString("rewardType", GSBackendKeys.ClaimReward.TYPE_PERSONALISED_ADS_GEM)
                                              .AddBoolean("consentFlag", false);
            view.backendService.ClaimReward(jsonData);
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_LOBBY);
            gdprDlgClosedSignal.Dispatch();
            analyticsService.Event(AnalyticsEventId.gdpr_player_interaction, AnalyticsContext.rejected);
        }

        public void OnAcceptAndCollectBtnClicked()
        {
            var jsonData = new GSRequestData().AddString("rewardType", GSBackendKeys.ClaimReward.TYPE_PERSONALISED_ADS_GEM)
                                              .AddBoolean("consentFlag", true);
            view.backendService.ClaimReward(jsonData);

            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_LOBBY);
            gdprDlgClosedSignal.Dispatch();
            analyticsService.Event(AnalyticsEventId.gdpr_player_interaction, AnalyticsContext.accepted);
        }
    }
}
