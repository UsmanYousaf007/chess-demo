/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-07 17:48:13 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;

using strange.extensions.signal.impl;

namespace TurboLabz.Gamebet
{
    // TODO: Organize these signals with proper comments and order
    // The main app start signal
    public class StartSignal : Signal {}
    public class BootSignal : Signal {}

    // NAVIGATOR signals
    public class NavigatorEventSignal : Signal<NavigatorEvent> {}
    public class NavigatorShowViewSignal : Signal<NavigatorViewId> {}
    public class NavigatorHideViewSignal : Signal<NavigatorViewId> {}

    // Command signals
    public class BackendErrorSignal : Signal<BackendResult> {}
    public class ConnectBackendSignal : Signal {}
    public class DisconnectBackendSignal : Signal {}
    public class AuthGuestSignal : Signal {}
    public class AuthFacebookSignal : Signal {}
    public class FindMatchSignal : Signal<FindMatchVO> {}
    public class GetGameStartTimeSignal : Signal {}
    public class ClockSyncedSignal : Signal<BackendResult> {}
    public class AppEventSignal : Signal<AppEvent> {}
    public class UpdateAppSignal : Signal {}
    public class LoadLobbySignal : Signal {}
    public class SetPlayerSocialNameSignal : Signal<string> {}
    public class LoadPlayerProfileSignal : Signal {}
    public class LoadRoomsSignal : Signal {}
    public class LoadShopSignal : Signal {}
    public class LoadInventorySignal : Signal {}
    public class LoadShopLootBoxesSignal : Signal {}
    public class LoadShopAvatarsSignal : Signal {}
    public class LoadShopChessSkinsSignal : Signal {}
    public class LoadShopChatSignal : Signal {}
    public class LoadShopCurrencySignal : Signal {}
    public class LoadInventoryAvatarsSignal : Signal {}
    public class LoadInventoryChessSkinsSignal : Signal {}
    public class LoadInventoryLootSignal : Signal {}
    public class GameAppEventSignal : Signal<AppEvent> {}
    public class GetPlayerProfilePictureSignal : Signal {}
    public class GetOpponentProfilePictureSignal : Signal {}
    public class LoadEndGameSignal : Signal {}
    public class PlayAdSignal : Signal {}
    public class BuildForgeCardsSignal : Signal<string, Signal> {}
    public class SellForgeCardsSignal : Signal<ForgeCardVO, Signal> {}
    public class ClaimLootBoxSignal : Signal<string, Signal> {}
    public class PurchaseShopItemSignal : Signal<string , Signal> {}
    public class ApplyPlayerInventorySignal : Signal {}

    // Modal View Command Signals
    public class LoadFreeCurrency1ModalSignal : Signal {}
    public class LoadOutOfCurrency1ModalSignal : Signal {}

    public class LoadShopAvatarsModalSignal : Signal <ShopVO>{}
    public class LoadShopAvatarsBorderModalSignal : Signal <ShopVO>{}
    public class LoadShopLootBoxesModalSignal : Signal <ShopVO>{}
    public class LoadShopChessSkinsModalSignal : Signal <ShopVO>{}
    public class LoadShopCurrency1ModalSignal : Signal <ShopVO>{}
    public class LoadShopCurrency2ModalSignal : Signal <ShopVO>{}
    public class LoadInventoryChessSkinsInfoModalSignal : Signal <ShopVO>{}
    public class LoadInventoryLootDismantleModalSignal : Signal <ShopVO>{}
    public class LoadInventoryLootInfoModalSignal : Signal <ShopVO>{}

    // View loading signals
    public class LoadViewSignal : Signal<ViewId> {}
    public class LoadPreviousViewSignal : Signal {}
    public class ShowViewSignal : Signal<ViewId> {}
    public class HideViewSignal : Signal<ViewId> {}

    // Modal View Loading signals
    public class LoadModalViewSignal : Signal<ModalViewId> {}
    public class CloseModalViewSignal : Signal {}
    public class ShowModalViewSignal : Signal<ModalViewId> {}
    public class HideModalViewSignal : Signal<ModalViewId> {}

    // Command-mediator signals
    public class UpdateRetryConnectionMessageSignal : Signal<BackendResult> {}
    public class UpdateLobbyViewSignal : Signal<LobbyVO> {}
    public class UpdateShopViewSignal : Signal <ShopVO> {}
    public class UpdateInventoryViewSignal : Signal <ShopVO> {}
    public class UpdateShopLootBoxesViewSignal : Signal <ShopVO> {}
    public class UpdateShopAvatarsViewSignal : Signal <ShopVO> {}
    public class UpdateShopChessSkinsViewSignal : Signal <ShopVO> {}
    public class UpdateShopChatViewSignal : Signal <ShopVO> {}
    public class UpdateShopCurrencyViewSignal : Signal <ShopVO> {}
    public class UpdateInventoryAvatarsViewSignal : Signal <ShopVO> {}
    public class UpdateInventoryChessSkinsViewSignal : Signal <ShopVO> {}
    public class UpdateInventoryLootViewSignal : Signal <ShopVO> {}
    public class UpdatePlayerProfilePictureSignal : Signal<Sprite> {}
    public class UpdateOpponentProfilePictureSignal : Signal<Sprite> {}
    public class UpdateMatchmakingViewPreMatchFoundSignal : Signal<PreMatchmakingVO> {}
    public class UpdateMatchmakingViewPostMatchFoundSignal : Signal<PostMatchmakingVO> {}
    public class UpdateSetPlayerSocialNameViewSignal : Signal<SetPlayerSocialNameVO> {}
    public class UpdatePlayerProfileViewSignal : Signal<PlayerProfileVO> {}
    public class UpdateRoomsViewSignal : Signal<RoomsVO> {}
    public class UpdateEndGameViewSignal : Signal<EndGameVO> {}

    public class UpdateFreeCurrency1ModalViewSignal : Signal<bool> {}
    public class UpdateOutOfCurrency1ModalViewSignal : Signal<bool> {}
    public class UpdateCurrency1RewardModalViewSignal : Signal<Currency1RewardModalVO> {}
    public class UpdateCurrency1RewardModalViewShowWaitForRewardSignal : Signal {}

    public class UpdateShopLootBoxesModalViewSignal : Signal<ShopVO> {}
    public class UpdateShopAvatarsModalViewSignal : Signal<ShopVO> {}
    public class UpdateShopAvatarsBorderModalViewSignal : Signal<ShopVO> {}
    public class UpdateShopChessSkinsModalViewSignal : Signal<ShopVO> {}
    public class UpdateShopCurrency1ModalViewSignal : Signal<ShopVO> {}
    public class UpdateShopCurrency2ModalViewSignal : Signal<ShopVO> {}
    public class UpdateInventoryChessSkinsInfoModalViewSignal : Signal<ShopVO> {}
    public class UpdateInventoryLootDismantleModalViewSignal : Signal<ShopVO> {}
    public class UpdateInventoryLootInfoModalViewSignal : Signal<ShopVO> {}

    // Gamebet-Game interfacing signals
    public class StartGameSignal :  Signal {} // Enter Game from Gamebet
    public class LoadCPUMenuSignal : Signal {} // Enter CPU mode from Gamebet
}
