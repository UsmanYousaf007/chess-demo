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
    public class GameDisconnectingSignal : Signal {}
    public class SetUpdateURLSignal : Signal<string> {}
    public class FindMatchSignal : Signal {}
    public class FindMatchCompleteSignal : Signal<string> {}
    public class TapLongMatchSignal : Signal<string> {}
    public class CreateLongMatchSignal : Signal<string> {}
    public class StartLongMatchSignal : Signal<string> {}
    public class MatchFoundSignal : Signal<ProfileVO> {}
    public class GetGameStartTimeSignal : Signal {}
    public class ShowFindMatchSignal : Signal {}
    public class AudioStateChangedSignal : Signal<bool> {}
    public class NavigatorEventSignal : Signal<NavigatorEvent> {}
    public class NavigatorShowViewSignal : Signal<NavigatorViewId> {}
    public class NavigatorHideViewSignal : Signal<NavigatorViewId> {}
    public class NavigatorIgnoreEventSignal : Signal<NavigatorEvent> {}
    public class GetInitDataSignal : Signal {}
    public class GetInitDataCompleteSignal : Signal {}
    public class AuthFaceBookSignal : Signal {}
    public class AuthFacebookResultSignal : Signal<bool, Sprite, string> {}
    public class UpdateProfileSignal : Signal<ProfileVO> {}
    public class UpdateOpponentProfileSignal : Signal<ProfileVO> {}
    public class RemoteStorePurchaseCompletedSignal : Signal<string> {}
    public class BackendErrorSignal : Signal<BackendResult> {}
	public class ReceptionSignal : Signal {}
    public class LoadLobbySignal : Signal {}
    public class SavePlayerInventorySignal : Signal {}
    public class SetSkinSignal : Signal<string> {}
    public class InitBackendOnceSignal : Signal {}
    public class PurchaseStoreItemSignal : Signal<string, bool> {}
    public class PurchaseStoreItemResultSignal : Signal<StoreItem, PurchaseResult> {}
    public class StartGameSignal : Signal {}
    public class WifiIsHealthySignal : Signal<bool> {}
    public class SetErrorAndHaltSignal : Signal<BackendResult> {}
    public class HaltSignal: Signal<BackendResult> {}
    public class UpdatePlayerBucksSignal : Signal<long> {}
    public class AddFriendsSignal : Signal<Dictionary<string, Friend>, bool> {}
    public class RefreshCommunitySignal : Signal {}
    public class RefreshFriendsSignal : Signal {}
    public class GetSocialPicsSignal : Signal<Dictionary<string, Friend>> {}
    public class NewFriendSignal : Signal<string> {}
    public class BlockFriendSignal : Signal<string> {}
    public class ClearCommunitySignal : Signal {}
    public class ClearFriendsSignal : Signal {}
    public class UpdateFriendPicSignal : Signal<string, Sprite> {}
    public class FriendsShowConnectFacebookSignal : Signal<bool> {}
    public class UpdateProfileDialogSignal : Signal<ProfileDialogVO> {}
    public class ShowProfileDialogSignal : Signal<string> {}
    public class LoadFriendsSignal : Signal {}
    public class ShareAppSignal : Signal {}
    public class ShowAdSignal : Signal<bool> {}
    public class UpdateFriendBarStatusSignal : Signal<LongPlayStatusVO> {}
    public class UpdateFriendBarSignal: Signal<string> {}
    public class ToggleFacebookButton : Signal<bool> {}
    public class AcceptSignal : Signal<string> {}
    public class DeclineSignal : Signal<string> {}


    // SKINS
    public class LoadSkinRefsSignal : Signal<string> {}
    public class RefreshSkinLinksSignal : Signal {}

}
