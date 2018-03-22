/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-07 16:38:23 UTC+05:00
/// 
/// @description
/// 

using UnityEngine;

using strange.extensions.context.api;
using strange.extensions.context.impl;

using TurboLabz.Chess;
using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public partial class GamebetContext : MVCSContext
    {
        public GamebetContext(MonoBehaviour view) : base(view)
        {
        }

        public GamebetContext(MonoBehaviour view, ContextStartupFlags flags) : base(view, flags)
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
            commandBinder.Bind<NavigatorEventSignal>().To<NavigatorCommand>();
            commandBinder.Bind<StartSignal>().To<StartCommand>().Once();
            commandBinder.Bind<BootSignal>().To<BootCommand>().Once();

            commandBinder.Bind<LoadViewSignal>().To<LoadViewCommand>();
            commandBinder.Bind<LoadPreviousViewSignal>().To<LoadPreviousViewCommand>();

            commandBinder.Bind<LoadModalViewSignal>().To<LoadModalViewCommand>();
            commandBinder.Bind<CloseModalViewSignal>().To<CloseModalViewCommand>();

            commandBinder.Bind<AuthGuestSignal>().To<AuthGuestCommand>();
            commandBinder.Bind<AuthFacebookSignal>().To<AuthFacebookCommand>();
            commandBinder.Bind<ConnectBackendSignal>().To<ConnectBackendCommand>();
            commandBinder.Bind<BackendErrorSignal>().To<BackendErrorCommand>();
            commandBinder.Bind<ClockSyncedSignal>().To<ReceptionCommand>();
            commandBinder.Bind<DisconnectBackendSignal>().To<DisconnectBackendCommand>();
            commandBinder.Bind<FindMatchSignal>().To<MatchmakingCommand>();
            commandBinder.Bind<GetGameStartTimeSignal>().To<GetGameStartTimeCommand>();
            commandBinder.Bind<AppEventSignal>().To<AppEventCommand>();
            commandBinder.Bind<UpdateAppSignal>().To<UpdateAppCommand>();
            commandBinder.Bind<LoadLobbySignal>().To<LoadLobbyCommand>();
            commandBinder.Bind<SetPlayerSocialNameSignal>().To<SetPlayerSocialNameCommand>();
            commandBinder.Bind<LoadPlayerProfileSignal>().To<LoadPlayerProfileCommand>();
            commandBinder.Bind<LoadRoomsSignal>().To<LoadRoomsCommand>();
            commandBinder.Bind<LoadShopSignal>().To<LoadShopCommand>();
            commandBinder.Bind<LoadInventorySignal>().To<LoadInventoryCommand>();
            commandBinder.Bind<LoadShopLootBoxesSignal>().To<LoadShopLootBoxesCommand>();
            commandBinder.Bind<LoadShopAvatarsSignal>().To<LoadShopAvatarsCommand>();
            commandBinder.Bind<LoadShopChessSkinsSignal>().To<LoadShopChessSkinsCommand>();
            commandBinder.Bind<LoadShopChatSignal>().To<LoadShopChatCommand>();
            commandBinder.Bind<LoadShopCurrencySignal>().To<LoadShopCurrencyCommand>();
            commandBinder.Bind<LoadInventoryAvatarsSignal>().To<LoadInventoryAvatarsCommand>();
            commandBinder.Bind<LoadInventoryChessSkinsSignal>().To<LoadInventoryChessSkinsCommand>();
            commandBinder.Bind<LoadInventoryLootSignal>().To<LoadInventoryLootCommand>();
            commandBinder.Bind<GameAppEventSignal>().To<GameAppEventCommand>(); // TODO(mubeeniqbal): Verify and move to Game mappings.
            commandBinder.Bind<GetPlayerProfilePictureSignal>().To<GetPlayerProfilePictureCommand>();
            commandBinder.Bind<GetOpponentProfilePictureSignal>().To<GetOpponentProfilePictureCommand>();
            commandBinder.Bind<LoadEndGameSignal>().To<EndGameCommand>();
            commandBinder.Bind<LoadFreeCurrency1ModalSignal>().To<LoadFreeCurrency1ModalCommand>();
            commandBinder.Bind<LoadOutOfCurrency1ModalSignal>().To<LoadOutOfCurrency1Command>();
            commandBinder.Bind<PlayAdSignal>().To<PlayAdCommand>();
            commandBinder.Bind<BuildForgeCardsSignal>().To<BuildForgeCardsCommand>();
            commandBinder.Bind<SellForgeCardsSignal>().To<SellForgeCardsCommand>();
            commandBinder.Bind<ClaimLootBoxSignal>().To<ClaimLootCommand>();
            commandBinder.Bind<PurchaseShopItemSignal>().To<PurchaseShopItemCommand>();
            commandBinder.Bind<ApplyPlayerInventorySignal>().To<ApplyPlayerInventoryCommand>();

            commandBinder.Bind<LoadShopLootBoxesModalSignal>().To<LoadShopLootBoxesModalCommand>();
            commandBinder.Bind<LoadShopAvatarsModalSignal>().To<LoadShopAvatarsModalCommand>();
            commandBinder.Bind<LoadShopAvatarsBorderModalSignal>().To<LoadShopAvatarsBorderModalCommand>();
            commandBinder.Bind<LoadShopChessSkinsModalSignal>().To<LoadShopChessSkinsModalCommand>();
            commandBinder.Bind<LoadShopCurrency1ModalSignal>().To<LoadShopCurrency1ModalCommand>();
            commandBinder.Bind<LoadShopCurrency2ModalSignal>().To<LoadShopCurrency2ModalCommand>();

            commandBinder.Bind<LoadInventoryChessSkinsInfoModalSignal>().To<LoadInventoryChessSkinsInfoModalCommand>();
            commandBinder.Bind<LoadInventoryLootDismantleModalSignal>().To<LoadInventoryLootDismantleModalCommand>();
            commandBinder.Bind<LoadInventoryLootInfoModalSignal>().To<LoadInventoryLootInfoModalCommand>();

            // Signals to launch game modes from the lobby
            commandBinder.Bind<StartGameSignal>().To<TurboLabz.MPChess.StartGameCommand>();
            commandBinder.Bind<LoadCPUMenuSignal>().To<TurboLabz.CPUChess.LoadCPUMenuCommand>();

            // Bind signals for dispatching to mediators
            injectionBinder.Bind<HideViewSignal>().ToSingleton();
            injectionBinder.Bind<ShowViewSignal>().ToSingleton();
            injectionBinder.Bind<HideModalViewSignal>().ToSingleton();
            injectionBinder.Bind<ShowModalViewSignal>().ToSingleton();
            injectionBinder.Bind<UpdateRetryConnectionMessageSignal>().ToSingleton();
            injectionBinder.Bind<UpdateLobbyViewSignal>().ToSingleton();
            injectionBinder.Bind<UpdateShopViewSignal>().ToSingleton();
            injectionBinder.Bind<UpdateInventoryViewSignal>().ToSingleton();
            injectionBinder.Bind<UpdateShopLootBoxesViewSignal>().ToSingleton();
            injectionBinder.Bind<UpdateShopAvatarsViewSignal>().ToSingleton();
            injectionBinder.Bind<UpdateShopChessSkinsViewSignal>().ToSingleton();
            injectionBinder.Bind<UpdateShopChatViewSignal>().ToSingleton();
            injectionBinder.Bind<UpdateShopCurrencyViewSignal>().ToSingleton();
            injectionBinder.Bind<UpdateInventoryLootViewSignal>().ToSingleton();
            injectionBinder.Bind<UpdateInventoryAvatarsViewSignal>().ToSingleton();
            injectionBinder.Bind<UpdateInventoryChessSkinsViewSignal>().ToSingleton();
            injectionBinder.Bind<UpdatePlayerProfilePictureSignal>().ToSingleton();
            injectionBinder.Bind<UpdateOpponentProfilePictureSignal>().ToSingleton();
            injectionBinder.Bind<UpdateMatchmakingViewPreMatchFoundSignal>().ToSingleton();
            injectionBinder.Bind<UpdateMatchmakingViewPostMatchFoundSignal>().ToSingleton();
            injectionBinder.Bind<UpdateSetPlayerSocialNameViewSignal>().ToSingleton();
            injectionBinder.Bind<UpdatePlayerProfileViewSignal>().ToSingleton();
            injectionBinder.Bind<UpdateRoomsViewSignal>().ToSingleton();
            injectionBinder.Bind<UpdateEndGameViewSignal>().ToSingleton();

            injectionBinder.Bind<UpdateFreeCurrency1ModalViewSignal>().ToSingleton();
            injectionBinder.Bind<UpdateOutOfCurrency1ModalViewSignal>().ToSingleton();
            injectionBinder.Bind<UpdateCurrency1RewardModalViewSignal>().ToSingleton();
            injectionBinder.Bind<UpdateCurrency1RewardModalViewShowWaitForRewardSignal>().ToSingleton();

            injectionBinder.Bind<UpdateShopLootBoxesModalViewSignal>().ToSingleton();
            injectionBinder.Bind<UpdateShopAvatarsModalViewSignal>().ToSingleton();
            injectionBinder.Bind<UpdateShopAvatarsBorderModalViewSignal>().ToSingleton();
            injectionBinder.Bind<UpdateShopChessSkinsModalViewSignal>().ToSingleton();
            injectionBinder.Bind<UpdateShopCurrency1ModalViewSignal>().ToSingleton();
            injectionBinder.Bind<UpdateShopCurrency2ModalViewSignal>().ToSingleton();
            injectionBinder.Bind<UpdateInventoryChessSkinsInfoModalViewSignal>().ToSingleton();
            injectionBinder.Bind<UpdateInventoryLootDismantleModalViewSignal>().ToSingleton();
            injectionBinder.Bind<UpdateInventoryLootInfoModalViewSignal>().ToSingleton();

            // Bind views to mediators
            mediationBinder.Bind<SplashView>().To<SplashMediator>();
            mediationBinder.Bind<LoadingView>().To<LoadingMediator>();
            mediationBinder.Bind<AuthView>().To<AuthMediator>();
            mediationBinder.Bind<RetryConnectionView>().To<RetryConnectionMediator>();
            mediationBinder.Bind<LobbyView>().To<LobbyMediator>();
            mediationBinder.Bind<AppEventView>().To<AppEventMediator>();
            mediationBinder.Bind<MatchmakingView>().To<MatchmakingMediator>();
            mediationBinder.Bind<EndGameView>().To<EndGameMediator>();
            mediationBinder.Bind<SetPlayerSocialNameView>().To<SetPlayerSocialNameMediator>();
            mediationBinder.Bind<PlayerProfileView>().To<PlayerProfileMediator>();
            mediationBinder.Bind<RoomsView>().To<RoomsMediator>();
            mediationBinder.Bind<ShopView>().To<ShopMediator>();
            mediationBinder.Bind<InventoryView>().To<InventoryMediator>();
            mediationBinder.Bind<UpdateAppView>().To<UpdateAppMediator>();

            mediationBinder.Bind<FreeCurrency1ModalView>().To<FreeCurrency1ModalMediator>();
            mediationBinder.Bind<OutOfCurrency1ModalView>().To<OutOfCurrency1ModalMediator>();
            mediationBinder.Bind<Currency1RewardModalView>().To<Currency1RewardModalMediator>();

            mediationBinder.Bind<ShopLootBoxesModalView>().To<ShopLootBoxesModalMediator>();
            mediationBinder.Bind<ShopAvatarsModalView>().To<ShopAvatarsModalMediator>();
            mediationBinder.Bind<ShopAvatarsBorderModalView>().To<ShopAvatarsBorderModalMediator>();
            mediationBinder.Bind<ShopChessSkinsModalView>().To<ShopChessSkinsModalMediator>();
            mediationBinder.Bind<ShopCurrency1ModalView>().To<ShopCurrency1ModalMediator>();
            mediationBinder.Bind<ShopCurrency2ModalView>().To<ShopCurrency2ModalMediator>();

            mediationBinder.Bind<InventoryChessSkinsInfoModalView>().To<InventoryChessSkinsInfoModalMediator>();
            mediationBinder.Bind<InventoryLootDismantleModalView>().To<InventoryLootDismantleModalMediator>();
            mediationBinder.Bind<InventoryLootInfoModalView>().To<InventoryLootInfoModalMediator>();

            // Bind services
            injectionBinder.Bind<IBackendService>().To<GSService>().ToSingleton();
            injectionBinder.Bind<IFacebookService>().To<FBService>().ToSingleton();
            injectionBinder.Bind<ILocalizationService>().To<LocalizationService>().ToSingleton();
            injectionBinder.Bind<IAdsService>().To<UnityAdsService>().ToSingleton();
            injectionBinder.Bind<ILocalDataService>().To<EasySaveService>().ToSingleton();

            // Bind models
            injectionBinder.Bind<IViewStateModel>().To<ViewStateModel>().ToSingleton();
            injectionBinder.Bind <IModalViewStateModel>().To<ModalViewStateModel>().ToSingleton();
            injectionBinder.Bind<IPlayerModel>().To<PlayerModel>().ToSingleton();
            injectionBinder.Bind<IMatchInfoModel>().To<MatchInfoModel>().ToSingleton();
            injectionBinder.Bind<IAppEventModel>().To<AppEventModel>().ToSingleton();
            injectionBinder.Bind<ILeagueSettingsModel>().To<LeagueSettingsModel>().ToSingleton();
            injectionBinder.Bind<ILevelSettingsModel>().To<LevelSettingsModel>().ToSingleton();
            injectionBinder.Bind<IRoomSettingsModel>().To<RoomSettingsModel>().ToSingleton();
            injectionBinder.Bind<ITitleSettingsModel>().To<TitleSettingsModel>().ToSingleton();
            injectionBinder.Bind<IShopSettingsModel>().To<ShopSettingsModel>().ToSingleton();
            injectionBinder.Bind<IForgeSettingsModel>().To<ForgeSettingsModel>().ToSingleton();
            injectionBinder.Bind<IInventoryModel>().To<InventoryModel>().ToSingleton();
            injectionBinder.Bind<IPromotionsModel>().To<PromotionsModel>().ToSingleton();
            injectionBinder.Bind<IAppInfoModel>().To<AppInfoModel>().ToSingleton();
            injectionBinder.Bind<IAdInfoModel>().To<AdInfoModel>().ToSingleton();

            // Bind utils
            injectionBinder.Bind<IRoutineRunner>().To<StrangeRoutineRunner>().ToSingleton();
            injectionBinder.Bind<IGameEngineInfo>().To<UnityInfo>().ToSingleton();
            injectionBinder.Bind<IServerClock>().To<ServerClock>().ToSingleton();
            injectionBinder.Bind<ITimeControl>().To<TimeControl>(); // Not singleton

            MapGameBindings();
        }

        // TODO: move this to the game folder
        private void MapGameBindings()
        {
            // Bind common services
            injectionBinder.Bind<IChessService>().To<ChessService>().ToSingleton();
            injectionBinder.Bind<IChessAiService>().To<ChessAiService>().ToSingleton();

            MapMultiplayerGameBindings();
            MapCPUGameBindings();
        }
    }
}
