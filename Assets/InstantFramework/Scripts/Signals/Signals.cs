/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;

using strange.extensions.signal.impl;

namespace TurboLabz.InstantFramework
{
    public class StartSignal : Signal {}
    public class AppEventSignal : Signal<AppEvent> {}
    public class GameAppEventSignal : Signal<AppEvent> {}
    public class FindMatchSignal : Signal {}
    public class MatchFoundSignal : Signal<Sprite> {}
    public class GetGameStartTimeSignal : Signal {}
    public class ShowFindMatchSignal : Signal {}
    public class AudioStateChangedSignal : Signal<bool> {}
    public class NavigatorEventSignal : Signal<NavigatorEvent> {}
    public class NavigatorShowViewSignal : Signal<NavigatorViewId> {}
    public class NavigatorHideViewSignal : Signal<NavigatorViewId> {}
    public class NavigatorIgnoreEventSignal : Signal<NavigatorEvent> {}
    public class InitGameDataSignal : Signal {}
    public class LoadMetaDataCompleteSignal : Signal {}
    public class AuthFaceBookSignal : Signal {}
    public class AuthFacebookResultSignal : Signal<bool, Sprite, string> {}
    public class UpdateProfileSignal : Signal<ProfileVO> {}
    public class RemoteStorePurchaseCompletedSignal : Signal<string> {}
    public class BackendErrorSignal : Signal<BackendResult> {}
	public class SplashAnimCompleteSignal : Signal {}
	public class ReceptionSignal : Signal {}
    public class LoadLobbySignal : Signal {}
    public class ApplyPlayerInventorySignal : Signal {}
    public class InitBackendOnceSignal : Signal {}
    public class PurchaseStoreItemSignal : Signal<string, bool> {}
    public class PurchaseStoreItemResultSignal : Signal<StoreItem, PurchaseResult> {}
    public class StartGameSignal : Signal {}
    public class WifiIsHealthySignal : Signal<bool> {}
    public class SetErrorAndHaltSignal : Signal<BackendResult> {}
    public class HaltSignal: Signal<BackendResult> {}

    // SKINS
    public class ApplySkinSignal : Signal<string> {}
    public class UpdateSkinSignal : Signal {}

}
