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
            commandBinder.Bind<AppEventSignal>().To<AppEventCommand>();
            commandBinder.Bind<LoadStatsSignal>().To<LoadStatsCommand>();
			commandBinder.Bind<LoadStoreSignal>().To<LoadStoreCommand>();
            commandBinder.Bind<FindMatchSignal>().To<FindMatchCommand>();
            commandBinder.Bind<GetGameStartTimeSignal>().To<GetGameStartTimeCommand>();
            commandBinder.Bind<ShowFindMatchSignal>().To<ShowFindMatchCommand>();
            commandBinder.Bind<StartGameSignal>().To<StartGameCommand>();
            commandBinder.Bind<GameAppEventSignal>().To<GameAppEventCommand>();
            commandBinder.Bind<NavigatorEventSignal>().To<NavigatorCommand>();
            commandBinder.Bind<ShareAppSignal>().To<ShareAppCommand>();
			commandBinder.Bind<PurchaseStoreItemSignal>().To<PurchaseStoreItemCommand>();
			commandBinder.Bind<LoadBuckPacksSignal>().To<LoadBuckPacksCommand>();
            commandBinder.Bind<NavigatorIgnoreEventSignal>().To<NavigatorIgnoreEventCommand>();

            //commandBinder.Bind<GetPlayerProfilePictureSignal>().To<GetPlayerProfilePictureCommand>();
            //commandBinder.Bind<GetOpponentProfilePictureSignal>().To<GetOpponentProfilePictureCommand>();
            commandBinder.Bind<ApplyPlayerInventorySignal>().To<ApplyPlayerInventoryCommand>();
            commandBinder.Bind<InitBackendOnceSignal>().To<InitBackendOnce>().Once();
            commandBinder.Bind<ReceptionSignal>().To<ReceptionCommand>();
            commandBinder.Bind<BackendErrorSignal>().To<BackendErrorCommand>();

			// Bind signals to models data loader commands
			commandBinder.Bind<InitGameDataSignal>().To<InitGameDataCommand>();

            // Bind signals to social commands
            commandBinder.Bind<AuthFaceBookSignal>().To<AuthFacebookCommand>();

            // Bind signals for dispatching to mediators
            injectionBinder.Bind<NavigatorShowViewSignal>().ToSingleton();
            injectionBinder.Bind<NavigatorHideViewSignal>().ToSingleton();
            injectionBinder.Bind<AudioStateChangedSignal>().ToSingleton();
            injectionBinder.Bind<LoadMetaDataCompleteSignal>().ToSingleton();
            injectionBinder.Bind<AuthFacebookSuccessSignal>().ToSingleton();
            injectionBinder.Bind<SplashAnimCompleteSignal>().ToSingleton();
            injectionBinder.Bind<UpdatePlayerProfilePictureSignal>().ToSingleton();
            injectionBinder.Bind<UpdatePlayerProfilePictureInfoSignal>().ToSingleton();
            injectionBinder.Bind<SetErrorAndHaltSignal>().ToSingleton();
            //injectionBinder.Bind<UpdateSetPlayerSocialNameViewSignal>().ToSingleton();

            // Bind views to mediators
            mediationBinder.Bind<SplashView>().To<SplashMediator>();
            mediationBinder.Bind<AppEventView>().To<AppEventMediator>();
            mediationBinder.Bind<HardStopView>().To<HardStopMediator>();

            // Bind services
            injectionBinder.Bind<ILocalizationService>().To<LocalizationService>().ToSingleton();
            injectionBinder.Bind<ILocalDataService>().To<EasySaveService>().ToSingleton();
            injectionBinder.Bind<IShareService>().To<MegacoolShareService>().ToSingleton();
            injectionBinder.Bind<IAndroidNativeService>().To<AndroidNativeService>().ToSingleton();
            injectionBinder.Bind<IAdsService>().To<UnityAdsService>().ToSingleton();
            injectionBinder.Bind<IAnalyticsService>().To<UnityAnalyticsService>().ToSingleton();
			injectionBinder.Bind<IStoreService>().To<UnityIAPService>().ToSingleton();
            injectionBinder.Bind<IBackendService>().To<GSService>().ToSingleton();
            injectionBinder.Bind<IFacebookService>().To<FBService>().ToSingleton();

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

            // Bind models
            injectionBinder.Bind<INavigatorModel>().To<NavigatorModel>().ToSingleton();
            injectionBinder.Bind<IPreferencesModel>().To<PreferencesModel>().ToSingleton();
			injectionBinder.Bind<IMetaDataModel>().To<MetaDataModel>().ToSingleton();
            injectionBinder.Bind<IPlayerModel>().To<PlayerModel>().ToSingleton();
            injectionBinder.Bind<IMatchInfoModel>().To<MatchInfoModel>().ToSingleton();
            injectionBinder.Bind<IStoreSettingsModel>().To<StoreSettingsModel>().ToSingleton();
            injectionBinder.Bind<IAppInfoModel>().To<AppInfoModel>().ToSingleton();
            injectionBinder.Bind<IAdsSettingsModel>().To<AdsSettingsModel>().ToSingleton();

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
            commandBinder.Bind<AdjustDurationSignal>().To<AdjustDurationCommand>();
            commandBinder.Bind<AdjustPlayerColorSignal>().To<AdjustPlayerColorCommand>();
			commandBinder.Bind<ShowAdSignal>().To<ShowAdCommand>();
            commandBinder.Bind<LoadLobbySignal>().To<LoadLobbyCommand>();
			commandBinder.Bind<RemoteStorePurchaseCompletedSignal>().To<RemoteStorePurchaseCompletedCommand>();
            commandBinder.Bind<UpdateAdsSignal>().To<UpdateAdCommand>();

            // Bind views to mediators
            mediationBinder.Bind<CPULobbyView>().To<CPULobbyMediator>();
            mediationBinder.Bind<CPUStatsView>().To<CPUStatsMediator>();
			mediationBinder.Bind<CPUStoreView>().To<CPUStoreMediator>();
			mediationBinder.Bind<BuckPacksDlgView>().To<BuckPacksDlgMediator>();

            // Skinning view/mediators
            mediationBinder.Bind<SkinLink>().To<SkinLinkMediator>();
            mediationBinder.Bind<SkinRefs>().To<SkinRefsMediator>();
            injectionBinder.Bind<ApplySkinSignal>().ToSingleton();
            injectionBinder.Bind<UpdateSkinSignal>().ToSingleton();

            // Bind signals for dispatching to/from mediators
            injectionBinder.Bind<UpdateMenuViewSignal>().ToSingleton();
            injectionBinder.Bind<UpdateStrengthSignal>().ToSingleton();
            injectionBinder.Bind<UpdateDurationSignal>().ToSingleton();
            injectionBinder.Bind<UpdatePlayerColorSignal>().ToSingleton();
			injectionBinder.Bind<UpdateThemeSignal>().ToSingleton();
            injectionBinder.Bind<UpdateStatsSignal>().ToSingleton();
			injectionBinder.Bind<UpdateStoreSignal>().ToSingleton();
            injectionBinder.Bind<UpdateStoreBuyDlgSignal>().ToSingleton();
			injectionBinder.Bind<UpdateStoreNotEnoughBucksDlgSignal>().ToSingleton();
			injectionBinder.Bind<UpdateStoreBuckPacksDlgSignal>().ToSingleton();
            injectionBinder.Bind<UpdateLobbyAdsSignal>().ToSingleton();
            injectionBinder.Bind<UpdateFreeBucksRewardSignal>().ToSingleton();
			injectionBinder.Bind<UpdatePlayerBucksDisplaySignal>().ToSingleton();
            injectionBinder.Bind<ToggleAdBlockerSignal>().ToSingleton();
            injectionBinder.Bind<PurchaseStoreItemResultSignal>().ToSingleton();

            // Bind models
            injectionBinder.Bind<IStatsModel>().To<StatsModel>().ToSingleton();
        }
    }
}
