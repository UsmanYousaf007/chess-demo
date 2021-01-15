/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @description
/// [add_description_here]
using strange.extensions.command.impl;
using TurboLabz.TLUtils;
using TurboLabz.InstantFramework;
using TurboLabz.CPU;
using TurboLabz.Multiplayer;
using System.Collections.Generic;
using UnityEngine;

namespace TurboLabz.InstantGame
{
    public class LoadLobbyCommand : Command
    {
        // Dispatch Signals
        [Inject] public SetSkinSignal setSkinSignal { get; set; }
        [Inject] public LoadGameSignal loadCPUGameDataSignal { get; set; }
        [Inject] public ResetActiveMatchSignal resetActiveMatchSignal { get; set; }
        [Inject] public LoadPromotionSingal loadPromotionSingal { get; set; }
        [Inject] public ToggleBannerSignal toggleBannerSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateMenuViewSignal updateMenuViewSignal { get; set; }
        [Inject] public FriendsShowConnectFacebookSignal friendsShowConnectFacebookSignal { get; set; }
        [Inject] public UpdateFriendBarSignal updateFriendBarSignal { get; set; }
        [Inject] public SetActionCountSignal setActionCountSignal { get; set; }
        [Inject] public UpdateProfileSignal updateProfileSignal { get; set; }
        [Inject] public UpdateRemoveAdsSignal updateRemoveAdsDisplaySignal { get; set; }
        [Inject] public SubscriptionDlgClosedSignal subscriptionDlgClosedSignal { get; set; }
        [Inject] public UpdateInboxMessageCountViewSignal updateInboxMessageCountViewSignal { get; set; }
        [Inject] public UpdateLeagueProfileSignal updateLeagueProfileSignal { get; set; }
        [Inject] public LoadRewardsSignal loadRewardsSignal { get; set; }
        [Inject] public UpdateLessonCardSignal updateLessonCardSignal { get; set; }
        [Inject] public LoadCareerCardSignal loadCareerCardSignal { get; set; }
        [Inject] public RateAppDlgClosedSignal rateAppDlgClosedSignal { get; set; }
        [Inject] public UpdateSpotCoinsWatchAdDlgSignal updateSpotCoinsWatchAdDlgSignal { get; set; }

