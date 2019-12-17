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
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public RefreshFriendsSignal refreshFriendsSignal { get; set; }
        [Inject] public RefreshCommunitySignal refreshCommunitySignal { get; set; }
        [Inject] public LoadPromotionSingal loadPromotionSingal { get; set; }

        // Models
        [Inject] public IMetaDataModel metaDataModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }

        // Services
        [Inject] public IFacebookService facebookService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IBackendService backendService { get; set; }

        public override void Execute()
        {
            CommandBegin();

            getInitDataSignal.Dispatch(isResume);

        }
            
        private void OnGetInitDataComplete()
        {
            // Check version information. Prompt the player if an update is needed.
            if (metaDataModel.appInfo.appBackendVersionValid == false)
            {
                TurboLabz.TLUtils.LogUtil.Log("ERROR: VERSION MISMATCH", "red");
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_UPDATE);
                CommandEnd();
                return;
            }

            if (metaDataModel.settingsModel.maintenanceFlag == true)
            {
                TurboLabz.TLUtils.LogUtil.Log("ERROR: GAME  MAINTENENCE ON", "red");
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MAINTENANCE_SCREEN);
                CommandEnd();
                return;
            }

            initBackendOnceSignal.Dispatch();

            if (!isResume)
            {
                loadLobbySignal.Dispatch();
                loadPromotionSingal.Dispatch();
            }

            refreshFriendsSignal.Dispatch();
            refreshCommunitySignal.Dispatch();

            SendAnalytics();

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
                analyticsService.Event(AnalyticsEventId.friends_facebook, AnalyticsParameter.count, facebookFriendCount);
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

            SendTimeSpentAnalytics();
        }

        private void CommandBegin()
        {
            Retain();
            getInitDataCompleteSignal.AddListener(OnGetInitDataComplete);
        }

        private void CommandEnd()
        {
            getInitDataCompleteSignal.RemoveListener(OnGetInitDataComplete);
            Release();
        }

        private void SendTimeSpentAnalytics()
        {
            var daysBetweenLastLogin = (TimeUtil.ToDateTime(backendService.serverClock.currentTimestamp) - preferencesModel.lastLaunchTime).TotalDays;

            if (daysBetweenLastLogin >= 1)
            {
                analyticsService.Event(AnalyticsEventId.time_spent_cpu_match, AnalyticsParameter.minutes, Math.Round(preferencesModel.timeSpentCpuMatch, 2));
                analyticsService.Event(AnalyticsEventId.time_spent_long_match, AnalyticsParameter.minutes, Math.Round(preferencesModel.timeSpentLongMatch, 2));
                analyticsService.Event(AnalyticsEventId.time_spent_quick_macth, AnalyticsParameter.minutes, Math.Round(preferencesModel.timeSpentQuickMatch, 2));
                analyticsService.Event(AnalyticsEventId.time_spent_lobby, AnalyticsParameter.minutes, Math.Round(preferencesModel.timeSpentLobby, 2));

                preferencesModel.ResetTimeSpentAnalyticsData();
            }

        }
    }
}