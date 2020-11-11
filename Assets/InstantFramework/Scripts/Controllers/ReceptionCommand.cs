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
        [Inject] public GetInitDataSignal getInitDataSignal { get; set; }
        [Inject] public GetInitDataCompleteSignal getInitDataCompleteSignal { get; set; }
        [Inject] public GetInitDataFailedSignal getInitDataFailedSignal { get; set; }
        [Inject] public PauseNotificationsSignal pauseNotificationsSignal { get; set; }
        [Inject] public ProfilePictureLoadedSignal profilePictureSignal { get; set; }

        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public RefreshFriendsSignal refreshFriendsSignal { get; set; }
        [Inject] public RefreshCommunitySignal refreshCommunitySignal { get; set; }
        [Inject] public LoadPromotionSingal loadPromotionSingal { get; set; }
        [Inject] public AuthFacebookResultSignal authFacebookResultSignal { get; set; }
        [Inject] public SetLeaguesSignal setLeaguesSignal { get; set; }
        [Inject] public AppUpdateSignal appUpdateSignal { get; set; }

        // Models
        [Inject] public IAppInfoModel appInfoModel { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IAdsSettingsModel adsSettingsModel { get; set; }
        [Inject] public IPicsModel picsModel { get; set; }
        [Inject] public IDownloadablesModel downloadablesModel { get; set; }
        [Inject] public ITournamentsModel tournamentsModel { get; set; }
        [Inject] public ILeaguesModel leaguesModel { get; set; }
        [Inject] public INotificationsModel notificationsModel { get; set; }

        // Services
        [Inject] public IFacebookService facebookService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public IAutoSubscriptionDailogueService autoSubscriptionDialogueService { get; set; }
        [Inject] public IPushNotificationService pushNotificationService { get; set; }
        [Inject] public IGameModesAnalyticsService gameModesAnalyticsService { get; set; }
        [Inject] public IProfilePicService profilePicService { get; set; }
        [Inject] public ISchedulerService schedulerService { get; set; }
        [Inject] public IAppUpdateService appUpdateService { get; set; }
        [Inject] public IPromotionsService promotionsService { get; set; }

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
            if (!appInfoModel.appBackendVersionValid)
            {
                TurboLabz.TLUtils.LogUtil.Log("ERROR: VERSION MISMATCH", "red");
                if (settingsModel.appUpdateFlag)
                {
                    appUpdateService.Init();
                }
                else
                {
                    appUpdateSignal.Dispatch(true);
                }
                CommandEnd();
                return;
            }

            if (settingsModel.maintenanceFlag)
            {
                TurboLabz.TLUtils.LogUtil.Log("ERROR: GAME  MAINTENENCE ON", "red");
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MAINTENANCE_SCREEN);
                CommandEnd();
                return;
            }

            if (!isResume && IsSkinDownloadRequired(playerModel.activeSkinId))
            {
                downloadablesModel.Get(playerModel.activeSkinId, OnSkinDownloadComplete);
            }

            else if (!isResume)
            {
                InitGame();
            }

            else
            {
                ResumeGame();
            }
        }

        private void InitGame()
        {
            DispatchGameSignals();
            schedulerService.Init();
            schedulerService.Subscribe(tournamentsModel.UpdateSchedule);
            schedulerService.Start();
            ResumeGame();
        }

        private void ResumeGame()
        {
            pauseNotificationsSignal.Dispatch(false);
            bool picWait = false;
            UpdateProfilePic(ref picWait);
            ConcludeCommand(picWait);
        }

        private void OnSkinDownloadComplete(BackendResult result, AssetBundle bundle)
        {
            InitGame();
        }

        private bool IsSkinDownloadRequired(string skinId)
        {
            return (downloadablesModel.downloadableItems.ContainsKey(skinId)
            && downloadablesModel.GetBundleFromVersionCache(skinId) == null);
        }

        private void DispatchGameSignals()
        {
            preferencesModel.sessionCount++;
            initBackendOnceSignal.Dispatch();
            setLeaguesSignal.Dispatch();
            loadLobbySignal.Dispatch();
            //loadPromotionSingal.Dispatch();
            //autoSubscriptionDialogueService.Show();
            pushNotificationService.Init();
            notificationsModel.Init();
            refreshFriendsSignal.Dispatch();
            refreshCommunitySignal.Dispatch(true);
            SendAnalytics();
            promotionsService.LoadPromotion();
        }

        private void UpdateProfilePic(ref bool picWait)
        {
            // Finally update profile pic. Must be last operation
            if (string.IsNullOrEmpty(playerModel.uploadedPicId) && facebookService.isLoggedIn())
            {
                picWait = true;
                facebookService.GetSocialPic(facebookService.GetFacebookId(), playerModel.id).Then(OnGetSocialPic);
            }
            else if (!string.IsNullOrEmpty(playerModel.uploadedPicId))
            {
                picWait = true;
                profilePicService.GetProfilePic(playerModel.id, playerModel.uploadedPicId).Then(OnGetProfilePic);
            }
        }

        private void ConcludeCommand(bool picWait)
        {
            // If there is no waiting for profile pic download then end this command
            // Otherwise remove init data comlete and fail handlers but retain the command for pic download response
            if (!picWait)
            {
                CommandEnd();
            }
            else
            {
                getInitDataCompleteSignal.RemoveListener(OnGetInitDataComplete);
                getInitDataFailedSignal.RemoveListener(OnGetInitDataFailed);
            }
        }

        private void OnGetSocialPic(FacebookResult result, Sprite sprite, string facebookUserId)
        {
            if (result == FacebookResult.SUCCESS)
            {
                picsModel.SetPlayerPic(playerModel.id, sprite);
            }

            AuthFacebookResultVO vo = new AuthFacebookResultVO();
            vo.isSuccessful = true;
            vo.pic = sprite;
            vo.name = playerModel.name;
            vo.playerId = playerModel.id;
            vo.rating = playerModel.eloScore;

            authFacebookResultSignal.Dispatch(vo);

            CommandEnd();
        }

        private void OnGetProfilePic(BackendResult result, Sprite sprite, string playerId)
        {
            if (result == BackendResult.SUCCESS)
            {
                picsModel.SetPlayerPic(playerModel.id, sprite);
                profilePictureSignal.Dispatch(playerModel.id, sprite);
            }

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

            if (leaguesModel.leagues.ContainsKey(playerModel.league.ToString()))
            {
                analyticsService.Event(AnalyticsEventId.current_league, AnalyticsParameter.context, leaguesModel.leagues[playerModel.league.ToString()].name);
            }

            // Logging target architecture event
            analyticsService.Event(AnalyticsEventId.target_architecture, UnityInfo.Is64Bit() ? AnalyticsContext.ARM64 : AnalyticsContext.ARM);

            tournamentsModel.LogConcludedJoinedTournaments();
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
            var daysBetweenLastLogin = (TimeUtil.ToDateTime(backendService.serverClock.currentTimestamp) - preferencesModel.lastLaunchTime).Days;

            if (daysBetweenLastLogin >= 1)
            {
                gameModesAnalyticsService.LogTimeSpent();
                gameModesAnalyticsService.LogInstallDayData();

                analyticsService.Event(AnalyticsEventId.trophies_earned, AnalyticsParameter.count, playerModel.trophies);

                analyticsService.Event(AnalyticsEventId.items_owned, AnalyticsParameter.gems, playerModel.gems);
                analyticsService.Event(AnalyticsEventId.items_owned, AnalyticsParameter.tickets, playerModel.GetInventoryItemCount(GSBackendKeys.ShopItem.SPECIAL_ITEM_TICKET));
                analyticsService.Event(AnalyticsEventId.items_owned, AnalyticsParameter.rating_boosters, playerModel.GetInventoryItemCount(GSBackendKeys.ShopItem.SPECIAL_ITEM_RATING_BOOSTER));
                analyticsService.Event(AnalyticsEventId.items_owned, AnalyticsParameter.hints, playerModel.GetInventoryItemCount(GSBackendKeys.ShopItem.SPECIAL_ITEM_HINT));
                analyticsService.Event(AnalyticsEventId.items_owned, AnalyticsParameter.keys, playerModel.GetInventoryItemCount(GSBackendKeys.ShopItem.SPECIAL_ITEM_KEY));

                SendResourceManagerAnalytics();
                preferencesModel.ResetDailyPrefers();
            }
        }

        private void SendResourceManagerAnalytics()
        {
            if (playerModel.HasSubscription())
            {
                return;
            }

            if (preferencesModel.dailyResourceManager[PrefKeys.RESOURCE_USED][GSBackendKeys.ShopItem.SPECIAL_ITEM_TICKET] > 0)
            {
                analyticsService.Event(AnalyticsEventId.resource_via_free, AnalyticsParameter.tickets, preferencesModel.dailyResourceManager[PrefKeys.RESOURCE_FREE][GSBackendKeys.ShopItem.SPECIAL_ITEM_TICKET]);
                analyticsService.Event(AnalyticsEventId.resource_via_gems, AnalyticsParameter.tickets, preferencesModel.dailyResourceManager[PrefKeys.RESOURCE_GEMS][GSBackendKeys.ShopItem.SPECIAL_ITEM_TICKET]);
                analyticsService.Event(AnalyticsEventId.resource_via_videos, AnalyticsParameter.tickets, preferencesModel.dailyResourceManager[PrefKeys.RESOURCE_VIDEOS][GSBackendKeys.ShopItem.SPECIAL_ITEM_TICKET]);
                analyticsService.Event(AnalyticsEventId.resource_via_bundle, AnalyticsParameter.tickets, preferencesModel.dailyResourceManager[PrefKeys.RESOURCE_BUNDLE][GSBackendKeys.ShopItem.SPECIAL_ITEM_TICKET]);
            }

            if (preferencesModel.dailyResourceManager[PrefKeys.RESOURCE_USED][GSBackendKeys.ShopItem.SPECIAL_ITEM_RATING_BOOSTER] > 0)
            {
                analyticsService.Event(AnalyticsEventId.resource_via_free, AnalyticsParameter.rating_boosters, preferencesModel.dailyResourceManager[PrefKeys.RESOURCE_FREE][GSBackendKeys.ShopItem.SPECIAL_ITEM_RATING_BOOSTER]);
                analyticsService.Event(AnalyticsEventId.resource_via_gems, AnalyticsParameter.rating_boosters, preferencesModel.dailyResourceManager[PrefKeys.RESOURCE_GEMS][GSBackendKeys.ShopItem.SPECIAL_ITEM_RATING_BOOSTER]);
                analyticsService.Event(AnalyticsEventId.resource_via_videos, AnalyticsParameter.rating_boosters, preferencesModel.dailyResourceManager[PrefKeys.RESOURCE_VIDEOS][GSBackendKeys.ShopItem.SPECIAL_ITEM_RATING_BOOSTER]);
                analyticsService.Event(AnalyticsEventId.resource_via_bundle, AnalyticsParameter.rating_boosters, preferencesModel.dailyResourceManager[PrefKeys.RESOURCE_BUNDLE][GSBackendKeys.ShopItem.SPECIAL_ITEM_RATING_BOOSTER]);
            }

            if (preferencesModel.dailyResourceManager[PrefKeys.RESOURCE_USED][GSBackendKeys.ShopItem.SPECIAL_ITEM_HINT] > 0)
            {
                analyticsService.Event(AnalyticsEventId.resource_via_free, AnalyticsParameter.hints, preferencesModel.dailyResourceManager[PrefKeys.RESOURCE_FREE][GSBackendKeys.ShopItem.SPECIAL_ITEM_HINT]);
                analyticsService.Event(AnalyticsEventId.resource_via_gems, AnalyticsParameter.hints, preferencesModel.dailyResourceManager[PrefKeys.RESOURCE_GEMS][GSBackendKeys.ShopItem.SPECIAL_ITEM_HINT]);
                analyticsService.Event(AnalyticsEventId.resource_via_videos, AnalyticsParameter.hints, preferencesModel.dailyResourceManager[PrefKeys.RESOURCE_VIDEOS][GSBackendKeys.ShopItem.SPECIAL_ITEM_HINT]);
                analyticsService.Event(AnalyticsEventId.resource_via_bundle, AnalyticsParameter.hints, preferencesModel.dailyResourceManager[PrefKeys.RESOURCE_BUNDLE][GSBackendKeys.ShopItem.SPECIAL_ITEM_HINT]);
            }

            if (preferencesModel.dailyResourceManager[PrefKeys.RESOURCE_USED][GSBackendKeys.ShopItem.SPECIAL_ITEM_KEY] > 0)
            {
                analyticsService.Event(AnalyticsEventId.resource_via_free, AnalyticsParameter.keys, preferencesModel.dailyResourceManager[PrefKeys.RESOURCE_FREE][GSBackendKeys.ShopItem.SPECIAL_ITEM_KEY]);
                analyticsService.Event(AnalyticsEventId.resource_via_gems, AnalyticsParameter.keys, preferencesModel.dailyResourceManager[PrefKeys.RESOURCE_GEMS][GSBackendKeys.ShopItem.SPECIAL_ITEM_KEY]);
                analyticsService.Event(AnalyticsEventId.resource_via_videos, AnalyticsParameter.keys, preferencesModel.dailyResourceManager[PrefKeys.RESOURCE_VIDEOS][GSBackendKeys.ShopItem.SPECIAL_ITEM_KEY]);
                analyticsService.Event(AnalyticsEventId.resource_via_bundle, AnalyticsParameter.keys, preferencesModel.dailyResourceManager[PrefKeys.RESOURCE_BUNDLE][GSBackendKeys.ShopItem.SPECIAL_ITEM_KEY]);
            }

            analyticsService.Event(AnalyticsEventId.resource_used, AnalyticsParameter.tickets, preferencesModel.dailyResourceManager[PrefKeys.RESOURCE_USED][GSBackendKeys.ShopItem.SPECIAL_ITEM_TICKET]);
            analyticsService.Event(AnalyticsEventId.resource_used, AnalyticsParameter.rating_boosters, preferencesModel.dailyResourceManager[PrefKeys.RESOURCE_USED][GSBackendKeys.ShopItem.SPECIAL_ITEM_RATING_BOOSTER]);
            analyticsService.Event(AnalyticsEventId.resource_used, AnalyticsParameter.hints, preferencesModel.dailyResourceManager[PrefKeys.RESOURCE_USED][GSBackendKeys.ShopItem.SPECIAL_ITEM_HINT]);
            analyticsService.Event(AnalyticsEventId.resource_used, AnalyticsParameter.keys, preferencesModel.dailyResourceManager[PrefKeys.RESOURCE_USED][GSBackendKeys.ShopItem.SPECIAL_ITEM_KEY]);

        }
    }
}