        // Services
        [Inject] public IFacebookService facebookService { get; set; }
        [Inject] public ISignInWithAppleService signInWithAppleService { get; set; }
        [Inject] public IRateAppService rateAppService { get; set; }
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IPushNotificationService firebasePushNotificationService { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IPicsModel picsModel { get; set; }
        [Inject] public IMetaDataModel metaDataModel { get; set; }
        [Inject] public ICPUGameModel cpuGameModel { get; set; }
        [Inject] public ICPUStatsModel cpuStatsModel { get; set; }
        [Inject] public IInboxModel inboxModel { get; set; }
        [Inject] public ITournamentsModel tournamentsModel { get; set; }
        [Inject] public ILessonsModel lessonsModel { get; set; }

        public override void Execute()
        {
            toggleBannerSignal.Dispatch(false);
            setSkinSignal.Dispatch(playerModel.activeSkinId);
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_LOBBY);
            resetActiveMatchSignal.Dispatch();
            loadCPUGameDataSignal.Dispatch();
            updateInboxMessageCountViewSignal.Dispatch(inboxModel.inboxMessageCount);


            var nextLesson = lessonsModel.GetNextLesson(playerModel.lastWatchedVideo);
            var nextLessonVO = new VideoLessonVO();
            var allLessonsWatched = false;

            if (string.IsNullOrEmpty(nextLesson))
            {
                allLessonsWatched = true;
            }
            else if (metaDataModel.store.items.ContainsKey(nextLesson))
            {
                nextLessonVO.storeItem = metaDataModel.store.items[nextLesson];
                nextLessonVO.name = nextLessonVO.storeItem.displayName;
                nextLessonVO.videoId = nextLesson;
                nextLessonVO.icon = StoreIconsContainer.Load().GetSprite(lessonsModel.GetTopicId(nextLesson));
                nextLessonVO.isLocked = !(playerModel.HasSubscription() || playerModel.OwnsVGood(nextLesson) || playerModel.OwnsVGood(GSBackendKeys.ShopItem.ALL_LESSONS_PACK));
                nextLessonVO.overallIndex = lessonsModel.lessonsMapping.IndexOf(nextLesson);
                nextLessonVO.duration = lessonsModel.GetLessonDuration(nextLesson);
            }

            updateLessonCardSignal.Dispatch(nextLessonVO, allLessonsWatched);


            if (facebookService.isLoggedIn() || signInWithAppleService.IsSignedIn())
            {
                friendsShowConnectFacebookSignal.Dispatch(false);
            }
            else
            {
                friendsShowConnectFacebookSignal.Dispatch(true);
            }

            // Update the timers on the bars
            foreach (string key in playerModel.friends.Keys)
            {
                updateFriendBarSignal.Dispatch(playerModel.friends[key], key);
            }

            LobbyVO vo = new LobbyVO(cpuGameModel, playerModel, metaDataModel);

            updateMenuViewSignal.Dispatch(vo);
            loadRewardsSignal.Dispatch();

            DispatchProfileSignal();
            DispatchRemoveAdsSignal();

            loadCareerCardSignal.Dispatch();
            
            if (preferencesModel.isLobbyLoadedFirstTime)
            {
                loadPromotionSingal.Dispatch();
            }

            if (SplashLoader.launchCode == 1)
            {
                var appsFlyerId = new KeyValuePair<string, object>("appsflyer_id", hAnalyticsService.GetAppsFlyerId());

                if (firebasePushNotificationService.IsNotificationOpened())
                {
                    hAnalyticsService.LogEvent("launch", "launch", "notification", appsFlyerId);
                }
                else
                {
                    hAnalyticsService.LogEvent("launch", "launch", appsFlyerId);
                }
                SplashLoader.launchCode = 3;
            }

            if (SplashLoader.FTUE)
            {
                subscriptionDlgClosedSignal.AddOnce(() => {
                    analyticsService.DesignEvent(AnalyticsEventId.ftue_lobby);
                    SplashLoader.FTUE = false;
                });
            }
        }

        private void DispatchProfileSignal() 
        {
            ProfileVO pvo = new ProfileVO();
            pvo.playerPic = picsModel.GetPlayerPic(playerModel.id);
            pvo.playerName = playerModel.name;
            pvo.eloScore = playerModel.eloScore;
            pvo.countryId = playerModel.countryId;
            pvo.isFacebookLoggedIn = facebookService.isLoggedIn();
            pvo.isAppleSignedIn = signInWithAppleService.IsSignedIn();
            pvo.isAppleSignInSupported = signInWithAppleService.IsSupported();
            pvo.playerId = playerModel.id;
            pvo.avatarId = playerModel.avatarId;
            pvo.avatarColorId = playerModel.avatarBgColorId;
            pvo.isPremium = playerModel.HasSubscription();
            pvo.trophies2 = playerModel.trophies2;
            var leagueAssets = tournamentsModel.GetLeagueSprites(playerModel.league.ToString());
            pvo.leagueBorder = leagueAssets != null ? leagueAssets.ringSprite : null;

            if (pvo.playerPic == null)
            {
                pvo.playerPic = picsModel.GetPlayerPic(playerModel.id);
            }

            updateProfileSignal.Dispatch(pvo);
            updateLeagueProfileSignal.Dispatch(playerModel.league.ToString());
        }

