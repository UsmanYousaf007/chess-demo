/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.context.api;
using strange.extensions.context.impl;
using TurboLabz.Chess;
using TurboLabz.InstantGame;
using TurboLabz.TLUtils;
using UnityEngine;


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
            commandBinder.Bind<NavigatorIgnoreEventSignal>().To<NavigatorIgnoreEventCommand>();
            commandBinder.Bind<UpdateFriendBarSignal>().To<UpdateFriendBarCommand>();
            commandBinder.Bind<ContactSupportSignal>().To<ContactSupportCommand>();
            commandBinder.Bind<MatchAnalyticsSignal>().To<MatchAnalyticsCommand>();

            commandBinder.Bind<SavePlayerInventorySignal>().To<SavePlayerInventoryCommand>();
            commandBinder.Bind<InitBackendOnceSignal>().To<InitBackendOnce>().Once();
            commandBinder.Bind<ReceptionSignal>().To<ReceptionCommand>();
            commandBinder.Bind<BackendErrorSignal>().To<BackendErrorCommand>();
            commandBinder.Bind<SaveLastWatchedVideoSignal>().To<SaveLastWatchedVideoCommand>();

            commandBinder.Bind<LoadArenaSignal>().To<LoadArenaCommand>();
            commandBinder.Bind<LoadRewardsSignal>().To<LoadRewardsCommand>();


            // Bind signals to models data loader commands
            commandBinder.Bind<GetInitDataSignal>().To<GetInitDataCommand>();
            commandBinder.Bind<UpdatePlayerDataSignal>().To<UpdatePlayerDataCommand>();

            // Bind signals to social commands
            commandBinder.Bind<AuthFaceBookSignal>().To<AuthFacebookCommand>();
            commandBinder.Bind<AuthSignInWithAppleSignal>().To<AuthSignInWithAppleCommand>();
            commandBinder.Bind<RefreshCommunitySignal>().To<RefreshCommunityCommand>();
            commandBinder.Bind<SearchFriendSignal>().To<SearchFriendCommand>();
            commandBinder.Bind<UpdateSearchResultsSignal>().To<UpdateSearchResultsCommand>();
            commandBinder.Bind<RefreshFriendsSignal>().To<RefreshFriendsCommand>();
            commandBinder.Bind<NewFriendSignal>().To<NewFriendCommand>();
            commandBinder.Bind<RemoveFriendSignal>().To<RemoveFriendCommand>();
            commandBinder.Bind<BlockFriendSignal>().To<BlockFriendCommand>();
            commandBinder.Bind<RemoveCommunityFriendSignal>().To<RemoveCommunityFriendCommand>();
            commandBinder.Bind<RemoveRecentlyPlayedSignal>().To<RemoveRecentlyPlayedCommand>();
            commandBinder.Bind<ShowProfileDialogSignal>().To<ShowProfileDialogCommand>();
            commandBinder.Bind<ShowShareScreenDialogSignal>().To<ShowShareDialogCommand>();
            commandBinder.Bind<UnblockFriendSignal>().To<UnblockFriendCommand>();

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
            commandBinder.Bind<RestorePurchasesSignal>().To<RestorePurchasesCommand>();
            commandBinder.Bind<SetSkinSignal>().To<SetSkinCommand>();
            commandBinder.Bind<SetDefaultSkinSignal>().To<SetDefaultSkinCommand>();
            commandBinder.Bind<PurchaseStoreItemSignal>().To<PurchaseStoreItemCommand>();
            commandBinder.Bind<ConsumeVirtualGoodSignal>().To<ConsumeVirtualGoodCommand>();
            commandBinder.Bind<RemoteStorePurchaseCompletedSignal>().To<RemoteStorePurchaseCompletedCommand>();
            commandBinder.Bind<ManageBlockedFriendsSignal>().To<ManageBlockedFriendsCommand>();
            commandBinder.Bind<UploadFileSignal>().To<UploadProfilePicCommand>();
            commandBinder.Bind<VirtualGoodsTransactionSignal>().To<VirtualGoodsTransactionCommand>();
            commandBinder.Bind<ShowInventoryRewardedVideoSignal>().To<ShowInventoryRewardedVideoCommand>();
            commandBinder.Bind<GetProfilePictureSignal>().To<GetProfilePictureCommand>();
            commandBinder.Bind<LoadSpotInventorySignal>().To<LoadSpotInventoryCommand>();
            commandBinder.Bind<LoadSpotCoinPurchaseSignal>().To<LoadSpotCoinPurchaseCommand>();

            commandBinder.Bind<GetAllTournamentsSignal>().To<GetAllTournamentsCommand>();
            commandBinder.Bind<GetTournamentLeaderboardSignal>().To<GetTournamentLeaderboardCommand>();
            commandBinder.Bind<FetchLiveTournamentRewardsSignal>().To<FetchLiveTournamentRewardsCommand>();
            commandBinder.Bind<UpdateTournamentsSignal>().To<UpdateTournamentsCommand>();

            // Bind signals for dispatching to mediators
            injectionBinder.Bind<NavigatorShowViewSignal>().ToSingleton();
            injectionBinder.Bind<NavigatorHideViewSignal>().ToSingleton();
            injectionBinder.Bind<AudioStateChangedSignal>().ToSingleton();
            injectionBinder.Bind<GetInitDataCompleteSignal>().ToSingleton();
            injectionBinder.Bind<GetInitDataFailedSignal>().ToSingleton();
            injectionBinder.Bind<AuthFacebookResultSignal>().ToSingleton();
            injectionBinder.Bind<AuthSignInWithAppleResultSignal>().ToSingleton();
            injectionBinder.Bind<SignOutSocialAccountSignal>().ToSingleton();
            injectionBinder.Bind<SetErrorAndHaltSignal>().ToSingleton();
            injectionBinder.Bind<FindMatchCompleteSignal>().ToSingleton();
            injectionBinder.Bind<FindRandomLongMatchCompleteSignal>().ToSingleton();
            injectionBinder.Bind<MatchFoundSignal>().ToSingleton();
            injectionBinder.Bind<UpdateProfileSignal>().ToSingleton();
            injectionBinder.Bind<UpdateOpponentProfileSignal>().ToSingleton();
            injectionBinder.Bind<UpdateChatOpponentPicSignal>().ToSingleton();
            injectionBinder.Bind<SetUpdateURLSignal>().ToSingleton();
            injectionBinder.Bind<UpdatePlayerRewardsPointsSignal>().ToSingleton();
            injectionBinder.Bind<UpdatePlayerInventorySignal>().ToSingleton();
            injectionBinder.Bind<UpdateRemoveAdsSignal>().ToSingleton();
            injectionBinder.Bind<PurchaseStoreItemResultSignal>().ToSingleton();
            injectionBinder.Bind<UpdatePurchasedStoreItemSignal>().ToSingleton();
            injectionBinder.Bind<GameDisconnectingSignal>().ToSingleton();
            injectionBinder.Bind<ReconnectionCompleteSignal>().ToSingleton();
            injectionBinder.Bind<FriendBarBusySignal>().ToSingleton();
            injectionBinder.Bind<SortFriendsSignal>().ToSingleton();
            injectionBinder.Bind<SortCommunitySignal>().ToSingleton();
            injectionBinder.Bind<SortSearchedSignal>().ToSingleton();
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
            injectionBinder.Bind<ShowViewBoardResultsPanelSignal>().ToSingleton();
            injectionBinder.Bind<PostShowNotificationSignal>().ToSingleton();
            injectionBinder.Bind<ChallengeAcceptedSignal>().ToSingleton();
            injectionBinder.Bind<OpponentPingedForConnectionSignal>().ToSingleton();
            injectionBinder.Bind<UpdateShareDialogSignal>().ToSingleton();
            injectionBinder.Bind<ChessboardBlockerEnableSignal>().ToSingleton();
            injectionBinder.Bind<PauseNotificationsSignal>().ToSingleton();
            injectionBinder.Bind<SyncReconnectDataSignal>().ToSingleton();
            injectionBinder.Bind<CancelSearchResultSignal>().ToSingleton();
            injectionBinder.Bind<FindMatchRequestCompleteSignal>().ToSingleton();
            injectionBinder.Bind<StoreAvailableSignal>().ToSingleton();
            injectionBinder.Bind<RewardUnlockedSignal>().ToSingleton();
            injectionBinder.Bind<ThemeAlertDisableSignal>().ToSingleton();
            injectionBinder.Bind<ShowPromotionDlgSignal>().ToSingleton();
            injectionBinder.Bind<ClosePromotionUpdateDlgSignal>().ToSingleton();
            injectionBinder.Bind<ShowPromotionUpdateDlgSignal>().ToSingleton();
            injectionBinder.Bind<ClosePromotionDlgSignal>().ToSingleton();
            injectionBinder.Bind<SubscriptionDlgClosedSignal>().ToSingleton();
            injectionBinder.Bind<ShowAdSkippedDlgSignal>().ToSingleton();
            injectionBinder.Bind<DisableModalBlockersSignal>().ToSingleton();
            injectionBinder.Bind<SelectTierSignal>().ToSingleton();
            injectionBinder.Bind<SetSubscriptionContext>().ToSingleton();
            injectionBinder.Bind<UpdateOfferDrawSignal>().ToSingleton();
            injectionBinder.Bind<SkillSelectedSignal>().ToSingleton();
            injectionBinder.Bind<PhotoPickerCompleteSignal>().ToSingleton();
            injectionBinder.Bind<VideoEventSignal>().ToSingleton();
            injectionBinder.Bind<VideoLoadFailedSignal>().ToSingleton();
            injectionBinder.Bind<UpdateVideoLessonViewSignal>().ToSingleton();
            injectionBinder.Bind<ShowVideoLessonSignal>().ToSingleton();
            injectionBinder.Bind<UpdateShopBundlePurchasedViewSignal>().ToSingleton();
            injectionBinder.Bind<VirtualGoodBoughtSignal>().ToSingleton();
            injectionBinder.Bind<InventoryVideoResultSignal>().ToSingleton();
            injectionBinder.Bind<VirtualGoodsTransactionResultSignal>().ToSingleton();
            injectionBinder.Bind<UpdateBottomNavSignal>().ToSingleton();
            injectionBinder.Bind<InboxAddMessagesSignal>().ToSingleton();
            injectionBinder.Bind<InboxRemoveMessagesSignal>().ToSingleton();
            injectionBinder.Bind<TournamentOpFailedSignal>().ToSingleton();
            injectionBinder.Bind<GetTournamentsSuccessSignal>().ToSingleton();
            injectionBinder.Bind<UpdateTournamentLeaderboardSignal>().ToSingleton();
            injectionBinder.Bind<UpdateTournamentLeaderboardPartialSignal>().ToSingleton();
            injectionBinder.Bind<UpdateTournamentLeaderboardViewSignal>().ToSingleton();
            injectionBinder.Bind<ToggleLeaderboardViewNavButtons>().ToSingleton();
            injectionBinder.Bind<UpdateTournamentsViewSignal>().ToSingleton();
            injectionBinder.Bind<UpdateLiveTournamentRewardsSuccessSignal>().ToSingleton();
            injectionBinder.Bind<UpdateInboxMessageCountViewSignal>().ToSingleton();
            injectionBinder.Bind<UpdateRewardDlgViewSignal>().ToSingleton();
            injectionBinder.Bind<DownloadableContentEventSignal>().ToSingleton();
            injectionBinder.Bind<UpdateChestInfoDlgViewSignal>().ToSingleton();
            injectionBinder.Bind<OnTournamentEndRewardViewClickedSignal>().ToSingleton();
            injectionBinder.Bind<LoadRewardDlgViewSignal>().ToSingleton();
            injectionBinder.Bind<InboxFetchingMessagesSignal>().ToSingleton();
            injectionBinder.Bind<SetLeaguesSignal>().ToSingleton();
            injectionBinder.Bind<PlayerModelUpdatedSignal>().ToSingleton();
            injectionBinder.Bind<UnlockCurrentJoinedTournamentSignal>().ToSingleton();
            injectionBinder.Bind<AppUpdateSignal>().ToSingleton();
            injectionBinder.Bind<AppResumedSignal>().ToSingleton();
            injectionBinder.Bind<UpdateSpotInventoryViewSignal>().ToSingleton();
            injectionBinder.Bind<SpotInventoryPurchaseCompletedSignal>().ToSingleton();
            injectionBinder.Bind<TournamentOverDialogueClosedSignal>().ToSingleton();
            injectionBinder.Bind<ClearInboxSignal>().ToSingleton();
            injectionBinder.Bind<ResetSubscirptionStatusSignal>().ToSingleton();
            injectionBinder.Bind<ActivePromotionSaleSingal>().ToSingleton();
            injectionBinder.Bind<ShowFadeBlockerSignal>().ToSingleton();
            injectionBinder.Bind<PromotionCycleOverSignal>().ToSingleton();
            injectionBinder.Bind<LoginAsGuestSignal>().ToSingleton();
            injectionBinder.Bind<UpdateSpotCoinsPurchaseDlgSignal>().ToSingleton();
            injectionBinder.Bind<UpdateSpotCoinsWatchAdDlgSignal>().ToSingleton();
            injectionBinder.Bind<RewardedVideoResultSignal>().ToSingleton();

            // Bind views to mediators
            mediationBinder.Bind<ChestInfoDialogView>().To<ChestContentDialogMediator>();
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
            mediationBinder.Bind<NotificationView>().To<NotificationMediator>();
            mediationBinder.Bind<ShareDialogView>().To<ShareDialogMediator>();
            mediationBinder.Bind<SkillLevelDlgView>().To<SkillLevelDlgMediator>();
            mediationBinder.Bind<MaintenanceView>().To<MaintenanceMediator>();
            mediationBinder.Bind<ConfirmDlgView>().To<ConfirmDlgMediator>();
            mediationBinder.Bind<RewardDlgView>().To<RewardDlgMediator>();
            mediationBinder.Bind<ProfilePicView>().To<ProfilePicMediator>();
            mediationBinder.Bind<SocialConnectionView>().To<SocialConnectionMediator>();
            mediationBinder.Bind<SocialConnectionDlgView>().To<SocialConnectionDlgMediator>();
            mediationBinder.Bind<LoginDlgView>().To<LoginDlgMediator>();

            // Bind services
            injectionBinder.Bind<ILocalizationService>().To<LocalizationService>().ToSingleton();
            injectionBinder.Bind<ILocalDataService>().To<EasySaveService>().ToSingleton();
            injectionBinder.Bind<IShareService>().To<NativeShareService>().ToSingleton();
            injectionBinder.Bind<IScreenCaptureService>().To<NativeScreenCapture>().ToSingleton();
            injectionBinder.Bind<IAndroidNativeService>().To<AndroidNativeService>().ToSingleton();
            injectionBinder.Bind<IAdsService>().To<TLAdsService>().ToSingleton();
            injectionBinder.Bind<IHAnalyticsService>().To<HAnalyticsService>().ToSingleton();


