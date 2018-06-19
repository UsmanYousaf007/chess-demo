/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;

using strange.extensions.signal.impl;

namespace TurboLabz.InstantFramework
{
    // General Signals
    public class StartSignal : Signal {}
    public class AppEventSignal : Signal<AppEvent> {}
    public class GameAppEventSignal : Signal<AppEvent> {}
    public class SavePlayerSignal : Signal {}

    // Preferences Signals
    public class AudioStateChangedSignal : Signal<bool> {}

    // Navigator Signals
    public class NavigatorEventSignal : Signal<NavigatorEvent> {}
    public class NavigatorShowViewSignal : Signal<NavigatorViewId> {}
    public class NavigatorHideViewSignal : Signal<NavigatorViewId> {}
    public class NavigatorIgnoreEventSignal : Signal<NavigatorEvent> {}

	// Model data load signals
	public class InitGameDataSignal : Signal {}
    public class LoadMetaDataCompleteSignal : Signal {}

    public class AuthFaceBookSignal : Signal {}
    public class AuthFacebookSuccessSignal : Signal {}

	public class RemoteStorePurchaseCompletedSignal : Signal<string> {}

	public class BackendErrorSignal : Signal<BackendResult> {}
	public class SplashAnimCompleteSignal : Signal {}
	public class ReceptionSignal : Signal {}
    public class LoadLobbySignal : Signal {}

    public class GetPlayerProfilePictureSignal : Signal {}
    public class GetOpponentProfilePictureSignal : Signal {}
    //public class UpdateSetPlayerSocialNameViewSignal : Signal<SetPlayerSocialNameVO> {}
    public class ApplyPlayerInventorySignal : Signal {}
    public class InitBackendOnceSignal : Signal {}


    public class UpdatePlayerProfilePictureSignal : Signal<Sprite> {}
    public class UpdatePlayerProfilePictureInfoSignal : Signal<Sprite> {}

    public class PurchaseStoreItemSignal : Signal<string, bool> {}
    public class PurchaseStoreItemResultSignal : Signal<StoreItem, PurchaseResult> {}
}