        private void DispatchRemoveAdsSignal() 
        {
            string localizedMins = localizationService.Get(LocalizationKey.FREE_NO_ADS_MINUTES);
            string localizedHours = localizationService.Get(LocalizationKey.FREE_NO_ADS_HOURS);
            string localizedDays = localizationService.Get(LocalizationKey.FREE_NO_ADS_DAYS);
            string timeRemain = TimeUtil.TimeToExpireString(playerModel.creationDate, metaDataModel.adsSettings.freeNoAdsPeriod,
                localizedMins, localizedHours, localizedDays);

            updateRemoveAdsDisplaySignal.Dispatch(timeRemain, playerModel.HasAdsFreePeriod(metaDataModel.adsSettings));
            
        }

        /*
        // Dispatch Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateMenuViewSignal updateMenuViewSignal { get; set; }
        [Inject] public UpdateAdsSignal updateAdsSignal { get; set; }
        [Inject] public SetSkinSignal setSkinSignal { get; set; }
        [Inject] public LoadGameSignal loadCPUGameDataSignal { get; set; }
        [Inject] public UpdatePlayerBucksSignal updatePlayerBucksDisplaySignal { get; set; }
        [Inject] public UpdateProfileSignal updateProfileSignal { get; set; }
        [Inject] public UpdateRemoveAdsSignal updateRemoveAdsDisplaySignal { get; set; }
        [Inject] public ResetActiveMatchSignal resetActiveMatchSignal{ get; set; }
        [Inject] public SetActionCountSignal setActionCountSignal { get; set; }

        // Models
        [Inject] public ICPUGameModel cpuGameModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
		[Inject] public IMetaDataModel metaDataModel { get; set; }
        [Inject] public IPicsModel picsModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public ICPUStatsModel cpuStatsModel { get; set; }

        // Services
        [Inject] public IFacebookService facebookService { get; set; }
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public IRateAppService rateAppService { get; set; }



        public void OldLobbyExecute() 
        {
            setSkinSignal.Dispatch(playerModel.activeSkinId);

            resetActiveMatchSignal.Dispatch();
            loadCPUGameDataSignal.Dispatch();
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_LOBBY);

            LobbyVO vo = new LobbyVO(cpuGameModel, playerModel, metaDataModel);

            updateMenuViewSignal.Dispatch(vo);
            updateAdsSignal.Dispatch();
            updatePlayerBucksDisplaySignal.Dispatch(playerModel.bucks);

            string localizedMins = localizationService.Get(LocalizationKey.FREE_NO_ADS_MINUTES);
            string localizedHours = localizationService.Get(LocalizationKey.FREE_NO_ADS_HOURS);
            string localizedDays = localizationService.Get(LocalizationKey.FREE_NO_ADS_DAYS);
            string timeRemain = TimeUtil.TimeToExpireString(playerModel.creationDate, metaDataModel.adsSettings.freeNoAdsPeriod,
                localizedMins, localizedHours, localizedDays);

            updateRemoveAdsDisplaySignal.Dispatch(timeRemain, playerModel.HasAdsFreePeriod(metaDataModel.adsSettings));

            if (!preferencesModel.isFriendScreenVisited)
            {
                setActionCountSignal.Dispatch(1);
            }

            ProfileVO pvo = new ProfileVO();
            pvo.playerPic = picsModel.GetPlayerPic(playerModel.id);
            pvo.playerName = playerModel.name;
            pvo.eloScore = playerModel.eloScore;
            pvo.countryId = playerModel.countryId;
            pvo.isFacebookLoggedIn = facebookService.isLoggedIn();
            pvo.playerId = playerModel.id;
            pvo.avatarId = playerModel.avatarId;

            if (pvo.isFacebookLoggedIn && pvo.playerPic == null)
            {
                pvo.playerPic = picsModel.GetPlayerPic(playerModel.id);
            }

            updateProfileSignal.Dispatch(pvo);

            if (!preferencesModel.hasRated && ((playerModel.totalGamesWon + cpuStatsModel.GetStarsCount()) >= metaDataModel.appInfo.rateAppThreshold))
            {
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_RATE_APP_DLG);
            }
        }*/
    }
}
