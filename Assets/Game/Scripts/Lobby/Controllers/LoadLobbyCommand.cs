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
        [Inject] public RefreshCommunitySignal refreshCommunitySignal { get; set; }
        [Inject] public RefreshFriendsSignal refreshFriendsSignal { get; set; }
        [Inject] public UpdateFriendBarSignal updateFriendBarSignal { get; set; }
        [Inject] public SetActionCountSignal setActionCountSignal { get; set; }
        [Inject] public UpdateProfileSignal updateProfileSignal { get; set; }
        [Inject] public UpdateRemoveAdsSignal updateRemoveAdsDisplaySignal { get; set; }
        [Inject] public AddFriendsSignal addFriendsSignal { get; set; }

        // Services
        [Inject] public IFacebookService facebookService { get; set; }
        [Inject] public IRateAppService rateAppService { get; set; }
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IPushNotificationService firebasePushNotificationService { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IPicsModel picsModel { get; set; }
        [Inject] public IMetaDataModel metaDataModel { get; set; }
        [Inject] public ICPUGameModel cpuGameModel { get; set; }
        [Inject] public ICPUStatsModel cpuStatsModel { get; set; }

        public override void Execute()
        {
            toggleBannerSignal.Dispatch(false);
            setSkinSignal.Dispatch(playerModel.activeSkinId);
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_LOBBY);
            resetActiveMatchSignal.Dispatch();
            loadCPUGameDataSignal.Dispatch();

            if (facebookService.isLoggedIn())
            {
                friendsShowConnectFacebookSignal.Dispatch(false);
            }
            else
            {
                friendsShowConnectFacebookSignal.Dispatch(true);
            }

            //refreshCommunitySignal.Dispatch();
            //refreshFriendsSignal.Dispatch();
            addFriendsSignal.Dispatch(playerModel.community, FriendCategory.COMMUNITY);

            // Update the timers on the bars
            foreach (string key in playerModel.friends.Keys)
            {
                updateFriendBarSignal.Dispatch(playerModel.friends[key], key);
            }

            LobbyVO vo = new LobbyVO(cpuGameModel, playerModel, metaDataModel);

            updateMenuViewSignal.Dispatch(vo);

            DispatchProfileSignal();
            DispatchRemoveAdsSignal();

            if (!preferencesModel.hasRated && ((playerModel.totalGamesWon + cpuStatsModel.GetStarsCount()) >= metaDataModel.appInfo.rateAppThreshold))
            {
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_RATE_APP_DLG);
            }

            if (preferencesModel.promotionCycleIndex == 0 && preferencesModel.isLobbyLoadedFirstTime)
            {
                loadPromotionSingal.Dispatch();
            }

            if (SplashLoader.launchCode == 1)
            {
                if (firebasePushNotificationService.IsNotificationOpened())
                {
                    hAnalyticsService.LogEvent("launch", "launch", "notification");
                }
                else
                {
                    hAnalyticsService.LogEvent("launch", "launch");
                }
                SplashLoader.launchCode = 3;
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
            pvo.playerId = playerModel.id;
            pvo.avatarId = playerModel.avatarId;
            pvo.avatarColorId = playerModel.avatarBgColorId;
            pvo.isPremium = playerModel.HasSubscription();

            if (pvo.isFacebookLoggedIn && pvo.playerPic == null)
            {
                pvo.playerPic = picsModel.GetPlayerPic(playerModel.id);
            }

            updateProfileSignal.Dispatch(pvo);
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
