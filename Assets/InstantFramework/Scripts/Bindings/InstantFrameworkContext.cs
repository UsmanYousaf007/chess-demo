/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using TurboLabz.Chess;
using TurboLabz.TLUtils;
using TurboLabz.InstantGame;

namespace TurboLabz.InstantFramework
{
    public partial class InstantFrameworkContext : MVCSContext
    {
        public InstantFrameworkContext(MonoBehaviour view) : base(view)
        {
        }

        public InstantFrameworkContext(MonoBehaviour view, ContextStartupFlags flags) : base(view, flags)
        {
        }

        // Override Start so that we can fire the StartSignal.
        public override IContext Start()
        {
            base.Start();
            StartSignal startSignal = injectionBinder.GetInstance<StartSignal>();
            startSignal.Dispatch();
            return this;
        }

        // TODO: Organize the order and comments for the bindings
        protected override void mapBindings()
        {
            // We are not using any implicit bindings at the moment, hence this code is commented.
            // Scan namespaces for implicit binding
            // implicitBinder.ScanForAnnotatedClasses("TurboLabz");

            // Bind signals to commands
            commandBinder.Bind<StartSignal>().To<StartCommand>().Once();
            commandBinder.Bind<InitFacebookSignal>().To<InitFacebookCommand>();
            commandBinder.Bind<AppEventSignal>().To<AppEventCommand>();
            commandBinder.Bind<LoadStatsSignal>().To<LoadStatsCommand>();
			commandBinder.Bind<LoadStoreSignal>().To<LoadStoreCommand>();
            commandBinder.Bind<LoadSpotPurchaseSignal>().To<LoadSpotPurchaseCommand>();
            commandBinder.Bind<LoadFriendsSignal>().To<LoadFriendsCommand>();
			commandBinder.Bind<FindMatchSignal>().To<FindMatchCommand>();
            commandBinder.Bind<TapLongMatchSignal>().To<TapLongMatchCommand>();
            commandBinder.Bind<StartLongMatchSignal>().To<StartLongMatchCommand>();
            commandBinder.Bind<CreateLongMatchSignal>().To<CreateLongMatchCommand>();
            commandBinder.Bind<GetGameStartTimeSignal>().To<GetGameStartTimeCommand>();
            commandBinder.Bind<ShowFindMatchSignal>().To<ShowFindMatchCommand>();
            commandBinder.Bind<StartGameSignal>().To<StartGameCommand>();
            commandBinder.Bind<GameAppEventSignal>().To<GameAppEventCommand>();
            commandBinder.Bind<NavigatorEventSignal>().To<NavigatorCommand>();
            commandBinder.Bind<ShareAppSignal>().To<ShareAppCommand>();
			commandBinder.Bind<PurchaseStoreItemSignal>().To<PurchaseStoreItemCommand>();
            commandBinder.Bind<ConsumeVirtualGoodSignal>().To<ConsumeVirtualGoodCommand>();
            commandBinder.Bind<NavigatorIgnoreEventSignal>().To<NavigatorIgnoreEventCommand>();
            commandBinder.Bind<UpdateFriendBarSignal>().To<UpdateFriendBarCommand>();

            commandBinder.Bind<SavePlayerInventorySignal>().To<SavePlayerInventoryCommand>();
            commandBinder.Bind<SetSkinSignal>().To<SetSkinCommand>();
            commandBinder.Bind<InitBackendOnceSignal>().To<InitBackendOnce>().Once();
            commandBinder.Bind<ReceptionSignal>().To<ReceptionCommand>();
            commandBinder.Bind<BackendErrorSignal>().To<BackendErrorCommand>();

			// Bind signals to models data loader commands
			commandBinder.Bind<GetInitDataSignal>().To<GetInitDataCommand>();
            commandBinder.Bind<UpdatePlayerDataSignal>().To<UpdatePlayerDataCommand>();

            // Bind signals to social commands
            commandBinder.Bind<AuthFaceBookSignal>().To<AuthFacebookCommand>();
            commandBinder.Bind<RefreshCommunitySignal>().To<RefreshCommunityCommand>();
            commandBinder.Bind<SearchFriendSignal>().To<SearchFriendCommand>();
            commandBinder.Bind<RefreshFriendsSignal>().To<RefreshFriendsCommand>();
            commandBinder.Bind<NewFriendSignal>().To<NewFriendCommand>();
            commandBinder.Bind<RemoveFriendSignal>().To<RemoveFriendCommand>();
            commandBinder.Bind<BlockFriendSignal>().To<BlockFriendCommand>();
            commandBinder.Bind<RemoveCommunityFriendSignal>().To<RemoveCommunityFriendCommand>();
            commandBinder.Bind<ShowProfileDialogSignal>().To<ShowProfileDialogCommand>();
            commandBinder.Bind<ShowShareScreenDialogSignal>().To<ShowShareDialogCommand>();
            

            commandBinder.Bind<GetSocialPicsSignal>().To<GetSocialPicsCommand>();
            commandBinder.Bind<AcceptSignal>().To<AcceptCommand>();
            commandBinder.Bind<DeclineSignal>().To<DeclineCommand>();
            commandBinder.Bind<CloseStripSignal>().To<CloseStripCommand>();
            commandBinder.Bind<UnregisterSignal>().To<UnregisterCommand>();
            commandBinder.Bind<SendChatMessageSignal>().To<SendChatMessageCommand>();
            commandBinder.Bind<ReceiveChatMessageSignal>().To<ReceiveChatMessageCommand>();
            commandBinder.Bind<ClearActiveChatSignal>().To<ClearActiveChatCommand>();
            commandBinder.Bind<ClearUnreadMessagesSignal>().To<ClearUnreadMessagesCommand>();
            commandBinder.Bind<ResumeMatchSignal>().To<ResumeMatchCommand>();
            commandBinder.Bind<ChangeUserDetailsSignal>().To<ChangeUserDetailsCommand>();

 


            // Bind signals for dispatching to mediators
            injectionBinder.Bind<NavigatorShowViewSignal>().ToSingleton();
            injectionBinder.Bind<NavigatorHideViewSignal>().ToSingleton();
            injectionBinder.Bind<AudioStateChangedSignal>().ToSingleton();
            injectionBinder.Bind<GetInitDataCompleteSignal>().ToSingleton();
            injectionBinder.Bind<AuthFacebookResultSignal>().ToSingleton();
            injectionBinder.Bind<SetErrorAndHaltSignal>().ToSingleton();
            injectionBinder.Bind<FindMatchCompleteSignal>().ToSingleton();
            injectionBinder.Bind<MatchFoundSignal>().ToSingleton();
            injectionBinder.Bind<UpdateProfileSignal>().ToSingleton();
            injectionBinder.Bind<UpdateOpponentProfileSignal>().ToSingleton();
            injectionBinder.Bind<UpdateChatOpponentPicSignal>().ToSingleton();
            injectionBinder.Bind<SetUpdateURLSignal>().ToSingleton();
            injectionBinder.Bind<UpdatePlayerBucksSignal>().ToSingleton();
            injectionBinder.Bind<UpdatePlayerInventorySignal>().ToSingleton();
            injectionBinder.Bind<UpdateRemoveAdsSignal>().ToSingleton();
            injectionBinder.Bind<PurchaseStoreItemResultSignal>().ToSingleton();
            injectionBinder.Bind<UpdatePurchasedStoreItemSignal>().ToSingleton();
            injectionBinder.Bind<UpdatePurchasedBundleStoreItemSignal>().ToSingleton();
            injectionBinder.Bind<GameDisconnectingSignal>().ToSingleton();
            injectionBinder.Bind<FriendBarBusySignal>().ToSingleton();
            injectionBinder.Bind<SortFriendsSignal>().ToSingleton();
            injectionBinder.Bind<SortCommunitySignal>().ToSingleton();
            injectionBinder.Bind<SortSearchedSignal>().ToSingleton();
            injectionBinder.Bind<StoreAvailableSignal>().ToSingleton();
            injectionBinder.Bind<SetActionCountSignal>().ToSingleton();
            injectionBinder.Bind<ShowFriendsHelpSignal>().ToSingleton();
            injectionBinder.Bind<AddUnreadMessagesToBarSignal>().ToSingleton();
            injectionBinder.Bind<ClearUnreadMessagesFromBarSignal>().ToSingleton();
            injectionBinder.Bind<ModelsSaveToDiskSignal>().ToSingleton();
            injectionBinder.Bind<ModelsLoadFromDiskSignal>().ToSingleton();
            injectionBinder.Bind<ModelsResetSignal>().ToSingleton();
            injectionBinder.Bind<NewFriendAddedSignal>().ToSingleton();
            injectionBinder.Bind<NotificationRecievedSignal>().ToSingleton();
            injectionBinder.Bind<PreShowNotificationSignal>().ToSingleton();
            injectionBinder.Bind<PostShowNotificationSignal>().ToSingleton();
            injectionBinder.Bind<ChallengeAcceptedSignal>().ToSingleton();
            injectionBinder.Bind<OpponentPingedForConnectionSignal>().ToSingleton();
            injectionBinder.Bind<UpdateShareDialogSignal>().ToSingleton();
            injectionBinder.Bind<ChessboardBlockerEnableSignal>().ToSingleton();
            injectionBinder.Bind<PauseNotificationsSignal>().ToSingleton();

            // Bind views to mediators
            mediationBinder.Bind<SplashView>().To<SplashMediator>();
            mediationBinder.Bind<AppEventView>().To<AppEventMediator>();
            mediationBinder.Bind<HardStopView>().To<HardStopMediator>();
            mediationBinder.Bind<ReconnectingView>().To<ReconnectingMediator>();
            mediationBinder.Bind<UpdateView>().To<UpdateMediator>();
            mediationBinder.Bind<BottomNavView>().To<BottomNavMediator>();
            mediationBinder.Bind<TopNavView>().To<TopNavMediator>();
            mediationBinder.Bind<ProfileView>().To<ProfileMediator>();
            mediationBinder.Bind<OpponentProfileView>().To<OpponentProfileMediator>();
            mediationBinder.Bind<ProfileDialogView>().To<ProfileDialogMediator>();
            mediationBinder.Bind<RateAppDialogView>().To<RateAppDialogMediator>();
            mediationBinder.Bind<TopInventoryBarView>().To<TopInventoryBarMediator>();
            mediationBinder.Bind<NotificationView>().To<NotificationMediator>();
            mediationBinder.Bind<ShareDialogView>().To<ShareDialogMediator>();

            // Bind services
            injectionBinder.Bind<ILocalizationService>().To<LocalizationService>().ToSingleton();
            injectionBinder.Bind<ILocalDataService>().To<EasySaveService>().ToSingleton();
            injectionBinder.Bind<IShareService>().To<NativeShareService>().ToSingleton();
            injectionBinder.Bind<IScreenCaptureService>().To<NativeScreenCapture>().ToSingleton();
            injectionBinder.Bind<IAndroidNativeService>().To<AndroidNativeService>().ToSingleton();
            injectionBinder.Bind<IAdsService>().To<MoPubService>().ToSingleton();
            injectionBinder.Bind<IAnalyticsService>().To<UnityAnalyticsService>().ToSingleton();
			injectionBinder.Bind<IStoreService>().To<UnityIAPService>().ToSingleton();
            injectionBinder.Bind<IBackendService>().To<GSService>().ToSingleton();
            injectionBinder.Bind<IFacebookService>().To<FBService>().ToSingleton();
            injectionBinder.Bind<IPushNotificationService>().To<FirebasePushNotificationService>().ToSingleton();
            injectionBinder.Bind<IRateAppService>().To<RateAppService>().ToSingleton();
            injectionBinder.Bind<IAppsFlyerService>().To<AppsFlyerService>().ToSingleton();

#if UNITY_ANDROID && !UNITY_EDITOR
            injectionBinder.Bind<IAudioService>().To<UnityAudioAndroid>().ToSingleton();
#else
            injectionBinder.Bind<IAudioService>().To<UnityAudio>().ToSingleton();
            #endif

            // Bind utils
            injectionBinder.Bind<IRoutineRunner>().To<StrangeRoutineRunner>().ToSingleton();
            injectionBinder.Bind<IGameEngineInfo>().To<UnityInfo>().ToSingleton();
            injectionBinder.Bind<IServerClock>().To<ServerClock>().ToSingleton();

            // Bind signals for dispatching to/from mediators
            injectionBinder.Bind<WifiIsHealthySignal>().ToSingleton();
            injectionBinder.Bind<ResetCapturedPiecesSignal>().ToSingleton();
            injectionBinder.Bind<UpdateFriendPicSignal>().ToSingleton();
            injectionBinder.Bind<UpdateEloScoresSignal>().ToSingleton();
            injectionBinder.Bind<AddFriendsSignal>().ToSingleton();
            injectionBinder.Bind<ClearCommunitySignal>().ToSingleton();
            injectionBinder.Bind<ClearFriendsSignal>().ToSingleton();
            injectionBinder.Bind<ClearFriendSignal>().ToSingleton();
            injectionBinder.Bind<FriendsShowConnectFacebookSignal>().ToSingleton();
            injectionBinder.Bind<UpdateProfileDialogSignal>().ToSingleton();
            injectionBinder.Bind<UpdateFriendBarStatusSignal>().ToSingleton();
            injectionBinder.Bind<UpdateFriendOnlineStatusSignal>().ToSingleton();
            injectionBinder.Bind<SplashWifiIsHealthySignal>().ToSingleton();


            // Bind models
            injectionBinder.Bind<INavigatorModel>().To<NavigatorModel>().ToSingleton(); // Lifecycle handled
            injectionBinder.Bind<IPreferencesModel>().To<PreferencesModel>().ToSingleton(); // Lifecycle handled
            injectionBinder.Bind<IMetaDataModel>().To<MetaDataModel>().ToSingleton(); // Lifecycle handled
            injectionBinder.Bind<IPlayerModel>().To<PlayerModel>().ToSingleton(); // Lifecyle handled
            injectionBinder.Bind<IMatchInfoModel>().To<MatchInfoModel>().ToSingleton(); // Lifecyle handled
            injectionBinder.Bind<IStoreSettingsModel>().To<StoreSettingsModel>().ToSingleton(); // Lifecyle handled
            injectionBinder.Bind<IAppInfoModel>().To<AppInfoModel>().ToSingleton(); // Lifecyle handled
            injectionBinder.Bind<IAdsSettingsModel>().To<AdsSettingsModel>().ToSingleton(); // Lifecyle handled
            injectionBinder.Bind<IChatModel>().To<ChatModel>().ToSingleton(); // Lifecyle handled
            injectionBinder.Bind<IRewardsSettingsModel>().To<RewardsSettingsModel>().ToSingleton(); // Lifecycle handled
           

            MapGameBindings();
            MapCPUGameBindings();
            MapMultiplayerGameBindings();
        }

