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
        [Inject] public IPushNotificationService pushNotificationService { get; set; }

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
                pushNotificationService.Init();
                refreshFriendsSignal.Dispatch();
                refreshCommunitySignal.Dispatch();
                SendAnalytics();
            }

            pauseNotificationsSignal.Dispatch(false);

            CommandEnd();
        }

        private void SendAnalytics()
        {
            if (facebookService.isLoggedIn())
            {
                int facebookFriendCount = 0;
                int communityFriendCount = 0;
                foreach (KeyValuePair<string, Friend> kvp in playerModel.friends)
                {
                    Friend friend = kvp.Value;
                    if (friend.friendType == GSBackendKeys.Friend.TYPE_SOCIAL)
                    {
                        facebookFriendCount++;
                    }
                    else if (friend.friendType == GSBackendKeys.Friend.TYPE_COMMUNITY)
                    {
                        communityFriendCount++;
                    }
                }

                analyticsService.Event(AnalyticsEventId.session_fb);
                analyticsService.Event(AnalyticsEventId.friends_community, AnalyticsParameter.count, communityFriendCount);
                analyticsService.Event(AnalyticsEventId.session_facebook, AnalyticsParameter.num_facebook_friends, facebookFriendCount);
                analyticsService.Event(AnalyticsEventId.friends_active_games, AnalyticsParameter.count, matchInfoModel.matches.Count);
                analyticsService.Event(AnalyticsEventId.friends_blocked, AnalyticsParameter.count, playerModel.blocked.Count);
            }
            else
            {
                analyticsService.Event(AnalyticsEventId.session_guest);
            }

            analyticsService.Event(AnalyticsEventId.player_elo, AnalyticsParameter.elo, playerModel.eloScore);
            analyticsService.Event(AnalyticsEventId.selected_theme, AnalyticsParameter.theme_name, playerModel.activeSkinId);

            if (playerModel.isPremium)
            {
                analyticsService.Event(AnalyticsEventId.session_premium);
            }

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
                analyticsService.Event(AnalyticsEventId.time_spent_cpu_match, AnalyticsParameter.minutes, Mathf.RoundToInt(preferencesModel.timeSpentCpuMatch));
                analyticsService.Event(AnalyticsEventId.time_spent_long_match, AnalyticsParameter.minutes, Mathf.RoundToInt(preferencesModel.timeSpentLongMatch));
                analyticsService.Event(AnalyticsEventId.time_spent_quick_macth, AnalyticsParameter.minutes, Mathf.RoundToInt(preferencesModel.timeSpentQuickMatch));
                analyticsService.Event(AnalyticsEventId.time_spent_lobby, AnalyticsParameter.minutes, Mathf.RoundToInt(preferencesModel.timeSpentLobby));

                float totalGames = preferencesModel.cpuMatchFinishedCount
                    + preferencesModel.longMatchFinishedCount
                    + preferencesModel.quickMatchFinishedCount;

                float THRESHOLD = 0.75f;

                if (totalGames > 0)
                {
                    var noOfDays = (preferencesModel.lastLaunchTime - TimeUtil.ToDateTime(playerModel.creationDate)).Days;
                    var playerType = AnalyticsEventId.multi_mode_player;

                    if (preferencesModel.cpuMatchFinishedCount / totalGames >= THRESHOLD)
                    {
                        playerType = AnalyticsEventId.cpu_mactch_player;
                    }
                    else if (preferencesModel.longMatchFinishedCount / totalGames >= THRESHOLD)
                    {
                        playerType = AnalyticsEventId.long_match_player;
                    }
                    else if (preferencesModel.quickMatchFinishedCount / totalGames >= THRESHOLD)
                    {
                        playerType = AnalyticsEventId.quick_match_player;
                    }

                    analyticsService.DesignEvent(AnalyticsEventId.mode_distribution, AnalyticsParameter.day, noOfDays, playerType);
                }

                preferencesModel.ResetDailyPrefers();
            }

        }
    }
}