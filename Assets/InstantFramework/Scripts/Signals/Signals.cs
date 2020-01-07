/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;

using strange.extensions.signal.impl;
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public class StartSignal : Signal {}
    public class AppEventSignal : Signal<AppEvent> {}
    public class GameAppEventSignal : Signal<AppEvent> {}
    public class InitFacebookSignal : Signal{}
    public class GameDisconnectingSignal : Signal {}
    public class SetUpdateURLSignal : Signal<string> {}
    public class FindMatchSignal : Signal<string> {}
    public class FindMatchCompleteSignal : Signal<string> {}
    public class TapLongMatchSignal : Signal<string, bool> {}
    public class CreateLongMatchSignal : Signal<string, bool> {}
    public class StartLongMatchSignal : Signal<string> {}
    public class MatchFoundSignal : Signal<ProfileVO> {}
    public class GetGameStartTimeSignal : Signal {}
    public class ShowFindMatchSignal : Signal<string> {}
    public class AudioStateChangedSignal : Signal<bool> {}
    public class NavigatorEventSignal : Signal<NavigatorEvent> {}
    public class NavigatorShowViewSignal : Signal<NavigatorViewId> {}
    public class NavigatorHideViewSignal : Signal<NavigatorViewId> {}
    public class NavigatorIgnoreEventSignal : Signal<NavigatorEvent> {}
    public class ModelsResetSignal : Signal {}
    public class ModelsSaveToDiskSignal : Signal {}
    public class ModelsLoadFromDiskSignal : Signal { }
    public class GetInitDataSignal : Signal<bool> {}
    public class GetInitDataCompleteSignal : Signal {}
    public class AuthFaceBookSignal : Signal {}
    public class AuthFacebookResultSignal : Signal<AuthFacebookResultVO> {}
    public class UpdateProfileSignal : Signal<ProfileVO> {}
    public class UpdateOpponentProfileSignal : Signal<ProfileVO> {}
    public class UpdateChatOpponentPicSignal : Signal<Sprite> {}
    public class RemoteStorePurchaseCompletedSignal : Signal<string> {}
    public class BackendErrorSignal : Signal<BackendResult> {}
	public class ReceptionSignal : Signal<bool> {}
    public class LoadLobbySignal : Signal {}
    public class SavePlayerInventorySignal : Signal {}
    public class SetSkinSignal : Signal<string> {}
    public class SkinUpdatedSignal : Signal { }
    public class InitBackendOnceSignal : Signal {}
    public class PurchaseStoreItemSignal : Signal<string, bool> {}
    public class PurchaseStoreItemResultSignal : Signal<StoreItem, PurchaseResult> {}
    public class UpdatePurchasedStoreItemSignal : Signal<StoreItem> { }
    public class ConsumeVirtualGoodSignal : Signal<string, int> {}
    public class StartGameSignal : Signal {}
    public class WifiIsHealthySignal : Signal<bool> {}
    public class SplashWifiIsHealthySignal : Signal {}
    public class ShowSplashContentSignal : Signal<bool> {}
    public class ResetCapturedPiecesSignal : Signal { }
    public class SetErrorAndHaltSignal : Signal<BackendResult, string> {}
    public class HaltSignal: Signal<BackendResult> {}
    public class UpdatePlayerBucksSignal : Signal<long> {}
    public class UpdatePlayerInventorySignal : Signal<PlayerInventoryVO> {}
    public class UpdateRemoveAdsSignal : Signal<string, bool> {}
    public class AddFriendsSignal : Signal<Dictionary<string, Friend>, FriendCategory> {}
    public class RefreshCommunitySignal : Signal {}
    public class SearchFriendSignal : Signal<string, int> {}
    public class RefreshFriendsSignal : Signal {}
    public class RemoveFriendSignal : Signal<string> {}
    public class GetSocialPicsSignal : Signal<Dictionary<string, Friend>> {}
    public class NewFriendSignal : Signal<string, bool> {}
    public class BlockFriendSignal : Signal<string> {}
    public class LoadChatSignal : Signal<string, bool> { }
    public class RemoveCommunityFriendSignal : Signal<string> { }
    public class UpdateFriendOnlineStatusSignal : Signal<ProfileVO> {}
    public class ClearCommunitySignal : Signal {}
    public class ClearFriendsSignal : Signal {}
    public class ClearFriendSignal : Signal<string> {}
    public class UpdateFriendPicSignal : Signal<string, Sprite> {}
    public class UpdateEloScoresSignal : Signal<EloVO> {}
    public class FriendsShowConnectFacebookSignal : Signal<bool> {}
    public class UpdateProfileDialogSignal : Signal<ProfileDialogVO> {}
    public class UpdateShareDialogSignal : Signal<Sprite> { }
    public class UpdateNewGameDialogSignal : Signal<string> {}
    public class ShowProfileDialogSignal : Signal<string, FriendBar> {}
    public class ShowShareScreenDialogSignal : Signal { }
    public class LoadFriendsSignal : Signal {}
    public class ShareAppSignal : Signal {}
    public class ShowAdSignal : Signal<ResultAdsVO> {}
    public class ToggleBannerSignal: Signal<bool> {}
    public class PauseNotificationsSignal : Signal<bool> { }
    public class RequestToggleBannerSignal : Signal {}
    public class UpdateFriendBarStatusSignal : Signal<LongPlayStatusVO> {}
    public class UpdateFriendBarSignal: Signal<Friend, string> {}
    public class FriendBarBusySignal : Signal<string, bool, CreateLongMatchAbortReason> {}
    public class ToggleFacebookButton : Signal<bool> {}
    public class AcceptSignal : Signal<string> {}
    public class DeclineSignal : Signal<string> {}
    public class CloseStripSignal : Signal<string> { }
    public class UnregisterSignal : Signal<string> {}
    public class SortFriendsSignal : Signal {}
    public class SortCommunitySignal : Signal { }
    public class SortSearchedSignal : Signal { }
    public class SetActionCountSignal : Signal<int> {}
    public class ShowFriendsHelpSignal : Signal{ }
    public class SendChatMessageSignal : Signal<ChatMessage> {}
    public class ReceiveChatMessageSignal : Signal<ChatMessage, bool> {}
    public class DisplayChatMessageSignal : Signal<ChatMessage> {}
    public class ClearActiveChatSignal : Signal<string> {}
    public class ClearUnreadMessagesSignal : Signal<string> {}
    public class AddUnreadMessagesToBarSignal : Signal<string, int> {}
    public class ClearUnreadMessagesFromBarSignal : Signal<string> {}
    public class RestorePurchasesSignal : Signal {}
    public class NewFriendAddedSignal : Signal<string> {}
    public class NotificationRecievedSignal : Signal<NotificationVO> {}
    public class PreShowNotificationSignal : Signal {}
    public class PostShowNotificationSignal : Signal {}
    public class ResumeMatchSignal : Signal<NavigatorViewId> {}
    public class UpdatePlayerNotificationCountSignal : Signal<int> {}
    public class UpdatePlayerDataSignal: Signal {}
    public class ChallengeAcceptedSignal : Signal {}
    public class ChangeUserDetailsSignal : Signal<string> {}
    public class OpponentPingedForConnectionSignal : Signal<bool> {}
    public class ShowInGameProfileSingal : Signal {}
    public class ChessboardBlockerEnableSignal : Signal<bool> {}
    public class ReconnectViewEnableSignal : Signal<bool> {}
    public class UpdateFindViewSignal : Signal<FindViewVO> {}
    public class UpdateConfirmDlgSignal : Signal<ConfirmDlgVO> {}
    public class ContactSupportSignal : Signal {}
    public class FindMatchRequestCompleteSignal : Signal<string> {}
    public class StoreAvailableSignal : Signal<bool> {}
    public class SyncReconnectDataSignal : Signal<string> { }
    public class CancelSearchResultSignal : Signal { }

    // SKINS
    public class LoadSkinRefsSignal : Signal<string> {}
    public class RefreshSkinLinksSignal : Signal {}
    public class ShowMaintenanceViewSignal : Signal<int> { }
}