        // TODO: move this to the game folder
        private void MapGameBindings()
        {
            // Bind common services
            injectionBinder.Bind<IChessService>().To<ChessService>().ToSingleton();
            injectionBinder.Bind<IChessAiService>().To<ChessAiService>().ToSingleton();

            // Bind signals to commands
            commandBinder.Bind<SaveStatsSignal>().To<SaveStatsCommand>();
            commandBinder.Bind<AdjustStrengthSignal>().To<AdjustStrengthCommand>();
			commandBinder.Bind<ShowAdSignal>().To<ShowAdCommand>();
            commandBinder.Bind<ToggleBannerSignal>().To<ToggleBannerCommand>();
            commandBinder.Bind<LoadHomeSignal>().To<LoadHomeCommand>();
            commandBinder.Bind<LoadLobbySignal>().To<LoadLobbyCommand>();
			commandBinder.Bind<RemoteStorePurchaseCompletedSignal>().To<RemoteStorePurchaseCompletedCommand>();
            commandBinder.Bind<UpdateAdsSignal>().To<UpdateAdCommand>();
            commandBinder.Bind<RestorePurchasesSignal>().To<RestorePurchasesCommand>();

            // Bind views to mediators
            mediationBinder.Bind<HomeView>().To<HomeMediator>();
            mediationBinder.Bind<LobbyView>().To<LobbyMediator>();
            mediationBinder.Bind<StatsView>().To<StatsMediator>();
            mediationBinder.Bind<FriendsView>().To<FriendsMediator>();
			mediationBinder.Bind<StoreView>().To<StoreMediator>();
            mediationBinder.Bind<SpotPurchaseView>().To<SpotPurchaseMediator>();

            // Skinning view/mediators
            mediationBinder.Bind<SkinLink>().To<SkinLinkMediator>();
            mediationBinder.Bind<SkinRefs>().To<SkinRefsMediator>();
            injectionBinder.Bind<LoadSkinRefsSignal>().ToSingleton();
            injectionBinder.Bind<RefreshSkinLinksSignal>().ToSingleton();

            // Bind signals for dispatching to/from mediators
            injectionBinder.Bind<UpdateHomeViewSignal>().ToSingleton();
            injectionBinder.Bind<UpdateLobbyViewSignal>().ToSingleton();
            injectionBinder.Bind<UpdateStrengthSignal>().ToSingleton();
            injectionBinder.Bind<UpdateDurationSignal>().ToSingleton();
            injectionBinder.Bind<UpdatePlayerColorSignal>().ToSingleton();
			injectionBinder.Bind<UpdateThemeSignal>().ToSingleton();
            injectionBinder.Bind<UpdateStatsSignal>().ToSingleton();
			injectionBinder.Bind<UpdateStoreSignal>().ToSingleton();
            injectionBinder.Bind<UpdateStoreBuyDlgSignal>().ToSingleton();
			injectionBinder.Bind<UpdateStoreNotEnoughBucksDlgSignal>().ToSingleton();
            injectionBinder.Bind<UpdateLobbyAdsSignal>().ToSingleton();
            injectionBinder.Bind<ToggleFacebookButton>().ToSingleton();
            injectionBinder.Bind<UpdateTopInventoryBarSignal>().ToSingleton();
            injectionBinder.Bind<ShowStoreTabSignal>().ToSingleton();
            injectionBinder.Bind<UpdateSpotPurchaseSignal>().ToSingleton();
            injectionBinder.Bind<PlayerProfilePicTappedSignal>().ToSingleton();
            injectionBinder.Bind<RequestToggleBannerSignal>().ToSingleton();
            injectionBinder.Bind<ReconnectViewEnableSignal>().ToSingleton();



            // Bind models
            injectionBinder.Bind<ICPUStatsModel>().To<CPUStatsModel>().ToSingleton();
            injectionBinder.Bind<IPicsModel>().To<PicsModel>().ToSingleton();
        }
    }
}
