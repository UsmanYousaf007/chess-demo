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

    // Preferences Signals
    public class AudioStateChangedSignal : Signal<bool> {}

    // Navigator Signals
    public class NavigatorEventSignal : Signal<NavigatorEvent> {}
    public class NavigatorShowViewSignal : Signal<NavigatorViewId> {}
    public class NavigatorHideViewSignal : Signal<NavigatorViewId> {}
    public class NavigatorIgnoreEventSignal : Signal<NavigatorEvent> {}

	// Model data load signals
	public class LoadMetaDataSignal : Signal {}

	public class RemoteStorePurchaseCompletedSignal : Signal<string> {}

	public class BackendErrorSignal : Signal<BackendResult> {}
	public class SplashAnimCompleteSignal : Signal {}
	public class ReceptionSignal : Signal {}
}
