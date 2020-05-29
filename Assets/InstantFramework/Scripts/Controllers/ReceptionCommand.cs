/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;
using strange.extensions.command.impl;
using TurboLabz.InstantGame;
using UnityEngine;
using System;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class ReceptionCommand : Command
    {
        // Parameters
        [Inject] public bool isResume { get; set; }

        // Dispatch signals
        [Inject] public LoadLobbySignal loadLobbySignal { get; set; }
        [Inject] public InitBackendOnceSignal initBackendOnceSignal { get; set; }
        [Inject] public GetInitDataSignal getInitDataSignal  { get; set; }
        [Inject] public GetInitDataCompleteSignal getInitDataCompleteSignal { get; set; }
        [Inject] public GetInitDataFailedSignal getInitDataFailedSignal { get; set; }
        [Inject] public PauseNotificationsSignal pauseNotificationsSignal { get; set; }


        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public RefreshFriendsSignal refreshFriendsSignal { get; set; }
        [Inject] public RefreshCommunitySignal refreshCommunitySignal { get; set; }
        [Inject] public LoadPromotionSingal loadPromotionSingal { get; set; }

        // Models
        [Inject] public IAppInfoModel appInfoModel { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IAdsSettingsModel adsSettingsModel { get; set; }

        // Services
        [Inject] public IFacebookService facebookService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public IAutoSubscriptionDailogueService autoSubscriptionDailogueService { get; set; }

        public override void Execute()
        {
            CommandBegin();

            getInitDataSignal.Dispatch(isResume);

        }

        private void OnGetInitDataFailed(BackendResult result)
        {
            if (result != BackendResult.CANCELED)
            {
                TLUtils.LogUtil.Log("ReceptionCommand::OnGetInitDataFailed() GetInitData failed!");
                getInitDataSignal.Dispatch(isResume);
            }
        }

        private void OnGetInitDataComplete()
        {
            // Check version information. Prompt the player if an update is needed.
            if (appInfoModel.appBackendVersionValid == false)
            {
                TurboLabz.TLUtils.LogUtil.Log("ERROR: VERSION MISMATCH", "red");
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_UPDATE);
                CommandEnd();
                return;
            }

            if (settingsModel.maintenanceFlag == true)
            {
                TurboLabz.TLUtils.LogUtil.Log("ERROR: GAME  MAINTENENCE ON", "red");
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MAINTENANCE_SCREEN);
                CommandEnd();
                return;
            }

            if (!isResume)
            {
                preferencesModel.sessionCount++;

                initBackendOnceSignal.Dispatch();

                loadLobbySignal.Dispatch();
                loadPromotionSingal.Dispatch();

                autoSubscriptionDailogueService.Show();

                refreshFriendsSignal.Dispatch();
                refreshCommunitySignal.Dispatch();

                SendAnalytics();
            }


            pauseNotificationsSignal.Dispatch(false);

            CommandEnd();
        }

        private void SendAnalytics()
        {
            if (playerModel.HasSubscription())
            {
                var context = playerModel.subscriptionType.Equals(GSBackendKeys.ShopItem.SUBSCRIPTION_SHOP_TAG) ? AnalyticsContext.monthly : AnalyticsContext.yearly;
                context = playerModel.isPremium ? AnalyticsContext.premium : context;
                analyticsService.Event(AnalyticsEventId.subscription_session, context);
            }

            if (preferencesModel.rankedMatchesFinishedCount >= 15)
            {
                analyticsService.Event(AnalyticsEventId.elo, AnalyticsParameter.elo, playerModel.eloScore);
            }

            // Logging target architecture event
            analyticsService.Event(AnalyticsEventId.target_architecture, UnityInfo.Is64Bit() ? AnalyticsContext.ARM64 : AnalyticsContext.ARM);

            SendDailyAnalytics();
        }

        private void CommandBegin()
        {
            Retain();
            getInitDataCompleteSignal.AddListener(OnGetInitDataComplete);
            getInitDataFailedSignal.AddListener(OnGetInitDataFailed);
        }

        private void CommandEnd()
        {
            getInitDataCompleteSignal.RemoveListener(OnGetInitDataComplete);
            getInitDataFailedSignal.RemoveListener(OnGetInitDataFailed);
            Release();
        }

        private void SendDailyAnalytics()
        {
            var daysBetweenLastLogin = (TimeUtil.ToDateTime(backendService.serverClock.currentTimestamp) - preferencesModel.lastLaunchTime).TotalDays;

            if (daysBetweenLastLogin >= 1)
            {
                analyticsService.Event("cpu_match", AnalyticsParameter.seconds, Mathf.RoundToInt(preferencesModel.timeSpentCpuMatch));
                analyticsService.Event("classic_match", AnalyticsParameter.seconds, Mathf.RoundToInt(preferencesModel.timeSpentLongMatch));
                analyticsService.Event("1m_match", AnalyticsParameter.seconds, Mathf.RoundToInt(preferencesModel.timeSpent1mMatch));
                analyticsService.Event("5m_match", AnalyticsParameter.seconds, Mathf.RoundToInt(preferencesModel.timeSpent5mMatch));
                analyticsService.Event("10m_match", AnalyticsParameter.seconds, Mathf.RoundToInt(preferencesModel.timeSpent10mMatch));
                preferencesModel.ResetDailyPrefers();
            }

        }
    }
}