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

namespace TurboLabz.InstantFramework
{
    // General Signals
    public class StartSignal : Signal {}
    public class AppEventSignal : Signal<AppEvent> {}
    public class GameAppEventSignal : Signal<AppEvent> {}
    public class LoadGameSignal : Signal {}

    // Navigator Signals
    public class NavigatorEventSignal : Signal<NavigatorEvent> {}
    public class NavigatorShowViewSignal : Signal<NavigatorViewId> {}
    public class NavigatorHideViewSignal : Signal<NavigatorViewId> {}


}
