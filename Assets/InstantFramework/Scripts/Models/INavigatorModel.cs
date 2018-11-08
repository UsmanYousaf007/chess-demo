/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public interface INavigatorModel
    {
        NS currentState { get; set; }
        NS previousState { get; set; }
        List<NavigatorViewId> history { get; set; }
        List<NavigatorViewId> viewStack { get; set; }
        NavigatorEvent ignoreEvent { get; set; }
        NavigatorViewId currentViewId { get; }

        void Reset();
    }
}