#if !UNITY_EDITOR && UNITY_ANDROID
            injectionBinder.Bind<IAppUpdateService>().To<AppUpdateServiceAndroid>().ToSingleton();
#elif !UNITY_EDITOR && UNITY_IOS
            injectionBinder.Bind<IAppUpdateService>().To<AppUpdateServiceIOS>().ToSingleton();
#else
            injectionBinder.Bind<IAppUpdateService>().To<AppUpdateServiceEditor>().ToSingleton();

#endif



#if UNITY_EDITOR
            injectionBinder.Bind<IAnalyticsService>().To<UnityAnalyticsServiceEditor>().ToSingleton();
#else
            injectionBinder.Bind<IAnalyticsService>().To<UnityAnalyticsService>().ToSingleton();
#endif
            injectionBinder.Bind<IStoreService>().To<UnityIAPService>().ToSingleton();
            injectionBinder.Bind<IBackendService>().To<GSService>().ToSingleton();
            injectionBinder.Bind<IAWSService>().To<AWSService>().ToSingleton();
            injectionBinder.Bind<IFacebookService>().To<FBService>().ToSingleton();
            injectionBinder.Bind<ISignInWithAppleService>().To<SignInWithAppleService>().ToSingleton();
            injectionBinder.Bind<IPushNotificationService>().To<FirebasePushNotificationService>().ToSingleton();
            injectionBinder.Bind<IRateAppService>().To<RateAppService>().ToSingleton();
            injectionBinder.Bind<IAppsFlyerService>().To<AppsFlyerService>().ToSingleton();
            injectionBinder.Bind<IAutoSubscriptionDailogueService>().To<AutoSubscriptionDailogueService>().ToSingleton();
            injectionBinder.Bind<IVideoPlaybackService>().To<AVProVideoPlayer>().ToSingleton();
            injectionBinder.Bind<IGameModesAnalyticsService>().To<GameModesAnalyticsService>().ToSingleton();
            injectionBinder.Bind<IPhotoService>().To<PhotoPickerService>().ToSingleton();
            injectionBinder.Bind<IProfilePicService>().To<ProfilePicService>().ToSingleton();
            injectionBinder.Bind<IDownloadablesService>().To<DownloadablesService>().ToSingleton();
            injectionBinder.Bind<ISchedulerService>().To<SchedulerService>().ToSingleton();
            injectionBinder.Bind<IPromotionsService>().To<PromotionsService>().ToSingleton();
            injectionBinder.Bind<IRewardsService>().To<RewardsService>().ToSingleton();

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
            injectionBinder.Bind<ForceUpdateFriendOnlineStatusSignal>().ToSingleton();
            injectionBinder.Bind<SplashWifiIsHealthySignal>().ToSingleton();
            injectionBinder.Bind<ShowSplashContentSignal>().ToSingleton();
            injectionBinder.Bind<UpdateManageBlockedFriendsViewSignal>().ToSingleton();
            injectionBinder.Bind<ResetUnblockButtonSignal>().ToSingleton();
            injectionBinder.Bind<LeagueProfileStripSetOnClickSignal>().ToSingleton();
            injectionBinder.Bind<UpdateLeagueProfileStripSignal>().ToSingleton();
            injectionBinder.Bind<UpdateLeagueProfileSignal>().ToSingleton();
            injectionBinder.Bind<ProfilePictureLoadedSignal>().ToSingleton();
            injectionBinder.Bind<UpdateCareerCardSignal>().ToSingleton();
            injectionBinder.Bind<UpdateAllStarLeaderboardSignal>().ToSingleton();
            injectionBinder.Bind<UpdateTimeSelectDlgSignal>().ToSingleton();

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
            injectionBinder.Bind<ISettingsModel>().To<SettingsModel>().ToSingleton();
            injectionBinder.Bind<ILessonsModel>().To<LessonsModel>().ToSingleton();
            injectionBinder.Bind<ITournamentsModel>().To<TournamentsModel>().ToSingleton();
            injectionBinder.Bind<IInboxModel>().To<InboxModel>().ToSingleton();
            injectionBinder.Bind<IDownloadablesModel>().To<DownloadablesModel>().ToSingleton();
            injectionBinder.Bind<ILeaguesModel>().To<LeaguesModel>().ToSingleton();
            injectionBinder.Bind<INotificationsModel>().To<NotificationsModel>().ToSingleton();
            injectionBinder.Bind<ILeaderboardModel>().To<LeaderboardModel>().ToSingleton();

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
            commandBinder.Bind<ShowRewardedAdSignal>().To<ShowRewardedAdCommand>();
            commandBinder.Bind<ToggleBannerSignal>().To<ToggleBannerCommand>();
            commandBinder.Bind<LoadLobbySignal>().To<LoadLobbyCommand>();
            commandBinder.Bind<UpdateAdsSignal>().To<UpdateAdCommand>();
            commandBinder.Bind<LoadPromotionSingal>().To<LoadPromotionCommand>();
            commandBinder.Bind<LoadChatSignal>().To<LoadChatCommand>();
            commandBinder.Bind<ShowInGameProfileSingal>().To<ShowInGameProfileCommand>();
            commandBinder.Bind<LoadVideoSignal>().To<LoadVideoCommand>();
            commandBinder.Bind<LoadTopicsViewSignal>().To<LoadTopicsViewCommand>();
            commandBinder.Bind<LoadLessonsViewSignal>().To<LoadLessonsViewCommand>();
            commandBinder.Bind<LoadRewardDlgViewSignal>().To<LoadRewardDlgCommand>();
            commandBinder.Bind<LoadLeaderboardSignal>().To<LoadLeaderboardCommand>();
            commandBinder.Bind<GetAllStarLeaderboardSignal>().To<GetAllStarLeaderboardCommand>();

            // Bind views to mediators
            mediationBinder.Bind<LobbyView>().To<LobbyMediator>();
            mediationBinder.Bind<StatsView>().To<StatsMediator>();
            mediationBinder.Bind<FriendsView>().To<FriendsMediator>();
            mediationBinder.Bind<ChatView>().To<ChatMediator>();
            mediationBinder.Bind<ThemeSelectionView>().To<ThemeSelectionMediator>();
            mediationBinder.Bind<SkinItemView>().To<SkinItemMediator>();
            mediationBinder.Bind<SubscriptionDlgView>().To<SubscriptionDlgMediator>();
            mediationBinder.Bind<SettingsView>().To<SettingsMediator>();
            mediationBinder.Bind<SoundView>().To<SoundMediator>();
            mediationBinder.Bind<PromotionDlgView>().To<PromotionDlgMediator>();
            mediationBinder.Bind<EarnRewardsView>().To<EarnRewardsMediator>();
            mediationBinder.Bind<SubscriptionTierView>().To<SubscriptionTierMediator>();
            mediationBinder.Bind<ManageSubscriptionView>().To<ManageSubscriptionMediator>();
            mediationBinder.Bind<ManageBlockedFriendsView>().To<ManageBlockedFriendsMediator>();
            mediationBinder.Bind<LessonsVideoPlayerView>().To<LessonsVideoPlayerMediator>();
            mediationBinder.Bind<TopicsView>().To<TopicsMediator>();
            mediationBinder.Bind<LessonsView>().To<LessonsMediator>();
            mediationBinder.Bind<ShopItemView>().To<ShopItemMediator>();
            mediationBinder.Bind<ShopView>().To<ShopMediator>();
            mediationBinder.Bind<ShopBundlePurchasedView>().To<ShopBundlePurchasedMediator>();
            mediationBinder.Bind<InventoryView>().To<InventoryMediator>();
            mediationBinder.Bind<InventoryItemView>().To<InventoryItemMediator>();
            mediationBinder.Bind<SpotPurchaseView>().To<SpotPurchaseMediator>();
            mediationBinder.Bind<LeagueProfileStripView>().To<LeagueProfileStripMediator>();
            mediationBinder.Bind<TournamentsView>().To<TournamentsMediator>();
            mediationBinder.Bind<TournamentLeaderboardView>().To<TournamentLeaderboardMediator>();
            mediationBinder.Bind<InboxView>().To<InboxMediator>();
            mediationBinder.Bind<LeaguePerksView>().To<LeaguePerksMediator>();
            mediationBinder.Bind<TournamentOverDlgView>().To<TournamentOverDlgMediator>();
            mediationBinder.Bind<SpotInventoryView>().To<SpotInventoryMediator>();
            mediationBinder.Bind<PromotionChessCourseBundleDlgView>().To<PromotionChessCourseBundleDlgMediator>();
            mediationBinder.Bind<PromotionChessSetsBundleDlgView>().To<PromotionChessSetsBundleDlgMediator>();
            mediationBinder.Bind<PromotionEliteBundleDlgView>().To<PromotionEliteBundleDlgMediator>();
            mediationBinder.Bind<PromotionRemoveAdsDlgView>().To<PromotionRemoveAdsDlgMediator>();
            mediationBinder.Bind<PromotionWelcomeBundleDlgView>().To<PromotionWelcomeBundleDlgMediator>();
            mediationBinder.Bind<CPUCardView>().To<CPUCardMediator>();
            mediationBinder.Bind<LessonCardView>().To<LessonCardMediator>();
            mediationBinder.Bind<CareerCardView>().To<CareerCardMediator>();
            mediationBinder.Bind<NewLobbyView>().To<NewLobbyMediator>();
            mediationBinder.Bind<LobbyProfileBarView>().To<LobbyProfileBarMediator>();
            mediationBinder.Bind<LobbyTickerView>().To<LobbyTickerMediator>();
            mediationBinder.Bind<LeaderboardView>().To<LeaderboardMediator>();
            mediationBinder.Bind<SelectTimeModeView>().To<SelectTimeModeMediator>();
            mediationBinder.Bind<ChampionshipResultDlgMediator>().To<ChampionshipResultDlgMediator>();
            mediationBinder.Bind<ChampionshipNewRankDlgView>().To<ChampionshipNewRankDlgMediator>();
            mediationBinder.Bind<ShopCoinItemView>().To<ShopCoinItemMediator>();
            mediationBinder.Bind<SpotCoinPurchaseView>().To<SpotCoinPurchaseMediator>();

            // Skinning view/mediators
            mediationBinder.Bind<SkinLink>().To<SkinLinkMediator>();
            mediationBinder.Bind<SkinRefs>().To<SkinRefsMediator>();
            injectionBinder.Bind<LoadSkinRefsSignal>().ToSingleton();
            injectionBinder.Bind<RefreshSkinLinksSignal>().ToSingleton();
            injectionBinder.Bind<SkinUpdatedSignal>().ToSingleton();
            injectionBinder.Bind<ShowMaintenanceViewSignal>().ToSingleton();


            // Bind signals for dispatching to/from mediators
            injectionBinder.Bind<UpdateMenuViewSignal>().ToSingleton();
            injectionBinder.Bind<UpdateStrengthSignal>().ToSingleton();
            injectionBinder.Bind<UpdateDurationSignal>().ToSingleton();
            injectionBinder.Bind<UpdatePlayerColorSignal>().ToSingleton();
            injectionBinder.Bind<UpdateThemeSignal>().ToSingleton();
            injectionBinder.Bind<UpdateStatsSignal>().ToSingleton();
            injectionBinder.Bind<UpdateLobbyAdsSignal>().ToSingleton();
            injectionBinder.Bind<ToggleFacebookButton>().ToSingleton();
            injectionBinder.Bind<PlayerProfilePicTappedSignal>().ToSingleton();
            injectionBinder.Bind<RequestToggleBannerSignal>().ToSingleton();
            injectionBinder.Bind<ReconnectViewEnableSignal>().ToSingleton();
            injectionBinder.Bind<UpdateFindViewSignal>().ToSingleton();
            injectionBinder.Bind<ShowPromotionSignal>().ToSingleton();
            injectionBinder.Bind<ShowCoachTrainingDailogueSignal>().ToSingleton();
            injectionBinder.Bind<ShowStrengthTrainingDailogueSignal>().ToSingleton();
            injectionBinder.Bind<RemoveLobbyPromotionSignal>().ToSingleton();
            injectionBinder.Bind<UpdateConfirmDlgSignal>().ToSingleton();
            injectionBinder.Bind<ShowProcessingSignal>().ToSingleton();
            injectionBinder.Bind<UpdateTopiscViewSignal>().ToSingleton();
            injectionBinder.Bind<UpdateLessonsViewSignal>().ToSingleton();
            injectionBinder.Bind<RatingBoostedSignal>().ToSingleton();
            injectionBinder.Bind<ShowBottomNavSignal>().ToSingleton();
            injectionBinder.Bind<ShowThemesInventoryTabSignal>().ToSingleton();

            // Bind models
            injectionBinder.Bind<ICPUStatsModel>().To<CPUStatsModel>().ToSingleton();
            injectionBinder.Bind<IPicsModel>().To<PicsModel>().ToSingleton();
        }
    }
}
