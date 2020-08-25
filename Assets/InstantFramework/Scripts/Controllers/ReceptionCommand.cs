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
        [Inject] public AuthFacebookResultSignal authFacebookResultSignal { get; set; }

        // Models
        [Inject] public IAppInfoModel appInfoModel { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IAdsSettingsModel adsSettingsModel { get; set; }
        [Inject] public IPicsModel picsModel { get; set; }

        // Services
        [Inject] public IFacebookService facebookService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public IAutoSubscriptionDailogueService autoSubscriptionDailogueService { get; set; }
        [Inject] public IPushNotificationService pushNotificationService { get; set; }
        [Inject] public IGameModesAnalyticsService gameModesAnalyticsService { get; set; }
        [Inject] public IProfilePicService profilePicService { get; set; }
        [Inject] public UpdateTournamentsViewSignal updateTournamentsViewSignal { get; set; }

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
                refreshCommunitySignal.Dispatch(true);
                updateTournamentsViewSignal.Dispatch();
                SendAnalytics();
            }

            pauseNotificationsSignal.Dispatch(false);


            bool picWait = false;

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
                gameModesAnalyticsService.LogTimeSpent();
                gameModesAnalyticsService.LogInstallDayData();
                preferencesModel.ResetDailyPrefers();
            }

        }
    }
}